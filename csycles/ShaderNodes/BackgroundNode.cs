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
	public class BackgroundInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Strength { get; set; }

		public BackgroundInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Strength = new FloatSocket(parentNode, "Strength");
			AddSocket(Strength);
		}
	}

	public class BackgroundOutputs : Outputs
	{
		public ClosureSocket Background { get; set; }

		public BackgroundOutputs(ShaderNode parentNode)
		{
			Background = new ClosureSocket(parentNode, "Background");
			AddSocket(Background);
		}
	}
	public class BackgroundNode : ShaderNode
	{
		public BackgroundInputs ins { get { return (BackgroundInputs)inputs; } set { inputs = value; } }
		public BackgroundOutputs outs { get { return (BackgroundOutputs)outputs; } set { outputs = value; }}
		/// <summary>
		/// Create a new Add closure.
		/// </summary>
		public BackgroundNode() :
			base(ShaderNodeType.Background)
		{
			/*AvailableOuts.Add("Background");*/
			inputs = new BackgroundInputs(this);
			outputs = new BackgroundOutputs(this);
			ins.Color.Value = new float4();
			ins.Strength.Value = 1.0f;
		}
	}
}
