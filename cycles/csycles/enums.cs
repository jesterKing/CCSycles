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

using System;

namespace ccl
{
	public enum DeviceType : uint
	{
		None,
		CPU,
		OpenCL,
		CUDA,
		Network,
		Multi
	}

	public enum ShadingSystem : uint
	{
		OSL,
		SVM
	}

	public enum IntegratorMethod : int
	{
		BranchedPath = 0,
		Path
	}

	public enum SamplingPattern : uint
	{
		Sobol = 0,
		CMJ
	}

// NOTE: keep in sync with available Cycles nodes
	public enum ShaderNodeType: uint
	{
		Background = 0,
		Output, // automatic node, but here for completeness
		Diffuse,
		Anisotropic,
		Translucent,
		Transparent,
		Velvet,
		Toon,
		Glossy,
		Glass,
		Refraction,
		Hair,
		Emission,
		AmbientOcclusion,
		AbsorptionVolume,
		ScatterVolume,
		SubsurfaceScattering,
		Value,
		Color,
		MixClosure,
		AddClosure,
		Invert,
		Mix,
		Gamma,
		Wavelength,
		Blackbody,
		Camera,
		Fresnel,
		CombineXyz,
		SeparateXyz,
		Math,
		ImageTexture,
		EnvironmentTexture,
		BrickTexture,
		SkyTexture,
		CheckerTexture,
		NoiseTexture,
		WaveTexture,
		TextureCoordinate,
		Bump,
		RgbToBw,
		LightPath,
		LightFalloff,
		LayerWeight,
		VoronoiTexture,
		SeparateHsv,
		SeparateRgb,
        Mapping,
        CombineRgb
	}

	public enum BvhType : uint
	{
		Dynamic,
		Static
	}

	public enum TileOrder : uint
	{
		Center,
		RightToLeft,
		LeftToRight,
		TopToBottom,
		BottomToTop
	}

	public enum CameraType : uint
	{
		Perspective,
		Orthographic,
		Panorama
	}

	public enum PanoramaType : uint
	{
		Equirectangular,
		FisheyeEquidistant,
		FisheyeEquisolid
	}

	public enum LightType : uint
	{
		Point = 0,
		Sun, /* also Hemi */
		Background,
		Area,
		Spot,
		Triangle,
	}

	public enum InterpolationType : int {
		None = -1,
		Linear = 0,
		Closest = 1,
		Cubic = 2,
		Smart = 3,
	};

	[FlagsAttribute]
	public enum PathRay : int
	{
		PATH_RAY_CAMERA = 1,
		PATH_RAY_REFLECT = 2,
		PATH_RAY_TRANSMIT = 4,
		PATH_RAY_DIFFUSE = 8,
		PATH_RAY_GLOSSY = 16,
		PATH_RAY_SINGULAR = 32,
		PATH_RAY_TRANSPARENT = 64,

		PATH_RAY_SHADOW_OPAQUE = 128,
		PATH_RAY_SHADOW_TRANSPARENT = 256,
		PATH_RAY_SHADOW = (PATH_RAY_SHADOW_OPAQUE | PATH_RAY_SHADOW_TRANSPARENT),

		PATH_RAY_CURVE = 512, /* visibility flag to define curve segments*/

		/* note that these can use maximum 12 bits, the other are for layers */
		PATH_RAY_ALL_VISIBILITY = (1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512),

		PATH_RAY_MIS_SKIP = 1024,
		PATH_RAY_DIFFUSE_ANCESTOR = 2048,
		PATH_RAY_GLOSSY_ANCESTOR = 4096,
		PATH_RAY_BSSRDF_ANCESTOR = 8192,
		PATH_RAY_SINGLE_PASS_DONE = 16384,
		PATH_RAY_VOLUME_SCATTER = 32768,

		/* we need layer member flags to be the 20 upper bits */
		PATH_RAY_LAYER_SHIFT = (32 - 20)
	}
}
