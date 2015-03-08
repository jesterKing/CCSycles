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
	/// Representation of a Cycles object.
	/// </summary>
	public class Object
	{
		/// <summary>
		/// Id of the Cycles object.
		/// </summary>
		public uint Id { get; private set; }
		/// <summary>
		/// Reference to the client.
		/// </summary>
		private Client Client { get; set; }

		/// <summary>
		/// Create a new mesh for the given client using shader as the default shader
		/// </summary>
		/// <param name="client"></param>
		/// <param name="shader"></param>
		public Object(Client client)
		{
			Client = client;

			Id = CSycles.scene_add_object(Client.Id, Client.Scene.Id);
		}

		private Mesh m_mesh;
		/// <summary>
		/// Get or set the mesh
		/// </summary>
		public Mesh Mesh
		{
			get
			{
				if (m_mesh != null) return m_mesh;
				var mid = CSycles.object_get_mesh(Client.Id, Client.Scene.Id, Id);
				return new Mesh(Client, mid, null);
			}
			set
			{
				m_mesh = value;
				CSycles.object_set_mesh(Client.Id, Client.Scene.Id, Id, value.Id);
			}
		}

		/// <summary>
		/// Set the object transformation
		/// </summary>
		public Transform Transform
		{
			set
			{
				CSycles.object_set_matrix(Client.Id, Client.Scene.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the visibility of this object to specific rays.
		/// </summary>
		public PathRay Visibility
		{
			set
			{
				CSycles.object_set_visibility(Client.Id, Client.Scene.Id, Id, value);
			}
		}
	}
}
