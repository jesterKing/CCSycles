﻿/**
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
	public class CombineXyzInputs : Inputs
	{
		public FloatSocket X { get; set; }
		public FloatSocket Y { get; set; }
		public FloatSocket Z { get; set; }

		public CombineXyzInputs(ShaderNode parentNode)
		{
			X = new FloatSocket(parentNode, "X");
			AddSocket(X);
			Y = new FloatSocket(parentNode, "Y");
			AddSocket(Y);
			Z = new FloatSocket(parentNode, "Z");
			AddSocket(Z);
		}
	}

	public class CombineXyzOutputs : Outputs
	{
		public Float4Socket VectorOutput { get; set; }

		public CombineXyzOutputs(ShaderNode parentNode)
		{
			VectorOutput = new Float4Socket(parentNode, "VectorOutput");
			AddSocket(VectorOutput);
		}
	}

	/// <summary>
	/// Add a Combine XYZ node, converting single X Y Z scalars to a vector output
	/// </summary>
	public class CombineXyzNode : ShaderNode
	{
		public CombineXyzInputs ins { get { return (CombineXyzInputs)inputs; } set { inputs = value; } }
		public CombineXyzOutputs outs { get { return (CombineXyzOutputs)outputs; } set { outputs = value; } }

		public CombineXyzNode() :
			base(ShaderNodeType.CombineXyz)
		{
			inputs = new MappingInputs(this);
			outputs = new MappingOutputs(this);

			ins.X.Value = 0.0f;
			ins.Y.Value = 0.0f;
			ins.Z.Value = 0.0f;
		}
	}
}
