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
		public FloatSocket R { get; set; }
		public FloatSocket G { get; set; }
		public FloatSocket B { get; set; }

		public SeparateHsvOutputs(ShaderNode parentNode)
		{
			R = new FloatSocket(parentNode, "Val");
			AddSocket(R);
			G = new FloatSocket(parentNode, "Val");
			AddSocket(G);
			B = new FloatSocket(parentNode, "Val");
			AddSocket(B);
		}
	}

	public class SeparateHsvNode : ShaderNode
	{
		public SeparateHsvInputs ins { get { return (SeparateHsvInputs)inputs; } set { inputs = value; } }
		public SeparateHsvOutputs outs { get { return (SeparateHsvOutputs)outputs; } set { outputs = value; } }

		/// <summary>
		/// Create new Separate HSV node.
		/// </summary>
		public SeparateHsvNode() :
			base(ShaderNodeType.SeparateHsv)
		{
			ins = new SeparateHsvInputs(this);
			outs = new SeparateHsvOutputs(this);
		}
	}
}
