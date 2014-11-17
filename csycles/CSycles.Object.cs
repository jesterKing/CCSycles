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
#endregion
	}
}
