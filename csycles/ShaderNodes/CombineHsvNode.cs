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
	public class CombineHsvInputs : Inputs
	{
		public FloatSocket H { get; set; }
		public FloatSocket S { get; set; }
		public FloatSocket V { get; set; }

		public CombineHsvInputs(ShaderNode parentNode)
		{
			H = new FloatSocket(parentNode, "H");
			AddSocket(H);
			S = new FloatSocket(parentNode, "S");
			AddSocket(S);
			V = new FloatSocket(parentNode, "V");
			AddSocket(V);
		}
	}

	public class CombineHsvOutputs : Outputs
	{
		public Float4Socket Image { get; set; }

		public CombineHsvOutputs(ShaderNode parentNode)
		{
			Image = new Float4Socket(parentNode, "Image");
			AddSocket(Image);
		}
	}

	/// <summary>
	/// Add a Combine HSV node, converting single H S V scalars to a vector output
	/// </summary>
	[ShaderNode("combine_hsv")]
	public class CombineHsvNode : ShaderNode
	{
		public CombineHsvInputs ins { get { return (CombineHsvInputs)inputs; } }
		public CombineHsvOutputs outs { get { return (CombineHsvOutputs)outputs; } }

		public CombineHsvNode() : this("A combine HSV node") { }
		public CombineHsvNode(string name) :
			base(ShaderNodeType.CombineHsv, name)
		{
			inputs = new CombineHsvInputs(this);
			outputs = new CombineHsvOutputs(this);

			ins.H.Value = 0.0f;
			ins.S.Value = 0.0f;
			ins.V.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.H, xmlNode.GetAttribute("H"));
			Utilities.Instance.get_float(ins.S, xmlNode.GetAttribute("S"));
			Utilities.Instance.get_float(ins.V, xmlNode.GetAttribute("V"));
		}
	}
}
