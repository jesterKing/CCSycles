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

std::vector<CCShader*> shaders;

std::vector<ccl::ImageManager::Image*> images;

void _cleanup_shaders()
{
	for (auto sh : shaders) {
		// just setting to nullptr, as scene disposal frees this memory.
		sh->graph = nullptr;
		sh->shader = nullptr;
		sh->scene_mapping.clear();
		delete sh;
	}
	shaders.clear();
}

void _cleanup_images()
{
	for (auto img : images) {
		if (img == nullptr) continue;

		delete [] img->builtin_data;
		img->builtin_data = nullptr;

		delete img;
	}
	images.clear();
}

/* Create a new shader.
 TODO: name for shader
*/
unsigned int cycles_create_shader(unsigned int client_id)
{
	CCShader* sh = new CCShader();
	sh->shader->graph = sh->graph;
	shaders.push_back(sh);

	return (unsigned int)(shaders.size() - 1);
}

/* Add shader to specified scene. */
unsigned int cycles_scene_add_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id)
{
	SCENE_FIND(scene_id)
		auto sh = shaders[shader_id];
		sh->shader->tag_update(sce);
		sce->shaders.push_back(sh->shader);
		auto shid = (unsigned int)(sce->shaders.size() - 1);
		sh->scene_mapping.insert({ scene_id, shid });
		return shid;
	SCENE_FIND_END()

	return (unsigned int)(-1);
}

/* Get Cycles shader ID in specific scene. */
unsigned int cycles_scene_shader_id(unsigned int client_id, unsigned int scene_id, unsigned int shader_id)
{
	auto sh = shaders[shader_id];
	if (sh->scene_mapping.find(scene_id) != sh->scene_mapping.end()) {
		return sh->scene_mapping[scene_id];
	}

	return (unsigned int)(-1);
}

void cycles_shader_set_name(unsigned int client_id, unsigned int shader_id, const char* name)
{
	SHADER_SET(shader_id, string, name, name);
}

void cycles_shader_set_use_mis(unsigned int client_id, unsigned int shader_id, unsigned int use_mis)
{
	SHADER_SET(shader_id, bool, use_mis, use_mis == 1)
}

void cycles_shader_set_use_transparent_shadow(unsigned int client_id, unsigned int shader_id, unsigned int use_transparent_shadow)
{
	SHADER_SET(shader_id, bool, use_transparent_shadow, use_transparent_shadow == 1)
}

void cycles_shader_set_heterogeneous_volume(unsigned int client_id, unsigned int shader_id, unsigned int heterogeneous_volume)
{
	SHADER_SET(shader_id, bool, heterogeneous_volume, heterogeneous_volume == 1)
}

