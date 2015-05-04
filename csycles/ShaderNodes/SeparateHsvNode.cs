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

namespace ccl.ShaderNodes
{
	public class SeparateHsvInputs : Inputs
	{
		public Float4Socket Color { get; set; }

		public SeparateHsvInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class SeparateHsvOutputs : Outputs
	{
		public FloatSocket H { get; set; }
		public FloatSocket S { get; set; }
		public FloatSocket V { get; set; }

		public SeparateHsvOutputs(ShaderNode parentNode)
		{
			H = new FloatSocket(parentNode, "H");
			AddSocket(H);
			S = new FloatSocket(parentNode, "S");
			AddSocket(S);
			V = new FloatSocket(parentNode, "V");
			AddSocket(V);
		}
	}

	public class SeparateHsvNode : ShaderNode
	{
		public SeparateHsvInputs ins { get { return (SeparateHsvInputs)inputs; } }
		public SeparateHsvOutputs outs { get { return (SeparateHsvOutputs)outputs; } }

		/// <summary>
		/// Create new Separate HSV node.
		/// </summary>
		public SeparateHsvNode() : this("a separate HSV node") { }
		public SeparateHsvNode(string name) :
			base(ShaderNodeType.SeparateHsv, name)
		{
			inputs = new SeparateHsvInputs(this);
			outputs = new SeparateHsvOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
