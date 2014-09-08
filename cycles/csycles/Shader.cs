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

using System.Collections.Generic;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;

namespace ccl
{
	public class Shader
	{
		public enum ShaderType
		{
			Material,
			World
		}
		/// <summary>
		/// Get the ID for this shader
		/// </summary>
		public uint Id { get; private set; }
		private Client Client { get; set; }
		public ShaderType Type { get; set; }

		public OutputNode Output { get; set; }

		/// <summary>
		/// Create a new shader for client.
		/// </summary>
		/// <param name="clientId">Client ID for C[CS]ycles API.</param>
		/// <param name="type">The type of shader to create</param>
		public Shader(Client client, ShaderType type)
		{
			Client = client;
			Type = type;
			Id = CSycles.create_shader(Client.Id);
			Output = new OutputNode();
			AddNode(Output);
		}

		readonly List<ShaderNode> nodes = new List<ShaderNode>();
		/// <summary>
		/// Add a ShaderNode to the shader
		/// </summary>
		/// <param name="node">ShaderNode to add</param>
		public void AddNode(ShaderNode node)
		{
			if (node is OutputNode)
			{
				node.Id = CSycles.OUTPUT_SHADERNODE_ID;
				nodes.Add(node);
				return;
			}

			var nodeid = CSycles.add_shader_node(Client.Id, Id, node.Type);
			node.Id = nodeid;
			nodes.Add(node);

			/* set node attributes */
			if (node.inputs != null)
			{
				foreach (var socket in node.inputs.Sockets)
				{
					var float_socket = socket as FloatSocket;
					if (float_socket != null)
					{
						CSycles.shadernode_set_attribute_float(Client.Id, Id, node.Id, float_socket.Name, float_socket.Value);
					}
					var int_socket = socket as IntSocket;
					if (int_socket != null)
					{
						CSycles.shadernode_set_attribute_int(Client.Id, Id, node.Id, int_socket.Name, int_socket.Value);
					}
					var string_socket = socket as StringSocket;
					if (string_socket != null)
					{
						CSycles.shadernode_set_attribute_string(Client.Id, Id, node.Id, socket.Name, string_socket.Value);
					}
					var float4_socket = socket as Float4Socket;
					if (float4_socket != null)
					{
						CSycles.shadernode_set_attribute_vec(Client.Id, Id, node.Id, float4_socket.Name, float4_socket.Value);
					}
				}
			}

			/* set enums */
			switch (node.Type)
			{
				case ShaderNodeType.Math:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((MathNode)node).Operation.ToString().Replace('_', ' '));
					break;
				case ShaderNodeType.Refraction:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((RefractionBsdfNode)node).Distribution);
					break;
				case ShaderNodeType.Mix:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((MixNode)node).BlendType);
					break;
				case ShaderNodeType.Glossy:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((GlossyBsdfNode)node).Distribution);
					break;
				case ShaderNodeType.Glass:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((GlassBsdfNode)node).Distribution);
					break;
				case ShaderNodeType.SkyTexture:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((SkyTexture)node).SkyType);
					break;
				case ShaderNodeType.WaveTexture:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((WaveTexture)node).WaveType);
					break;
			}

			/* set direct member variables */
			switch (node.Type)
			{
				case ShaderNodeType.Math:
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "use_clamp", ((MathNode)node).UseClamp);
					break;
				case ShaderNodeType.Value:
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "value", ((ValueNode)node).Value);
					break;
				case ShaderNodeType.Color:
					var val = ((ColorNode) node).Value;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "value", val.x, val.y, val.z);
					break;
				case ShaderNodeType.ImageTexture:
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "projection_blend", ((ImageTextureNode)node).ProjectionBlend);
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "is_linear", ((ImageTextureNode)node).IsLinear);
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "is_float", ((ImageTextureNode)node).IsFloat);
					break;
				case ShaderNodeType.BrickTexture:
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "offset", ((BrickTexture)node).Offset);
					CSycles.shadernode_set_member_int(Client.Id, Id, node.Id, node.Type, "offset_frequency", ((BrickTexture)node).OffsetFrequency);
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "squash", ((BrickTexture)node).Squash);
					CSycles.shadernode_set_member_int(Client.Id, Id, node.Id, node.Type, "squash_frequency", ((BrickTexture)node).SquashFrequency);
					break;
				case ShaderNodeType.SkyTexture:
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "turbidity", ((SkyTexture)node).Turbidity);
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "ground_albedo", ((SkyTexture)node).GroundAlbedo);
					var sd = ((SkyTexture) node).SunDirection;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "sun_direction", sd.x, sd.y, sd.z);
					break;
			}
		}

		SocketBase lastNode { get; set; }

		/// <summary>
		/// Specify which ShaderNode is the last node for this Shader graph.
		/// Its output will be connected to the shader closure
		/// </summary>
		/// <param name="last">The ShaderNode that is last in the shader graph</param>
		/// <param name="outp">The output of the ShaderNode to connect to the final output</param>
		public void LastSocket(SocketBase last)
		{
			lastNode = last;
		}

		/// <summary>
		/// Push the defined shader graph to Cycles.
		/// </summary>
		public void FinalizeGraph()
		{
			foreach (var node in nodes)
			{
				if (node.inputs == null) continue;

				foreach(var socket in node.inputs.Sockets)
				{
					var from = socket.ConnectionFrom;
					if (from == null) continue;
					Connect(from.Parent, from.Name, node, socket.Name);
				}
			}

			//ConnectToOutput(lastNode.Parent, lastNode.Name);
		}

		private void Connect(ShaderNode from, string fromout, ShaderNode to, string toin)
		{
			if (nodes.Contains(from) && nodes.Contains(to))
			{
				CSycles.shader_connect_nodes(Client.Id, Id, from.Id, fromout, to.Id, toin);
			}
		}

		/** TODO: devise way to connect to different output types
		 * - Surface
		 * - Volume
		 * - World
		 */
		private void ConnectToOutput(ShaderNode from, string fromout)
		{
			if (nodes.Contains(from))
			{

				CSycles.shader_connect_nodes(Client.Id, Id, from.Id, fromout, CSycles.OUTPUT_SHADERNODE_ID, "Surface");
			}
		}

		/// <summary>
		/// Set the name of the Shader
		/// </summary>
		private string _name;
		public string Name
		{
			set
			{
				_name = value;
				CSycles.shader_set_name(Client.Id, Id, _name);
			}
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Set to true if Mis should be used.
		/// \todo find out what Mis actually means
		/// </summary>
		public bool UseMis
		{
			set
			{
				CSycles.shader_set_use_mis(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this shader supports transparent shadows.
		/// </summary>
		public bool UseTransparentShadow
		{
			set
			{
				CSycles.shader_set_use_transparent_shadow(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this shader is used for a heterogeneous volume.
		/// </summary>
		public bool HeterogeneousVolume
		{
			set
			{
				CSycles.shader_set_heterogeneous_volume(Client.Id, Id, value);
			}
		}
	}
}
