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
	public class CheckerInputs : Inputs
	{
		public Float4Socket Vector { get; set; }
		public Float4Socket Color1 { get; set; }
		public Float4Socket Color2 { get; set; }
		public FloatSocket Scale { get; set; }

		public CheckerInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
			Color1 = new Float4Socket(parentNode, "Color1");
			AddSocket(Color1);
			Color2 = new Float4Socket(parentNode, "Color2");
			AddSocket(Color2);
			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
		}
	}

	public class CheckerOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public CheckerOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("checker_texture")]
	public class CheckerTexture : TextureNode
	{
		public CheckerInputs ins { get { return (CheckerInputs)inputs; } }
		public CheckerOutputs outs { get { return (CheckerOutputs)outputs; } }
		public CheckerTexture() : this("a checker texture") { }
		public CheckerTexture(string name)
			: base(ShaderNodeType.CheckerTexture, name)
		{
			inputs = new CheckerInputs(this);
			outputs = new CheckerOutputs(this);
			ins.Color1.Value = new float4(1.0f, 0.5f, 0.25f);
			ins.Color2.Value = new float4(0.25f, 0.5f, 1.0f);
			ins.Scale.Value = 5.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color1, xmlNode.GetAttribute("Color1"));
			Utilities.Instance.get_float4(ins.Color2, xmlNode.GetAttribute("Color2"));
		}
	}
}
