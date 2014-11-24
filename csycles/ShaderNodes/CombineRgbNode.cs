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
    public class CombineRgbInputs : Inputs
    {
        public FloatSocket R { get; set; }
        public FloatSocket G { get; set; }
        public FloatSocket B { get; set; }

        public CombineRgbInputs(ShaderNode parentNode)
        {
            R = new FloatSocket(parentNode, "R");
            AddSocket(R);
            G = new FloatSocket(parentNode, "G");
            AddSocket(G);
            B = new FloatSocket(parentNode, "B");
            AddSocket(B);
        }
    }

    public class CombineRgbOutputs : Outputs
    {
        public Float4Socket ImageOutput { get; set; }

        public CombineRgbOutputs(ShaderNode parentNode)
        {
            ImageOutput = new Float4Socket(parentNode, "ImageOutput");
            AddSocket(ImageOutput);
        }
    }

    /// <summary>
    /// Add a Combine RGB node, converting single R G B scalars to a vector output
    /// </summary>
    public class CombineRgbNode : ShaderNode
    {
        public CombineRgbInputs ins { get { return (CombineRgbInputs)inputs; } set { inputs = value; } }
        public CombineRgbOutputs outs { get { return (CombineRgbOutputs)outputs; } set { outputs = value; } }

        public CombineRgbNode() :
            base(ShaderNodeType.CombineRgb)
        {
            inputs = new MappingInputs(this);
            outputs = new MappingOutputs(this);

            ins.R.Value = 0.0f;
            ins.G.Value = 0.0f;
            ins.B.Value = 0.0f;
        }
    }
}
