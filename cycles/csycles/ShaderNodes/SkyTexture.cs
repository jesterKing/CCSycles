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
	public class SkyInputs : Inputs
	{
		public Float4Socket Vector { get; set; }

		public SkyInputs(ShaderNode parentNode)
		{
			Vector = new Float4Socket(parentNode, "Vector");
			AddSocket(Vector);
		}
	}

	public class SkyOutputs : Outputs
	{
		public Float4Socket Color { get; set; }

		public SkyOutputs(ShaderNode parentNode)
		{
			Color = new Float4Socket(parentNode, "Color");
			AddSocket(Color);
		}
	}
	public class SkyTexture : TextureNode
	{
		public SkyInputs ins { get { return (SkyInputs)inputs; } set { inputs = value; } }
		public SkyOutputs outs { get { return (SkyOutputs)outputs; } set { outputs = value; } }

		public SkyTexture()
			: base(ShaderNodeType.SkyTexture)
		{
			ins = new SkyInputs(this);
			outs = new SkyOutputs(this);

			SunDirection = new float4(0.0f, 0.0f, 1.0f);
			Turbidity = 2.2f;
			GroundAlbedo = 0.3f;
			SkyType = "Hosek / Wilkie";
		}

		public float4 SunDirection { get; set; }
		public float Turbidity { get; set; }
		public float GroundAlbedo { get; set; }

		/// <summary>
		/// One of:
		/// - Preetham
		/// - Hosek / Wilkie
		/// </summary>
		public string SkyType { get; set; }
	}
}