unsigned int cycles_add_shader_node(unsigned int client_id, unsigned int shader_id, shadernode_type shn_type)
{
	ccl::ShaderNode* node = nullptr;
	switch (shn_type) {
		case shadernode_type::BACKGROUND:
			node = new ccl::BackgroundNode();
			break;
		case shadernode_type::DIFFUSE:
			node = new ccl::DiffuseBsdfNode();
			break;
		case shadernode_type::ANISOTROPIC:
			node = new ccl::AnisotropicBsdfNode();
			break;
		case shadernode_type::TRANSLUCENT:
			node = new ccl::TranslucentBsdfNode();
			break;
		case shadernode_type::TRANSPARENT:
			node = new ccl::TransparentBsdfNode();
			break;
		case shadernode_type::VELVET:
			node = new ccl::VelvetBsdfNode();
			break;
		case shadernode_type::TOON:
			node = new ccl::ToonBsdfNode();
			break;
		case shadernode_type::GLOSSY:
			node = new ccl::GlossyBsdfNode();
			break;
		case shadernode_type::GLASS:
			node = new ccl::GlassBsdfNode();
			break;
		case shadernode_type::REFRACTION:
		{
			auto refrnode = new ccl::RefractionBsdfNode();
			refrnode->distribution = OpenImageIO::v1_3::ustring("Beckmann");
			node = dynamic_cast<ccl::ShaderNode *>(refrnode);
		}
			break;
		case shadernode_type::HAIR:
			node = new ccl::HairBsdfNode();
			break;
		case shadernode_type::EMISSION:
			node = new ccl::EmissionNode();
			break;
		case shadernode_type::AMBIENT_OCCLUSION:
			node = new ccl::AmbientOcclusionNode();
			break;
		case shadernode_type::ABSORPTION_VOLUME:
			node = new ccl::AbsorptionVolumeNode();
			break;
		case shadernode_type::SCATTER_VOLUME:
			node = new ccl::ScatterVolumeNode();
			break;
		case shadernode_type::SUBSURFACE_SCATTERING:
			node = new ccl::SubsurfaceScatteringNode();
			break;
		case shadernode_type::VALUE:
			node = new ccl::ValueNode();
			break;
		case shadernode_type::COLOR:
			node = new ccl::ColorNode();
			break;
		case shadernode_type::MIX_CLOSURE:
			node = new ccl::MixClosureNode();
			break;
		case shadernode_type::ADD_CLOSURE:
			node = new ccl::AddClosureNode();
			break;
		case shadernode_type::INVERT:
			node = new ccl::InvertNode();
			break;
		case shadernode_type::MIX:
			node = new ccl::MixNode();
			break;
		case shadernode_type::GAMMA:
			node = new ccl::GammaNode();
			break;
		case shadernode_type::WAVELENGTH:
			node = new ccl::WavelengthNode();
			break;
		case shadernode_type::BLACKBODY:
			node = new ccl::BlackbodyNode();
			break;
		case shadernode_type::CAMERA:
			node = new ccl::CameraNode();
			break;
		case shadernode_type::FRESNEL:
			node = new ccl::FresnelNode();
			break;
		case shadernode_type::COMBINE_XYZ:
			node = new ccl::CombineXYZNode();
			break;
		case shadernode_type::SEPARATE_XYZ:
			node = new ccl::SeparateXYZNode();
			break;
		case shadernode_type::MATH:
			node = new ccl::MathNode();
			break;
		case shadernode_type::IMAGE_TEXTURE:
			node = new ccl::ImageTextureNode();
			break;
		case shadernode_type::BRICK_TEXTURE:
			node = new ccl::BrickTextureNode();
			break;
		case shadernode_type::SKY_TEXTURE:
			node = new ccl::SkyTextureNode();
			break;
		case shadernode_type::CHECKER_TEXTURE:
			node = new ccl::CheckerTextureNode();
			break;
		case shadernode_type::NOISE_TEXTURE:
			node = new ccl::NoiseTextureNode();
			break;
		case shadernode_type::WAVE_TEXTURE:
			node = new ccl::WaveTextureNode();
			break;
		case shadernode_type::TEXTURE_COORDINATE:
			node = new ccl::TextureCoordinateNode();
			break;
	}

	if (node) {
		shaders[shader_id]->graph->add(node);
		return (unsigned int)(node->id);
	}
	else {
		return (unsigned int)-1;
	}
}

enum class attr_type {
	INT,
	FLOAT,
	FLOAT4,
	CHARP
};

struct attrunion {
	attr_type type;
	union {
		int i;
		float f;
		ccl::float4 f4;
		const char* cp;
	};
};

void shadernode_set_attribute(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, attrunion v)
{
	auto attr = string(attribute_name);
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			for (auto* inp : (*psh)->inputs) {
				auto inpname = string(inp->name);
				if (ccl::string_iequals(inpname, attribute_name)) {
					switch (v.type) {
					case attr_type::INT:
						inp->value.x = (float)v.i;
						logger.logit(client_id, "shader_id: ", shader_id, " -> shnode_id: ", shnode_id, " |> setting attribute: ", attribute_name, " to: ", v.i);
						break;
					case attr_type::FLOAT:
						inp->value.x = v.f;
						logger.logit(client_id, "shader_id: ", shader_id, " -> shnode_id: ", shnode_id, " |> setting attribute: ", attribute_name, " to: ", v.f);
						break;
					case attr_type::FLOAT4:
						inp->value.x = v.f4.x;
						inp->value.y = v.f4.y;
						inp->value.z = v.f4.z;
						logger.logit(client_id, "shader_id: ", shader_id, " -> shnode_id: ", shnode_id, " |> setting attribute: ", attribute_name, " to: ", v.f4.x, ",", v.f4.y, ",", v.f4.z);
						break;
					case attr_type::CHARP:
						inp->value_string = string(v.cp);
						logger.logit(client_id, "shader_id: ", shader_id, " -> shnode_id: ", shnode_id, " |> setting attribute: ", attribute_name, " to: ", v.cp);
						break;
					}
					return;
				}
			}
			break;
		}
		++psh;
	}
}

void _set_enum_val(unsigned int client_id, OpenImageIO::v1_3::ustring* str, ccl::ShaderEnum& enm, const OpenImageIO::v1_3::ustring val)
{
	if (enm.exists(val)) {
		*str = val;
	}
	else {
		logger.logit(client_id, "Unknown value ", val);
	}
}

/* TODO: add all enum possibilities.
 * Note that this particular API function won't deal yet very well with
 * nodes that have multiple enums.
 */
