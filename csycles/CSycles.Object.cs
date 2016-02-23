using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region object
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_set_matrix", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_matrix(uint clientId, uint sceneId, uint objectId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l,
			float m, float n, float o, float p);
		public static void object_set_matrix(uint clientId, uint sceneId, uint objectId, Transform t)
		{
			cycles_scene_object_set_matrix(clientId, sceneId, objectId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w,
				t.w.x, t.w.y, t.w.z, t.w.w);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_set_mesh", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_mesh(uint clientId, uint sceneId, uint objectId, uint meshId);
		public static void object_set_mesh(uint clientId, uint sceneId, uint objectId, uint meshId)
		{
			cycles_scene_object_set_mesh(clientId, sceneId, objectId, meshId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_get_mesh", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_object_get_mesh(uint clientId, uint sceneId, uint objectId);
		public static uint object_get_mesh(uint clientId, uint sceneId, uint objectId)
		{
			return cycles_scene_object_get_mesh(clientId, sceneId, objectId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_set_visibility", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_visibility(uint clientId, uint sceneId, uint objectId, uint visibility);
		public static void object_set_visibility(uint clientId, uint sceneId, uint objectId, PathRay visibility)
		{
			cycles_scene_object_set_visibility(clientId, sceneId, objectId, (uint)visibility);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_set_is_shadowcatcher", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_is_shadowcatcher(uint clientId, uint sceneId, uint objectId, bool is_shadowcatcher);
		public static void object_set_is_shadowcatcher(uint clientId, uint sceneId, uint objectId, bool is_shadowcatcher)
		{
			cycles_scene_object_set_is_shadowcatcher(clientId, sceneId, objectId, is_shadowcatcher);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_object_tag_update", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_object_tag_update(uint clientId, uint sceneId, uint objectId);
		public static void object_tag_update(uint clientId, uint sceneId, uint objectId)
		{
			cycles_object_tag_update(clientId, sceneId, objectId);
		}

		#endregion
	}
}
