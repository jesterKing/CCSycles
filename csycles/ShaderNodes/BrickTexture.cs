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

using System.Xml;
using ccl.ShaderNodes.Sockets;
using ccl.Attributes;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// BrickTexture node input sockets
	/// </summary>
	public class BrickInputs : Inputs
	{
		/// <summary>
		/// BrickTexture first brick color to vary with (see Bias)
		/// </summary>
		public Float4Socket Color1 { get; set; }
		/// <summary>
		/// BrickTexture second brick color to vary with
		/// </summary>
		public Float4Socket Color2 { get; set; }
		/// <summary>
		/// BrickTexture  mortar color
		/// </summary>
		public Float4Socket Mortar { get; set; }
		/// <summary>
		/// BrickTexture texture coordinate to sample at. 
		/// </summary>
		public Float4Socket Vector { get; set; }

		/// <summary>
		/// BrickTexture overall texture scale. Larger values mean smaller bricks
		/// </summary>
		public FloatSocket Scale { get; set; }

		/// <summary>
		/// BrickTexture thickness of mortar size
		/// </summary>
		public FloatSocket MortarSize { get; set; }

		/// <summary>
		/// BrickTexture color variance. -1.0f means Color1, 1.0f means Color2. Values in between will
		/// mix the colors
		/// </summary>
		public FloatSocket Bias { get; set; }

		/// <summary>
		/// BrickTexture brick width
		/// </summary>
		public FloatSocket BrickWidth { get; set; }

		/// <summary>
		/// BrickTexture row (brick) height
		/// </summary>
		public FloatSocket RowHeight { get; set; }

		internal BrickInputs(ShaderNode parentNode)
		{
			Color1 = new Float4Socket(parentNode, "Color1");
			AddSocket(Color1);
			Color2 = new Float4Socket(parentNode, "Color2");
			AddSocket(Color2);
			Mortar = new Float4Socket(parentNode, "Mortar");
			AddSocket(Mortar);
			Vector= new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);

			Scale = new FloatSocket(parentNode, "Scale");
			AddSocket(Scale);
			MortarSize = new FloatSocket(parentNode, "Mortar Size");
			AddSocket(MortarSize);
			Bias = new FloatSocket(parentNode, "Bias");
			AddSocket(Bias);
			BrickWidth = new FloatSocket(parentNode, "Brick Width");
			AddSocket(BrickWidth);
			RowHeight = new FloatSocket(parentNode, "Row Height");
			AddSocket(RowHeight);
		}
	}

	/// <summary>
	/// BrickTexture output sockets
	/// </summary>
	public class BrickOutputs : Outputs
	{
		/// <summary>
		/// BrickTexture color output
		/// </summary>
		public Float4Socket Color { get; set; }
		/// <summary>
		/// BrickTexture mortar mask (1.0 = mortar)
		/// </summary>
		public FloatSocket Fac { get; set; }

		internal BrickOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	/// <summary>
	/// BrickTexture node.
	/// </summary>
	[ShaderNode("brick_texture")]
	public class BrickTexture : TextureNode
	{
		/// <summary>
		/// BrickTexture input sockets
		/// </summary>
		public BrickInputs ins { get { return (BrickInputs)inputs; } }
		/// <summary>
		/// BrickTexture output sockets
		/// </summary>
		public BrickOutputs outs { get { return (BrickOutputs)outputs; } }

		/// <summary>
		/// Create a brick texture
		/// </summary>
		public BrickTexture() : this("a brick texture") { }

		/// <summary>
		/// Create a brick texture with name
		/// </summary>
		/// <param name="name"></param>
		public BrickTexture(string name) :
			base(ShaderNodeType.BrickTexture, name)
		{
			inputs = new BrickInputs(this);
			outputs = new BrickOutputs(this);

			Offset = 0.5f;
			OffsetFrequency = 2;
			Squash = 1.0f;
			SquashFrequency = 2;

			ins.Color1.Value = new float4(0.1f, 0.2f, 0.3f);
			ins.Color2.Value = new float4(0.3f, 0.2f, 0.1f);
			ins.Mortar.Value = new float4();

			ins.Scale.Value = 5.0f;
			ins.MortarSize.Value = 0.02f;
			ins.Bias.Value = 0.0f;
			ins.BrickWidth.Value = 0.5f;
			ins.RowHeight.Value = 0.25f;
		}

#region direct member variables
		/// <summary>
		/// Offset of brick start on row per row. 0.5f means
		/// regular brick pattern (with OffsetFrequency 2), 0.0f means bricks on top of
		/// each other
		/// </summary>
		public float Offset { get; set; }
		/// <summary>
		/// Frequency to use offset by (row).
		/// </summary>
		public int OffsetFrequency { get; set; }
		/// <summary>
		/// Factor on brick width. Larger than 1.0f means longer bricks, lower than 1.0f shorter bricks.
		/// </summary>
		public float Squash { get; set; }
		/// <summary>
		/// Use squash factor every nth row
		/// </summary>
		public int SquashFrequency { get; set; }
#endregion

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "offset", Offset);
			CSycles.shadernode_set_member_int(clientId, shaderId, Id, Type, "offset_frequency", OffsetFrequency);
			CSycles.shadernode_set_member_float(clientId, shaderId, Id, Type, "squash", Squash);
			CSycles.shadernode_set_member_int(clientId, shaderId, Id, Type, "squash_frequency", SquashFrequency);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			var offset = 0.0f;
			var offset_frequency = 0;
			var squash = 0.0f;
			var squash_frequency = 0;
			if (Utilities.Instance.get_float(ref offset, xmlNode.GetAttribute("offset")))
				Offset = offset;
			if (Utilities.Instance.get_int(ref offset_frequency, xmlNode.GetAttribute("offset_frequency")))
				OffsetFrequency = offset_frequency;
			if (Utilities.Instance.get_float(ref squash, xmlNode.GetAttribute("squash")))
				Squash = squash;
			if (Utilities.Instance.get_int(ref squash_frequency, xmlNode.GetAttribute("squash_frequency")))
				SquashFrequency = squash_frequency;

			Utilities.Instance.get_float4(ins.Color1, xmlNode.GetAttribute("color1"));
			Utilities.Instance.get_float4(ins.Color2, xmlNode.GetAttribute("color2"));
			Utilities.Instance.get_float4(ins.Mortar, xmlNode.GetAttribute("mortar"));
			Utilities.Instance.get_float(ins.Bias, xmlNode.GetAttribute("bias"));
			Utilities.Instance.get_float(ins.BrickWidth, xmlNode.GetAttribute("brick_width"));
			Utilities.Instance.get_float(ins.RowHeight, xmlNode.GetAttribute("row_height"));
		}
	}
}
