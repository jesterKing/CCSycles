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

using System;
using System.Xml;
using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// Base class for shader nodes.
	/// </summary>
	public class ShaderNode
	{
		/// <summary>
		/// Set a name for this node
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Get the node ID. This is set when created in Cycles.
		/// </summary>
		public uint Id { get; internal set; }
		/// <summary>
		/// Get the shader node type. Set in the constructor.
		/// </summary>
		public ShaderNodeType Type { get; private set; }

		/// <summary>
		/// Generic access to input sockets.
		/// </summary>
		internal Inputs inputs { get; set; }
		/// <summary>
		/// Generic access to output sockets.
		/// </summary>
		internal Outputs outputs { get; set; }

		/// <summary>
		/// Create node of type ShaderNodeType type
		/// </summary>
		/// <param name="type"></param>
		internal ShaderNode(ShaderNodeType type) : this(type, String.Empty)
		{
		}

		/// <summary>
		/// Create node of type ShaderNodeType and with given name
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		internal ShaderNode(ShaderNodeType type, string name)
		{
			Type = type;
			Name = name;
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has enumerations that need to be committed to Cycles
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="shaderId"></param>
		virtual internal void SetEnums(uint clientId, uint shaderId)
		{
			// do nothing
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has direct members that need to be committed to Cycles
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="shaderId"></param>
		virtual internal void SetDirectMembers(uint clientId, uint shaderId)
		{
			// do nothing
		}

		internal void SetSockets(uint clientId, uint shaderId)
		{
			/* set node attributes */
			if (inputs != null)
			{
				foreach (var socket in inputs.Sockets)
				{
					var float_socket = socket as FloatSocket;
					if (float_socket != null)
					{
						CSycles.shadernode_set_attribute_float(clientId, shaderId, Id, float_socket.Name, float_socket.Value);
					}
					var int_socket = socket as IntSocket;
					if (int_socket != null)
					{
						CSycles.shadernode_set_attribute_int(clientId, shaderId, Id, int_socket.Name, int_socket.Value);
					}
					var string_socket = socket as StringSocket;
					if (string_socket != null)
					{
						CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, socket.Name, string_socket.Value);
					}
					var float4_socket = socket as Float4Socket;
					if (float4_socket != null)
					{
						CSycles.shadernode_set_attribute_vec(clientId, shaderId, Id, float4_socket.Name, float4_socket.Value);
					}
				}
			}
		}

		public override string ToString()
		{
			var str = String.Format("{0} ({1})", Name, Type);
			return str;
		}

		/// <summary>
		/// Implement ParseXml to support proper XMl support.
		/// </summary>
		/// <param name="xmlNode"></param>
		virtual internal void ParseXml(XmlReader xmlNode)
		{
			
		}
	}
}
