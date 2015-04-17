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
	public class VoronoiInputs : Inputs
	{
		public Float4Socket Vector { get; set; }
		public FloatSocket Scale { get; set; }

		public VoronoiInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
		}
	}

	public class VoronoiOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public VoronoiOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}
	public class VoronoiTexture : TextureNode
	{
		public VoronoiInputs ins { get { return (VoronoiInputs)inputs; } set { inputs = value; } }
		public VoronoiOutputs outs { get { return (VoronoiOutputs)outputs; } set { outputs = value; } }

		public VoronoiTexture()
			: base(ShaderNodeType.VoronoiTexture)
		{
			ins = new VoronoiInputs(this);
			outs = new VoronoiOutputs(this);

			ins.Scale.Value = 1.0f;

			Coloring = "Cells";
		}

		/// <summary>
		/// One of:
		/// - Cells
		/// - Intensity
		/// </summary>
		public string Coloring { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, Coloring);
		}
	}
}
