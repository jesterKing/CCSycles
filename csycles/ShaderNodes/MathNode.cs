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

using System;
using System.Xml;
using ccl.ShaderNodes.Sockets;
using ccl.Attributes;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// MathNode input sockets
	/// </summary>
	public class MathInputs : Inputs
	{
		/// <summary>
		/// MathNode Value1 input socket
		/// </summary>
		public FloatSocket Value1 { get; set; }
		/// <summary>
		/// MathNode Value2 input socket
		/// </summary>
		public FloatSocket Value2 { get; set; }

		/// <summary>
		/// Create MathNode input sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MathInputs(ShaderNode parentNode)
		{
			Value1 = new FloatSocket(parentNode, "Value1");
			AddSocket(Value1);
			Value2 = new FloatSocket(parentNode, "Value2");
			AddSocket(Value2);
		}
	}

	/// <summary>
	/// MathNode output sockets
	/// </summary>
	public class MathOutputs : Outputs
	{
		/// <summary>
		/// The resulting value of the MathNode operation
		/// </summary>
		public FloatSocket Value { get; set; }

		/// <summary>
		/// Create MathNode output sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MathOutputs(ShaderNode parentNode)
		{
			Value = new FloatSocket(parentNode, "Value");
			AddSocket(Value);
		}
	}

	/// <summary>
	/// Add a Math node, setting output Value with any of the following <c>Operation</c>s using Value1 and Value2
	///
	/// Note that some operations use only Value1
	/// </summary>
	[ShaderNode("math")]
	public class MathNode : ShaderNode
	{
		/// <summary>
		/// Math operations the MathNode can do.
		/// </summary>
		public enum Operations
		{
			/// <summary>
			/// Value = Value1 + Value2
			/// </summary>
			Add,
			/// <summary>
			/// Value = Value1 - Value2
			/// </summary>
			Subtract,
			/// <summary>
			/// Value = Value1 * Value2
			/// </summary>
			Multiply,
			/// <summary>
			/// Value = Value1 / Value2
			/// </summary>
			Divide,
			/// <summary>
			/// Value = sin(Value1). Value2 ignored
			/// </summary>
			Sine,
			/// <summary>
			/// Value = cos(Value1). Value2 ignored
			/// </summary>
			Cosine,
			/// <summary>
			/// Value = tan(Value1). Value2 ignored
			/// </summary>
			Tangent,
			/// <summary>
			/// Value = asin(Value1). Value2 ignored
			/// </summary>
			Arcsine,
			/// <summary>
			/// Value = acos(Value1). Value2 ignored
			/// </summary>
			Arccosine,
			/// <summary>
			/// Value = atan(Value1). Value2 ignored
			/// </summary>
			Arctangent,
			/// <summary>
			/// Value = Value1 ** Value2
			/// </summary>
			Power,
			/// <summary>
			/// Value = log(Value1) / log(Value2). 0.0f if either input is 0.0f
			/// </summary>
			Logarithm,
			/// <summary>
			/// Value = min(Value1, Value2)
			/// </summary>
			Minimum,
			/// <summary>
			/// Value = max(Value1, Value2)
			/// </summary>
			Maximum,
			/// <summary>
			/// Value = floor(Value1 + 0.5). Value2 ignored
			/// </summary>
			Round,
			/// <summary>
			/// Value = Value1 &lt; Value2
			/// </summary>
			Less_Than,
			/// <summary>
			/// Value = Value1 &gt; Value2
			/// </summary>
			Greater_Than,
			/// <summary>
			/// Value = mod(Value1, Value2)
			/// </summary>
			Modulo,
			/// <summary>
			/// Value = abs(Value1). Value2 ignored
			/// </summary>
			Absolute
		}

		/// <summary>
		/// MathNode input sockets
		/// </summary>
		public MathInputs ins { get { return (MathInputs)inputs; } }
		/// <summary>
		/// MathNode output sockets
		/// </summary>
		public MathOutputs outs { get { return (MathOutputs)outputs; } }

		/// <summary>
		/// Math node operates on float inputs (note, some operations use only Value1)
		/// </summary>
		public MathNode() :
			this("a mathnode")
		{
			
		}

		public MathNode(string name) :
			base(ShaderNodeType.Math, name)
		{
			inputs = new MathInputs(this);
			outputs = new MathOutputs(this);

			Operation = Operations.Add;
			ins.Value1.Value = 0.0f;
			ins.Value2.Value = 0.0f;
		}

		/// <summary>
		/// The operation this node does on Value1 and Value2
		/// </summary>
		public Operations Operation { get; set; }

		private void SetOperation(string op)
		{
			op = op.Replace(" ", "_");
			Operation = (Operations)Enum.Parse(typeof(Operations), op, true);
		}

		/// <summary>
		/// Set to true [IN] if math output in Value should be clamped 0.0..1.0
		/// </summary>
		public bool UseClamp { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "operation", Operation.ToString().Replace('_', ' '));
		}

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_bool(clientId, shaderId, Id, Type, "use_clamp", UseClamp);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Value1, xmlNode.GetAttribute("value1"));
			Utilities.Instance.get_float(ins.Value2, xmlNode.GetAttribute("value2"));
			var operation = xmlNode.GetAttribute("type");
			if (!string.IsNullOrEmpty(operation))
			{
				SetOperation(operation);
			}
		}
	}
}
