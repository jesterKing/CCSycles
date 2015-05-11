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

namespace ccl.ShaderNodes
{
	/// <summary>
	/// GradientTexture input sockets
	/// </summary>
	public class GradientInputs : Inputs
	{
		/// <summary>
		/// GradientTextureNode vector input
		/// </summary>
		public Float4Socket Vector { get; set; }

		internal GradientInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// GradientTexture output sockets
	/// </summary>
	public class GradientOutputs : Outputs
	{
		/// <summary>
		/// GradientTextureNode color output
		/// </summary>
		public Float4Socket Color { get; set; }
		/// <summary>
		/// GradientTextureNode factor output
		/// </summary>
		public FloatSocket Fac { get; set; }

		internal GradientOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	/// <summary>
	/// GradientTextureNode gives different types of gradient
	/// </summary>
	public class GradientTextureNode : TextureNode
	{
		/// <summary>
		/// Gradient types for GradientTextureNode
		/// 
		/// @todo implement all, currently only Linear supported
		/// </summary>
		public enum GradientType
		{
			/// <summary>
			/// Linear interpolation for gradient
			/// </summary>
			Linear,
			/// <summary>
			/// Quadratic interpolation for gradient
			/// </summary>
			Quadratic,
			/// <summary>
			/// Easing interpolation for gradient
			/// </summary>
			Easing,
			/// <summary>
			/// Diagonal interpolation for gradient
			/// </summary>
			Diagonal,
			/// <summary>
			/// Radial interpolation for gradient
			/// </summary>
			Radial,
			/// <summary>
			/// Quadratic sphere interpolation for gradient
			/// </summary>
			Quadratic_Sphere,
			/// <summary>
			/// Spherical interpolation for gradient
			/// </summary>
			Spherical,
		}

		/// <summary>
		/// GradientTextureNode input sockets
		/// </summary>
		public GradientInputs ins { get { return (GradientInputs)inputs; } }
		/// <summary>
		/// GradientTextureNode output sockets
		/// </summary>
		public GradientOutputs outs { get { return (GradientOutputs)outputs; } }

		/// <summary>
		/// Create GradientTextureNode
		/// </summary>
		public GradientTextureNode() : this("a gradient texture") { }
		public GradientTextureNode(string name)
			: base(ShaderNodeType.GradientTexture, name)
		{
			inputs = new GradientInputs(this);
			outputs = new GradientOutputs(this);
			Gradient = GradientType.Linear;
		}

		/// <summary>
		/// Get or set the gradient type
		/// </summary>
		public GradientType Gradient { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "gradient", Gradient.ToString().Replace('_', ' '));
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
		}
	}
}
