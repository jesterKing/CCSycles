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

namespace ccl.ShaderNodes
{

	/// <summary>
	/// RgbToBw node to convert a color to a value
	/// </summary>
	public class RgbToLuminanceNode : ShaderNode
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
		public RgbToLuminanceNode() : this("An RgbToLuminance node") { }
			
		/// <summary>
		/// Create new RgbToLuminance node with given name
		/// </summary>
		public RgbToLuminanceNode(string name) :
			base(ShaderNodeType.RgbToLuminance, name)
		{
			inputs = new ConvertRgbInputs(this);
			outputs = new ConvertValOutputs(this);
		}
	}
}
