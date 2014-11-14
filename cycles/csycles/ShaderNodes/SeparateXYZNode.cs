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
    public class SeparateXYZInputs : Inputs
    {
        public Float4Socket VectorInput { get; set; }

        public SeparateXYZInputs(ShaderNode parentNode)
        {
            VectorInput = new Float4Socket(parentNode, "VectorInput");
            AddSocket(VectorInput);
        }
    }

    public class SeparateXYZOutputs : Outputs
    {
        public FloatSocket X { get; set; }
        public FloatSocket Y { get; set; }
        public FloatSocket Z { get; set; }

        public SeparateXYZOutputs(ShaderNode parentNode)
        {
            X = new FloatSocket(parentNode, "X");
            AddSocket(X);
            Y = new FloatSocket(parentNode, "Y");
            AddSocket(Y);
            Z = new FloatSocket(parentNode, "Z");
            AddSocket(Z);
        }
    }

    /// <summary>
    /// Add a Separate XYZ node, converting a vector input to single X Y Z scalar nodes
    /// </summary>
    public class SeparateXYZNode : ShaderNode
    {
        public SeparateXYZInputs ins { get { return (SeparateXYZInputs)inputs; } set { inputs = value; } }
        public SeparateXYZOutputs outs { get { return (SeparateXYZOutputs)outputs; } set { outputs = value; } }

        public SeparateXYZNode() :
            base(ShaderNodeType.SeparateXyz)
        {
            inputs = new MappingInputs(this);
            outputs = new MappingOutputs(this);

            ins.VectorInput.Value = new float4(0.0f);
        }
    }
}
