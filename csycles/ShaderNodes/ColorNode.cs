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

namespace ccl.ShaderNodes
{
	/// <summary>
	/// ColorNode output sockets
	/// </summary>
	public class ColorOutputs : Outputs
	{
		/// <summary>
		/// ColorNode output color. Note only RGB is used
		/// </summary>
		public Float4Socket Color { get; set; }

		internal ColorOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// ColorNode input sockets. Empty, used for completeness
	/// </summary>
	public class ColorInputs : Inputs
	{
		internal ColorInputs(ShaderNode parentNode)
		{
			
		}
	}

	/// <summary>
	/// ColorNode is a RGB input node.
	/// </summary>
	public class ColorNode : ShaderNode
	{
		/// <summary>
		/// ColorNode input sockets
		/// </summary>
		public ColorInputs ins { get { return (ColorInputs)inputs; } }
		/// <summary>
		/// ColorNode output sockets
		/// </summary>
		public ColorOutputs outs { get { return (ColorOutputs)outputs; } }

		/// <summary>
		/// Create a ColorNode
		/// </summary>
		public ColorNode() : this("a rgb input")
		{
		}

		/// <summary>
		/// Create a ColorNode with name
		/// </summary>
		/// <param name="name"></param>
		public ColorNode(string name) :
			base(ShaderNodeType.Color, name)
		{
			inputs = null;
			outputs = new ColorOutputs(this);
			Value = new float4(0.8f);
		}

		/// <summary>
		/// Set Color member variable [IN] for ColorNode. Only RGB is used
		/// </summary>
		public float4 Value { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			var val = Value;
			CSycles.shadernode_set_member_vec(clientId, shaderId, Id, Type, "value", val.x, val.y, val.z);
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
			var f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("value"));
			Value = f4;
		}
	}
}
