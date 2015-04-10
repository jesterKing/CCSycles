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

namespace ccl.ShaderNodes.Sockets
{
	/// <summary>
	/// FloatSocket is used to communicate a single float value between
	/// nodes.
	/// </summary>
	public class FloatSocket : SocketBase
	{
		/// <summary>
		/// Get or set the value for the float socket
		/// </summary>
		public float Value { get; set; }
		/// <summary>
		/// Create a new FloatSocket.
		/// 
		/// The name of the socket has to correspond to the names in Cycles.
		/// </summary>
		/// <param name="parentNode">The ShaderNode for which the socket is created</param>
		/// <param name="name">Name of the socket</param>
		public FloatSocket(ShaderNode parentNode, string name)
			: base(parentNode, name)
		{
			Value = 0.0f;
		}
	}
}
