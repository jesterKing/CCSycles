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
	public class OutputInputs : Inputs
	{
		public ClosureSocket Surface { get; set; }
		public ClosureSocket Volume { get; set; }
		public FloatSocket Displacement { get; set; }

		public OutputInputs(ShaderNode parentNode)
		{
			Surface = new ClosureSocket(parentNode, "Surface");
			AddSocket(Surface);
			Volume = new ClosureSocket(parentNode, "Volume");
			AddSocket(Volume);
			Displacement = new FloatSocket(parentNode, "Displacement");
			AddSocket(Displacement);
		}
	}

	public class OutputOutputs : Outputs
	{
		public OutputOutputs(ShaderNode parentNode)
		{
			
		}
	}

	public class OutputNode : ShaderNode
	{
		public OutputInputs ins { get { return (OutputInputs) inputs; } set { inputs = value; } }
		public OutputOutputs outs { get { return (OutputOutputs) outputs; } set { outputs = value; } }

		public OutputNode() :
			base(ShaderNodeType.AddClosure)
		{
			ins = new OutputInputs(this);
			outs = new OutputOutputs(this);
		}
	}

}
