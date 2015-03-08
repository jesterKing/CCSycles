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
	public class ColorOutputs : Outputs
	{
		public Float4Socket Color { get; set; }

		public ColorOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}

	public class ColorInputs : Inputs
	{
		public ColorInputs(ShaderNode parentNode)
		{
			
		}
	}

	public class ColorNode : ShaderNode
	{
		public ColorInputs ins { get { return (ColorInputs)inputs; } set { inputs = value; } }
		public ColorOutputs outs { get { return (ColorOutputs)outputs; } set { outputs = value; }}
		public ColorNode() :
			base(ShaderNodeType.Color)
		{
			inputs = null;
			outputs = new ColorOutputs(this);
			Value = new float4(0.8f);
		}

		/// <summary>
		/// Set Color member variable [IN] for ColorNode.
		/// </summary>
		public float4 Value { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			var val = Value;
			CSycles.shadernode_set_member_vec(clientId, shaderId, Id, Type, "value", val.x, val.y, val.z);
		}
	}
}
