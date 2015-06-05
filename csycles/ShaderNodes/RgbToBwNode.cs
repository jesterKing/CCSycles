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
using ccl.Attributes;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// RgbToBw input sockets
	/// </summary>
	public class ConvertRgbInputs : Inputs
	{
		/// <summary>
		/// RgbToBw input color
		/// </summary>
		public Float4Socket Color { get; set; }

		internal ConvertRgbInputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// RgbToBw output sockets
	/// </summary>
	public class ConvertValOutputs : Outputs
	{
		/// <summary>
		/// RgbToBw value calculated from input color
		/// </summary>
		public FloatSocket Val { get; set; }

		internal ConvertValOutputs(ShaderNode parentNode)
		{
			Val = new FloatSocket(parentNode, "Val");
			AddSocket(Val);
		}
	}

	/// <summary>
	/// RgbToBw node to convert a color to a value
	/// </summary>
	[ShaderNode("rgb_to_bw")]
	public class RgbToBwNode : ShaderNode
	{
		/// <summary>
		/// RgbToBw input sockets
		/// </summary>
		public ConvertRgbInputs ins { get { return (ConvertRgbInputs)inputs; } }
		/// <summary>
		/// RgbToBw output sockets
		/// </summary>
		public ConvertValOutputs outs { get { return (ConvertValOutputs)outputs; } }

		/// <summary>
		/// Create new RgbToBw node
		/// </summary>
		public RgbToBwNode() : this("An RgbToBw node") { }
			
		/// <summary>
		/// Create new RgbToBw node with given name
		/// </summary>
		public RgbToBwNode(string name) :
			base(ShaderNodeType.RgbToBw, name)
		{
			inputs = new ConvertRgbInputs(this);
			outputs = new ConvertValOutputs(this);
		}
	}
}
