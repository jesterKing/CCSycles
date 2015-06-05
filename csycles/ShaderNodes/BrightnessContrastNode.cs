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
	public class BrightnessContrastInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Bright { get; set; }
		public FloatSocket Contrast { get; set; }

		public BrightnessContrastInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Bright = new FloatSocket(parentNode, "Bright");
			AddSocket(Bright);
			Contrast = new FloatSocket(parentNode, "Contrast");
			AddSocket(Contrast);
		}
	}

	public class BrightnessContrastOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public BrightnessContrastOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	[ShaderNode("brightness")]
	public class BrightnessContrastNode : ShaderNode
	{
		public BrightnessContrastInputs ins { get { return (BrightnessContrastInputs)inputs; } }
		public BrightnessContrastOutputs outs { get { return (BrightnessContrastOutputs)outputs; } }

		public BrightnessContrastNode() : this("a brightness contrast node") { }
		public BrightnessContrastNode(string name)
			: base(ShaderNodeType.BrightnessContrast, name)
		{
			inputs = new BrightnessContrastInputs(this);
			outputs = new BrightnessContrastOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Bright.Value = 0.0f;
			ins.Contrast.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Bright, xmlNode.GetAttribute("bright"));
			Utilities.Instance.get_float(ins.Contrast, xmlNode.GetAttribute("contrast"));
		}
	}
}
