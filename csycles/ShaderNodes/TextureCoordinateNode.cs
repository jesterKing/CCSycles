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
using ccl.Attributes;

namespace ccl.ShaderNodes
{
	public class TextureCoordinateOutputs : Outputs
	{
		public Float4Socket Generated { get; set; }
		public Float4Socket Normal { get; set; }
		public Float4Socket UV { get; set; }
		public Float4Socket Object { get; set; }
		public Float4Socket Camera { get; set; }
		public Float4Socket Window { get; set; }
		public Float4Socket Reflection { get; set; }
		public Float4Socket WcsBox { get; set; }
		public Float4Socket EnvSpherical { get; set; }
		public Float4Socket EnvEmap { get; set; }
		public Float4Socket EnvBox { get; set; }
		public Float4Socket EnvLightProbe { get; set; }
		public Float4Socket EnvCubemap { get; set; }
		public Float4Socket EnvCubemapVerticalCross { get; set; }
		public Float4Socket EnvCubemapHorizontalCross { get; set; }
		public Float4Socket EnvHemispherical { get; set; }

		public TextureCoordinateOutputs(ShaderNode parentNode)
		{
			Generated = new Float4Socket(parentNode, "Generated");
			AddSocket(Generated);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
			UV = new Float4Socket(parentNode, "UV");
			AddSocket(UV);
			Object = new Float4Socket(parentNode, "Object");
			AddSocket(Object);
			Camera = new Float4Socket(parentNode, "Camera");
			AddSocket(Camera);
			Window = new Float4Socket(parentNode, "Window");
			AddSocket(Window);
			Reflection = new Float4Socket(parentNode, "Reflection");
			AddSocket(Reflection);
			WcsBox = new Float4Socket(parentNode, "WcsBox");
			AddSocket(WcsBox);
			EnvSpherical = new Float4Socket(parentNode, "EnvSpherical");
			AddSocket(EnvSpherical);
			EnvEmap = new Float4Socket(parentNode, "EnvEmap");
			AddSocket(EnvEmap);
			EnvBox = new Float4Socket(parentNode, "EnvBox");
			AddSocket(EnvBox);
			EnvLightProbe = new Float4Socket(parentNode, "EnvLightProbe");
			AddSocket(EnvLightProbe);
			EnvCubemap = new Float4Socket(parentNode, "EnvCubemap");
			AddSocket(EnvCubemap);
			EnvCubemapVerticalCross = new Float4Socket(parentNode, "EnvCubemapVerticalCross");
			AddSocket(EnvCubemapVerticalCross);
			EnvCubemapHorizontalCross = new Float4Socket(parentNode, "EnvCubemapHorizontalCross");
			AddSocket(EnvCubemapHorizontalCross);
			EnvHemispherical = new Float4Socket(parentNode, "EnvHemi");
			AddSocket(EnvHemispherical);
		}
	}

	public class TextureCoordinateInputs : Inputs
	{
		public TextureCoordinateInputs(ShaderNode parentNode)
		{
			
		}
	}

	[ShaderNode("texture_coordinate")]
	public class TextureCoordinateNode : ShaderNode
	{
		public TextureCoordinateInputs ins { get { return (TextureCoordinateInputs)inputs; } }
		public TextureCoordinateOutputs outs { get { return (TextureCoordinateOutputs)outputs; } }

		public TextureCoordinateNode()
			: this("a texcoord") { }
		public TextureCoordinateNode(string name)
			: base(ShaderNodeType.TextureCoordinate, name)
		{
			inputs = null;
			outputs = new TextureCoordinateOutputs(this);

			UseTransform = false;
			ObjectTransform = ccl.Transform.Identity();
		}


		public ccl.Transform ObjectTransform { get; set; }
		public bool UseTransform { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_bool(clientId, shaderId, Id, ShaderNodeType.TextureCoordinate, "use_transform", UseTransform);

			if (UseTransform)
			{
				var obt = ObjectTransform;

				CSycles.shadernode_set_member_vec4_at_index(clientId, shaderId, Id, ShaderNodeType.TextureCoordinate, "object_transform_x", obt.x.x, obt.x.y, obt.x.z, obt.x.w, 0);
				CSycles.shadernode_set_member_vec4_at_index(clientId, shaderId, Id, ShaderNodeType.TextureCoordinate, "object_transform_y", obt.y.x, obt.y.y, obt.y.z, obt.y.w, 1);
				CSycles.shadernode_set_member_vec4_at_index(clientId, shaderId, Id, ShaderNodeType.TextureCoordinate, "object_transform_z", obt.z.x, obt.z.y, obt.z.z, obt.z.w, 2);
				CSycles.shadernode_set_member_vec4_at_index(clientId, shaderId, Id, ShaderNodeType.TextureCoordinate, "object_transform_w", obt.w.x, obt.w.y, obt.w.z, obt.w.w, 3);
			}
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
		}
	}

}
