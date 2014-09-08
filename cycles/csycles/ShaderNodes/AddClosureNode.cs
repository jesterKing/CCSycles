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

/**
 * \defgroup cclshadernodes CSycles Shader Nodes
 */

/**
 * \ingroup cclshadernodes
 *  This pacakage contains the high-level <c>ccl.ShaderNode</c>s to be used in a <c>ccl.Shader</c>.
 *  
 *  The classes give a clear and concise way of setting up <c>ccl.ShaderNode</c>s with a minimum of
 *  fuss.
 */

namespace ccl.ShaderNodes
{
	public class AddClosureInputs : Inputs
	{
		public ClosureSocket Closure1 { get; set; }
		public ClosureSocket Closure2 { get; set; }

		public AddClosureInputs(ShaderNode parentNode)
		{
			Closure1 = new ClosureSocket(parentNode, "Closure1");
			AddSocket(Closure1);
			Closure2 = new ClosureSocket(parentNode, "Closure2");
			AddSocket(Closure2);
		}
	}

	public class AddClosureOutputs : Outputs
	{
		public ClosureSocket Closure { get; set; }

		public AddClosureOutputs(ShaderNode parentNode)
		{
			Closure = new ClosureSocket(parentNode, "Closure");
			AddSocket(Closure);
		}
	}
	/// <summary>
	/// An Add closure.
	/// This closure takes two inputs, <c>Closure1</c> and <c>Closure2</c>. These
	/// will be simply added together.
	/// 
	/// There is one output <c>Closure</c>
	/// </summary>
	public class AddClosureNode : ShaderNode
	{
		public AddClosureInputs ins { get { return (AddClosureInputs)inputs; } set { inputs = value; } }
		public AddClosureOutputs outs { get { return (AddClosureOutputs)outputs; } set { outputs = value; }}
		/// <summary>
		/// Create a new Add closure.
		/// </summary>
		public AddClosureNode() :
			base(ShaderNodeType.AddClosure)
		{
			inputs = new AddClosureInputs(this);
			outputs = new AddClosureOutputs(this);
		}
	}
}
