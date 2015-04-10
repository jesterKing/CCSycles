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
	/// A client is a main point of entry for C[CS]?ycles API user
	/// </summary>
	public class Client
	{
		/// <summary>
		/// Id for this client
		/// </summary>
		public uint Id { get; private set; }

		/// <summary>
		/// Scene reference of this client
		/// </summary>
		public Scene Scene { get; set; }

		/// <summary>
		/// Create a new client
		/// </summary>
		public Client()
		{
			Id = CSycles.new_client();
		}

		/// <summary>
		/// Destroy a client.
		/// </summary>
		~Client()
		{
			CSycles.release_client(Id);
		}
	}
}
