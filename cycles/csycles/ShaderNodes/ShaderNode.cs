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
	public class ShaderNode
	{
		public uint Id { get; internal set; }
		public ShaderNodeType Type { get; private set; }

		public Inputs inputs { get; set; }
		public Outputs outputs { get; set; }

		public ShaderNode(ShaderNodeType type)
		{
			Type = type;
		}

		readonly Dictionary<PropertyDescriptor, Tuple<ShaderNode, PropertyDescriptor>> m_output_dictionary = new Dictionary<PropertyDescriptor, Tuple<ShaderNode, PropertyDescriptor>>();
		public void ConnectTo(PropertyDescriptor from, ShaderNode to, PropertyDescriptor toin)
		{
			if (to != null)
			{
				if (!outputs.HasSocket(from.Name) || !to.inputs.HasSocket(toin.Name))
				{
					throw new ArgumentException(String.Format("Output {0} not found. Cannot connect to {1}-{2}", from.Name, to,
						toin.Name));
				}

				if (!m_output_dictionary.ContainsKey(from))
				{
					m_output_dictionary.Add(from, Tuple.Create(to, toin));
				}
			}
			else
			{
				if (outputs.HasSocket(from.Name) && m_output_dictionary.ContainsKey(from))
				{
					m_output_dictionary.Remove(from);
				}
				else
				{
					throw new ArgumentException(String.Format("Output {0} not found, cannot clear.", from.Name));
				}
			}
		}

		public bool IsConnected
		{
			get { return m_output_dictionary.Count > 0; }
		}
		
		public IEnumerable<Tuple<PropertyDescriptor, ShaderNode, PropertyDescriptor>> ConnectedOutputs
		{
			get
			{
				foreach (var k in m_output_dictionary.Keys)
				{
					yield return Tuple.Create(k, m_output_dictionary[k].Item1, m_output_dictionary[k].Item2);
				}
			}
		}
	}
}
