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

namespace ccl.ShaderNodes
{
	public class MixInputs : Inputs
	{
		public FloatSocket Fac { get; set; }
		public Float4Socket Color1 { get; set; }
		public Float4Socket Color2 { get; set; }

		internal MixInputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
			Color1 = new Float4Socket(parentNode, "Color1");
			AddSocket(Color1);
			Color2 = new Float4Socket(parentNode, "Color2");
			AddSocket(Color2);
		}
	}

	public class MixOutputs : Outputs
	{
		public Float4Socket Color { get; set; }

		internal MixOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	[ShaderNode("mix")]
	public class MixNode : ShaderNode
	{
		public MixInputs ins { get { return (MixInputs)inputs; } }
		public MixOutputs outs { get { return (MixOutputs)outputs; } }

		/// <summary>
		/// Create new MixNode with blend type Mix.
		/// </summary>
		public MixNode() : this("a mix color node")
		{
		}

		public MixNode(string name) :
			base(ShaderNodeType.Mix, name)
		{
			inputs = new MixInputs(this);
			outputs = new MixOutputs(this);

			BlendType = "Mix";
			ins.Fac.Value = 0.5f;
			ins.Color1.Value = new float4();
			ins.Color2.Value = new float4();
		}

		public string BlendType { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "blend", BlendType);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color1, xmlNode.GetAttribute("color1"));
			Utilities.Instance.get_float4(ins.Color2, xmlNode.GetAttribute("color2"));
			Utilities.Instance.get_float(ins.Fac, xmlNode.GetAttribute("fac"));
		}
	}
}
