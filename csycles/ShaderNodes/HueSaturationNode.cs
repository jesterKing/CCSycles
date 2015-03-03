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
	public class HueSaturationInputs : Inputs
	{
		public FloatSocket Hue { get; set; }
		public FloatSocket Saturation { get; set; }
		public FloatSocket Value { get; set; }
		public FloatSocket Fac { get; set; }
		public Float4Socket Color { get; set; }

		public HueSaturationInputs(ShaderNode parentNode)
		{
			Hue = new FloatSocket(parentNode, "Hue");
			AddSocket(Hue);
			Saturation = new FloatSocket(parentNode, "Saturation");
			AddSocket(Saturation);
			Value = new FloatSocket(parentNode, "Value");
			AddSocket(Value);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class HueSaturationOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public HueSaturationOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class HueSaturationNode : ShaderNode
	{
		public HueSaturationInputs ins { get { return (HueSaturationInputs)inputs; } set { inputs = value; } }
		public HueSaturationOutputs outs { get { return (HueSaturationOutputs)outputs; } set { outputs = value; }}

		public HueSaturationNode()
			: base(ShaderNodeType.HueSaturation)
		{
			inputs = new HueSaturationInputs(this);
			outputs = new HueSaturationOutputs(this);

			ins.Hue.Value = 0.5f;
			ins.Saturation.Value = 1.0f;
			ins.Value.Value = 1.0f;
			ins.Fac.Value = 1.0f;
			ins.Color.Value = new float4(0.8f);
		}
	}
}
