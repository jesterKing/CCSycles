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
	public class SeparateXyzInputs : Inputs
	{
		public Float4Socket Vector { get; set; }

		public SeparateXyzInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	public class SeparateXyzOutputs : Outputs
	{
		public FloatSocket X { get; set; }
		public FloatSocket Y { get; set; }
		public FloatSocket Z { get; set; }

		public SeparateXyzOutputs(ShaderNode parentNode)
		{
			X = new FloatSocket(parentNode, "X");
			AddSocket(X);
			Y = new FloatSocket(parentNode, "Y");
			AddSocket(Y);
			Z = new FloatSocket(parentNode, "Z");
			AddSocket(Z);
		}
	}

	/// <summary>
	/// Add a Separate XYZ node, converting a vector input to single X Y Z scalar nodes
	/// </summary>
	[ShaderNode("separate_xyz")]
	public class SeparateXyzNode : ShaderNode
	{
		public SeparateXyzInputs ins { get { return (SeparateXyzInputs)inputs; } }
		public SeparateXyzOutputs outs { get { return (SeparateXyzOutputs)outputs; } }

		public SeparateXyzNode() : this("a separate XYZ node") { }
		public SeparateXyzNode(string name) :
			base(ShaderNodeType.SeparateXyz, name)
		{
			inputs = new SeparateXyzInputs(this);
			outputs = new SeparateXyzOutputs(this);

			ins.Vector.Value = new float4(0.0f);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
		}
	}
}
