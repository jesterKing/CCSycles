/**
Copyright 2014-2015 Robert McNeel and Associates

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

#include "util_path.h"
#include "ccycles.h"

extern void _cleanup_scenes();
extern void _cleanup_sessions();
extern void _cleanup_shaders();
extern void _init_shaders();

std::ostream& operator<<(std::ostream& out, shadernode_type const &snt) {
	out << (int)snt;
	return out;
}

/* Hold the device information found on the system after initialisation. */
std::vector<ccl::DeviceInfo> devices;

/* Hold the logger function that potentially gets registered by a client. */
LOGGER_FUNC_CB logger_func = nullptr;

/* Need to initialise only once :) */
bool initialised{ false };
Logger logger;

std::vector<LOGGER_FUNC_CB> loggers;

void _cleanup_loggers()
{
	for (int i = 0; i < loggers.size(); i++) {
		loggers[i] = nullptr;
	}

	loggers.clear();
}

void cycles_path_init(const char* path, const char* user_path)
{
	ccl::path_init(string(path), string(user_path));
}

void cycles_initialise()
{
	if (!initialised) {
		devices = ccl::Device::available_devices();
		_init_shaders();
	}
}

void cycles_shutdown()
{
	if (!initialised) {
		return;
	}

	logger_func = nullptr;

	_cleanup_loggers();

	_cleanup_scenes();
	_cleanup_sessions();
	_cleanup_shaders();
}

void cycles_log_to_stdout(int tostdout)
{
	logger.tostdout = tostdout == 1;
}

void cycles_set_logger(unsigned int client_id, LOGGER_FUNC_CB logger_func_)
{
	loggers[client_id] = logger_func_;
}

unsigned int cycles_new_client()
{
	unsigned int logfunc_count{ 0 };
	for(auto logfunc : loggers) {
		if (logfunc == nullptr)
		{
			return logfunc_count;
		}
		++logfunc_count;
	}
	loggers.push_back(nullptr);
	assert(loggers.size() == logfunc_count+1);
	return logfunc_count;
}

void cycles_release_client(unsigned int client_id)
{
	loggers[client_id] = nullptr;
}

void cycles_f4_add(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a + b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_sub(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a - b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_mul(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a * b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

void cycles_f4_div(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a / b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

static void _tfm_copy(const ccl::Transform& source, ccl::Transform& target) {
	target.x.x = source.x.x;
	target.x.y = source.x.y;
	target.x.z = source.x.z;
	target.x.w = source.x.w;

	target.y.x = source.y.x;
	target.y.y = source.y.y;
	target.y.z = source.y.z;
	target.y.w = source.y.w;

	target.z.x = source.z.x;
	target.z.y = source.z.y;
	target.z.z = source.z.z;
	target.z.w = source.z.w;

	target.w.x = source.w.x;
	target.w.y = source.w.y;
	target.w.z = source.w.z;
	target.w.w = source.w.w;
}

void cycles_tfm_inverse(const ccl::Transform& t, ccl::Transform& res) {
	ccl::Transform r = ccl::transform_inverse(t);

	_tfm_copy(t, res);
}

void cycles_tfm_rotate_around_axis(float angle, const ccl::float3& axis, ccl::Transform& res)
{
	ccl::Transform r = ccl::transform_rotate(angle, axis);

	_tfm_copy(r, res);
}

void cycles_tfm_lookat(const ccl::float3& position, const ccl::float3& look, const ccl::float3& up, ccl::Transform &res)
{
	ccl::Transform r = ccl::transform_identity();
	r[0][3] = position.x;
	r[1][3] = position.y;
	r[2][3] = position.z;
	r[3][3] = 1.0f;

	ccl::float3 dir = ccl::normalize(look - position);

	
	ccl::float3 right = ccl::cross(ccl::normalize(up), dir); //, ccl::normalize(up));
	ccl::float3 new_up = ccl::cross(dir, right); //right, dir);

	r[0][0] = right.x;
	r[1][0] = right.y;
	r[2][0] = right.z;
	r[3][0] = 0.0f;
	r[0][1] = new_up.x;
	r[1][1] = new_up.y;
	r[2][1] = new_up.z;
	r[3][1] = 0.0f;
	r[0][2] = dir.x;
	r[1][2] = dir.y;
	r[2][2] = dir.z;
	r[3][2] = 0.0f;

	//r = ccl::transform_inverse(r);

	_tfm_copy(r, res);
}
