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
	public class ValueInputs : Inputs
	{
		public ValueInputs(ShaderNode parentNode)
		{
			
		}
	}
	public class ValueOutputs : Outputs
	{
		public FloatSocket Value { get; set; }

		public ValueOutputs(ShaderNode parentNode)
		{
			Value = new FloatSocket(parentNode, "Value");
			AddSocket(Value);
		}
	}

	public class ValueNode : ShaderNode
	{

		public ValueInputs ins { get { return (ValueInputs)inputs; } set { inputs = value; } }
		public ValueOutputs outs { get { return (ValueOutputs)outputs; } set { outputs = value; } }
		public ValueNode() :
			base(ShaderNodeType.Value)
		{
			ins = null;
			outs = new ValueOutputs(this);
		}

		/// <summary>
		/// Set member variable [IN] for ValueNode.
		/// </summary>
		public float Value { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "value", Value);
		}
	}
}
