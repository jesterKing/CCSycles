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

void cycles_camera_set_size(unsigned int client_id, unsigned int scene_id, unsigned int width, unsigned int height)
{
	SCENE_FIND(scene_id)
		sce->camera->width = width;
		sce->camera->height = height;
		// TODO: APIfy need_[device_]update
		sce->camera->need_update = true;
		sce->camera->need_device_update = true;
	SCENE_FIND_END()
}

unsigned int cycles_camera_get_width(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		return (unsigned int)sce->camera->width;
	SCENE_FIND_END()

	return 0;
}

unsigned int cycles_camera_get_height(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		return (unsigned int)sce->camera->height;
	SCENE_FIND_END()

	return 0;
}

void cycles_camera_set_type(unsigned int client_id, unsigned int scene_id, camera_type type)
{
	SCENE_FIND(scene_id)
		sce->camera->type = (ccl::CameraType)type;
	SCENE_FIND_END()
}

void cycles_camera_set_panorama_type(unsigned int client_id, unsigned int scene_id, panorama_type type)
{
	SCENE_FIND(scene_id)
		sce->camera->panorama_type = (ccl::PanoramaType)type;
	SCENE_FIND_END()
}

void cycles_camera_set_matrix(unsigned int client_id, unsigned int scene_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l,
	float m, float n, float o, float p
	)
{
	SCENE_FIND(scene_id)
		auto mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p);
		logger.logit(client_id, "Setting camera matrix in scene ", scene_id, " to\n",
			"\t[", a, ",", b, ",", c, ",", d, "\n",
			"\t ", e, ",", f, ",", g, ",", h, "\n",
			"\t ", i, ",", j, ",", k, ",", l, "\n",
			"\t ", m, ",", n, ",", o, ",", p, "]\n"
			);
		sce->camera->matrix = mat;
	SCENE_FIND_END()
}

void cycles_camera_compute_auto_viewplane(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Computing auto viewplane for scene ", scene_id); 
		sce->camera->compute_auto_viewplane();
	SCENE_FIND_END()
}

void cycles_camera_set_viewplane(unsigned int client_id, unsigned int scene_id, float left, float right, float top, float bottom)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Set viewplane for scene ", scene_id, " to ", left, ":", right, ":", top, ":", bottom); 
		sce->camera->viewplane.left = left;
		sce->camera->viewplane.right = right;
		sce->camera->viewplane.top = top;
		sce->camera->viewplane.bottom = bottom;
	SCENE_FIND_END()

}

void cycles_camera_update(unsigned int client_id, unsigned int scene_id)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Updating camera for scene ", scene_id); 
		sce->camera->need_update = true;
		sce->camera->update();
	SCENE_FIND_END()
}

void cycles_camera_set_fov(unsigned int client_id, unsigned int scene_id, float fov)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera fov to ", fov);
		sce->camera->fov = fov;
	SCENE_FIND_END()
}

void cycles_camera_set_sensor_width(unsigned int client_id, unsigned int scene_id, float sensor_width)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera sensor_width to ", sensor_width);
		sce->camera->sensorwidth = sensor_width;
	SCENE_FIND_END()
}

void cycles_camera_set_sensor_height(unsigned int client_id, unsigned int scene_id, float sensor_height)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera sensor_height to ", sensor_height);
		sce->camera->sensorheight = sensor_height;
	SCENE_FIND_END()
}

void cycles_camera_set_nearclip(unsigned int client_id, unsigned int scene_id, float nearclip)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera nearclip to ", nearclip);
		sce->camera->nearclip = nearclip;
	SCENE_FIND_END()
}

void cycles_camera_set_farclip(unsigned int client_id, unsigned int scene_id, float farclip)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera farclip to ", farclip);
		sce->camera->farclip = farclip;
	SCENE_FIND_END()
}

void cycles_camera_set_aperturesize(unsigned int client_id, unsigned int scene_id, float aperturesize)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera aperturesize to ", aperturesize);
		sce->camera->aperturesize = aperturesize;
	SCENE_FIND_END()
}

void cycles_camera_set_aperture_ratio(unsigned int client_id, unsigned int scene_id, float aperture_ratio)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera aperture_ratio to ", aperture_ratio);
		sce->camera->aperture_ratio = aperture_ratio;
	SCENE_FIND_END()
}

void cycles_camera_set_blades(unsigned int client_id, unsigned int scene_id, unsigned int blades)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera blades to ", blades);
		sce->camera->blades = blades;
	SCENE_FIND_END()
}

void cycles_camera_set_bladesrotation(unsigned int client_id, unsigned int scene_id, float bladesrotation)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera bladesrotation to ", bladesrotation);
		sce->camera->bladesrotation = bladesrotation;
	SCENE_FIND_END()
}

void cycles_camera_set_focaldistance(unsigned int client_id, unsigned int scene_id, float focaldistance)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera focaldistance to ", focaldistance);
		sce->camera->focaldistance = focaldistance;
	SCENE_FIND_END()
}

void cycles_camera_set_shuttertime(unsigned int client_id, unsigned int scene_id, float shuttertime)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera shuttertime to ", shuttertime);
		sce->camera->shuttertime = shuttertime;
	SCENE_FIND_END()
}

void cycles_camera_set_fisheye_fov(unsigned int client_id, unsigned int scene_id, float fisheye_fov)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera fisheye_fov to ", fisheye_fov);
		sce->camera->fisheye_fov = fisheye_fov;
	SCENE_FIND_END()
}

void cycles_camera_set_fisheye_lens(unsigned int client_id, unsigned int scene_id, float fisheye_lens)
{
	SCENE_FIND(scene_id)
		logger.logit(client_id, "Setting camera fisheye_lens to ", fisheye_lens);
		sce->camera->fisheye_lens = fisheye_lens;
	SCENE_FIND_END()
}
