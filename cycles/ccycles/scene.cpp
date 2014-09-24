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

// need access to devices
extern std::vector<ccl::DeviceInfo> devices;

extern std::vector<ccl::SceneParams> scene_params;
std::vector<CCScene> scenes;

/* implement CCScene methods*/

void CCScene::builtin_image_info(const string& builtin_name, void* builtin_data, bool& is_float, int& width, int& height, int& depth, int& channels)
{
	auto img = static_cast<CCImage*>(builtin_data);
	width = img->width;
	height = img->height;
	depth = img->depth;
	channels = img->channels;
	is_float = img->is_float;
}

bool CCScene::builtin_image_pixels(const string& builtin_name, void* builtin_data, unsigned char* pixels)
{
	auto img = static_cast<CCImage*>(builtin_data);
	memcpy(pixels, img->builtin_data, img->width*img->height*img->channels*sizeof(unsigned char));
	return false;
}

bool CCScene::builtin_image_float_pixels(const string& builtin_name, void* builtin_data, float* pixels)
{
	auto img = static_cast<CCImage*>(builtin_data);
	memcpy(pixels, img->builtin_data, img->width*img->height*img->channels*sizeof(float));
	return false;
}

/* *** */

void _cleanup_scenes()
{
	// clear out scene params vector
	scene_params.clear();

	// loop over scenes, free the ccl::Scenes before clearing out vector
	for (auto sce : scenes) {
		delete sce.scene;
	}

	scenes.clear();
}

unsigned int cycles_scene_create(unsigned int client_id, unsigned int scene_params_id, unsigned int device_id)
{
	CCScene scene;

	ccl::SceneParams params;
	ccl::DeviceInfo di;

	bool found_params = false;
	bool found_di = false;

	if (0 <= scene_params_id && scene_params_id < scene_params.size()) {
		params = scene_params[scene_params_id];
		found_params = true;
	}

	if (device_id >= 0 && device_id <= devices.size()) {
		di = devices[device_id];
		found_di = true;
	}

	if (found_di && found_params) {
		scene.scene = new ccl::Scene(params, di);

		scene.scene->image_manager->builtin_image_info_cb = function_bind(&CCScene::builtin_image_info, scene, _1, _2, _3, _4, _5, _6, _7);
		scene.scene->image_manager->builtin_image_pixels_cb = function_bind(&CCScene::builtin_image_pixels, scene, _1, _2, _3);
		scene.scene->image_manager->builtin_image_float_pixels_cb = function_bind(&CCScene::builtin_image_float_pixels, scene, _1, _2, _3);

		scenes.push_back(scene);

		logger.logit(client_id, "Created scene ", scenes.size() - 1, " with scene_params ", scene_params_id, " and device ", di.id);

		return (unsigned int)(scenes.size() - 1);
	}
	else {
		return UINT_MAX;
	}
}

void cycles_scene_set_default_surface_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		sce->default_surface = (int)shader_id;
		logger.logit(client_id, "Scene ", scene_id, " set default surface shader ", shader_id);
	SCENE_FIND_END()
}

unsigned int cycles_scene_get_default_surface_shader(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		return (unsigned int)(sce->default_surface);
	SCENE_FIND_END()

	return UINT_MAX;
}

