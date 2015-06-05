using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;

namespace ccl
{
	public class Utilities
	{
		public static Utilities g_utilities;

		public NumberFormatInfo NumberFormatInfo { get; set; }
		public Utilities()
		{
			NumberFormatInfo = CultureInfo.InvariantCulture.NumberFormat;
		}

		public static Utilities Instance
		{
			get
			{
				return g_utilities ?? (g_utilities = new Utilities());
			}
		}

		public float[] parse_floats(string floats)
		{
			floats = floats.Trim();
			floats = floats.Replace("  ", " ");
			floats = floats.Replace(",", "");
			var fs = floats.Split(' ');
			var realfloats = new float[fs.Length];
			for (var i = 0; i < fs.Length; i++)
			{
				realfloats[i] = float.Parse(fs[i], NumberFormatInfo);
			}

			return realfloats;
		}

		public void get_float4(Float4Socket socket, string floats)
		{
			if (string.IsNullOrEmpty(floats)) return;

			var vec = parse_floats(floats);
			socket.Value = new float4(vec[0], vec[1], vec[2]);
		}

		public void get_float(FloatSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = float.Parse(nr, NumberFormatInfo);
		}

		public void get_float4(float4 f4, string floats)
		{
			if (string.IsNullOrEmpty(floats)) return;

			var vec = parse_floats(floats);
			if (vec.Length < 3 || vec.Length > 4) return;

			f4.x = vec[0];
			f4.y = vec[1];
			f4.z = vec[2];
			if (vec.Length == 4)
				f4.w = vec[3];
		}

		public bool read_float(ref float val, string floatstring)
		{
			if (string.IsNullOrEmpty(floatstring)) return false;

			val = float.Parse(floatstring, NumberFormatInfo);
			return true;
		}

		public void get_int(IntSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = int.Parse(nr);
		}

		public int[] parse_ints(string ints)
		{
			ints = ints.Trim();
			ints = ints.Replace("  ", " ");
			ints = ints.Replace(",", "");
			var fs = ints.Split(' ');
			var realints = new int[fs.Length];
			for (var i = 0; i < fs.Length; i++)
			{
				realints[i] = int.Parse(fs[i]);
			}

			return realints;
		}

		public bool read_bool(ref bool val, string booleanstring)
		{
			if (string.IsNullOrEmpty(booleanstring))
			{
				val = false;
				return false;
			}

			val = bool.Parse(booleanstring);
			return true;
		}

		public bool read_int(ref int val, string intstring)
		{
			if (string.IsNullOrEmpty(intstring)) return false;

			val = int.Parse(intstring);
			return true;
		}

		public bool read_string(ref string val, string stringstring)
		{
			if (string.IsNullOrEmpty(stringstring)) return false;

			val = stringstring;
			return true;
		}


		public void ReadNodeGraph(ref Shader shader, System.Xml.XmlReader xmlNode)
		{
			var nodes = new Dictionary<string, ShaderNode> {{"output", shader.Output}};

			while (xmlNode.Read())
			{
				ShaderNode shader_node = null;
				if (!xmlNode.IsStartElement()) continue;
				var nodename = xmlNode.GetAttribute("name");
				if (string.IsNullOrEmpty(nodename) && xmlNode.Name != "connect") continue;

				if (string.IsNullOrEmpty(nodename)) nodename = "";

				switch (xmlNode.Name)
				{
					case "connect":
						var fromstring = xmlNode.GetAttribute("from");
						var tostring = xmlNode.GetAttribute("to");
						if (fromstring != null && tostring != null)
						{
							var from = fromstring.Split(' ');
							var to = tostring.Split(' ');

							var fromnode = nodes[from[0]];
							var fromsocket = fromnode.outputs.Socket(from[1]);

							var tonode = nodes[to[0]];
							var tosocket = tonode.inputs.Socket(to[1]);

							fromsocket.Connect(tosocket);
						}
						break;
					default:
						shader_node = CSycles.CreateShaderNode(xmlNode.Name, nodename);
						break;
				}
				if (shader_node != null)
				{
					shader_node.ParseXml(xmlNode);
					nodes.Add(nodename, shader_node);
					shader.AddNode(shader_node);
				}
			}

			shader.FinalizeGraph();
		}
	}
}
