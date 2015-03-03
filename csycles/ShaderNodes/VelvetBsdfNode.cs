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
	public class VelvetBsdfInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Sigma { get; set; }
		public Float4Socket Normal { get; set; }

		public VelvetBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Sigma = new FloatSocket(parentNode, "Sigma");
			AddSocket(Sigma);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class VelvetBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public VelvetBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}
	
	/// <summary>
	/// A Velvet BSDF closure.
	/// This closure takes two inputs, <c>Color</c> and <c>Sigma</c>. The result
	/// will be a regular diffuse shading.
	/// 
	/// There is one output <c>Closure</c>
	/// </summary>
	public class VelvetBsdfNode : ShaderNode
	{
		public VelvetBsdfInputs ins { get { return (VelvetBsdfInputs)inputs; } set { inputs = value; } }
		public VelvetBsdfOutputs outs { get { return (VelvetBsdfOutputs)outputs; } set { outputs = value; }}
		/// <summary>
		/// Create a new Velvet BSDF closure.
		/// </summary>
		public VelvetBsdfNode() :
			base(ShaderNodeType.Velvet)
		{
			inputs = new VelvetBsdfInputs(this);
			outputs = new VelvetBsdfOutputs(this);
			ins.Color.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Sigma.Value = 0.0f;
		}
	}
}
