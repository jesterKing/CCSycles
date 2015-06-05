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
	public class DiffuseBsdfInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public Float4Socket Normal { get; set; }

		public DiffuseBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness");
			AddSocket(Roughness);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	public class DiffuseBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public DiffuseBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}
	
	/// <summary>
	/// A Diffuse BSDF closure.
	/// This closure takes two inputs, <c>Color</c> and <c>Roughness</c>. The result
	/// will be a regular diffuse shading.
	/// 
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("diffuse_bsdf")]
	public class DiffuseBsdfNode : ShaderNode
	{
		public DiffuseBsdfInputs ins { get { return (DiffuseBsdfInputs)inputs; } }
		public DiffuseBsdfOutputs outs { get { return (DiffuseBsdfOutputs)outputs; } }
		/// <summary>
		/// Create a new Diffuse BSDF closure.
		/// </summary>
		public DiffuseBsdfNode() : this("a diffuse bsdf node") { }
		public DiffuseBsdfNode(string name) :
			base(ShaderNodeType.Diffuse, name)
		{
			inputs = new DiffuseBsdfInputs(this);
			outputs = new DiffuseBsdfOutputs(this);
			ins.Color.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Roughness.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
