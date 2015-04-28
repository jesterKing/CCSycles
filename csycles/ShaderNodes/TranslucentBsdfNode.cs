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
	public class TranslucentBsdfInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public Float4Socket Normal { get; set; }

		internal TranslucentBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);

		}
	}

	public class TranslucentBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		internal TranslucentBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}
	
	/// <summary>
	/// A Translucent BSDF closure.

	/// There is one output <c>BSDF</c>
	/// </summary>
	public class TranslucentBsdfNode : ShaderNode
	{
		public TranslucentBsdfInputs ins { get { return (TranslucentBsdfInputs)inputs; } }
		public TranslucentBsdfOutputs outs { get { return (TranslucentBsdfOutputs)outputs; } }
		/// <summary>
		/// Create a new Translucent BSDF closure. Default Color is white
		/// </summary>
		public TranslucentBsdfNode() : this("a translucent bsdf node") { }
		/// <summary>
		/// Create a new Translucent BSDF closure. Default Color is white
		/// </summary>
		public TranslucentBsdfNode(string name) :
			base(ShaderNodeType.Translucent, name)
		{
			inputs = new TranslucentBsdfInputs(this);
			outputs = new TranslucentBsdfOutputs(this);
			ins.Color.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
		}
	}
}
