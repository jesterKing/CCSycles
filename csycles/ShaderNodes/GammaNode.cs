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
	public class GammaInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Gamma { get; set; }

		public GammaInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Gamma = new FloatSocket(parentNode, "Gamma");
			AddSocket(Gamma);
		}
	}

	public class GammaOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public GammaOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	[ShaderNode("gamma")]
	public class GammaNode : ShaderNode
	{
		public GammaInputs ins { get { return (GammaInputs)inputs; } }
		public GammaOutputs outs { get { return (GammaOutputs)outputs; } }

		public GammaNode() : this("a gamma node") {}
		public GammaNode(string name)
			: base(ShaderNodeType.Gamma, name)
		{
			inputs = new GammaInputs(this);
			outputs = new GammaOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Gamma.Value = 1.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Gamma, xmlNode.GetAttribute("gamma"));
		}
	}
}
