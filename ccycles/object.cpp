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

unsigned int cycles_scene_add_object(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = new ccl::Object();
		// TODO: APIfy object matrix setting, for now hard-code to be closer to PoC plugin
		ob->tfm = ccl::transform_identity();
		sce->objects.push_back(ob);

		logger.logit(client_id, "Added object ", sce->objects.size() - 1, " to scene ", scene_id);

		ob->tag_update(sce);

		return (unsigned int)(sce->objects.size() - 1);
	SCENE_FIND_END()

	return UINT_MAX;
}

void cycles_scene_object_set_mesh(unsigned int client_id, unsigned int scene_id, unsigned int object_id, unsigned int mesh_id)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = sce->objects[object_id];
		ccl::Mesh* me = sce->meshes[mesh_id];
		ob->mesh = me;
		ob->tag_update(sce);
	SCENE_FIND_END()
}

void cycles_object_tag_update(unsigned int client_id, unsigned int scene_id, unsigned int object_id)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = sce->objects[object_id];
		ob->tag_update(sce);
	SCENE_FIND_END()
}

unsigned int cycles_scene_object_get_mesh(unsigned int client_id, unsigned int scene_id, unsigned int object_id)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = sce->objects[object_id];
		auto cmeshit = sce->meshes.begin();
		auto cmeshend = sce->meshes.end();
		unsigned int i = 0;
		while (cmeshit != cmeshend) {
			if ((*cmeshit) == ob->mesh) {
				return i;
			}
			++i;
		}
	SCENE_FIND_END()
	return UINT_MAX;
}

void cycles_scene_object_set_visibility(unsigned int client, unsigned int scene_id, unsigned int object_id, unsigned int visibility)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = sce->objects[object_id];
		ob->visibility = visibility;
		ob->tag_update(sce);
	SCENE_FIND_END()
}

void cycles_scene_object_set_matrix(unsigned int client_id, unsigned int scene_id, unsigned int object_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l,
	float m, float n, float o, float p
	)
{
	SCENE_FIND(scene_id)
		ccl::Object* ob = sce->objects[object_id];
		ccl::Transform mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p);
		ob->tfm = mat;
		ob->tag_update(sce);
	SCENE_FIND_END()
}
