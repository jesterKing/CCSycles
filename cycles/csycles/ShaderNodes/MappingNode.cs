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
    public class MappingInputs : Inputs
    {
        public Float4Socket Translation { get; set; }
        public Float4Socket Rotation { get; set; }
        public Float4Socket Scale { get; set; }
        public Float4Socket Min { get; set; }
        public Float4Socket Max { get; set; }

        public MappingInputs(ShaderNode parentNode)
        {
            Translation = new Float4Socket(parentNode, "Translation");
            AddSocket(Translation);
            Rotation = new Float4Socket(parentNode, "Rotation");
            AddSocket(Rotation);
            Rotation = new Float4Socket(parentNode, "Scale");
            AddSocket(Scale);
            Rotation = new Float4Socket(parentNode, "Min");
            AddSocket(Min);
            Rotation = new Float4Socket(parentNode, "Max");
            AddSocket(Max);
        }
    }

    public class MappingOutputs : Outputs
    {
        public Float4Socket VectorOutput { get; set; }

        public MappingOutputs(ShaderNode parentNode)
        {
            VectorOutput = new Float4Socket(parentNode, "VectorOutput");
            AddSocket(VectorOutput);
        }
    }

    /// <summary>
    /// Add a Mapping node, setting output Value with any of the following <c>vector types</c>s:
    /// - Texture: Transform a texture by inverse mapping the texture coordinate
    /// - Point: Transform a point
    /// - Vector: Transform a direction vector
    /// - Normal: Transform a normal vector with unit length
    ///
    /// \todo figure out how to do UseMIN and UseMAX
    /// </summary>
    public class MappingNode : ShaderNode
    {
        public enum vector_types
        {
            TEXTURE,
            POINT,
            VECTOR,
            NORMAL
        }

        public MappingInputs ins { get { return (MappingInputs)inputs; } set { inputs = value; } }
        public MappingOutputs outs { get { return (MappingOutputs)outputs; } set { outputs = value; } }

        public MappingNode() :
            base(ShaderNodeType.Mapping)
        {
            inputs = new MappingInputs(this);
            outputs = new MappingOutputs(this);

            vector_type = vector_types.POINT;
            ins.Translation.Value = new float4(0.0f);
            ins.Rotation.Value = new float4(0.0f);
            ins.Scale.Value = new float4(0.0f);
            ins.Min.Value = new float4(0.0f);
            ins.Max.Value = new float4(0.0f);
        }

        public vector_types vector_type { get; set; }

        /// <summary>
        /// Set to true [IN] if mapping output in Value should have a minimum value 0.0..1.0
        /// </summary>
        public bool UseMin { get; set; }

        /// <summary>
        /// Set to true [IN] if mapping output in Value should have a maximum value 0.0..1.0
        /// </summary>
        public bool UseMax { get; set; }

    }
}
