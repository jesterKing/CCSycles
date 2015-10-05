/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#include "internal_types.h"

extern std::vector<CCScene> scenes;
extern std::vector<ccl::DeviceInfo> devices;
extern std::vector<ccl::SessionParams> session_params;

/* Hold all created sessions. */
std::vector<CCSession*> sessions;

/* Four vectors to hold registered callback functions.
 * For each created session a corresponding idx into these
 * vectors will exist, but in the case no callback
 * was registered the value for idx will be nullptr.
 */
std::vector<STATUS_UPDATE_CB> status_cbs;
std::vector<TEST_CANCEL_CB> cancel_cbs;
std::vector<RENDER_TILE_CB> update_cbs;
std::vector<RENDER_TILE_CB> write_cbs;

/* Wrap status update callback. */
void CCSession::status_update(void) {
	if (status_cbs[this->id] != nullptr) {
		status_cbs[this->id](this->id);
	}
}

/* Wrap status update callback. */
void CCSession::test_cancel(void) {
	if (cancel_cbs[this->id] != nullptr) {
		cancel_cbs[this->id](this->id);
	}
}

/* floats per pixel (rgba). */
const int stride = 4;

/* copy the pixel buffer from RenderTile to the final pixel buffer in CCSession. */
void copy_pixels_to_ccsession(ccl::RenderTile &tile, unsigned int sid) {

	ccl::RenderBuffers* buffers = tile.buffers;
	/* always do copy_from_device(). This is necessary when rendering is done
	 * on i.e. GPU or network node.
	 */
	buffers->copy_from_device();
	ccl::BufferParams& params = buffers->params;

	/* have a local float buffer to copy tile buffer to. */
	std::vector<float> pixels(params.width*params.height * stride, 0.5f);

	CCSession* se = sessions[sid];
	int scewidth = params.full_width;
	int sceheight = params.full_height;

	int tilex = params.full_x - se->session->tile_manager.params.full_x;
	int tiley = params.full_y - se->session->tile_manager.params.full_y;

	/* Copy the tile buffer to pixels. */
	if (!buffers->get_pass_rect(ccl::PassType::PASS_COMBINED, 1.0f, tile.sample, stride, &pixels[0])) {
		return;
	}
	
	/* Copy pixels to final image buffer. */
	bool firstpass = true;
	for (int y = 0; y < params.height; y++) {
		for (int x = 0; x < params.width; x++) {
			/* from tile pixels coord. */
			int tileidx = y * params.width * stride + x * stride;
			/* to full image pixels coord. */
			int fullimgidx = (tiley + y) * scewidth * stride + (tilex + x) * stride;

			/* copy the tile pixels from pixels into session final pixel buffer. */
			se->pixels[fullimgidx + 0] = pixels[tileidx + 0];
			se->pixels[fullimgidx + 1] = pixels[tileidx + 1];
			se->pixels[fullimgidx + 2] = pixels[tileidx + 2];
			se->pixels[fullimgidx + 3] = pixels[tileidx + 3];
		}
	}
}

/* Wrapper callback for render tile update. Copies tile result into session full image buffer. */
void CCSession::update_render_tile(ccl::RenderTile &tile)
{
	copy_pixels_to_ccsession(tile, this->id);

	ccl::RenderBuffers* buffers = tile.buffers;
	ccl::BufferParams& params = buffers->params;

	CCSession* se = sessions[this->id];

	int tilex = params.full_x - se->session->tile_manager.params.full_x;
	int tiley = params.full_y - se->session->tile_manager.params.full_y;

	if (update_cbs[this->id] != nullptr) {
		update_cbs[this->id](this->id, tilex, tiley, params.width, params.height, 4);
	}
}

/* Wrapper callback for render tile write. Copies tile result into session full image buffer. */
void CCSession::write_render_tile(ccl::RenderTile &tile)
{
	copy_pixels_to_ccsession(tile, this->id);

	ccl::RenderBuffers* buffers = tile.buffers;
	ccl::BufferParams& params = buffers->params;

	auto se = sessions[this->id];

	auto tilex = params.full_x - se->session->tile_manager.params.full_x;
	auto tiley = params.full_y - se->session->tile_manager.params.full_y;
	if (write_cbs[this->id] != nullptr) {
		write_cbs[this->id](this->id, tilex, tiley, params.width, params.height, 4);
	}
}

/**
 * Clean up resources acquired during this run of Cycles.
 */
void _cleanup_sessions()
{
	for (CCSession* se : sessions) {
		delete [] se->pixels;
		delete se->session;
		delete se;
	}

	sessions.clear();
	session_params.clear();

	status_cbs.clear();
	cancel_cbs.clear();
	update_cbs.clear();
	write_cbs.clear();
}

