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
		internal Scene Scene { get; set; }
		public Integrator(Scene scene)
		{
			Scene = scene;
		}

		public void TagForUpdate()
		{
			CSycles.integrator_tag_update(Scene.Client.Id, Scene.Id);
		}

		public int MaxBounce
		{
			set
			{
				CSycles.integrator_set_max_bounce(Scene.Client.Id, Scene.Id, value);
				
			}
		}

		public int MinBounce
		{
			set
			{
				CSycles.integrator_set_min_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int TransparentMinBounce
		{
			set
			{
				CSycles.integrator_set_transparent_min_bounce (Scene.Client.Id, Scene.Id, value);
			}
		}

		public int TransparentMaxBounce
		{
			set
			{
				CSycles.integrator_set_transparent_max_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int MaxDiffuseBounce
		{
			set
			{
				CSycles.integrator_set_max_diffuse_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int MaxGlossyBounce
		{
			set
			{
				CSycles.integrator_set_max_glossy_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int MaxTransmissionBounce
		{
			set
			{
				CSycles.integrator_set_max_transmission_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int MaxVolumeBounce
		{
			set
			{
				CSycles.integrator_set_max_volume_bounce(Scene.Client.Id, Scene.Id, value);
			}
		}

		public bool NoCaustics
		{
			set
			{
				CSycles.integrator_set_no_caustics(Scene.Client.Id, Scene.Id, value);
			}
		}

		public bool TransparentShadows
		{
			set
			{
				CSycles.integrator_set_transparent_shadows(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int DiffuseSamples
		{
			set
			{
				CSycles.integrator_set_diffuse_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int GlossySamples
		{
			set
			{
				CSycles.integrator_set_glossy_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int TransmissionSamples
		{
			set
			{
				CSycles.integrator_set_transmission_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int AoSamples
		{
			set
			{
				CSycles.integrator_set_ao_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int MeshLightSamples
		{
			set
			{
				CSycles.integrator_set_mesh_light_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int SubsurfaceSamples
		{
			set
			{
				CSycles.integrator_set_subsurface_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int VolumeSamples
		{
			set
			{
				CSycles.integrator_set_volume_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int AaSamples
		{
			set
			{
				CSycles.integrator_set_aa_samples(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float FilterGlossy
		{
			set
			{
				CSycles.integrator_set_filter_glossy(Scene.Client.Id, Scene.Id, value);
			}
		}

		public IntegratorMethod IntegratorMethod
		{
			set
			{
				CSycles.integrator_set_method(Scene.Client.Id, Scene.Id, value);
			}
		}

		public bool SampleAllLightsDirect
		{
			set
			{
				CSycles.integrator_set_sample_all_lights_direct(Scene.Client.Id, Scene.Id, value);
			}
		}

		public bool SampleAllLightsIndirect
		{
			set
			{
				CSycles.integrator_set_sample_all_lights_indirect(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int VolumeHomogeneousSampling
		{
			set
			{
				CSycles.integrator_set_volume_homogeneous_sampling(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float VolumeStepSize
		{
			set
			{
				CSycles.integrator_set_volume_step_size(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int VolumeMaxSteps
		{
			set
			{
				CSycles.integrator_set_volume_max_steps(Scene.Client.Id, Scene.Id, value);
			}
		}

		public int Seed
		{
			set
			{
				CSycles.integrator_set_seed(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float SampleClampDirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_direct(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float SampleClampIndirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_indirect(Scene.Client.Id, Scene.Id, value);
			}
		}

		public SamplingPattern SamplingPattern
		{
			set
			{
				CSycles.integrator_set_sampling_pattern(Scene.Client.Id, Scene.Id, value);
			}
		}
	}
}
