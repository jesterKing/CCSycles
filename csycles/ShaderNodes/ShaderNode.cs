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
using System.Collections.Generic;
using System.ComponentModel;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// Base class for shader nodes.
	/// </summary>
	public class ShaderNode
	{
		/// <summary>
		/// Get the node ID. This is set when created in Cycles.
		/// </summary>
		public uint Id { get; internal set; }
		/// <summary>
		/// Get the shader node type. Set in the constructor.
		/// </summary>
		public ShaderNodeType Type { get; private set; }

		/// <summary>
		/// Generic access to input sockets.
		/// </summary>
		internal Inputs inputs { get; set; }
		/// <summary>
		/// Generic access to output sockets.
		/// </summary>
		internal Outputs outputs { get; set; }

		public ShaderNode(ShaderNodeType type)
		{
			Type = type;
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has enumerations that need to be committed to Cycles
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="shaderId"></param>
		virtual internal void SetEnums(uint clientId, uint shaderId)
		{
			// do nothing
		}

		/// <summary>
		/// A node deriving from ShaderNode should override this if
		/// it has direct members that need to be committed to Cycles
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="shaderId"></param>
		virtual internal void SetDirectMembers(uint clientId, uint shaderId)
		{
			// do nothing
		}
	}
}
