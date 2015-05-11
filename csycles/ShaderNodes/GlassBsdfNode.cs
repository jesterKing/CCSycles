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

namespace ccl.ShaderNodes
{
	public class GlassInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public FloatSocket IOR { get; set; }
		public Float4Socket Normal { get; set; }

		public GlassInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness");
			AddSocket(Roughness);
			IOR = new FloatSocket(parentNode, "IOR");
			AddSocket(IOR);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class GlassOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public GlassOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}

	public class GlassBsdfNode : ShaderNode
	{

		public enum GlassDistribution
		{
			Sharp,
			Beckmann,
			GGX
		}

		public GlassInputs ins { get { return (GlassInputs)inputs; } }
		public GlassOutputs outs { get { return (GlassOutputs)outputs; } }
		public GlassBsdfNode() : this("a glass bsdf") { }
		public GlassBsdfNode(string name)
			: base(ShaderNodeType.Glass, name)
		{
			inputs = new GlassInputs(this);
			outputs = new GlassOutputs(this);

			Distribution = GlassDistribution.Beckmann;
		}

		public GlassDistribution Distribution { get; set; }

		public void SetDistribution(string dist)
		{
			Distribution = (GlassDistribution) Enum.Parse(typeof (GlassDistribution), dist, true);
		}

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "distribution", Distribution.ToString());
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
			Utilities.Instance.get_float(ins.Roughness, xmlNode.GetAttribute("roughness"));
			Utilities.Instance.get_float(ins.IOR, xmlNode.GetAttribute("ior"));
			var glassdistribution = xmlNode.GetAttribute("distribution");
			if (!string.IsNullOrEmpty(glassdistribution))
			{
				SetDistribution(glassdistribution);
			}
		}
	}
}