CCSession* CCSession::create(int width, int height, unsigned int buffer_stride) {
	int img_size = width * height;
	float* pixels_ = new float[img_size*buffer_stride];
	CCSession* se = new CCSession(pixels_, img_size*buffer_stride, buffer_stride);

	return se;
}
void CCSession::reset(int width, int height, unsigned int buffer_stride_) {
	int img_size = width * height;
	if (img_size*buffer_stride_ != buffer_size || buffer_stride_ != buffer_stride) {
		delete[] pixels;

		pixels = new float[img_size*buffer_stride_];
		buffer_size = img_size*buffer_stride_;
		buffer_stride = buffer_stride_;
	}
}

unsigned int cycles_session_create(unsigned int client_id, unsigned int session_params_id, unsigned int scene_id)
{
	ccl::SessionParams params;
	if (session_params_id >= 0 && session_params_id < session_params.size()) {
		params = session_params[session_params_id];
	}

	CCScene& sce = scenes[scene_id];

	auto csesid = -1;
	auto hid = 0;

	CCSession* session = CCSession::create(sce.scene->camera->width, sce.scene->camera->height, 4);
	// TODO: pass ccl::Session into CCSession::create
	session->session = new ccl::Session(params);
	session->session->scene = sce.scene;

	session->session->update_render_tile_cb = function_bind<void>(&CCSession::update_render_tile, session, std::placeholders::_1);
	session->session->write_render_tile_cb = function_bind<void>(&CCSession::write_render_tile, session, std::placeholders::_1);

	auto csessit = sessions.begin();
	auto csessend = sessions.end();
	while (csessit != csessend) {
		if ((*csessit) == nullptr) {
			csesid = hid;
		}
		++hid;
		++csessit;
	}

	if (csesid == -1) {
		sessions.push_back(session);
		csesid = (unsigned int)(sessions.size() - 1);
		status_cbs.push_back(nullptr);
		cancel_cbs.push_back(nullptr);
		update_cbs.push_back(nullptr);
		write_cbs.push_back(nullptr);
	}
	else {
		sessions[csesid] = session;
		status_cbs[csesid] = nullptr;
		update_cbs[csesid] = nullptr;
		write_cbs[csesid] = nullptr;
	}


	session->id = csesid;

	logger.logit(client_id, "Created session ", session->id, " for scene ", scene_id, " with session_params ", session_params_id);

	return session->id;
}

void cycles_session_destroy(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id)

	CCSession* ccses = sessions[session_id];

	for (CCScene& csc : scenes) {
		if (csc.scene == session->scene) {
			csc.scene = nullptr; /* don't delete here, since session deconstructor takes care of it. */
		}
	}

	delete ccses;

	sessions[session_id] = nullptr;

	SESSION_FIND_END()
}

void cycles_session_reset(unsigned int client_id, unsigned int session_id, unsigned int width, unsigned int height, unsigned int samples)
{
	SESSION_FIND(session_id)
		logger.logit(client_id, "Reset session ", session_id, ". width ", width, " height ", height, " samples ", samples);
		CCSession* se = sessions[session_id];
		se->reset(width, height, 4);
		ccl::BufferParams bufParams;
		bufParams.width = bufParams.full_width = width;
		bufParams.height = bufParams.full_height = height;
		session->reset(bufParams, (int)samples);
	SESSION_FIND_END()
}

void cycles_session_set_update_callback(unsigned int client_id, unsigned int session_id, void(*update)(unsigned int sid))
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		status_cbs[session_id] = update;
		session->progress.set_update_callback(function_bind<void>(&CCSession::status_update, se));
		logger.logit(client_id, "Set status update callback for session ", session_id);
	SESSION_FIND_END()
}

void cycles_session_set_cancel_callback(unsigned int client_id, unsigned int session_id, void(*cancel)(unsigned int sid))
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		cancel_cbs[session_id] = cancel;
		session->progress.set_cancel_callback(function_bind<void>(&CCSession::test_cancel, se));
		logger.logit(client_id, "Set status cancel callback for session ", session_id);
	SESSION_FIND_END()
}

void cycles_session_set_update_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB update_tile_cb)
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		update_cbs[session_id] = update_tile_cb;
		logger.logit(client_id, "Set render tile update callback for session ", session_id);
	SESSION_FIND_END()
}

void cycles_session_set_write_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB write_tile_cb)
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		write_cbs[session_id] = write_tile_cb;
		logger.logit(client_id, "Set render tile write callback for session ", session_id);
	SESSION_FIND_END()
}

