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
	public class LightPathOutputs : Outputs
	{
		public FloatSocket IsCameraRay { get; set; }
		public FloatSocket IsShadowRay { get; set; }
		public FloatSocket IsDiffuseRay { get; set; }
		public FloatSocket IsGlossyRay { get; set; }
		public FloatSocket IsSingularRay { get; set; }
		public FloatSocket IsReflectionRay { get; set; }
		public FloatSocket IsTransmissionRay { get; set; }
		public FloatSocket IsVolumeScatterRay { get; set; }
		public FloatSocket RayLength { get; set; }
		public FloatSocket RayDepth { get; set; }
		public FloatSocket TransparentDepth { get; set; }

		public LightPathOutputs(ShaderNode parentNode)
		{
			IsCameraRay = new FloatSocket(parentNode, "Is Camera Ray");
			AddSocket(IsCameraRay);
			IsShadowRay = new FloatSocket(parentNode, "Is Shadow Ray");
			AddSocket(IsShadowRay);
			IsDiffuseRay = new FloatSocket(parentNode, "Is Diffuse Ray");
			AddSocket(IsDiffuseRay);
			IsGlossyRay = new FloatSocket(parentNode, "Is Glossy Ray");
			AddSocket(IsGlossyRay);
			IsSingularRay = new FloatSocket(parentNode, "Is Singular Ray");
			AddSocket(IsSingularRay);
			IsReflectionRay = new FloatSocket(parentNode, "Is Reflection Ray");
			AddSocket(IsReflectionRay);
			IsTransmissionRay = new FloatSocket(parentNode, "Is Transmission Ray");
			AddSocket(IsTransmissionRay);
			IsVolumeScatterRay = new FloatSocket(parentNode, "Is VolumeScatter Ray");
			AddSocket(IsVolumeScatterRay);
			RayLength = new FloatSocket(parentNode, "Ray Length");
			AddSocket(RayLength);
			RayDepth = new FloatSocket(parentNode, "Ray Depth");
			AddSocket(RayDepth);
			TransparentDepth = new FloatSocket(parentNode, "Transparent Depth");
			AddSocket(TransparentDepth);
		}
	}

	public class LightPathInputs : Inputs
	{
		public LightPathInputs(ShaderNode parentNode)
		{
			
		}
	}

	public class LightPathNode : ShaderNode
	{
		public LightPathInputs ins { get { return (LightPathInputs)inputs; } set { inputs = value; } }
		public LightPathOutputs outs { get { return (LightPathOutputs)outputs; } set { outputs = value; } }

		public LightPathNode()
			: base(ShaderNodeType.LightPath)
		{
			ins = null;
			outs = new LightPathOutputs(this);
		}
	}
}
