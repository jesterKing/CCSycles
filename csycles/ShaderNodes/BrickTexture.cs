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
	public class BrickInputs : Inputs
	{
		public Float4Socket Color1 { get; set; }
		public Float4Socket Color2 { get; set; }
		public Float4Socket Mortar { get; set; }

		public Float4Socket Vector { get; set; }

		public FloatSocket Scale { get; set; }

		public FloatSocket MortarSize { get; set; }

		public FloatSocket Bias { get; set; }

		public FloatSocket BrickWidth { get; set; }

		public FloatSocket RowHeight { get; set; }

		public BrickInputs(ShaderNode parentNode)
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

	public class BrickOutputs : Outputs
	{
		public Float4Socket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public BrickOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac");
			AddSocket(Fac);
		}
	}

	public class BrickTexture : TextureNode
	{
		public BrickInputs ins { get { return (BrickInputs)inputs; } set { inputs = value; } }
		public BrickOutputs outs { get { return (BrickOutputs)outputs; } set { outputs = value; }}
		public BrickTexture() :
			base(ShaderNodeType.BrickTexture)
		{
			ins = new BrickInputs(this);
			outs = new BrickOutputs(this);

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
		public float Offset { get; set; }
		public float Squash { get; set; }
		public int OffsetFrequency { get; set; }
		public int SquashFrequency { get; set; }
#endregion
	}
}