void cycles_session_cancel(unsigned int client_id, unsigned int session_id, const char *cancel_message)
{
	SESSION_FIND(session_id)
		logger.logit(client_id, "Cancel session ", session_id, " with message ", cancel_message);
		session->progress.set_cancel(std::string(cancel_message));
	SESSION_FIND_END()
}

void cycles_session_start(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id)
		logger.logit(client_id, "Starting session ", session_id);
		session->start();
	SESSION_FIND_END()
}

void cycles_session_wait(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id)
		logger.logit(client_id, "Waiting for session ", session_id);
		session->wait();
	SESSION_FIND_END()
}

void cycles_session_get_buffer_info(unsigned int client_id, unsigned int session_id, unsigned int* buffer_size, unsigned int* buffer_stride)
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		*buffer_size = se->buffer_size;
		*buffer_stride = se->buffer_stride;
		logger.logit(client_id, "Session ", session_id, " get_buffer_info. size ", *buffer_size, " stride ", *buffer_stride);
	SESSION_FIND_END()
}

float* __cdecl cycles_session_get_buffer(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id);
		CCSession* se = sessions[session_id];
		return se->pixels;
	SESSION_FIND_END();

	return nullptr;
}

void cycles_session_copy_buffer(unsigned int client_id, unsigned int session_id, float* pixel_buffer)
{
	SESSION_FIND(session_id)
		CCSession* se = sessions[session_id];
		memcpy(pixel_buffer, se->pixels, se->buffer_size*sizeof(float));
		logger.logit(client_id, "Session ", session_id, " copy complete pixel buffer");
	SESSION_FIND_END()
}

void cycles_session_draw(unsigned int client_id, unsigned int session_id)
{
	static ccl::DeviceDrawParams draw_params = ccl::DeviceDrawParams();

	SESSION_FIND(session_id)
		ccl::BufferParams session_buf_params;
		session_buf_params.width = session_buf_params.full_width = session->scene->camera->width;
		session_buf_params.height = session_buf_params.full_height = session->scene->camera->height;

		session->draw(session_buf_params, draw_params);

	SESSION_FIND_END()
}

void cycles_progress_reset(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id)
		session->progress.reset();
	SESSION_FIND_END()
}

int cycles_progress_get_sample(unsigned int client_id, unsigned int session_id)
{
	SESSION_FIND(session_id)
		ccl::TileManager tm = session->tile_manager;
		return tm.state.sample;
	SESSION_FIND_END()

	return INT_MIN;
}

void cycles_progress_get_tile(unsigned int client_id, unsigned int session_id, int *tile, double *total_time, double* sample_time, double* tile_time)
{
	SESSION_FIND(session_id)
		return session->progress.get_tile(*tile, *total_time, *sample_time, *tile_time);
	SESSION_FIND_END()
}

void cycles_tilemanager_get_sample_info(unsigned int client_id, unsigned int session_id, unsigned int* samples, unsigned int* total_samples)
{
	SESSION_FIND(session_id)
		*samples = session->tile_manager.state.sample + 1;
		*total_samples = session->tile_manager.num_samples;
	SESSION_FIND_END()
}

/* Get cycles render progress. Note that progress will be clamped to 1.0f. */
void cycles_progress_get_progress(unsigned int client_id, unsigned int session_id, float* progress, double* total_time, double* render_time, double* tile_time)
{
	SESSION_FIND(session_id)
		double tile_time_, total_time_, render_time_;
		int tile, sample, samples_per_tile;
		int tile_total = session->tile_manager.state.num_tiles;

		*progress = 0.0f;

		session->progress.get_tile(tile, total_time_, render_time_, tile_time_);
		*total_time = total_time_;
		*render_time = render_time_;
		*tile_time = tile_time_;
		sample = session->progress.get_sample();
		samples_per_tile = session->tile_manager.num_samples;

		if (samples_per_tile > 0 && tile_total > 0) {
			*progress = ((float)sample / (float)(tile_total *samples_per_tile));

			if (*progress > 1.0f) *progress = 1.0f;
		}
	SESSION_FIND_END()
}

const char* cycles_progress_get_status(unsigned int client_id, unsigned int session_id)
{
	/* static here, since otherwise string goes out of scope on return. */
	static string status;
	status = "";
	SESSION_FIND(session_id)
		string substatus{ "" };
		session->progress.get_status(status, substatus);
		return status.c_str();
	SESSION_FIND_END()

	return nullptr;
}

const char* cycles_progress_get_substatus(unsigned int client_id, unsigned int session_id)
{
	/* static here, since otherwise string goes out of scope on return. */
	static string substatus;
	substatus = "";
	SESSION_FIND(session_id)
		string status{ "" };
		session->progress.get_status(status, substatus);
		return substatus.c_str();
	SESSION_FIND_END()

	return nullptr;
}