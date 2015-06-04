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
					case "background":
						shader_node = new BackgroundNode(nodename);
						break;
					case "color_ramp":
						shader_node = new ColorRampNode(nodename);
						break;
					case "color":
						shader_node = new ColorNode(nodename);
						break;
					case "gamma":
						shader_node = new GammaNode(nodename);
						break;
					case "voronoi_texture":
						shader_node = new VoronoiTexture(nodename);
						break;
					case "checker_texture":
						shader_node = new CheckerTexture(nodename);
						break;
					case "brick_texture":
						shader_node = new BrickTexture(nodename);
						break;
					case "noise_texture":
						shader_node = new NoiseTexture(nodename);
						break;
					case "gradient_texture":
						shader_node = new GradientTextureNode(nodename);
						break;
					case "sky_texture":
						shader_node = new SkyTexture(nodename);
						break;
					case "wave_texture":
						shader_node = new WaveTexture(nodename);
						break;
					case "texture_coordinate":
						shader_node = new TextureCoordinateNode(nodename);
						break;
					case "image_texture":
						shader_node = new ImageTextureNode(nodename);
						break;
					case "diffuse_bsdf":
						shader_node = new DiffuseBsdfNode(nodename);
						break;
					case "translucent_bsdf":
						shader_node = new TranslucentBsdfNode(nodename);
						break;
					case "emission_bsdf":
						shader_node = new EmissionNode(nodename);
						break;
					case "velvet_bsdf":
						shader_node = new VelvetBsdfNode(nodename);
						break;
					case "glass_bsdf":
						shader_node = new GlassBsdfNode(nodename);
						break;
					case "glossy_bsdf":
						shader_node = new GlossyBsdfNode(nodename);
						break;
					case "layer_weight":
						shader_node = new LayerWeightNode(nodename);
						break;
					case "fresnel":
						shader_node = new FresnelNode(nodename);
						break;
					case "math":
						shader_node = new MathNode(nodename);
						break;
					case "mapping":
						shader_node = new MappingNode(nodename);
						break;
					case "combine_rgb":
						shader_node = new CombineRgbNode(nodename);
						break;
					case "separate_rgb":
						shader_node = new SeparateRgbNode(nodename);
						break;
					case "combine_hsv":
						shader_node = new CombineHsvNode(nodename);
						break;
					case "separate_hsv":
						shader_node = new SeparateHsvNode(nodename);
						break;
					case "combine_xyz":
						shader_node = new CombineXyzNode(nodename);
						break;
					case "separate_xyz":
						shader_node = new SeparateXyzNode(nodename);
						break;
					case "mixrgb":
						shader_node = new MixNode(nodename);
						break;
					case "mixclosure":
						shader_node = new MixClosureNode(nodename);
						break;
					case "addclosure":
						shader_node = new AddClosureNode();
						break;
					case "holdout":
						shader_node = new HoldoutNode();
						break;
					case "hue_saturation":
						shader_node = new HueSaturationNode(nodename);
						break;
					case "brightness":
						shader_node = new BrightnessContrastNode(nodename);
						break;
					case "light_falloff":
						shader_node = new LightFalloffNode(nodename);
						break;
					case "rgb_to_luminance":
						shader_node = new RgbToLuminanceNode(nodename);
						break;
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
