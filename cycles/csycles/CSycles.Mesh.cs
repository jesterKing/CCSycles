using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region mesh
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_verts", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_verts(uint clientId, uint sceneId, uint meshId, float* verts, uint vcount);
		public static void mesh_set_verts(uint clientId, uint sceneId, uint meshId, ref float[] verts, uint vcount)
		{
			unsafe
			{
				fixed (float* pverts = verts)
				{
					cycles_mesh_set_verts(clientId, sceneId, meshId, pverts, vcount);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_uvs", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_uvs(uint clientId, uint sceneId, uint meshId, float* uvs, uint uvcount);
		public static void mesh_set_uvs(uint clientId, uint sceneId, uint meshId, ref float[] uvs, uint uvcount)
		{
			unsafe
			{
				fixed (float* puvs = uvs)
				{
					cycles_mesh_set_uvs(clientId, sceneId, meshId, puvs, uvcount);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_vertex_normals", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_vertex_normals(uint clientId, uint sceneId, uint meshId, float* vertex_normals, uint vncount);
		public static void mesh_set_vertex_normals(uint clientId, uint sceneId, uint meshId, ref float[] vertex_normals, uint vncount)
		{
			unsafe
			{
				fixed (float* pvertex_normals = vertex_normals)
				{
					cycles_mesh_set_vertex_normals(clientId, sceneId, meshId, pvertex_normals, vncount);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_tris", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_tris(uint clientId, uint sceneId, uint meshId, int* faces, uint fcount, uint shaderId);
		public static void mesh_set_tris(uint clientId, uint sceneId, uint meshId, ref int[] tris, uint fcount, uint shaderId)
		{
			unsafe
			{
				fixed (int* ptris = tris)
				{
					cycles_mesh_set_tris(clientId, sceneId, meshId, ptris, fcount, shaderId);
				}
			}

		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_add_triangle", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_add_triangle(uint clientId, uint sceneId, uint meshId, uint v0, uint v1, uint v2, uint shaderId, uint smooth);

		public static void mesh_add_triangle(uint clientId, uint sceneId, uint meshId, uint v0, uint v1, uint v2,
			uint shaderId, bool smooth)
		{
			cycles_mesh_add_triangle(clientId, sceneId, meshId, v0, v1, v2, shaderId, (uint)(smooth ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_smooth", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_set_smooth(uint clientId, uint sceneId, uint meshId, uint smooth);

		public static void mesh_set_smooth(uint clientId, uint sceneId, uint meshId, bool smooth)
		{
			cycles_mesh_set_smooth(clientId, sceneId, meshId, (uint)(smooth ? 1 : 0));
		}

#endregion

	}
}
