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
	public class Mesh
	{
		public uint Id { get; private set; }
		private Client Client { get; set; }
		private Shader Shader { get; set; }

		/// <summary>
		/// Create a new mesh for the given client using shader as the default shader
		/// </summary>
		/// <param name="client"></param>
		/// <param name="shader"></param>
		public Mesh(Client client, Shader shader)
		{
			Client = client;
			Shader = shader;

			Id = CSycles.scene_add_mesh(Client.Id, Client.Scene.Id, Client.Scene.ShaderSceneId(shader));
		}

		internal Mesh(Client client, uint id, Shader shader)
		{
			Client = client;
			Shader = shader;

			Id = id;
		}

		/// <summary>
		/// Set vertex coordinates
		/// </summary>
		/// <param name="verts"></param>
		public void SetVerts(ref float[] verts)
		{
			CSycles.mesh_set_verts(Client.Id, Client.Scene.Id, Id, ref verts, (uint) (verts.Length/3));
		}

		/// <summary>
		/// Set trifaces
		/// </summary>
		/// <param name="faces"></param>
		/// <param name="smooth"></param>
		public void SetVertTris(ref int[] faces, bool smooth)
		{
			CSycles.mesh_set_tris(Client.Id, Client.Scene.Id, Id, ref faces, (uint) (faces.Length/3), Client.Scene.ShaderSceneId(Shader), smooth);
		}

		/// <summary>
		/// Set vertex normals
		/// </summary>
		/// <param name="vertex_normals"></param>
		public void SetVertNormals(ref float[] vertex_normals)
		{
			CSycles.mesh_set_vertex_normals(Client.Id, Client.Scene.Id, Id, ref vertex_normals, (uint) (vertex_normals.Length/3));
		}

		/// <summary>
		/// Set UVs
		/// </summary>
		/// <param name="uvs"></param>
		public void SetUvs(ref float[] uvs)
		{
			CSycles.mesh_set_uvs(Client.Id, Client.Scene.Id, Id, ref uvs, (uint) (uvs.Length/2));
		}
	}
}
