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

/* Set shader_id as default background shader for scene_id.
 * Note that shader_id is the ID for the shader specific to this scene.
 * 
 * The correct ID can be found with cycles_scene_shader_id. The ID is also
 * returned from cycles_scene_add_shader.
 */
void cycles_scene_set_background_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		sce->default_background = shader_id;
		sce->background->shader = shader_id;
		sce->background->tag_update(sce);
		logger.logit(client_id, "Scene ", scene_id, " set background shader ", shader_id);
	SCENE_FIND_END()
}

void cycles_scene_set_background_ao_factor(unsigned int client_id, unsigned int scene_id, float ao_factor)
{
	SCENE_FIND(scene_id)
		sce->background->ao_factor = ao_factor;
		sce->background->tag_update(sce);
		logger.logit(client_id, "Scene ", scene_id, " set background ao factor ", ao_factor);
	SCENE_FIND_END()
}

void cycles_scene_set_background_ao_distance(unsigned int client_id, unsigned int scene_id, float ao_distance)
{
	SCENE_FIND(scene_id)
		sce->background->ao_distance = ao_distance;
		sce->background->tag_update(sce);
		logger.logit(client_id, "Scene ", scene_id, " set background ao distance ", ao_distance);
	SCENE_FIND_END()
}

void cycles_scene_set_background_visibility(unsigned int client_id, unsigned int scene_id, int path_ray_flag)
{
	SCENE_FIND(scene_id)
		sce->background->visibility = (ccl::PathRayFlag)path_ray_flag;
		sce->background->tag_update(sce);
		logger.logit(client_id, "Scene ", scene_id, " set background path ray visibility ", path_ray_flag);
	SCENE_FIND_END()
}