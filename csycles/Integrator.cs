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

namespace ccl
{
	/// <summary>
	/// Access to scene Integrator settings for Cycles
	/// </summary>
	public class Integrator
	{
		/// <summary>
		/// Reference to scene to which these integrator settings belong.
		/// </summary>
		internal Scene Scene { get; set; }

		/// <summary>
		/// Create a new integrator settings representation
		/// </summary>
		/// <param name="scene"></param>
		internal Integrator(Scene scene)
		{
			Scene = scene;
		}

		/// <summary>
		/// Tag the integrator as changed.
		/// </summary>
		public void TagForUpdate()
		{
			CSycles.integrator_tag_update(Scene.Client.Id, Scene.Id);
		}

		/// <summary>
		/// Set the maximum amount of bounces for a ray.
		/// </summary>
		public int MaxBounce
		{
			set
			{
				CSycles.integrator_set_max_bounce(Scene.Client.Id, Scene.Id, value);
				
			}
		}

		/// <summary>
		/// Set the minimum amount of bounces for a ray.
		/// </summary>
		public int MinBounce
		{
			set
			{
				CSycles.integrator_set_min_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the minimum amount of bounces for a transparency ray.
		/// </summary>
		public int TransparentMinBounce
		{
			set
			{
				CSycles.integrator_set_transparent_min_bounce (Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a transparency ray.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int TransparentMaxBounce
		{
			set
			{
				CSycles.integrator_set_transparent_max_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a diffuse ray.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxDiffuseBounce
		{
			set
			{
				CSycles.integrator_set_max_diffuse_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a glossy ray.
		/// </summary>
		public int MaxGlossyBounce
		{
			set
			{
				CSycles.integrator_set_max_glossy_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximimum amount of bounces for transmission rays.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxTransmissionBounce
		{
			set
			{
				CSycles.integrator_set_max_transmission_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for volume rays.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxVolumeBounce
		{
			set
			{
				CSycles.integrator_set_max_volume_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set to true if no caustics should be used
		/// </summary>
		public bool NoCaustics
		{
			set
			{
				CSycles.integrator_set_no_caustics(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set to true if transparent shadows should be traced.
		/// </summary>
		public bool TransparentShadows
		{
			set
			{
				CSycles.integrator_set_transparent_shadows(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples for diffuse rays to take.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int DiffuseSamples
		{
			set
			{
				CSycles.integrator_set_diffuse_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples for glossy rays to take.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int GlossySamples
		{
			set
			{
				CSycles.integrator_set_glossy_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples for transmission rays to take.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int TransmissionSamples
		{
			set
			{
				CSycles.integrator_set_transmission_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples for AO rays to take.
		/// 
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int AoSamples
		{
			set
			{
				CSycles.integrator_set_ao_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the sample count for mesh lights
		/// </summary>
		public int MeshLightSamples
		{
			set
			{
				CSycles.integrator_set_mesh_light_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the sample count for subsurface
		/// </summary>
		public int SubsurfaceSamples
		{
			set
			{
				CSycles.integrator_set_subsurface_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of volume samples
		/// </summary>
		public int VolumeSamples
		{
			set
			{
				CSycles.integrator_set_volume_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of AA samples per ray
		/// </summary>
		public int AaSamples
		{
			set
			{
				CSycles.integrator_set_aa_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the glossy filter size
		/// </summary>
		public float FilterGlossy
		{
			set
			{
				CSycles.integrator_set_filter_glossy(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the integration method to use (Path or BranchedPath)
		/// </summary>
		public IntegratorMethod IntegratorMethod
		{
			set
			{
				CSycles.integrator_set_method(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set to true if all direct lights should be sampled.
		/// </summary>
		public bool SampleAllLightsDirect
		{
			set
			{
				CSycles.integrator_set_sample_all_lights_direct(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set to true if all indirect lights should be sampled.
		/// </summary>
		public bool SampleAllLightsIndirect
		{
			set
			{
				CSycles.integrator_set_sample_all_lights_indirect(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the step size for volume tracing
		/// </summary>
		public float VolumeStepSize
		{
			set
			{
				CSycles.integrator_set_volume_step_size(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of steps for volume tracing.
		/// </summary>
		public int VolumeMaxSteps
		{
			set
			{
				CSycles.integrator_set_volume_max_steps(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the seed for sampling
		/// </summary>
		public int Seed
		{
			set
			{
				CSycles.integrator_set_seed(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the clamp value for direct samples.
		/// </summary>
		public float SampleClampDirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_direct(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the clamp value for indirect samples.
		/// </summary>
		public float SampleClampIndirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_indirect(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the sampling pattern to use (CMJ or Sobol).
		/// </summary>
		public SamplingPattern SamplingPattern
		{
			set
			{
				CSycles.integrator_set_sampling_pattern(Scene.Client.Id, Scene.Id, value);
			}
		}
	}
}
