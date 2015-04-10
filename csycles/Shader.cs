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
		/// Get the ID for this shader. This ID is given by CCycles
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
		/// Clear the shader graph for this node, so it can be repopulated.
		/// </summary>
		public void Recreate()
		{
			CSycles.shader_new_graph(Client.Id, Id);

			m_nodes.Clear();

			Output = new OutputNode();
			AddNode(Output);
		}

		/// <summary>
		/// Tag shader for device update
		/// </summary>
		public void Tag()
		{
			CSycles.scene_tag_shader(Client.Id, Client.Scene.Id, Id);
		}

		/// <summary>
		/// Static constructor for wrapping default surface shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultSurfaceShader(Client client)
		{
			var shader = new Shader(client, ShaderType.Material, CSycles.DEFAULT_SURFACE_SHADER) {Name = "default_surface"};

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
			var shader = new Shader(client, ShaderType.Material, CSycles.DEFAULT_LIGHT_SHADER) {Name = "default_light"};

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
			var shader = new Shader(client, ShaderType.World, CSycles.DEFAULT_BACKGROUND_SHADER) {Name = "default_background"};

			return shader;
		}

		/// <summary>
		/// Static constructor for wrapping default empty shader created by Cycles shader manager.
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		static public Shader WrapDefaultEmptyShader(Client client)
		{
			var shader = new Shader(client, ShaderType.Material, CSycles.DEFAULT_EMPTY_SHADER) {Name = "default_empty"};

			return shader;
		}

		readonly List<ShaderNode> m_nodes = new List<ShaderNode>();
		/// <summary>
		/// Add a ShaderNode to the shader. This will create the node in Cycles, set
		/// any values for sockets and direct members.
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
		}

		/// <summary>
		/// Finalizes the graph by connecting all sockets in Cycles as specified
		/// through code.
		///
		/// This step also commits any values set to input sockets, enumerations
		/// and direct member variables.
		/// </summary>
		public void FinalizeGraph()
		{
			System.Diagnostics.Debug.WriteLine(String.Format("Finalizing {0}", Name));
			foreach (var node in m_nodes)
			{
				/* set enumerations */
				node.SetEnums(Client.Id, Id);

				/* set direct member variables */
				node.SetDirectMembers(Client.Id, Id);

				if (node.inputs == null) continue;

				node.SetSockets(Client.Id, Id);

				foreach (var socket in node.inputs.Sockets)
				{
					var from = socket.ConnectionFrom;
					if (from == null) continue;
					System.Diagnostics.Debug.WriteLine("Shader {0}: Connecting {1} to {2}", Name, from.Path, socket.Path);
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
		public static void ShaderFromXml(ref Shader shader, string shaderXml)
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
		}

	}
}
