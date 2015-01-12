using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region integrator
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_tag_update", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_tag_update(uint clientId, uint sceneId);
		public static void integrator_tag_update(uint clientId, uint sceneId)
		{
			cycles_integrator_tag_update(clientId, sceneId);
		}
		
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_min_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_min_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_min_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_min_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_diffuse_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_diffuse_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_diffuse_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_diffuse_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_glossy_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_glossy_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_glossy_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_glossy_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_transmission_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_transmission_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_transmission_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_transmission_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_volume_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_volume_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_volume_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_volume_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_max_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_max_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_transparent_max_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transparent_max_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_min_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_min_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_transparent_min_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transparent_min_bounce(clientId, sceneId, value);
		}


		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_diffuse_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_diffuse_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_diffuse_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_diffuse_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_glossy_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_glossy_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_glossy_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_glossy_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transmission_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transmission_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_transmission_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transmission_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_ao_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_ao_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_ao_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_mesh_light_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_mesh_light_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_mesh_light_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_mesh_light_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_subsurface_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_subsurface_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_subsurface_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_subsurface_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_volume_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_volume_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_volume_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_aa_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_aa_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_aa_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_aa_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_no_caustics", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_no_caustics(uint clientId, uint sceneId, bool value);
		public static void integrator_set_no_caustics(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_no_caustics(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_shadows", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_shadows(uint clientId, uint sceneId, bool value);
		public static void integrator_set_transparent_shadows(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_transparent_shadows(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_filter_glossy", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_filter_glossy(uint clientId, uint sceneId, float value);
		public static void integrator_set_filter_glossy(uint clientId, uint sceneId, float value)
		{
			cycles_integrator_set_filter_glossy(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_method", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_method(uint clientId, uint sceneId, int value);
		public static void integrator_set_method(uint clientId, uint sceneId, IntegratorMethod value)
		{
			cycles_integrator_set_method(clientId, sceneId, (int)value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_sample_all_lights_direct", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_all_lights_direct(uint clientId, uint sceneId, bool value);
		public static void integrator_set_sample_all_lights_direct(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_sample_all_lights_direct(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_sample_all_lights_indirect", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_all_lights_indirect(uint clientId, uint sceneId, bool value);
		public static void integrator_set_sample_all_lights_indirect(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_sample_all_lights_indirect(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_volume_step_size", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_step_size(uint clientId, uint sceneId, float value);
		public static void integrator_set_volume_step_size(uint clientId, uint sceneId, float value)
		{
			cycles_integrator_set_volume_step_size(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_volume_max_steps", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_max_steps(uint clientId, uint sceneId, int value);
		public static void integrator_set_volume_max_steps(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_volume_max_steps(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_seed", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_seed(uint clientId, uint sceneId, int value);
		public static void integrator_set_seed(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_seed(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_sampling_pattern", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sampling_pattern(uint clientId, uint sceneId, uint value);
		public static void integrator_set_sampling_pattern(uint clientId, uint sceneId, SamplingPattern value)
		{
			cycles_integrator_set_sampling_pattern(clientId, sceneId, (uint)value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_sample_clamp_direct", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_clamp_direct(uint clientId, uint sceneId, float value);
		public static void integrator_set_sample_clamp_direct(uint clientId, uint sceneId, float value)
		{
			cycles_integrator_set_sample_clamp_direct(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_sample_clamp_indirect", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_sample_clamp_indirect(uint clientId, uint sceneId, float value);
		public static void integrator_set_sample_clamp_indirect(uint clientId, uint sceneId, float value)
		{
			cycles_integrator_set_sample_clamp_indirect(clientId, sceneId, value);
		}

#endregion
	}
}