void cycles_shadernode_set_enum(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* value)
{
	auto val = OpenImageIO::v1_3::ustring(value);

	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::MATH:
					{
						auto node = dynamic_cast<ccl::MathNode*>(*psh);
						_set_enum_val(client_id, &node->type, ccl::MathNode::type_enum, val);
					}
					break;
				case shadernode_type::MIX:
					{
						auto node = dynamic_cast<ccl::MixNode*>(*psh);
						_set_enum_val(client_id, &node->type, ccl::MixNode::type_enum, val);
					}
					break;
				case shadernode_type::REFRACTION:
					{
						auto node = dynamic_cast<ccl::RefractionBsdfNode*>(*psh);
						_set_enum_val(client_id, &node->distribution, ccl::RefractionBsdfNode::distribution_enum, val);
					}
					break;
				case shadernode_type::GLOSSY:
					{
						auto node = dynamic_cast<ccl::GlossyBsdfNode*>(*psh);
						_set_enum_val(client_id, &node->distribution, ccl::GlossyBsdfNode::distribution_enum, val);
					}
					break;
				case shadernode_type::GLASS:
					{
						auto node = dynamic_cast<ccl::GlassBsdfNode*>(*psh);
						_set_enum_val(client_id, &node->distribution, ccl::GlassBsdfNode::distribution_enum, val);
					}
					break;
				case shadernode_type::WAVE_TEXTURE:
					{
						auto node = dynamic_cast<ccl::WaveTextureNode*>(*psh);
						_set_enum_val(client_id, &node->type, ccl::WaveTextureNode::type_enum, val);
					}
					break;
				case shadernode_type::SKY_TEXTURE:
					{
						auto node = dynamic_cast<ccl::SkyTextureNode*>(*psh);
						_set_enum_val(client_id, &node->type, ccl::SkyTextureNode::type_enum, val);
					}
					break;
			}
			break;
		}
		++psh;
	}
}

void cycles_shadernode_set_member_float_img(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, float* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	auto mname = string(member_name);
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::IMAGE_TEXTURE:
					{
						auto nimg = new ccl::ImageManager::Image();
						auto imgdata = new float[width*height*channels];
						memcpy(imgdata, img, sizeof(float)*width*height*channels);
						nimg->builtin_data = imgdata;
						nimg->filename = string(img_name);
						nimg->width = (int)width;
						nimg->height = (int)height;
						nimg->depth = (int)depth;
						nimg->channels = (int)channels;
						nimg->is_float = true;
						images.push_back(nimg);
						auto imtex = dynamic_cast<ccl::ImageTextureNode*>(*psh);
						imtex->builtin_data = nimg;
						imtex->is_float = true;
						imtex->is_linear = true;
						imtex->interpolation = ccl::InterpolationType::INTERPOLATION_LINEAR;
					}	
					break;
			}
		}
		++psh;
	}
}
void cycles_shadernode_set_member_byte_img(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, unsigned char* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	auto mname = string(member_name);
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::IMAGE_TEXTURE:
					{
						auto nimg = new ccl::ImageManager::Image();
						auto imgdata = new ccl::uchar[width*height*channels];
						memcpy(imgdata, img, sizeof(unsigned char)*width*height*channels);
						nimg->builtin_data = imgdata;
						nimg->filename = string(img_name);
						nimg->width = (int)width;
						nimg->height = (int)height;
						nimg->depth = (int)depth;
						nimg->channels = (int)channels;
						nimg->is_float = false;
						images.push_back(nimg);
						auto imtex = dynamic_cast<ccl::ImageTextureNode*>(*psh);
						imtex->builtin_data = nimg;
						imtex->is_float = false;
						imtex->is_linear = true;
						imtex->interpolation = ccl::InterpolationType::INTERPOLATION_LINEAR;
					}
					break;
			}
		}
		++psh;
	}
}

void cycles_shadernode_set_member_bool(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, bool value)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	auto mname = string(member_name);
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::MATH:
					{
						auto mnode = dynamic_cast<ccl::MathNode*>(*psh);
						mnode->use_clamp = value;
					}
					break;
				case shadernode_type::IMAGE_TEXTURE:
					{
						auto imtexnode = dynamic_cast<ccl::ImageTextureNode*>(*psh);
						if (mname == "is_linear") {
							imtexnode->is_linear = value;
						}
						else if (mname == "if_float") {
							imtexnode->is_float = value;
						}
					}
					break;
			}
		}
	++psh;
	}

}

