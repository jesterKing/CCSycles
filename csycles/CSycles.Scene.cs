using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region scene
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_create(uint clientId, uint sceneParamsId, uint deviceid);
		public static uint scene_create(uint clientId, uint sceneParamsId, uint deviceid)
		{
			return cycles_scene_create(clientId, sceneParamsId, deviceid);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_reset", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_reset(uint clientId, uint sceneId);
		public static void scene_reset(uint clientId, uint sceneId)
		{
			cycles_scene_reset(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_add_object", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_object(uint clientId, uint sceneId);
		public static uint scene_add_object(uint clientId, uint sceneId)
		{
			return cycles_scene_add_object(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_add_mesh", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_mesh(uint clientId, uint sceneId, uint objectId, uint shaderId);
		public static uint scene_add_mesh(uint clientId, uint sceneId, uint objectId, uint shaderId)
		{
			return cycles_scene_add_mesh(clientId, sceneId, objectId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_shader(uint clientId, uint sceneId, uint shaderId);
		public static void scene_set_background_shader(uint clientId, uint sceneId, uint shaderId)
		{
			cycles_scene_set_background_shader(clientId, sceneId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_get_background_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_get_background_shader(uint clientId, uint sceneId);
		public static uint scene_get_background_shader(uint clientId, uint sceneId)
		{
			return cycles_scene_get_background_shader(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_default_surface_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_default_surface_shader(uint clientId, uint sceneId, uint shaderId);
		/// <summary>
		/// Set the default surface shader for sceneId to shaderId.
		/// 
		/// Note that this shaderId has to be the scene-specific shader id.
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="sceneId">Scene for which the default shader is set</param>
		/// <param name="shaderId">The shader to which the default shader is set</param>
		public static void scene_set_default_surface_shader(uint clientId, uint sceneId, uint shaderId)
		{
			cycles_scene_set_default_surface_shader(clientId, sceneId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_get_default_surface_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_get_default_surface_shader(uint clientId, uint sceneId);
		/// <summary>
		/// Get the default surface shader id for sceneId
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="sceneId"></param>
		/// <returns></returns>
		public static uint scene_get_default_surface_shader(uint clientId, uint sceneId)
		{
			return cycles_scene_get_default_surface_shader(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_ao_factor", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_ao_factor(uint clientId, uint sceneId, float aoFactor);

		public static void scene_set_background_ao_factor(uint clientId, uint sceneId, float aoFactor)
		{
			cycles_scene_set_background_ao_factor(clientId, sceneId, aoFactor);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_ao_distance", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_ao_distance(uint clientId, uint sceneId, float aoDistance);

		public static void scene_set_background_ao_distance(uint clientId, uint sceneId, float aoDistance)
		{
			cycles_scene_set_background_ao_distance(clientId, sceneId, aoDistance);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_visibility", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_visibility(uint clientId, uint sceneId, int raypathFlag);

		public static void scene_set_background_visibility(uint clientId, uint sceneId, PathRay raypathFlag)
		{
			cycles_scene_set_background_visibility(clientId, sceneId, (int)raypathFlag);
		}
#endregion

#region scene parameters
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_params_create(uint clientId, uint shadingsystem, uint bvhtype, uint bvhcache, uint bvhspatialsplit, uint qbvh, uint persistentdata);
		public static uint scene_params_create(uint clientId, ShadingSystem shadingSystem, BvhType bvhType, bool bvhCache, bool bvhSpatialSplit, bool qbvh, bool persistentData)
		{
			return cycles_scene_params_create(clientId, (uint)shadingSystem, (uint)bvhType, (uint)(bvhCache?1:0), (uint)(bvhSpatialSplit?1:0), (uint)(qbvh?1:0), (uint)(persistentData?1:0));
		}
		  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_type(uint clientId, uint sceneParamsId, uint type);
		public static void scene_params_set_bvh_type(uint clientId, uint sceneParamsId, BvhType type)
		{
			cycles_scene_params_set_bvh_type(clientId, sceneParamsId, (uint)type);
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_cache", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_cache(uint clientId, uint sceneParamsId, uint cache);
		public static void scene_params_set_bvh_cache(uint clientId, uint sceneParamsId, bool type)
		{
			cycles_scene_params_set_bvh_cache(clientId, sceneParamsId, (uint)(type?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_spatial_split", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_spatial_split(uint clientId, uint sceneParamsId, uint split);
		public static void scene_params_set_bvh_spatial_split(uint clientId, uint sceneParamsId, bool split)
		{
			cycles_scene_params_set_bvh_spatial_split(clientId, sceneParamsId, (uint)(split?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_qbvh", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_qbvh(uint clientId, uint sceneParamsId, uint qbvh);
		public static void scene_params_set_qbvh(uint clientId, uint sceneParamsId, bool qbvh)
		{
			cycles_scene_params_set_qbvh(clientId, sceneParamsId, (uint)(qbvh?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_shadingsystem", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_shadingsystem(uint clientId, uint sceneParamsId, uint shadingsystem);
		public static void scene_params_set_shadingsystem(uint clientId, uint sceneParamsId, ShadingSystem system)
		{
			cycles_scene_params_set_shadingsystem(clientId, sceneParamsId, (uint)system);
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_persistent_data", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_persistent_data(uint clientId, uint sceneParamsId, uint persistentData);
		public static void scene_params_set_persistent_data(uint clientId, uint sceneParamsId, bool persistent)
		{
			cycles_scene_params_set_persistent_data(clientId, sceneParamsId, (uint)(persistent?1:0));
		}
  
#endregion


	}
}
