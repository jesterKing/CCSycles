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
	public class RefractionBsdfInputs : Inputs
	{
		public FloatSocket IOR { get; set; }
		public FloatSocket Roughness { get; set; }
		public Float4Socket Color { get; set; }
		public Float4Socket Normal { get; set; }

		internal RefractionBsdfInputs(ShaderNode parentNode)
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

	public class RefractionBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		internal RefractionBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}

	public class RefractionBsdfNode : ShaderNode
	{
		public enum RefractionDistribution
		{
			Sharp,
			Beckmann,
			GGX
		}
		public RefractionBsdfInputs ins { get { return (RefractionBsdfInputs)inputs; } }
		public RefractionBsdfOutputs outs { get { return (RefractionBsdfOutputs)outputs; } }

		public RefractionBsdfNode() : this("a refraction bsdf node") { }
		public RefractionBsdfNode(string name) :
			base(ShaderNodeType.Refraction, name)
		{
			inputs = new RefractionBsdfInputs(this);
			outputs = new RefractionBsdfOutputs(this);

			Distribution = RefractionDistribution.Beckmann;
			ins.IOR.Value = 1.45f;
			ins.Roughness.Value = 0.0f;
			ins.Color.Value = new float4(0.8f);
		}

		public RefractionDistribution Distribution { get; set; }

		public void SetDistribution(string dist)
		{
			Distribution = (RefractionDistribution) Enum.Parse(typeof (RefractionDistribution), dist, true);
		}

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "distribution", Distribution.ToString());
		}
	}
}
