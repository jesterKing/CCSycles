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

namespace ccl
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

		//internal List<string> AvailableOuts = new List<string>();

		Dictionary<PropertyDescriptor, Tuple<ShaderNode, PropertyDescriptor>> output_dictionary = new Dictionary<PropertyDescriptor, Tuple<ShaderNode, PropertyDescriptor>>();
		public void ConnectTo(PropertyDescriptor from, ShaderNode to, PropertyDescriptor toin)
		{
			if (to != null)
			{
				if (!outputs.HasSocket(from.Name) || !to.inputs.HasSocket(toin.Name))
				{
					throw new ArgumentException(String.Format("Output {0} not found. Cannot connect to {1}-{2}", from.Name, to,
						toin.Name));
				}

				if (!output_dictionary.ContainsKey(from))
				{
					output_dictionary.Add(from, Tuple.Create(to, toin));
				}
			}
			else
			{
				if (outputs.HasSocket(from.Name) && output_dictionary.ContainsKey(from))
				{
					output_dictionary.Remove(from);
				}
				else
				{
					throw new ArgumentException(String.Format("Output {0} not found, cannot clear.", from.Name));
				}
			}
		}

		public bool IsConnected
		{
			get { return output_dictionary.Count > 0; }
		}
		
		public IEnumerable<Tuple<PropertyDescriptor, ShaderNode, PropertyDescriptor>> ConnectedOutputs
		{
			get
			{
				foreach (var k in output_dictionary.Keys)
				{
					yield return Tuple.Create(k, output_dictionary[k].Item1, output_dictionary[k].Item2);
				}
			}
		}
#if notused
		/*Dictionary<string, object> inputs = new Dictionary<string,object>();*/
		public object this[string key]
		{
			get {
				return inputs[key];
			}
			set
			{
				/*if (!inputs.ContainsKey(key))
				{
					inputs.Add(key, value);
				}
				else
				{
					inputs[key] = value;
				}
				 */
				inputs[key] = value;
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				foreach (var k in inputs.Keys)
				{
					yield return k;
				}
			}
		}
		*/
#endif
	}
}
