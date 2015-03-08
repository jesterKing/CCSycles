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
	public class MathInputs : Inputs
	{
		public FloatSocket Value1 { get; set; }
		public FloatSocket Value2 { get; set; }

		public MathInputs(ShaderNode parentNode)
		{
			Value1 = new FloatSocket(parentNode, "Value1");
			AddSocket(Value1);
			Value2 = new FloatSocket(parentNode, "Value2");
			AddSocket(Value2);
		}
	}

	public class MathOutputs : Outputs
	{
		public FloatSocket Value { get; set; }

		public MathOutputs(ShaderNode parentNode)
		{
			Value = new FloatSocket(parentNode, "Value");
			AddSocket(Value);
		}
	}

	/// <summary>
	/// Add a Math node, setting output Value with any of the following <c>Operation</c>s using Value1 and Value2
	/// - Add. Value = Value1 + Value2
	/// - Subtract Value = Value1 - Value2
	/// - Multiply Value = Value1 * Value2
	/// - Divide Value = Value1 / Value2
	/// - Sine Value = sin(Value1)
	/// - Cosine Value = cos(Value1)
	/// - Tangent Value = tan(Value1)
	/// - Arcsine Value = asin(Value1)
	/// - Arccosine Value = acos(Value1)
	/// - Arctangent Value = atan(Value1)
	/// - Power Value = Value1 ** Value2
	/// - Logarithm Value = log(Value1) / log(Value2) (0.0 if either 0.0)
	/// - Minimum Value = min(Value1, Value2)
	/// - Maximum Value = max(Value1, Value2)
	/// - Round Value = floor(Value1 + 0.5)
	/// - Less Than Value = Value1 &lt; Value2
	/// - Greater Than Value = Value1 &gt; Value2
	/// - Modulo Value = module(Value1, Value2)
	/// - Absolute = abs(Value1)
	///
	/// \todo figure out how to do CLAMP
	/// </summary>
	public class MathNode : ShaderNode
	{
		public enum Operations
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			Sine,
			Cosine,
			Tangent,
			Arcsine,
			Arccosine,
			Arctangent,
			Power,
			Logarithm,
			Minimum,
			Maximum,
			Round,
			Less_Than,
			Greater_Than,
			Modulo,
			Absolute
		}

		public MathInputs ins { get { return (MathInputs)inputs; } set { inputs = value; } }
		public MathOutputs outs { get { return (MathOutputs)outputs; } set { outputs = value; } }

		public MathNode() :
			base(ShaderNodeType.Math)
		{
			inputs = new MathInputs(this);
			outputs = new MathOutputs(this);

			Operation = Operations.Add;
			ins.Value1.Value = 0.0f;
			ins.Value2.Value = 0.0f;
		}

		public Operations Operation { get; set; }

		/// <summary>
		/// Set to true [IN] if math output in Value should be clamped 0.0..1.0
		/// </summary>
		public bool UseClamp { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, Operation.ToString().Replace('_', ' '));
		}
	}
}
