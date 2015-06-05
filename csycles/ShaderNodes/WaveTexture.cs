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
	public class WaveInputs : Inputs
	{
		public Float4Socket Vector { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Distortion { get; set; }
		public FloatSocket Detail { get; set; }
		public FloatSocket DetailScale { get; set; }

		public WaveInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
			Distortion = new FloatSocket(parentNode, "Distortion");
			AddSocket(Distortion);
			Detail = new FloatSocket(parentNode, "Detail");
			AddSocket(Detail);
			DetailScale = new FloatSocket(parentNode, "Detail Scale");
			AddSocket(DetailScale);
		}
	}

	public class WaveOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public WaveOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("wave_texture")]
	public class WaveTexture : TextureNode
	{
		public WaveInputs ins { get { return (WaveInputs)inputs; } }
		public WaveOutputs outs { get { return (WaveOutputs)outputs; } }

		public WaveTexture() : this("a wave texture") { }
		public WaveTexture(string name)
			: base(ShaderNodeType.WaveTexture, name)
		{
			inputs = new WaveInputs(this);
			outputs = new WaveOutputs(this);

			ins.Scale.Value = 1.0f;
			ins.Distortion.Value = 0.0f;
			ins.Detail.Value = 2.0f;
			ins.DetailScale.Value = 1.0f;
		}

		/// <summary>
		/// wave->type, one of
		/// - Bands
		/// - Rings
		/// </summary>
		public string WaveType { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "wave", WaveType);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Distortion, xmlNode.GetAttribute("distortion"));
			Utilities.Instance.get_float(ins.Detail, xmlNode.GetAttribute("detail"));
			Utilities.Instance.get_float(ins.DetailScale, xmlNode.GetAttribute("detail_scale"));

			var wavetype = xmlNode.GetAttribute("wave_type");
			if (!string.IsNullOrEmpty(wavetype))
			{
				WaveType = wavetype;
			}
		}
	}
}
