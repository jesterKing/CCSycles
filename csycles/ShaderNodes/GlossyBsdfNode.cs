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
using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	public class GlossyInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public Float4Socket Normal { get; set; }

		internal GlossyInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness");
			AddSocket(Roughness);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class GlossyOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		internal GlossyOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}

	public class GlossyBsdfNode : ShaderNode
	{

		public enum GlossyDistribution
		{
			Sharp,
			Beckmann,
			GGX,
			Asihkmin_Shirley,
			Phong
		}

		public GlossyInputs ins { get { return (GlossyInputs)inputs; } }
		public GlossyOutputs outs { get { return (GlossyOutputs)outputs; } }

		public GlossyBsdfNode() : this("a glossy bsdf node") { }
		public GlossyBsdfNode(string name) :
			base(ShaderNodeType.Glossy, name)
		{
			inputs = new GlossyInputs(this);
			outputs = new GlossyOutputs(this);
			Distribution = GlossyDistribution.Beckmann;
			ins.Color.Value = new float4();
			ins.Roughness.Value = 0.0f;
		}

		public GlossyDistribution Distribution { get; set; }

		public void SetDistribution(string dist)
		{
			dist = dist.Replace("-", "_");
			Distribution = (GlossyDistribution)Enum.Parse(typeof(GlossyDistribution), dist, true);
		}

		private string GlossyToString(GlossyDistribution dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", "-");
			return str;
		}

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, GlossyToString(Distribution));
		}
	}
}
