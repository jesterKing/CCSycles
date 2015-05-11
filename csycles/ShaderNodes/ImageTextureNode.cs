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
using System.Drawing;
using System.Xml;
using ccl.ShaderNodes.Sockets;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// ImageTexture input sockets
	/// </summary>
	public class ImageTextureInputs : Inputs
	{
		/// <summary>
		/// ImageTexture space coordinate to sample texture at
		/// </summary>
		public Float4Socket Vector { get; set; }

		internal ImageTextureInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// ImageTexture output sockets
	/// </summary>
	public class ImageTextureOutputs : Outputs
	{
		/// <summary>
		/// ImageTexture Color output
		/// </summary>
		public Float4Socket Color { get; set; }
		/// <summary>
		/// ImageTexture alpha output
		/// </summary>
		public FloatSocket Alpha { get; set; }

		internal ImageTextureOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha");
			AddSocket(Alpha);
		}
	}

	public class ImageTextureNode : TextureNode
	{
		/// <summary>
		/// Image texture input sockets
		/// </summary>
		public ImageTextureInputs ins { get { return (ImageTextureInputs)inputs; } }
		/// <summary>
		/// Image texture output sockets
		/// </summary>
		public ImageTextureOutputs outs { get { return (ImageTextureOutputs)outputs; } }

		public ImageTextureNode() : this("an image texture node")
		{
		}

		public ImageTextureNode(string name) :
			base(ShaderNodeType.ImageTexture, name)
		{

			inputs = new ImageTextureInputs(this);
			outputs = new ImageTextureOutputs(this);

			ProjectionBlend = 0.0f;
			Interpolation = InterpolationType.Linear;
			ColorSpace = TextureColorSpace.Color;
			Projection = TextureProjection.Flat;
		}

		/// <summary>
		/// ImageTexture color space
		/// </summary>
		public TextureColorSpace ColorSpace { get; set; }
		/// <summary>
		/// ImageTexture texture projection
		/// </summary>
		public TextureProjection Projection { get; set; }
		/// <summary>
		/// ImageTexture texture projection blend
		/// </summary>
		public float ProjectionBlend { get; set; }
		/// <summary>
		/// ImageTexture texture interpolation
		/// </summary>
		public InterpolationType Interpolation { get; set; }
		/// <summary>
		/// ImageTexture linear
		/// </summary>
		public bool IsLinear { get; set; }
		/// <summary>
		/// ImageTexture float image
		/// </summary>
		public bool IsFloat { get; set; }
		/// <summary>
		/// ImageTexture use alpha changel if true
		/// </summary>
		public bool UseAlpha { get; set; }
		/// <summary>
		/// ImageTexture image data name
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// ImageTexture float image data 
		/// </summary>
		public float[] FloatImage { set; get; }

		/// <summary>
		/// ImageTexture byte image data 
		/// </summary>
		public byte[] ByteImage { set; get; }

		/// <summary>
		/// ImageTexture image width in pixels
		/// </summary>
		public uint Width { get; set; }
		/// <summary>
		/// ImageTexture image height in pixels
		/// </summary>
		public uint Height { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			//CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, "color_space", ColorSpace.ToString());
			//CSycles.shadernode_set_attribute_string(clientId, shaderId, Id, "projection", Projection.ToString());
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "color_space", ColorSpace.ToString());
			CSycles.shadernode_set_enum(clientId, shaderId, Id, Type, "projection", Projection.ToString());
		}

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "projection_blend", ProjectionBlend);
			CSycles.shadernode_set_member_int(clientId, shaderId, Id, Type, "interpolation", (int)Interpolation);
			CSycles.shadernode_set_member_bool(clientId, shaderId, Id, Type, "use_alpha", UseAlpha);
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

		internal override void ParseXml(XmlReader xmlNode)
		{
			var imgsrc = xmlNode.GetAttribute("src");
			if (!string.IsNullOrEmpty(imgsrc))
			{
				using (var bmp = new Bitmap(imgsrc))
				{
					var l = bmp.Width * bmp.Height * 4;
					var bmpdata = new byte[l];
					for (var x = 0; x < bmp.Width; x++)
					{
						for (var y = 0; y < bmp.Height; y++)
						{
							var pos = y * bmp.Width * 4 + x * 4;
							var pixel = bmp.GetPixel(x, y);
							bmpdata[pos] = pixel.R;
							bmpdata[pos + 1] = pixel.G;
							bmpdata[pos + 2] = pixel.B;
							bmpdata[pos + 3] = pixel.A;
						}
					}
					ByteImage = bmpdata;
					Width = (uint)bmp.Width;
					Height = (uint)bmp.Height;
				}
			}
		}
	}
}
