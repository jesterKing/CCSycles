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
using System.Linq;
using System.Reflection;
using ccl.ShaderNodes.Sockets;

namespace ccl
{
	/// <summary>
	/// Base helper class for managing input and output sockets for shader nodes.
	/// </summary>
	public class SocketCollection
	{
		readonly List<SocketBase> m_socketlist = new List<SocketBase>();

		internal void AddSocket(SocketBase sock)
		{
			m_socketlist.Add(sock);
		}

		public object this[string key]
		{
			get
			{
				Type t = this.GetType();
				PropertyInfo pi = t.GetProperty(key);
				return pi.GetValue(this, null);
			}
			set
			{
				Type t = this.GetType();
				PropertyInfo pi = t.GetProperty(key);
				pi.SetValue(this, value);
			}
		}

		public SocketBase Socket(string name)
		{
			foreach (var socket in Sockets)
			{
				if (socket.Name.ToLowerInvariant().Equals(name.ToLowerInvariant())) return socket;
			}

			throw new ArgumentException(string.Format("Socket {0} doesn't exist", name), "name");
		}

		/// <summary>
		/// Iterate over the available property names
		/// </summary>
		public IEnumerable<string> PropertyNames
		{
			get
			{
				var p = TypeDescriptor.GetProperties(this);

				for (var i = 0; i < p.Count; i++)
				{
					if (p[i].Name.Equals("PropertyNames")) continue;

					yield return p[i].Name;
				}
			}
		}

		/// <summary>
		/// True if the requested property exists.
		/// 
		/// Case insensitive.
		/// </summary>
		/// <param name="n">Name of property to search for</param>
		/// <returns></returns>
		public bool HasSocket(string n)
		{
			return PropertyNames.Any(pname => pname.ToLowerInvariant().Equals(n.ToLowerInvariant()));
		}

		public bool HasSockt(SocketBase s)
		{
			return m_socketlist.Contains(s);
		}

		/// <summary>
		/// Get an IEnumerable over sockets.
		/// </summary>
		public IEnumerable<SocketBase> Sockets
		{
			get
			{
				return m_socketlist;
			}
		}
	}
}
