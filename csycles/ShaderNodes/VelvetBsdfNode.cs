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

using System.Xml;
using ccl.ShaderNodes.Sockets;
using ccl.Attributes;

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
	[ShaderNode("velvet_bsdf")]
	public class VelvetBsdfNode : ShaderNode
	{
		public VelvetBsdfInputs ins { get { return (VelvetBsdfInputs)inputs; } }
		public VelvetBsdfOutputs outs { get { return (VelvetBsdfOutputs)outputs; } }
		/// <summary>
		/// Create a new Velvet BSDF closure.
		/// </summary>
		public VelvetBsdfNode() : this("a velvet bsdf node") { }
		public VelvetBsdfNode(string name) :
			base(ShaderNodeType.Velvet, name)
		{
			inputs = new VelvetBsdfInputs(this);
			outputs = new VelvetBsdfOutputs(this);
			ins.Color.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Sigma.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Sigma, xmlNode.GetAttribute("sigma"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
		}
	}
}
