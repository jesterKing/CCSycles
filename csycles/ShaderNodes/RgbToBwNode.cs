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
	public class ConvertRgbInputs : Inputs
	{
		public Float4Socket Color { get; set; }

		public ConvertRgbInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class ConvertValOutputs : Outputs
	{
		public FloatSocket Val { get; set; }

		public ConvertValOutputs(ShaderNode parentNode)
		{
			Val = new FloatSocket(parentNode, "Val");
			AddSocket(Val);
		}
	}

	public class RgbToBwNode : ShaderNode
	{
		public ConvertRgbInputs ins { get { return (ConvertRgbInputs)inputs; } set { inputs = value; } }
		public ConvertValOutputs outs { get { return (ConvertValOutputs)outputs; } set { outputs = value; } }

		/// <summary>
		/// Create new ConvertNode with blend type RgbToBw.
		/// </summary>
		public RgbToBwNode() :
			base(ShaderNodeType.RgbToBw)
		{
			ins = new ConvertRgbInputs(this);
			outs = new ConvertValOutputs(this);
		}
	}
}
