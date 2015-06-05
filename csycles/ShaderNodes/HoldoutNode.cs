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
	public class HoldoutInputs : Inputs
	{
		public FloatSocket SurfaceMixWeight { get; set; }
		public FloatSocket VolumeMixWeight { get; set; }

		public HoldoutInputs(ShaderNode parentNode)
		{
			SurfaceMixWeight = new FloatSocket(parentNode, "SurfaceMixWeight");
			AddSocket(SurfaceMixWeight);
			VolumeMixWeight = new FloatSocket(parentNode, "VolumeMixWeight");
			AddSocket(VolumeMixWeight);
		}
	}

	public class HoldoutOutputs : Outputs
	{
		public ClosureSocket Holdout { get; set; }

		public HoldoutOutputs(ShaderNode parentNode)
		{
			Holdout = new ClosureSocket(parentNode, "Holdout");
			AddSocket(Holdout);
		}
	}

	[ShaderNode("holdout")]
	public class HoldoutNode : ShaderNode
	{
		public HoldoutInputs ins { get { return (HoldoutInputs)inputs; } }
		public HoldoutOutputs outs { get { return (HoldoutOutputs)outputs; } }

		public HoldoutNode() : this("a holdout node") { }

		public HoldoutNode(string name)
			: base(ShaderNodeType.Holdout, name)
		{
			inputs = new HoldoutInputs(this);
			outputs = new HoldoutOutputs(this);

			ins.SurfaceMixWeight.Value = 0.0f;
			ins.VolumeMixWeight.Value = 0.0f;
		}
	}
}
