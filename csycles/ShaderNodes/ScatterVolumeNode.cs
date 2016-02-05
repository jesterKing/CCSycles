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
	public class ScatterVolumeInputs : Inputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Density { get; set; }
		public FloatSocket Anisotropy { get; set; }

		public ScatterVolumeInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Density = new FloatSocket(parentNode, "Density");
			AddSocket(Density);
			Anisotropy = new FloatSocket(parentNode, "Anisotropy");
			AddSocket(Anisotropy);
		}
	}

	public class ScatterVolumeOutputs : Outputs
	{
		public ClosureSocket Volume { get; set; }

		public ScatterVolumeOutputs(ShaderNode parentNode)
		{
			Volume = new ClosureSocket(parentNode, "Volume");
			AddSocket(Volume);
		}
	}
	
	/// <summary>
	/// A scatter volume node.
	/// </summary>
	[ShaderNode("scatter_volume")]
	public class ScatterVolumeNode : ShaderNode
	{
		public ScatterVolumeInputs ins { get { return (ScatterVolumeInputs)inputs; } }
		public ScatterVolumeOutputs outs { get { return (ScatterVolumeOutputs)outputs; } }
		/// <summary>
		/// Create a new Scatter volume node
		/// </summary>
		public ScatterVolumeNode() : this("a scatter volume node") { }
		public ScatterVolumeNode(string name) :
			base(ShaderNodeType.ScatterVolume, name)
		{
			inputs = new ScatterVolumeInputs(this);
			outputs = new ScatterVolumeOutputs(this);
			ins.Color.Value = new float4(1.0f);
			ins.Density.Value = 1.0f;
			ins.Anisotropy.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Density, xmlNode.GetAttribute("density"));
			Utilities.Instance.get_float(ins.Anisotropy, xmlNode.GetAttribute("anisotropy"));
		}
	}
}
