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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;

namespace ccl
{
	/// <summary>
	/// Representation of a Cycles shader
	/// </summary>
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

		private bool created_in_cycles { get; set; }

		/// <summary>
		/// Get the output node for this shader.
		/// </summary>
		public OutputNode Output { get; private set; }

		/// <summary>
		/// Create a new shader for client.
		/// </summary>
		/// <param name="client">Client ID for C[CS]ycles API.</param>
		/// <param name="type">The type of shader to create</param>
		public Shader(Client client, ShaderType type)
		{
			Client = client;
			Type = type;
			Id = CSycles.create_shader(Client.Id);
			Output = new OutputNode();
			AddNode(Output);
			created_in_cycles = false;
		}

		/// <summary>
		/// Create front for existing shader. Note that
		/// functionality through this can be limited.
		/// Generally used only for interal matters
		/// </summary>
		/// <param name="client"></param>
		/// <param name="type"></param>
		/// <param name="id"></param>
		internal Shader(Client client, ShaderType type, uint id)
		{
			Client = client;
			Type = type;
			Id = id;
			Output = new OutputNode();
			AddNode(Output);
			created_in_cycles = true;
		}

		/// <summary>
		/// Static constructor for wrapping default surface shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultSurfaceShader(Client client)
		{
			var shader = new Shader(client, ShaderType.Material, 0) {Name = "default_surface"};

			// just add nodes so we have local node presentation, but no need to actually finalise
			// since it already exists in Cycles.
			var diffuse_bsdf = new DiffuseBsdfNode();
			diffuse_bsdf.ins.Color.Value = new float4(0.8f);

			shader.AddNode(diffuse_bsdf);

			diffuse_bsdf.outs.BSDF.Connect(shader.Output.ins.Surface);

			return shader;
		}

		/// <summary>
		/// Static constructor for wrapping default light shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultLightShader(Client client)
		{
			var shader = new Shader(client, ShaderType.Material, 1) {Name = "default_light"};

			// just add nodes so we have local node presentation, but no need to actually finalise
			// since it already exists in Cycles.
			var emission_node = new EmissionNode();
			emission_node.ins.Color.Value = new float4(0.8f);
			emission_node.ins.Strength.Value = 0.0f;

			shader.AddNode(emission_node);

			emission_node.outs.Emission.Connect(shader.Output.ins.Surface);

			return shader;
		}

		/// <summary>
		/// Static constructor for wrapping default background shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultBackgroundShader(Client client)
		{
			var shader = new Shader(client, ShaderType.World, 2) {Name = "default_background"};

			return shader;
		}

		/// <summary>
		/// Static constructor for wrapping default empty shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultEmptyShader(Client client)
		{
			var shader = new Shader(client, ShaderType.Material, 3) {Name = "default_empty"};

			return shader;
		}

