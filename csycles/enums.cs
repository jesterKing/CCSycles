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
	/// <summary>
	/// Device types that Cycles can support.
	/// 
	/// Note that currently focus is on CPU and CUDA
	/// and CUDA MULTI support, but others can be
	/// added when needed and possible
	/// </summary>
	public enum DeviceType : uint
	{
		None,
		CPU,
		OpenCL,
		CUDA,
		Network,
		Multi
	}

	/// <summary>
	/// Shading systems available in Cycles.
	/// 
	/// Note that currently only SVM is supported
	/// in C[CS]?ycles
	/// </summary>
	public enum ShadingSystem : uint
	{
		OSL,
		SVM
	}

	/// <summary>
	/// Integration method used for ray casting.
	/// </summary>
	public enum IntegratorMethod : int
	{
		/// <summary>
		/// On each hit rays get split up for all possible equivalents
		/// </summary>
		BranchedPath = 0,
		Path
	}

	/// <summary>
	/// Sampling patterns available in
	/// Cycles.
	/// </summary>
	public enum SamplingPattern : uint
	{
		Sobol = 0,
		CMJ
	}

	/// <summary>
	/// Available Cycles shader nodes.
	/// @note keep in sync with available Cycles nodes
	/// </summary>
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
		RgbToLuminance,
		LightPath,
		LightFalloff,
		LayerWeight,
		VoronoiTexture,
		CombineXyz,
		SeparateXyz,
		SeparateHsv,
		SeparateRgb,
		CombineHsv,
		CombineRgb,
		Mapping,
		Holdout,
		HueSaturation,
		BrightnessContrast,
		GradientTexture,
		ColorRamp,
		VectorMath
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

	public enum FilterType : uint
	{
		Box = 0,
		Gaussian
	}

	public enum LightType : uint
	{
		Point = 0,
		Distant,
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
	}

	[FlagsAttribute]
	public enum PathRay : uint
	{
		Hidden = 0,
		Camera = 1,
		Reflect = 2,
		Transmit = 4,
		Diffuse = 8,
		Glossy = 16,
		Singular = 32,
		Transparent = 64,

		ShadowOpaque = 128,
		ShadowTransparent = 256,
		Shadow = (ShadowOpaque | ShadowTransparent),

		Curve = 512, /* visibility flag to define curve segments*/
		VolumeScatter = 1024,

		/* note that these can use maximum 12 bits, the other are for layers */

		AllVisibility =
			(Camera | Reflect | Transmit | Diffuse | Glossy | Singular | Transparent | Shadow | Curve | VolumeScatter),

		MisSkip = 2048,
		DiffuseAncestor = 4096,
		GlossyAncestor = 8192,
		BssrdfAncestor = 16384,
		SinglePassDone = 32768,

		/* we need layer member flags to be the 20 upper bits */
		LayerShift = (32 - 20)
	}
}
