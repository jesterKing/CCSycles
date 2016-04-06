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
	public class AnisotropicBsdfInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public Float4Socket Tangent { get; set; }
		public Float4Socket Normal { get; set; }
		public FloatSocket Roughness { get; set; }
		public FloatSocket Anisotropy { get; set; }
		public FloatSocket Rotation { get; set; }

		public AnisotropicBsdfInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Tangent = new Float4Socket(parentNode, "Tangent");
			AddSocket(Tangent);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
			Roughness = new FloatSocket(parentNode, "Roughness");
			AddSocket(Roughness);
			Anisotropy = new FloatSocket(parentNode, "Anisotropy");
			AddSocket(Anisotropy);
			Rotation = new FloatSocket(parentNode, "Rotation");
			AddSocket(Rotation);
		}
	}

	public class AnisotropicBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public AnisotropicBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF");
			AddSocket(BSDF);
		}
	}
	
	/// <summary>
	/// A Anisotropic BSDF closure.
	/// 
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("anisotropic_bsdf")]
	public class AnisotropicBsdfNode : ShaderNode
	{
		public enum AnisotropicDistribution
		{
			Beckmann,
			GGX,
			Asihkmin_Shirley,
		}
		public AnisotropicBsdfInputs ins { get { return (AnisotropicBsdfInputs)inputs; } }
		public AnisotropicBsdfOutputs outs { get { return (AnisotropicBsdfOutputs)outputs; } }
		/// <summary>
		/// Create a new Anisotropic BSDF closure.
		/// </summary>
		public AnisotropicBsdfNode() : this("a anisotropic bsdf node") { }
		public AnisotropicBsdfNode(string name) :
			base(ShaderNodeType.Anisotropic, name)
		{
			inputs = new AnisotropicBsdfInputs(this);
			outputs = new AnisotropicBsdfOutputs(this);
			ins.Color.Value = new float4(1.0f);
			ins.Tangent.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Roughness.Value = 0.2f;
			ins.Anisotropy.Value = 0.5f;
			ins.Rotation.Value = 0.0f;

			Distribution = AnisotropicDistribution.GGX;
		}
		public void SetDistribution(string dist)
		{
			dist = dist.Replace("-", "_");
			Distribution = (AnisotropicDistribution)System.Enum.Parse(typeof(AnisotropicDistribution), dist, true);
		}

		private string AnisotropicToString(AnisotropicDistribution dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", "-");
			return str;
		}

		AnisotropicDistribution Distribution { get; set; }
		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "distribution", AnisotropicToString(Distribution));
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
			Utilities.Instance.get_float4(ins.Tangent, xmlNode.GetAttribute("tangent"));
			Utilities.Instance.get_float(ins.Roughness, xmlNode.GetAttribute("roughness"));
			Utilities.Instance.get_float(ins.Anisotropy, xmlNode.GetAttribute("anisotropy"));
			Utilities.Instance.get_float(ins.Rotation, xmlNode.GetAttribute("rotation"));
			var anidistribution = xmlNode.GetAttribute("distribution");
			if (!string.IsNullOrEmpty(anidistribution))
			{
				SetDistribution(anidistribution);
			}

		}
	}
}
