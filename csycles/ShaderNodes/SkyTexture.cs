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
	public class SkyInputs : Inputs
	{
		public Float4Socket Vector { get; set; }

		public SkyInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	public class SkyOutputs : Outputs
	{
		public Float4Socket Color { get; set; }

		public SkyOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	[ShaderNode("sky_texture")]
	public class SkyTexture : TextureNode
	{
		public SkyInputs ins { get { return (SkyInputs)inputs; } }
		public SkyOutputs outs { get { return (SkyOutputs)outputs; } }

		public SkyTexture() : this("a sky texture") { }
		public SkyTexture(string name)
			: base(ShaderNodeType.SkyTexture, name)
		{
			inputs = new SkyInputs(this);
			outputs = new SkyOutputs(this);

			SunDirection = new float4(0.0f, 0.0f, 1.0f);
			Turbidity = 2.2f;
			GroundAlbedo = 0.3f;
			SkyType = "Hosek / Wilkie";
		}

		public float4 SunDirection { get; set; }
		public float Turbidity { get; set; }
		public float GroundAlbedo { get; set; }

		/// <summary>
		/// One of:
		/// - Preetham
		/// - Hosek / Wilkie
		/// </summary>
		public string SkyType { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "sky", SkyType);
		}

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "turbidity", Turbidity);
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "ground_albedo", GroundAlbedo);
			var sd = SunDirection;
			CSycles.shadernode_set_member_vec(clientId, shaderId, Id, Type, "sun_direction", sd.x, sd.y, sd.z);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));

			var sun_direction = xmlNode.GetAttribute("sun_direction");
			var turbidity = xmlNode.GetAttribute("turbidity");
			var ground_albedo = xmlNode.GetAttribute("ground_albedo");
			var sky_type = xmlNode.GetAttribute("type");

			if (!string.IsNullOrEmpty(sun_direction))
			{
				Utilities.Instance.get_float4(SunDirection, sun_direction);
			}
			if (!string.IsNullOrEmpty(turbidity))
			{
				var turb = 0.0f;
				if(Utilities.Instance.get_float(ref turb, turbidity)) Turbidity = turb;
			}
			if (!string.IsNullOrEmpty(ground_albedo))
			{
				var ground_alb = 0.0f;
				if (Utilities.Instance.get_float(ref ground_alb, ground_albedo)) GroundAlbedo = ground_alb;
			}
			if (!string.IsNullOrEmpty(sky_type))
			{
				SkyType = sky_type;
			}
		}
	}
}
