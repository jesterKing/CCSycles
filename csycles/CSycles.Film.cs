using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region film
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_film_set_filter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_filter(uint clientId, uint sceneId, uint filter_type, float filter_width);
		public static void film_set_filter(uint clientId, uint sceneId, FilterType filter_type, float filter_width)
		{
			cycles_film_set_filter(clientId, sceneId, (uint)filter_type, filter_width);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_film_set_exposure", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_exposure(uint clientId, uint sceneId, float exposure);
		public static void film_set_exposure(uint clientId, uint sceneId, float exposure)
		{
			cycles_film_set_exposure(clientId, sceneId, exposure);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_film_set_use_sample_clamp", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_set_use_sample_clamp(uint clientId, uint sceneId, bool use_sample_clamp);
		public static void film_set_use_sample_clamp(uint clientId, uint sceneId, bool use_sample_clamp)
		{
			cycles_film_set_use_sample_clamp(clientId, sceneId, use_sample_clamp);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_film_tag_update", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_film_tag_update(uint clientId, uint sceneId);
		public static void film_tag_update(uint clientId, uint sceneId)
		{
			cycles_film_tag_update(clientId, sceneId);
		}

#endregion
	}
}
