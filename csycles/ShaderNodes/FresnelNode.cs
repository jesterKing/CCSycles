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
	public class FresnelInputs : Inputs
	{
		public Float4Socket Normal { get; set; }
		public FloatSocket IOR { get; set; }

		internal FresnelInputs(ShaderNode parentNode)
		{
			IOR = new FloatSocket(parentNode, "IOR");
			AddSocket(IOR);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class FresnelOutputs : Outputs
	{
		public FloatSocket Fac { get; set; }

		internal FresnelOutputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("fresnel")]
	public class FresnelNode : ShaderNode
	{
		public FresnelInputs ins { get { return (FresnelInputs)inputs; } }
		public FresnelOutputs outs { get { return (FresnelOutputs)outputs; } }
		public FresnelNode() : this("a fresnel input node") { }
		public FresnelNode(string name) :
			base(ShaderNodeType.Fresnel, name)
		{
			inputs = new FresnelInputs(this);
			outputs = new FresnelOutputs(this);
			ins.IOR.Value = 1.45f;
			ins.Normal.Value = new float4();
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.IOR, xmlNode.GetAttribute("ior"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
		}
	}
}
