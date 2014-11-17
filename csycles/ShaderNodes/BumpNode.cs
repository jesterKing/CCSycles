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
	public class BumpInputs : Inputs
	{
		public FloatSocket Height { get; set; }
		public Float4Socket Normal { get; set; }
		public FloatSocket Strength { get; set; }
		public FloatSocket Distance { get; set; }

		public BumpInputs(ShaderNode parentNode)
		{
			Height = new FloatSocket(parentNode, "Height");
			AddSocket(Height);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
			Strength = new FloatSocket(parentNode, "Strength");
			AddSocket(Strength);
			Distance = new FloatSocket(parentNode, "Distance");
			AddSocket(Distance);
		}
	}

	public class BumpOutputs : Outputs
	{
		public Float4Socket Normal { get; set; }

		public BumpOutputs(ShaderNode parentNode)
		{
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class BumpNode : ShaderNode
	{
		public BumpInputs ins { get { return (BumpInputs)inputs; } set { inputs = value; } }
		public BumpOutputs outs { get { return (BumpOutputs)outputs; } set { outputs = value; } }

		/// <summary>
		/// Create new BumpNode with blend type Bump.
		/// </summary>
		public BumpNode() :
			base(ShaderNodeType.Bump)
		{
			ins = new BumpInputs(this);
			outs = new BumpOutputs(this);

			Invert = false;
			ins.Strength.Value = 1.0f;
			ins.Distance.Value = 0.15f;
		}

		public bool Invert { get; set; }

	}
}
