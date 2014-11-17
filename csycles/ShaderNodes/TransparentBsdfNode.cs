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
	public class TransparentBsdfInputs : Inputs
	{
		public Float4Socket Color { get; set; }

		public TransparentBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class TransparentBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public TransparentBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}
	
	/// <summary>
	/// A Transparent BSDF closure.

	/// There is one output <c>BSDF</c>
	/// </summary>
	public class TransparentBsdfNode : ShaderNode
	{
		public TransparentBsdfInputs ins { get { return (TransparentBsdfInputs)inputs; } set { inputs = value; } }
		public TransparentBsdfOutputs outs { get { return (TransparentBsdfOutputs)outputs; } set { outputs = value; }}
		/// <summary>
		/// Create a new Transparent BSDF closure.
		/// </summary>
		public TransparentBsdfNode() :
			base(ShaderNodeType.Transparent)
		{
			inputs = new TransparentBsdfInputs(this);
			outputs = new TransparentBsdfOutputs(this);
			ins.Color.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
