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
	public class LayerWeightInputs : Inputs
	{
		public FloatSocket Blend { get; set; }
		public Float4Socket Normal { get; set; }

		public LayerWeightInputs(ShaderNode parentNode)
		{
			Blend = new FloatSocket(parentNode, "Blend");
			AddSocket(Blend);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class LayerWeightOutputs : Outputs
	{
		public FloatSocket Fresnel { get; set; }
		public FloatSocket Facing { get; set; }

		public LayerWeightOutputs(ShaderNode parentNode)
		{
			Fresnel = new FloatSocket(parentNode, "Fresnel");
			AddSocket(Fresnel);
			Facing = new FloatSocket(parentNode, "Facing");
			AddSocket(Facing);
		}
	}

	public class LayerWeightNode : ShaderNode
	{
		public LayerWeightInputs ins { get { return (LayerWeightInputs)inputs; } set { inputs = value; } }
		public LayerWeightOutputs outs { get { return (LayerWeightOutputs)outputs; } set { outputs = value; }}

		public LayerWeightNode()
			: base(ShaderNodeType.LayerWeight)
		{
			inputs = new LayerWeightInputs(this);
			outputs = new LayerWeightOutputs(this);

			ins.Normal.Value = new float4(0.0f);
			ins.Blend.Value = 0.5f;
		}
	}
}
