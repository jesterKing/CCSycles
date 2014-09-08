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

using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	public class RefractionBsdfInputs : Inputs
	{
		public FloatSocket IOR { get; set; }
		public FloatSocket Roughness { get; set; }
		public Float4Socket Color { get; set; }

		public RefractionBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness");
			AddSocket(Roughness);
			IOR = new FloatSocket(parentNode, "IOR");
			AddSocket(IOR);
		}
	}

	public class RefractionBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public RefractionBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}

	public class RefractionBsdfNode : ShaderNode
	{
		public RefractionBsdfInputs ins { get { return (RefractionBsdfInputs)inputs; } set { inputs = value; } }
		public RefractionBsdfOutputs outs { get { return (RefractionBsdfOutputs)outputs; } set { outputs = value; } }

		public RefractionBsdfNode() :
			base(ShaderNodeType.Refraction)
		{
			ins = new RefractionBsdfInputs(this);
			outs = new RefractionBsdfOutputs(this);

			Distribution = "Beckmann";
			ins.IOR.Value = 1.45f;
			ins.Roughness.Value = 0.0f;
			ins.Color.Value = new float4(0.8f);
		}

		public string Distribution { get; set; }
	}
}
