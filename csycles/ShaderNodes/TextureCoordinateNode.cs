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
	public class TextureCoordinateOutputs : Outputs
	{
		public Float4Socket Generated { get; set; }
		public Float4Socket Normal { get; set; }
		public Float4Socket UV { get; set; }
		public Float4Socket Object { get; set; }
		public Float4Socket Camera { get; set; }
		public Float4Socket Window { get; set; }
		public Float4Socket Reflection { get; set; }

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
		}
	}

	public class TextureCoordinateInputs : Inputs
	{
		public TextureCoordinateInputs(ShaderNode parentNode)
		{
			
		}
	}

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
			//from_dupli = false;
		}
	}
}
