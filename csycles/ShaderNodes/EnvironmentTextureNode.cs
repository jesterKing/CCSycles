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
	/// <summary>
	/// EnvironmentTextureNode input sockets
	/// </summary>
	public class EnvironmentTextureInputs : Inputs
	{
		/// <summary>
		/// EnvironmentTextureNode vector input 
		/// </summary>
		public Float4Socket Vector { get; set; }

		internal EnvironmentTextureInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// EnvironmentTextureNode output sockets
	/// </summary>
	public class EnvironmentTextureOutputs : Outputs
	{
		/// <summary>
		/// EnvironmentTextureNode color output
		/// </summary>
		public Float4Socket Color { get; set; }
		/// <summary>
		/// EnvironmentTextureNode alpha output
		/// </summary>
		public FloatSocket Alpha { get; set; }

		internal EnvironmentTextureOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha");
			AddSocket(Alpha);
		}
	}

	/// <summary>
	/// EnvironmentTextureNode
	/// </summary>
	public class EnvironmentTextureNode : TextureNode
	{
		/// <summary>
		/// EnvironmentTextureNode input sockets
		/// </summary>
		public EnvironmentTextureInputs ins { get { return (EnvironmentTextureInputs)inputs; } set { inputs = value; } }
		/// <summary>
		/// EnvironmentTextureNode output sockets
		/// </summary>
		public EnvironmentTextureOutputs outs { get { return (EnvironmentTextureOutputs)outputs; } set { outputs = value; } }

		/// <summary>
		/// Create an EnvironmentTextureNode
		/// </summary>
		public EnvironmentTextureNode() :
			base(ShaderNodeType.EnvironmentTexture)
		{

			inputs = new EnvironmentTextureInputs(this);
			outputs = new EnvironmentTextureOutputs(this);

			ColorSpace = TextureColorSpace.Color;
			Projection = EnvironmentProjection.Equirectangular;
		}

		/// <summary>
		/// Color space to operate in
		/// </summary>
		public TextureColorSpace ColorSpace { get; set; }
		/// <summary>
		/// Get or set environment projection
		/// </summary>
		public EnvironmentProjection Projection { get; set; }
		/// <summary>
		/// Get or set image name
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Get or set the float data for image. Use for HDR images
		/// </summary>
		public float[] FloatImage { set; get; }
		
		/// <summary>
		/// Get or set the byte data for image
		/// </summary>
		public byte[] ByteImage { set; get; }

		/// <summary>
		/// Get or set image resolution width
		/// </summary>
		public uint Width { get; set; }
		/// <summary>
		/// Get or set image resolution height
		/// </summary>
		public uint Height { get; set; }

		internal override void SetEnums(uint clientId, uint shaderId)
		{
			var projection = Projection == EnvironmentProjection.Equirectangular
				? "Equirectangular"
				: "Mirror Ball";
			var colspace = ColorSpace == TextureColorSpace.Color ? "Color" : "None";
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
