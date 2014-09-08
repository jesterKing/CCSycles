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
	public class MixClosureInputs : Inputs
	{
		public FloatSocket Fac { get; set; }
		public ClosureSocket Closure1 { get; set; }
		public ClosureSocket Closure2 { get; set; }

		public MixClosureInputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
			Closure1 = new ClosureSocket(parentNode, "Closure1");
			AddSocket(Closure1);
			Closure2 = new ClosureSocket(parentNode, "Closure2");
			AddSocket(Closure2);
		}
	}

	public class MixClosureOutputs : Outputs
	{
		public ClosureSocket Closure { get; set; }

		public MixClosureOutputs(ShaderNode parentNode)
		{
			Closure = new ClosureSocket(parentNode, "Closure");
			AddSocket(Closure);
		}
	}

	public class MixClosureNode : ShaderNode
	{
		public MixClosureInputs ins { get { return (MixClosureInputs)inputs; } set { inputs = value; } }
		public MixClosureOutputs outs { get { return (MixClosureOutputs)outputs; } set { outputs = value; } }

		public MixClosureNode() :
			base(ShaderNodeType.MixClosure)
		{
			ins = new MixClosureInputs(this);
			outs = new MixClosureOutputs(this);
			ins.Fac.Value = 0.5f;
		}

	}
}
