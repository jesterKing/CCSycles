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

#define SCENE_PARAM_BOOL(scene_params_id, varname) \
	PARAM_BOOL(scene_params, scene_params_id, varname)

#define SCENE_PARAM_CAST(scene_params_id, typecast, varname) \
	PARAM_CAST(scene_params, scene_params_id, typecast, varname)

std::vector<ccl::SceneParams> scene_params;

/* Create scene parameters, to be used when creating a new scene. */
unsigned int cycles_scene_params_create(unsigned int client_id, 
	unsigned int shadingsystem, unsigned int bvh_type, 
	unsigned int use_bvh_cache, unsigned int use_bvh_spatial_split, 
	unsigned int use_qbvh, unsigned int persistent_data)
{
	ccl::SceneParams params;

	params.shadingsystem = (ccl::ShadingSystem)shadingsystem;
	params.bvh_type = (ccl::SceneParams::BVHType)bvh_type;
	params.use_bvh_cache = use_bvh_cache == 1;
	params.use_bvh_spatial_split = use_bvh_spatial_split == 1;
	params.use_qbvh = use_qbvh == 1;
	params.persistent_data = persistent_data == 1;

	scene_params.push_back(params);

	logger.logit(client_id, "Created scene parameters ", scene_params.size() - 1, "\n\tshading system: ", params.shadingsystem, "\n\tbvh_type: ", params.bvh_type, "\n\tuse_bvh_cache: ", params.use_bvh_cache, "\n\tuse_bvh_spatial_split: ", params.use_bvh_spatial_split, "\n\tuse_qbvh: ", params.use_qbvh, "\n\tpersistent data: ", params.persistent_data);

	return (unsigned int)(scene_params.size() - 1);
}

/* Set scene parameters*/
void cycles_scene_params_set_bvh_type(unsigned int client_id, unsigned int scene_params_id, unsigned int bvh_type)
{
	SCENE_PARAM_CAST(scene_params_id, ccl::SceneParams::BVHType, bvh_type)
}

void cycles_scene_params_set_bvh_cache(unsigned int client_id, unsigned int scene_params_id, unsigned int use_bvh_cache)
{
	SCENE_PARAM_BOOL(scene_params_id, use_bvh_cache)
}

void cycles_scene_params_set_bvh_spatial_split(unsigned int client_id, unsigned int scene_params_id, unsigned int use_bvh_spatial_split)
{
	SCENE_PARAM_BOOL(scene_params_id, use_bvh_spatial_split)
}
void cycles_scene_params_set_qbvh(unsigned int client_id, unsigned int scene_params_id, unsigned int use_qbvh)
{
	SCENE_PARAM_BOOL(scene_params_id, use_qbvh)
}

void cycles_scene_params_set_shadingsystem(unsigned int client_id, unsigned int scene_params_id, unsigned int shadingsystem)
{
	SCENE_PARAM_CAST(scene_params_id, ccl::ShadingSystem, shadingsystem)
}
void cycles_scene_params_set_persistent_data(unsigned int client_id, unsigned int scene_params_id, unsigned int persistent_data)
{
	SCENE_PARAM_BOOL(scene_params_id, persistent_data)
}