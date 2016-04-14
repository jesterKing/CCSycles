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

using System.Xml;
using ccl.ShaderNodes.Sockets;
using ccl.Attributes;
using System;

namespace ccl.ShaderNodes
{
	public class SubsurfaceScatteringInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Sharpness { get; set; }
		public FloatSocket TextureBlur { get; set; }
		public Float4Socket Radius { get; set; }
		public Float4Socket Normal { get; set; }

		public SubsurfaceScatteringInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
			Sharpness = new FloatSocket(parentNode, "Sharpness");
			AddSocket(Sharpness);
			TextureBlur = new FloatSocket(parentNode, "Texture Blur");
			AddSocket(TextureBlur);
			Radius = new Float4Socket(parentNode, "Radius");
			AddSocket(Radius);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class SubsurfaceScatteringOutputs : Outputs
	{
		public ClosureSocket BSSRDF { get; set; }

		public SubsurfaceScatteringOutputs(ShaderNode parentNode)
		{
			BSSRDF = new ClosureSocket(parentNode, "BSSRDF");
			AddSocket(BSSRDF);
		}
	}
	
	/// <summary>
	/// A scatter volume node.
	/// </summary>
	[ShaderNode("subsurface_scattering")]
	public class SubsurfaceScatteringNode : ShaderNode
	{
		public enum FalloffTypes
		{
			Cubic,
			Guassian,
			Burley
		}

		public SubsurfaceScatteringInputs ins { get { return (SubsurfaceScatteringInputs)inputs; } }
		public SubsurfaceScatteringOutputs outs { get { return (SubsurfaceScatteringOutputs)outputs; } }
		/// <summary>
		/// Create a new Scatter volume node
		/// </summary>
		public SubsurfaceScatteringNode() : this("a subsurface scattering node") { }
		public SubsurfaceScatteringNode(string name) :
			base(ShaderNodeType.SubsurfaceScattering, name)
		{
			inputs = new SubsurfaceScatteringInputs(this);
			outputs = new SubsurfaceScatteringOutputs(this);
			ins.Color.Value = new float4(1.0f);
			ins.Scale.Value = 1.0f;
			ins.Sharpness.Value = 0.0f;
			ins.TextureBlur.Value = 0.0f;
			ins.Radius.Value = new float4(1.0f);
			ins.Normal.Value = new float4(0.0f);
			Falloff = FalloffTypes.Cubic;
		}

		public FalloffTypes Falloff { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "falloff", Falloff.ToString());
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Sharpness, xmlNode.GetAttribute("sharpness"));
			Utilities.Instance.get_float(ins.TextureBlur, xmlNode.GetAttribute("texture_blur"));

			var falloff = xmlNode.GetAttribute("falloff");
			if(!string.IsNullOrEmpty(falloff))
			{
				FalloffTypes ft = FalloffTypes.Cubic;
				if(Enum.TryParse(falloff, out ft))
				{
					Falloff = ft;
				}
			}
		}
	}
}