void cycles_shadernode_set_member_int(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, int value)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	auto mname = string(member_name);
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::BRICK_TEXTURE:
					{
						auto bricknode = dynamic_cast<ccl::BrickTextureNode*>(*psh);
						if (mname == "offset_frequency")
							bricknode->offset_frequency = value;
						else if (mname == "squash_frequency")
							bricknode->squash_frequency = value;
					}
					break;
			}
		}
		++psh;
	}
}


void cycles_shadernode_set_member_float(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, float value)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	auto mname = string(member_name);
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
				case shadernode_type::VALUE:
					{
						auto valuenode = dynamic_cast<ccl::ValueNode*>(*psh);
						valuenode->value = value;
					}
					break;
				case shadernode_type::IMAGE_TEXTURE:
					{
						auto imtexnode = dynamic_cast<ccl::ImageTextureNode*>(*psh);
						if (mname == "projection_blend") {
							imtexnode->projection_blend = value;
						}
					}
					break;
				case shadernode_type::BRICK_TEXTURE:
					{
						auto bricknode = dynamic_cast<ccl::BrickTextureNode*>(*psh);
						if (mname == "offset")
							bricknode->offset = value;
						else if (mname == "squash")
							bricknode->squash = value;
					}
					break;
				case shadernode_type::SKY_TEXTURE:
					{
						auto skynode = dynamic_cast<ccl::SkyTextureNode*>(*psh);
						if (mname == "turbidity")
							skynode->turbidity = value;
						else if (mname == "ground_albedo")
							skynode->ground_albedo = value;
					}
					break;
			}
		}
		++psh;
	}
}

void cycles_shadernode_set_member_vec(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, float x, float y, float z)
{
	auto& sh = shaders[shader_id];
	auto psh = sh->graph->nodes.begin();
	while (psh != sh->graph->nodes.end())
	{
		if ((*psh)->id == shnode_id) {
			switch (shn_type) {
			case shadernode_type::COLOR:
					{
						auto colnode = dynamic_cast<ccl::ColorNode*>(*psh);
						colnode->value.x = x;
						colnode->value.y = y;
						colnode->value.z = z;
					}
					break;
			case shadernode_type::SKY_TEXTURE:
					{
						auto sunnode = dynamic_cast<ccl::SkyTextureNode*>(*psh);
						sunnode->sun_direction.x = x;
						sunnode->sun_direction.y = y;
						sunnode->sun_direction.z = z;
					}
					break;
			}
		}
		++psh;
	}
}

/*
Set an integer attribute with given name to value. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_int(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, int value)
{
	attrunion v;
	v.type = attr_type::INT;
	v.i = value;
	shadernode_set_attribute(client_id, shader_id, shnode_id, attribute_name, v);
}

/*
Set a float attribute with given name to value. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_float(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, float value)
{
	attrunion v;
	v.type = attr_type::FLOAT;
	v.f = value;
	shadernode_set_attribute(client_id, shader_id, shnode_id, attribute_name, v);
}

/*
Set a vector of floats attribute with given name to x, y and z. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_vec(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, float x, float y, float z)
{
	attrunion v;
	v.type = attr_type::FLOAT4;
	v.f4.x = x;
	v.f4.y = y;
	v.f4.z = z;
	shadernode_set_attribute(client_id, shader_id, shnode_id, attribute_name, v);
}

/*
Set a string attribute with given name to value. shader_id is the global shader ID.
*/
void cycles_shadernode_set_attribute_string(unsigned int client_id, unsigned int shader_id, unsigned int shnode_id, const char* attribute_name, const char* value)
{
	attrunion v;
	v.type = attr_type::CHARP;
	v.cp = value;
	shadernode_set_attribute(client_id, shader_id, shnode_id, attribute_name, v);
}

void cycles_shader_connect_nodes(unsigned int client_id, unsigned int shader_id, unsigned int from_id, const char* from, unsigned int to_id, const char* to)
{
	auto& sh = shaders[shader_id];
	auto shfrom = sh->graph->nodes.begin();
	auto shfrom_end = sh->graph->nodes.end();
	auto shto = sh->graph->nodes.begin();
	auto shto_end = sh->graph->nodes.end();

	while (shfrom != shfrom_end) {
		if ((*shfrom)->id == from_id) {
			break;
		}
		++shfrom;
	}

	while (shto != shto_end) {
		if ((*shto)->id == to_id) {
			break;
		}
		++shto;
	}

	if (shfrom == shfrom_end || shto == shto_end) {
		return; // TODO: figure out what to do on errors like this
	}
	logger.logit(client_id, "Shader ", shader_id, " :: ", from_id, ":", from, " -> ", to_id, ":", to);

	sh->graph->connect((*shfrom)->output(from), (*shto)->input(to));
}

