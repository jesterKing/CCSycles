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

unsigned int cycles_scene_add_mesh(unsigned int client_id, unsigned int scene_id, unsigned int object_id, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		auto mesh = new ccl::Mesh();
		
		auto ob = sce->objects[object_id];
		ob->mesh = mesh;

		mesh->used_shaders.push_back(shader_id);
		sce->meshes.push_back(mesh);

		logger.logit(client_id, "Add mesh ", sce->meshes.size() - 1, " to object ", object_id, " in scene ", scene_id, " using default surface shader ", shader_id);

		return (unsigned int)(sce->meshes.size() - 1);
	SCENE_FIND_END()

	return UINT_MAX;
}

/*
 * TODO: ponder if this should be in public API as well.
 */
void cycles_mesh_set_shader(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		auto me = sce->meshes[mesh_id];

		auto it = me->used_shaders.begin();
		auto end = me->used_shaders.end();
		auto found = false;

		while (it != end) {
			if (*it == shader_id) break;
			++it;
		}

		if (it == end) me->used_shaders.push_back(shader_id);

	SCENE_FIND_END()
}

void cycles_mesh_set_verts(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, float *verts, unsigned int vcount)
{
	SCENE_FIND(scene_id)
		auto me = sce->meshes[mesh_id];

		ccl::float3 f3;

		for (auto i = 0; i < (int)vcount*3; i+=3) {
			f3.x = verts[i];
			f3.y = verts[i+1];
			f3.z = verts[i+2];
			logger.logit(client_id, "v: ", f3.x, ",", f3.y, ",", f3.z);
			me->verts.push_back(f3);
		}
	SCENE_FIND_END()
}

// TODO: add to API: set smooth

void cycles_mesh_set_tris(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, int *faces, unsigned int fcount, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		auto me = sce->meshes[mesh_id];

		cycles_mesh_set_shader(client_id, scene_id, mesh_id, shader_id);

		for (auto i = 0; i < (int)fcount*3; i += 3) {
			logger.logit(client_id, "f: ", faces[i], ",", faces[i + 1], ",", faces[i + 2]);
			me->add_triangle(faces[i], faces[i + 1], faces[i + 2], shader_id, false);
		}
		
		// TODO: APIfy next call, right now keep here to be closer to PoC plugin
		me->attributes.remove(ccl::ATTR_STD_VERTEX_NORMAL);
	SCENE_FIND_END()
}

void cycles_mesh_add_triangle(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, unsigned int v0, unsigned int v1, unsigned int v2, unsigned int shader_id, unsigned int smooth)
{
	SCENE_FIND(scene_id)
		auto me = sce->meshes[mesh_id];
		me->add_triangle((int)v0, (int)v1, (int)v2, shader_id, smooth == 1);
	SCENE_FIND_END()
}

void cycles_mesh_set_uvs(unsigned int client_id, unsigned int scene_id, unsigned int mesh_id, float *uvs, unsigned int uvcount)
{
	SCENE_FIND(scene_id)
		auto me = sce->meshes[mesh_id];

	if (me->need_attribute(sce, ccl::ATTR_STD_UV)) {
		auto attr = me->attributes.add(ccl::ATTR_STD_UV, ccl::ustring("uvmap"));
		auto fdata = attr->data_float3();

		ccl::float3 f3;

		for (auto i = 0, j = 0; i < (int)uvcount * 2; i += 2, j++) {
			f3.x = uvs[i];
			f3.y = uvs[i + 1];
			f3.z = 0.0f;
			fdata[j] = f3;
		}
	}
	SCENE_FIND_END()
}