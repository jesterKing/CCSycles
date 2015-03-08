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
	public class ImageTextureInputs : Inputs
	{
		public Float4Socket Vector { get; set; }

		public ImageTextureInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	public class ImageTextureOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Alpha { get; set; }

		public ImageTextureOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha");
			AddSocket(Alpha);
		}
	}

	public class ImageTextureNode : TextureNode
	{

		public ImageTextureInputs ins { get { return (ImageTextureInputs)inputs; } set { inputs = value; } }
		public ImageTextureOutputs outs { get { return (ImageTextureOutputs)outputs; } set { outputs = value; } }

		public ImageTextureNode() :
			base(ShaderNodeType.ImageTexture)
		{

			inputs = new ImageTextureInputs(this);
			outputs = new ImageTextureOutputs(this);

			ProjectionBlend = 0.0f;
			Interpolation = InterpolationType.Linear;
			ColorSpace = TextureColorSpace.Color;
			Projection = TextureProjection.Flat;
		}

		public TextureColorSpace ColorSpace { get; set; }
		public TextureProjection Projection { get; set; }
		public float ProjectionBlend { get; set; }
		public InterpolationType Interpolation { get; set; }
		public bool IsLinear { get; set; }
		public bool IsFloat { get; set; }
		public bool UseAlpha { get; set; }
		public string Filename { get; set; }

		public float[] FloatImage { set; get; }

		public byte[] ByteImage { set; get; }

		/* two helpers for image resolution */
		public uint Width { get; set; }
		public uint Height { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, "color_space", ColorSpace.ToString());
			CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, "projection", ColorSpace.ToString());
			CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, "interpolation", ColorSpace.ToString());
		}
	}
}
