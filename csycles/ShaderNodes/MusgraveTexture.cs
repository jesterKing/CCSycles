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
	public class MusgraveInputs : Inputs
	{
		public Float4Socket Vector { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Detail { get; set; }
		public FloatSocket Dimension{ get; set; }
		public FloatSocket Lacunarity{ get; set; }
		public FloatSocket Offset{ get; set; }
		public FloatSocket Gain{ get; set; }

		public MusgraveInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
			Detail = new FloatSocket(parentNode, "Detail");
			AddSocket(Detail);
			Dimension = new FloatSocket(parentNode, "Dimension");
			AddSocket(Dimension);
			Lacunarity = new FloatSocket(parentNode, "Lacunarity");
			AddSocket(Lacunarity);
			Offset = new FloatSocket(parentNode, "Offset");
			AddSocket(Offset);
			Gain = new FloatSocket(parentNode, "Gain");
			AddSocket(Gain);
		}
	}

	public class MusgraveOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public MusgraveOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("musgrave_texture")]
	public class MusgraveTexture : TextureNode
	{
		public enum MusgraveTypes
		{
			Multifractal,
			fBM,
			Hybrid_Multifractal,
			Ridged_Multifractal,
			Hetero_Terrain,
		}

		public MusgraveInputs ins { get { return (MusgraveInputs)inputs; } }
		public MusgraveOutputs outs { get { return (MusgraveOutputs)outputs; } }

		public MusgraveTexture() : this("a musgrave texture") { }
		public MusgraveTexture(string name)
			: base(ShaderNodeType.MusgraveTexture, name)
		{
			inputs = new MusgraveInputs(this);
			outputs = new MusgraveOutputs(this);

			MusgraveType = MusgraveTypes.fBM;

			ins.Scale.Value = 1.0f;
			ins.Detail.Value = 2.0f;
			ins.Dimension.Value = 2.0f;
			ins.Lacunarity.Value = 1.0f;
			ins.Offset.Value = 0.0f;
			ins.Gain.Value = 1.0f;
		}

		/// <summary>
		/// musgrave->type
		/// </summary>
		public MusgraveTypes MusgraveType { get; set; }
		public void SetType(string dist)
		{
			dist = dist.Replace(" ", "_");
			MusgraveType= (MusgraveTypes)System.Enum.Parse(typeof(MusgraveTypes), dist, true);
		}

		private string MusgraveToString(MusgraveTypes dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", " ");
			return str;
		}

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "musgrave", MusgraveToString(MusgraveType));
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Dimension, xmlNode.GetAttribute("dimension"));
			Utilities.Instance.get_float(ins.Detail, xmlNode.GetAttribute("detail"));
			Utilities.Instance.get_float(ins.Lacunarity, xmlNode.GetAttribute("lacunarity"));
			Utilities.Instance.get_float(ins.Offset, xmlNode.GetAttribute("offset"));
			Utilities.Instance.get_float(ins.Gain, xmlNode.GetAttribute("gain"));

			var musgravetype = xmlNode.GetAttribute("musgrave_type");
			if (!string.IsNullOrEmpty(musgravetype))
			{
				SetType(musgravetype);
			}
		}
	}
}
