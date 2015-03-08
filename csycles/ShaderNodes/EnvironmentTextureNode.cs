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
using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	public class EnvironmentTextureInputs : Inputs
	{
		public Float4Socket Vector { get; set; }

		public EnvironmentTextureInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	public class EnvironmentTextureOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Alpha { get; set; }

		public EnvironmentTextureOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha");
			AddSocket(Alpha);
		}
	}

	public class EnvironmentTextureNode : TextureNode
	{

		public EnvironmentTextureInputs ins { get { return (EnvironmentTextureInputs)inputs; } set { inputs = value; } }
		public EnvironmentTextureOutputs outs { get { return (EnvironmentTextureOutputs)outputs; } set { outputs = value; } }

		public EnvironmentTextureNode() :
			base(ShaderNodeType.EnvironmentTexture)
		{

			inputs = new EnvironmentTextureInputs(this);
			outputs = new EnvironmentTextureOutputs(this);

			ColorSpace = TextureColorSpace.Color;
			Projection = EnvironmentProjection.Equirectangular;
		}

		public TextureColorSpace ColorSpace { get; set; }
		public EnvironmentProjection Projection { get; set; }
		public string Filename { get; set; }

		public float[] FloatImage { set; get; }

		public byte[] ByteImage { set; get; }

		/* two helpers for image resolution */
		public uint Width { get; set; }
		public uint Height { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			var projection = Projection == TextureNode.EnvironmentProjection.Equirectangular
				? "Equirectangular"
				: "Mirror Ball";
			var colspace = ColorSpace == TextureNode.TextureColorSpace.Color ? "Color" : "None";
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, projection);
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, colspace);
		}

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			if (FloatImage != null)
			{
				var flimg = FloatImage;
				CSycles.shadernode_set_member_float_img(clientId, shaderId, Id, Type, "builtin-data", Filename ?? String.Format("{0}-{0}-{0}", clientId, shaderId, Id), ref flimg, Width, Height, 1, 4);
			}
			else if (ByteImage != null)
			{
				var bimg = ByteImage;
				CSycles.shadernode_set_member_byte_img(clientId, shaderId, Id, Type, "builtin-data", Filename ?? String.Format("{0}-{0}-{0}", clientId, shaderId, Id), ref bimg, Width, Height, 1, 4);
			}
		}
	}
}
