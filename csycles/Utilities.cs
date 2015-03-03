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


		public void ReadNodeGraph(ref Shader shader, System.Xml.XmlReader node)
		{
			var nodes = new Dictionary<string, ShaderNode> {{"output", shader.Output}};

			while (node.Read())
			{
				if (!node.IsStartElement()) continue;
				var nodename = node.GetAttribute("name");
				if (string.IsNullOrEmpty(nodename) && node.Name != "connect") continue;

				if (string.IsNullOrEmpty(nodename)) nodename = "";

				switch (node.Name)
				{
					case "background":
						var bgnode = new BackgroundNode();
						get_float4(bgnode.ins.Color, node.GetAttribute("color"));
						get_float(bgnode.ins.Strength, node.GetAttribute("strength"));
						nodes.Add(nodename, bgnode);
						shader.AddNode(bgnode);
						break;
					case "gamma":
						var gamma = new GammaNode();
						get_float4(gamma.ins.Color, node.GetAttribute("color"));
						get_float(gamma.ins.Gamma, node.GetAttribute("gamma"));
						nodes.Add(nodename, gamma);
						shader.AddNode(gamma);
						break;
					case "voronoi_texture":
						var voronoi = new VoronoiTexture();
						get_float4(voronoi.ins.Vector, node.GetAttribute("vector"));
						get_float(voronoi.ins.Scale, node.GetAttribute("scale"));
						var coloring = node.GetAttribute("coloring");
						if (!string.IsNullOrEmpty(coloring))
						{
							voronoi.Coloring = coloring;
						}
						nodes.Add(nodename, voronoi);
						shader.AddNode(voronoi);
						break;
					case "checker_texture":
						var checkernode = new CheckerTexture();
						get_float4(checkernode.ins.Color1, node.GetAttribute("Color1"));
						get_float4(checkernode.ins.Color2, node.GetAttribute("Color2"));
						nodes.Add(nodename, checkernode);
						shader.AddNode(checkernode);
						break;
					case "brick_texture":
						var bricktex = new BrickTexture();
						var offset = 0.0f;
						var offset_frequency = 0;
						var squash = 0.0f;
						var squash_frequency = 0;
						if (read_float(ref offset, node.GetAttribute("offset")))
							bricktex.Offset = offset;
						if (read_int(ref offset_frequency, node.GetAttribute("offset_frequency")))
							bricktex.OffsetFrequency = offset_frequency;
						if (read_float(ref squash, node.GetAttribute("squash")))
							bricktex.Squash = squash;
						if (read_int(ref squash_frequency, node.GetAttribute("squash_frequency")))
							bricktex.SquashFrequency = squash_frequency;

						get_float4(bricktex.ins.Color1, node.GetAttribute("color1"));
						get_float4(bricktex.ins.Color2, node.GetAttribute("color2"));
						get_float4(bricktex.ins.Mortar, node.GetAttribute("mortar"));
						get_float(bricktex.ins.Bias, node.GetAttribute("bias"));
						get_float(bricktex.ins.BrickWidth, node.GetAttribute("brick_width"));
						get_float(bricktex.ins.RowHeight, node.GetAttribute("row_height"));

						nodes.Add(nodename, bricktex);
						shader.AddNode(bricktex);
						break;
					case "noise_texture":
						var noisenode = new NoiseTexture();
						get_float4(noisenode.ins.Vector, node.GetAttribute("vector"));
						get_float(noisenode.ins.Detail, node.GetAttribute("detail"));
						get_float(noisenode.ins.Distortion, node.GetAttribute("distortion"));
						get_float(noisenode.ins.Scale, node.GetAttribute("scale"));
						nodes.Add(nodename, noisenode);
						shader.AddNode(noisenode);
						break;
					case "sky_texture":
						var skynode = new SkyTexture();

						get_float4(skynode.ins.Vector, node.GetAttribute("vector"));

						var sun_direction = node.GetAttribute("sun_direction");
						var turbidity = node.GetAttribute("turbidity");
						var ground_albedo = node.GetAttribute("ground_albedo");
						var sky_type = node.GetAttribute("type");

						if (!string.IsNullOrEmpty(sun_direction))
						{
							get_float4(skynode.SunDirection, sun_direction);
						}
						if (!string.IsNullOrEmpty(turbidity))
						{
							skynode.Turbidity = float.Parse(turbidity, NumberFormatInfo);
						}
						if (!string.IsNullOrEmpty(ground_albedo))
						{
							skynode.GroundAlbedo = float.Parse(ground_albedo, NumberFormatInfo);
						}
						if(!string.IsNullOrEmpty(sky_type))
						{
							skynode.SkyType = sky_type;
						}

						nodes.Add(nodename, skynode);
						shader.AddNode(skynode);
						break;
					case "wave_texture":
						var wavenode = new WaveTexture();

						get_float4(wavenode.ins.Vector, node.GetAttribute("vector"));
						get_float(wavenode.ins.Scale, node.GetAttribute("scale"));
						get_float(wavenode.ins.Distortion, node.GetAttribute("distortion"));
						get_float(wavenode.ins.Detail, node.GetAttribute("detail"));
						get_float(wavenode.ins.DetailScale, node.GetAttribute("detail_scale"));

						var wavetype = node.GetAttribute("wave_type");
						if (!string.IsNullOrEmpty(wavetype))
						{
							wavenode.WaveType = wavetype;
						}

						nodes.Add(nodename, wavenode);
						shader.AddNode(wavenode);
						break;
					case "texture_coordinate":
						var texcoord = new TextureCoordinateNode();
						nodes.Add(nodename, texcoord);
						shader.AddNode(texcoord);
						break;
					case "image_texture":
						var imgtex = new ImageTextureNode();
						var imgsrc = node.GetAttribute("src");
						if (!string.IsNullOrEmpty(imgsrc))
						{
							using (var bmp = new Bitmap(imgsrc))
							{
								var l = bmp.Width*bmp.Height*4;
								var bmpdata = new byte[l];
								for (var x = 0; x < bmp.Width; x++)
								{
									for (var y = 0; y < bmp.Height; y++)
									{
										var pos = y*bmp.Width*4 + x*4;
										var pixel = bmp.GetPixel(x, y);
										bmpdata[pos] = pixel.R;
										bmpdata[pos + 1] = pixel.G;
										bmpdata[pos + 2] = pixel.B;
										bmpdata[pos + 3] = pixel.A;
									}
								}
								imgtex.ByteImage = bmpdata;
								imgtex.Width = (uint)bmp.Width;
								imgtex.Height = (uint)bmp.Height;
							}
						}
						nodes.Add(nodename, imgtex);
						shader.AddNode(imgtex);
						break;
					case "diffuse_bsdf":
						var diffbsdf = new DiffuseBsdfNode();
						get_float4(diffbsdf.ins.Color, node.GetAttribute("color"));
						nodes.Add(nodename, diffbsdf);
						shader.AddNode(diffbsdf);
						break;
					case "emission_bsdf":
						var emisbsdf = new EmissionNode();
						get_float4(emisbsdf.ins.Color, node.GetAttribute("color"));
						get_float(emisbsdf.ins.Strength, node.GetAttribute("strength"));
						nodes.Add(nodename, emisbsdf);
						shader.AddNode(emisbsdf);
						break;
					case "velvet_bsdf":
						var velvetbsdf = new VelvetBsdfNode();
						get_float4(velvetbsdf.ins.Color, node.GetAttribute("color"));
						get_float(velvetbsdf.ins.Sigma, node.GetAttribute("sigma"));
						get_float4(velvetbsdf.ins.Normal, node.GetAttribute("normal"));
						nodes.Add(nodename, velvetbsdf);
						shader.AddNode(velvetbsdf);
						break;
					case "glass_bsdf":
						var glassbsdf = new GlassBsdfNode();
						get_float4(glassbsdf.ins.Color, node.GetAttribute("color"));
						get_float4(glassbsdf.ins.Normal, node.GetAttribute("normal"));
						get_float(glassbsdf.ins.Roughness, node.GetAttribute("roughness"));
						get_float(glassbsdf.ins.IOR, node.GetAttribute("ior"));
						var glassdistribution = node.GetAttribute("distribution");
						if (!string.IsNullOrEmpty(glassdistribution))
						{
							glassbsdf.Distribution = glassdistribution;
						}
						nodes.Add(nodename, glassbsdf);
						shader.AddNode(glassbsdf);
						break;
					case "glossy_bsdf":
						var glossybsdf = new GlossyBsdfNode();
						get_float4(glossybsdf.ins.Color, node.GetAttribute("color"));
						get_float4(glossybsdf.ins.Normal, node.GetAttribute("normal"));
						get_float(glossybsdf.ins.Roughness, node.GetAttribute("roughness"));
						var glossydistribution = node.GetAttribute("distribution");
						if (!string.IsNullOrEmpty(glossydistribution))
						{
							glossybsdf.Distribution = glossydistribution;
						}
						nodes.Add(nodename, glossybsdf);
						shader.AddNode(glossybsdf);
						break;
					case "layer_weight":
						var layer_weight = new LayerWeightNode();
						get_float(layer_weight.ins.Blend, node.GetAttribute("blend"));
						get_float4(layer_weight.ins.Normal, node.GetAttribute("normal"));
						nodes.Add(nodename, layer_weight);
						shader.AddNode(layer_weight);
						break;
					case "fresnel":
						var fresnel = new FresnelNode();
						get_float(fresnel.ins.IOR, node.GetAttribute("ior"));
						get_float4(fresnel.ins.Normal, node.GetAttribute("normal"));
						nodes.Add(nodename, fresnel);
						shader.AddNode(fresnel);
						break;
					case "math":
						var math = new MathNode();
						get_float(math.ins.Value1, node.GetAttribute("value1"));
						get_float(math.ins.Value2, node.GetAttribute("value2"));
						var operation = node.GetAttribute("type");
						if (!string.IsNullOrEmpty(operation))
						{
							operation = operation.ToLowerInvariant();
							try
							{
								var op = (MathNode.Operations)Enum.Parse(typeof(MathNode.Operations), operation, true);
								math.Operation = op;
							}
							catch (ArgumentException)
							{
								math.Operation = MathNode.Operations.Add;
							}
						}
						nodes.Add(nodename, math);
						shader.AddNode(math);
						break;
					case "mapping":
						var mapping = new MappingNode();
						get_float4(mapping.ins.Vector, node.GetAttribute("vector"));
						nodes.Add(nodename, mapping);
						shader.AddNode(mapping);
						break;
					case "combine_rgb":
						var combine_rgb = new CombineRgbNode();
						get_float(combine_rgb.ins.R, node.GetAttribute("R"));
						get_float(combine_rgb.ins.G, node.GetAttribute("G"));
						get_float(combine_rgb.ins.B, node.GetAttribute("B"));
						nodes.Add(nodename, combine_rgb);
						shader.AddNode(combine_rgb);
						break;
					case "separate_rgb":
						var separate_rgb = new SeparateRgbNode();
						get_float4(separate_rgb.ins.Color, node.GetAttribute("color"));
						nodes.Add(nodename, separate_rgb);
						shader.AddNode(separate_rgb);
						break;
					case "combine_hsv":
						var combine_hsv = new CombineHsvNode();
						get_float(combine_hsv.ins.H, node.GetAttribute("H"));
						get_float(combine_hsv.ins.S, node.GetAttribute("S"));
						get_float(combine_hsv.ins.V, node.GetAttribute("V"));
						nodes.Add(nodename, combine_hsv);
						shader.AddNode(combine_hsv);
						break;
					case "separate_hsv":
						var separate_hsv = new SeparateHsvNode();
						get_float4(separate_hsv.ins.Color, node.GetAttribute("color"));
						nodes.Add(nodename, separate_hsv);
						shader.AddNode(separate_hsv);
						break;
					case "combine_xyz":
						var combine_xyz = new CombineXyzNode();
						get_float(combine_xyz.ins.X, node.GetAttribute("X"));
						get_float(combine_xyz.ins.Y, node.GetAttribute("Y"));
						get_float(combine_xyz.ins.Z, node.GetAttribute("Z"));
						nodes.Add(nodename, combine_xyz);
						shader.AddNode(combine_xyz);
						break;
					case "separate_xyz":
						var separate_xyz = new SeparateXyzNode();
						get_float4(separate_xyz.ins.Vector, node.GetAttribute("vector"));
						nodes.Add(nodename, separate_xyz);
						shader.AddNode(separate_xyz);
						break;
					case "mixrgb":
						var mixrgb = new MixNode();
						get_float4(mixrgb.ins.Color1, node.GetAttribute("color1"));
						get_float4(mixrgb.ins.Color2, node.GetAttribute("color2"));
						get_float(mixrgb.ins.Fac, node.GetAttribute("fac"));
						nodes.Add(nodename, mixrgb);
						shader.AddNode(mixrgb);
						break;
					case "mixclosure":
						var mixclosure = new MixClosureNode();
						get_float(mixclosure.ins.Fac, node.GetAttribute("fac"));
						nodes.Add(nodename, mixclosure);
						shader.AddNode(mixclosure);
						break;
					case "addclosure":
						var add = new AddClosureNode();
						nodes.Add(nodename, add);
						shader.AddNode(add);
						break;
					case "holdout":
						var holdout = new HoldoutNode();
						nodes.Add(nodename, holdout);
						shader.AddNode(holdout);
						break;
					case "hue_saturation":
						var huesat = new HueSaturationNode();
						get_float(huesat.ins.Hue, node.GetAttribute("hue"));
						get_float(huesat.ins.Saturation, node.GetAttribute("saturation"));
						get_float(huesat.ins.Value, node.GetAttribute("value"));
						get_float(huesat.ins.Fac, node.GetAttribute("fac"));
						get_float4(huesat.ins.Color, node.GetAttribute("color"));
						nodes.Add(nodename, huesat);
						shader.AddNode(huesat);
						break;
					case "brightness":
						var brightness = new BrightnessContrastNode();
						get_float4(brightness.ins.Color, node.GetAttribute("color"));
						get_float(brightness.ins.Bright, node.GetAttribute("bright"));
						get_float(brightness.ins.Contrast, node.GetAttribute("contrast"));
						nodes.Add(nodename, brightness);
						shader.AddNode(brightness);
						break;
					case "light_falloff":
						var light_falloff = new LightFalloffNode();
						get_float(light_falloff.ins.Smooth, node.GetAttribute("smooth"));
						get_float(light_falloff.ins.Strength, node.GetAttribute("strength"));
						nodes.Add(nodename, light_falloff);
						shader.AddNode(light_falloff);
						break;
					case "connect":
						var fromstring = node.GetAttribute("from");
						var tostring = node.GetAttribute("to");
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
			}

			shader.FinalizeGraph();
		}
	}
}
