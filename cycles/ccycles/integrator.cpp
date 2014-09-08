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

// Integrator settings
void cycles_integrator_set_max_bounce(unsigned int client_id, unsigned int scene_id, int max_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->max_bounce = max_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_min_bounce(unsigned int client_id, unsigned int scene_id, int min_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->min_bounce = min_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_no_caustics(unsigned int client_id, unsigned int scene_id, bool no_caustics)
{
	SCENE_FIND(scene_id)
		sce->integrator->no_caustics = no_caustics;
	SCENE_FIND_END()
}

void cycles_integrator_set_transparent_shadows(unsigned int client_id, unsigned int scene_id, bool transparent_shadows)
{
	SCENE_FIND(scene_id)
		sce->integrator->transparent_shadows = transparent_shadows;
	SCENE_FIND_END()
}

void cycles_integrator_set_diffuse_samples(unsigned int client_id, unsigned int scene_id, int diffuse_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->diffuse_samples = diffuse_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_glossy_samples(unsigned int client_id, unsigned int scene_id, int glossy_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->glossy_samples = glossy_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_transmission_samples(unsigned int client_id, unsigned int scene_id, int transmission_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->transmission_samples = transmission_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_ao_samples(unsigned int client_id, unsigned int scene_id, int ao_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->ao_samples = ao_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_mesh_light_samples(unsigned int client_id, unsigned int scene_id, int mesh_light_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->mesh_light_samples = mesh_light_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_subsurface_samples(unsigned int client_id, unsigned int scene_id, int subsurface_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->subsurface_samples = subsurface_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_volume_samples(unsigned int client_id, unsigned int scene_id, int volume_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->volume_samples = volume_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_max_diffuse_bounce(unsigned int client_id, unsigned int scene_id, int max_diffuse_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->max_diffuse_bounce = max_diffuse_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_max_glossy_bounce(unsigned int client_id, unsigned int scene_id, int max_glossy_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->max_glossy_bounce = max_glossy_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_max_transmission_bounce(unsigned int client_id, unsigned int scene_id, int max_transmission_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->max_transmission_bounce = max_transmission_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_max_volume_bounce(unsigned int client_id, unsigned int scene_id, int max_volume_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->max_volume_bounce = max_volume_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_transparent_min_bounce(unsigned int client_id, unsigned int scene_id, int transparent_min_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->transparent_min_bounce = transparent_min_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_transparent_max_bounce(unsigned int client_id, unsigned int scene_id, int transparent_max_bounce)
{
	SCENE_FIND(scene_id)
		sce->integrator->transparent_max_bounce = transparent_max_bounce;
	SCENE_FIND_END()
}

void cycles_integrator_set_aa_samples(unsigned int client_id, unsigned int scene_id, int aa_samples)
{
	SCENE_FIND(scene_id)
		sce->integrator->aa_samples = aa_samples;
	SCENE_FIND_END()
}

void cycles_integrator_set_filter_glossy(unsigned int client_id, unsigned int scene_id, float filter_glossy)
{
	SCENE_FIND(scene_id)
		sce->integrator->filter_glossy = filter_glossy;
	SCENE_FIND_END()
}

void cycles_integrator_set_method(unsigned int client_id, unsigned int scene_id, int method)
{
	SCENE_FIND(scene_id)
		sce->integrator->method = (ccl::Integrator::Method)method;
	SCENE_FIND_END()
}