		readonly List<ShaderNode> m_nodes = new List<ShaderNode>();
		/// <summary>
		/// Add a ShaderNode to the shader. This will create the node in Cycles, set
		/// any values for sockets and direct members.
		/// This means that any values set to sockets or shader node direct members after
		/// AddNode() won't have effect.
		///
		/// @todo Move non-socket data setting to node implementation.
		/// </summary>
		/// <param name="node">ShaderNode to add</param>
		public void AddNode(ShaderNode node)
		{
			if (node is OutputNode)
			{
				node.Id = CSycles.OUTPUT_SHADERNODE_ID;
				m_nodes.Add(node);
				return;
			}

			if (created_in_cycles)
			{
				m_nodes.Add(node);
				return;
			}

			var nodeid = CSycles.add_shader_node(Client.Id, Id, node.Type);
			node.Id = nodeid;
			m_nodes.Add(node);

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
				case ShaderNodeType.EnvironmentTexture:
					var envnode = node as EnvironmentTextureNode;
					if (envnode != null)
					{
						var projection = envnode.Projection == TextureNode.EnvironmentProjection.Equirectangular
							? "Equirectangular"
							: "Mirror Ball";
						var colspace = envnode.ColorSpace == TextureNode.TextureColorSpace.Color ? "Color" : "None";
						CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, projection);
						CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, colspace);
					}
					break;
				case ShaderNodeType.Mapping:
					CSycles.shadernode_set_enum(Client.Id, Id, node.Id, node.Type, ((MappingNode)node).vector_type.ToString());
					break;
				case ShaderNodeType.ImageTexture:
					CSycles.shadernode_set_attribute_string(Client.Id, Id, node.Id, "color_space", ((ImageTextureNode)node).ColorSpace.ToString());
					CSycles.shadernode_set_attribute_string(Client.Id, Id, node.Id, "projection", ((ImageTextureNode)node).ColorSpace.ToString());
					CSycles.shadernode_set_attribute_string(Client.Id, Id, node.Id, "interpolation", ((ImageTextureNode)node).ColorSpace.ToString());
					break;
			}

			/* set direct member variables */
			switch (node.Type)
			{
				case ShaderNodeType.Math:
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "use_clamp", ((MathNode)node).UseClamp);
					break;
				case ShaderNodeType.Mapping:
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "use_min", ((MappingNode)node).UseMin);
					CSycles.shadernode_set_member_bool(Client.Id, Id, node.Id, node.Type, "use_max", ((MappingNode)node).UseMax);
					var tr = ((MappingNode) node).Translation;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "translation", tr.x, tr.y, tr.z);
					var rt = ((MappingNode) node).Rotation;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "rotation", rt.x, rt.y, rt.z);
					var sc = ((MappingNode) node).Scale;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "scale", sc.x, sc.y, sc.z);
					var mi = ((MappingNode) node).Min;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "min", mi.x, mi.y, mi.z);
					var ma = ((MappingNode) node).Max;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "max", ma.x, ma.y, ma.z);
					break;
				case ShaderNodeType.Value:
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "value", ((ValueNode)node).Value);
					break;
				case ShaderNodeType.Color:
					var val = ((ColorNode) node).Value;
					CSycles.shadernode_set_member_vec(Client.Id, Id, node.Id, node.Type, "value", val.x, val.y, val.z);
					break;
				case ShaderNodeType.ImageTexture:
					var imgtexnode = (ImageTextureNode) node;
					CSycles.shadernode_set_member_float(Client.Id, Id, node.Id, node.Type, "projection_blend", imgtexnode.ProjectionBlend);
					if (imgtexnode.FloatImage != null)
					{
						var flimg = imgtexnode.FloatImage;
						CSycles.shadernode_set_member_float_img(Client.Id, Id, node.Id, node.Type, "builtin-data", imgtexnode.Filename ?? String.Format("{0}-{0}-{0}", Client.Id, Id, node.Id), ref flimg, imgtexnode.Width, imgtexnode.Height, 1, 4);
					}
					else if (imgtexnode.ByteImage != null)
					{
						var bimg = imgtexnode.ByteImage;
						CSycles.shadernode_set_member_byte_img(Client.Id, Id, node.Id, node.Type, "builtin-data", imgtexnode.Filename ?? String.Format("{0}-{0}-{0}", Client.Id, Id, node.Id), ref bimg, imgtexnode.Width, imgtexnode.Height, 1, 4);
					}
					break;
				case ShaderNodeType.EnvironmentTexture:
					var envtexnode = (EnvironmentTextureNode) node;
					if (envtexnode.FloatImage != null)
					{
						var flenv = envtexnode.FloatImage;
						CSycles.shadernode_set_member_float_img(Client.Id, Id, node.Id, node.Type, "builtin-data", envtexnode.Filename ?? String.Format("{0}-{0}-{0}", Client.Id, Id, node.Id), ref flenv, envtexnode.Width, envtexnode.Height, 1, 4);
					}
					else if (envtexnode.ByteImage != null)
					{
						var benv = envtexnode.ByteImage;
						CSycles.shadernode_set_member_byte_img(Client.Id, Id, node.Id, node.Type, "builtin-data", envtexnode.Filename ?? String.Format("{0}-{0}-{0}", Client.Id, Id, node.Id), ref benv, envtexnode.Width, envtexnode.Height, 1, 4);
					}
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

		/// <summary>
		/// Finalizes the graph by connecting all sockets in Cycles as specified
		/// through code.
		/// </summary>
		public void FinalizeGraph()
		{
			foreach (var node in m_nodes)
			{
				if (node.inputs == null) continue;

				foreach(var socket in node.inputs.Sockets)
				{
					var from = socket.ConnectionFrom;
					if (from == null) continue;
					Connect(from.Parent, from.Name, node, socket.Name);
				}
			}
		}

		/// <summary>
		/// Make the actual connection between nodes.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="fromout"></param>
		/// <param name="to"></param>
		/// <param name="toin"></param>
		private void Connect(ShaderNode from, string fromout, ShaderNode to, string toin)
		{
			if (m_nodes.Contains(from) && m_nodes.Contains(to))
			{
				CSycles.shader_connect_nodes(Client.Id, Id, from.Id, fromout, to.Id, toin);
			}
		}

		private string m_name;
		/// <summary>
		/// Set the name of the Shader
		/// </summary>
		public string Name
		{
			set
			{
				m_name = value;
				if(!created_in_cycles) CSycles.shader_set_name(Client.Id, Id, m_name);
			}
			get
			{
				return m_name;
			}
		}

		/// <summary>
		/// Set to true if multiple importance sampling is to be used
		/// </summary>
		public bool UseMis
		{
			set
			{
				if(!created_in_cycles) CSycles.shader_set_use_mis(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this shader supports transparent shadows.
		/// </summary>
		public bool UseTransparentShadow
		{
			set
			{
				if(!created_in_cycles) CSycles.shader_set_use_transparent_shadow(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if this shader is used for a heterogeneous volume.
		/// </summary>
		public bool HeterogeneousVolume
		{
			set
			{
				if(!created_in_cycles) CSycles.shader_set_heterogeneous_volume(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Create node graph in the given shader from the passed XML
		/// </summary>
		/// <param name="shader"></param>
		/// <param name="shaderXml"></param>
		/// <returns></returns>
		public static Shader ShaderFromXml(ref Shader shader, string shaderXml)
		{
			var xmlmem = Encoding.UTF8.GetBytes(shaderXml);
			using (var xmlstream = new MemoryStream(xmlmem))
			{
				var settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Fragment,
					IgnoreComments = true,
					IgnoreProcessingInstructions = true,
					IgnoreWhitespace = true
				};
				var reader = XmlReader.Create(xmlstream, settings);
				Utilities.Instance.ReadNodeGraph(ref shader, reader);
			}

			return shader;
		}

	}
}
