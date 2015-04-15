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
	/// <summary>
	/// BumpNode input sockets
	/// </summary>
	public class BumpInputs : Inputs
	{
		/// <summary>
		/// BumpNode height of the bump
		/// </summary>
		public FloatSocket Height { get; set; }
		/// <summary>
		/// BumpNode input normal. If not connected will use default shading normal
		/// </summary>
		public Float4Socket Normal { get; set; }
		/// <summary>
		/// BumpNode strength of the bump effect
		/// </summary>
		public FloatSocket Strength { get; set; }
		/// <summary>
		/// BumpNode distance
		/// </summary>
		public FloatSocket Distance { get; set; }

		internal BumpInputs(ShaderNode parentNode)
		{
			Height = new FloatSocket(parentNode, "Height");
			AddSocket(Height);
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
			Strength = new FloatSocket(parentNode, "Strength");
			AddSocket(Strength);
			Distance = new FloatSocket(parentNode, "Distance");
			AddSocket(Distance);
		}
	}

	/// <summary>
	/// BumpNode output sockets
	/// </summary>
	public class BumpOutputs : Outputs
	{
		/// <summary>
		/// BumpNode new Normal
		/// </summary>
		public Float4Socket Normal { get; set; }

		internal BumpOutputs(ShaderNode parentNode)
		{
			Normal = new Float4Socket(parentNode, "Normal");
			AddSocket(Normal);
		}
	}

	/// <summary>
	/// BumpNode
	/// </summary>
	public class BumpNode : ShaderNode
	{
		/// <summary>
		/// BumpNode input sockets
		/// </summary>
		public BumpInputs ins { get { return (BumpInputs)inputs; } }
		/// <summary>
		/// BumpNode output sockets
		/// </summary>
		public BumpOutputs outs { get { return (BumpOutputs)outputs; } }

		/// <summary>
		/// Create new BumpNode with blend type Bump.
		/// </summary>
		public BumpNode() : this("a bump node") { }
		public BumpNode(string name) :
			base(ShaderNodeType.Bump, name)
		{
			inputs = new BumpInputs(this);
			outputs = new BumpOutputs(this);

			Invert = false;
			ins.Strength.Value = 1.0f;
			ins.Distance.Value = 0.15f;
		}

		/// <summary>
		/// BumpNode set to true to invert result
		/// </summary>
		public bool Invert { get; set; }

		internal override void SetDirectMembers(uint clientId, uint shaderId)
		{
			CSycles.shadernode_set_member_bool(clientId, shaderId, Id, Type, "invert", Invert);
		}
	}
}
