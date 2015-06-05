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
using ccl.Attributes;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// MappingNode input sockets
	/// </summary>
	public class MappingInputs : Inputs
	{
		/// <summary>
		/// MappingNode input vector that should be transformed
		/// </summary>
		public Float4Socket Vector { get; set; }

		internal MappingInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// MappingNode output sockets
	/// </summary>
	public class MappingOutputs : Outputs
	{
		/// <summary>
		/// MappingNode output vector
		/// </summary>
		public Float4Socket Vector { get; set; }

		internal MappingOutputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// Mapping node to transform an input vector utilising one of the four
	/// types
	/// - Texture: Transform a texture by inverse mapping the texture coordinate
	/// - Point: Transform a point
	/// - Vector: Transform a direction vector
	/// - Normal: Transform a normal vector with unit length
	/// </summary>
	[ShaderNode("mapping")]
	public class MappingNode : ShaderNode
	{
		/// <summary>
		/// MappingNode input sockets
		/// </summary>
		public MappingInputs ins { get { return (MappingInputs)inputs; } }
		/// <summary>
		/// MappingNode output sockets
		/// </summary>
		public MappingOutputs outs { get { return (MappingOutputs)outputs; } }

		/// <summary>
		/// Mapping type to transform according
		/// </summary>
		public enum MappingType : uint
		{
			/// <summary>
			/// Transform as point
			/// </summary>
			Point = 0,
			/// <summary>
			/// Transform texture space
			/// </summary>
			Texture,
			/// <summary>
			/// Transform vector
			/// </summary>
			Vector,
			/// <summary>
			/// Transform normal
			/// </summary>
			Normal
		}

		/// <summary>
		/// Create new MappingNode
		/// </summary>
		public MappingNode() : this("a mapping node")
		{
		}

		public MappingNode(string name) :
			base(ShaderNodeType.Mapping, name)
		{
			inputs = new MappingInputs(this);
			outputs = new MappingOutputs(this);
			Mapping = MappingType.Texture;
			UseMin = false;
			UseMax = false;
			Translation = new float4(0.0f);
			Rotation = new float4(0.0f);
			Scale = new float4(0.0f);
			Min = new float4(0.0f);
			Max = new float4(0.0f);
		}

		
		/// <summary>
		/// Get or set the mapping type to use
		/// </summary>
		public MappingType Mapping { get; set; }

		/// <summary>
		/// Translate input vector with this
		/// </summary>
		public float4 Translation { get; set; }
		/// <summary>
		/// Rotate input vector with this
		/// </summary>
		public float4 Rotation { get; set; }
		/// <summary>
		/// Scale input vector with this
		/// </summary>
		public float4 Scale { get; set; }
		/// <summary>
		/// If set, use this as minimum values for resulting vector
		/// </summary>
		public float4 Min { get; set; }
		/// <summary>
		/// If set, use this as maximum values for resulting vector
		/// </summary>
		public float4 Max { get; set; }

		/// <summary>
		/// Set to true [IN] if mapping output in Value should be floored to Min
		/// </summary>
		public bool UseMin { get; set; }

		/// <summary>
		/// Set to true [IN] if mapping output in Value should be ceiled to Max
		/// </summary>
		public bool UseMax { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_bool(clientId, shaderId, Id, ShaderNodeType.Mapping, "useminmax", UseMin || UseMax);
			if (UseMin)
			{
				CSycles.shadernode_set_member_vec(clientId, shaderId, Id, ShaderNodeType.Mapping, "min", Min.x, Min.y, Min.z);
			}
			if (UseMax)
			{
				CSycles.shadernode_set_member_vec(clientId, shaderId, Id, ShaderNodeType.Mapping, "max", Max.x, Max.y, Max.z);
			}
			var tr = Translation;
			CSycles.shadernode_texmapping_set_transformation(clientId, shaderId, Id, ShaderNodeType.Mapping, 0, tr.x, tr.y, tr.z);
			var rt = Rotation;
			CSycles.shadernode_texmapping_set_transformation(clientId, shaderId, Id, ShaderNodeType.Mapping, 1, rt.x, rt.y, rt.z);
			var sc = Scale;
			CSycles.shadernode_texmapping_set_transformation(clientId, shaderId, Id, ShaderNodeType.Mapping, 2, sc.x, sc.y, sc.z);

			CSycles.shadernode_texmapping_set_type(clientId, shaderId, Id, ShaderNodeType.Mapping, (uint)Mapping);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			var mapping_type = xmlNode.GetAttribute("mapping_type");
			if (!string.IsNullOrEmpty(mapping_type))
			{
				try
				{
					var mt = (MappingType)Enum.Parse(typeof(MappingType), mapping_type, true);
					Mapping = mt;
				}
				catch (ArgumentException)
				{
					Mapping = MappingType.Texture;
				}
			}
			var f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("rotation"));
			Rotation = f4;
			f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("translation"));
			Translation = f4;
			f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("scale"));
			Scale = f4;
			bool b = false;
			Utilities.Instance.read_bool(ref b, xmlNode.GetAttribute("useminmax"));
			f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("min"));
			if (b && !f4.IsZero(false))
			{
				Min = f4;
			}
			f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("max"));
			if (b && !f4.IsZero(false))
			{
				Max = f4;
			}

		}
	}
}
