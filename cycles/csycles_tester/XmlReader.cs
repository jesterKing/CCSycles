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

using System.Configuration;
using System.Globalization;
using System.Linq;
using ccl;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace csycles_tester
{
	public class XmlReader
	{
		private Client Client { get; set; }
		private string Path { get; set; }
		private NumberFormatInfo NumberFormatInfo { get; set; }
		public XmlReader(Client client, string path)
		{
			Client = client;
			Path = path;
			NumberFormatInfo = NumberFormatInfo.InvariantInfo;
		}

		public static float DegToRad(float ang)
		{
			return ang * (float)Math.PI / 180.0f;
		}

		private void ReadCamera(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();
			var width = node.GetAttribute("width");
			var height = node.GetAttribute("height");
			var type = node.GetAttribute("type");
			var fov = node.GetAttribute("fov");
			var nearclip = node.GetAttribute("nearclip");
			var farclip = node.GetAttribute("farclip");
			var aperturesize = node.GetAttribute("aperturesize");
			var focaldistance = node.GetAttribute("focaldistance");
			var shuttertime = node.GetAttribute("shuttertime");
			var panorama_type = node.GetAttribute("panorama_type");
			var fisheye_fov = node.GetAttribute("fisheye_fov");
			var fisheye_lens = node.GetAttribute("fisheye_lens");
			var sensorwidth = node.GetAttribute("sensorwidth");
			var sensorheight = node.GetAttribute("sensorheight");

			if (!string.IsNullOrEmpty(width) && !string.IsNullOrEmpty(height)) Client.Scene.Camera.Size = new Size(int.Parse(width), int.Parse(height));

			if (!string.IsNullOrEmpty(type))
			{
				CameraType camera_type;
				if (Enum.TryParse(type, true, out camera_type))
				{
					Client.Scene.Camera.Type = camera_type;
				}
			}

			if (!string.IsNullOrEmpty(panorama_type))
			{
				PanoramaType panoramatype;
				if (Enum.TryParse(panorama_type, true, out panoramatype))
				{
					Client.Scene.Camera.PanoramaType = panoramatype;
				}
			}

			if (!string.IsNullOrEmpty(fov)) Client.Scene.Camera.Fov = float.Parse(fov, NumberFormatInfo);

			if (!string.IsNullOrEmpty(nearclip)) Client.Scene.Camera.NearClip = float.Parse(nearclip, NumberFormatInfo);

			if (!string.IsNullOrEmpty(farclip)) Client.Scene.Camera.FarClip = float.Parse(farclip, NumberFormatInfo);

			if (!string.IsNullOrEmpty(aperturesize)) Client.Scene.Camera.ApertureSize = float.Parse(aperturesize, NumberFormatInfo);

			if (!string.IsNullOrEmpty(focaldistance)) Client.Scene.Camera.FocalDistance = float.Parse(focaldistance, NumberFormatInfo);

			if (!string.IsNullOrEmpty(shuttertime)) Client.Scene.Camera.ShutterTime = float.Parse(shuttertime, NumberFormatInfo);

			if (!string.IsNullOrEmpty(fisheye_fov)) Client.Scene.Camera.FishEyeFov = float.Parse(fisheye_fov, NumberFormatInfo);

			if (!string.IsNullOrEmpty(fisheye_lens)) Client.Scene.Camera.FishEyeLens = float.Parse(fisheye_lens, NumberFormatInfo);

			if (!string.IsNullOrEmpty(sensorwidth)) Client.Scene.Camera.SensorWidth = float.Parse(sensorwidth, NumberFormatInfo);

			if (!string.IsNullOrEmpty(sensorheight)) Client.Scene.Camera.SensorHeight = float.Parse(sensorheight, NumberFormatInfo);

			Client.Scene.Camera.Matrix = state.Transform;
			Client.Scene.Camera.ComputeAutoViewPlane();
			Client.Scene.Camera.Update();

		}

		private void ReadBackground(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();
			Console.WriteLine("Background shader");

			var shader = new Shader(Client, Shader.ShaderType.World) {Name = Guid.NewGuid().ToString()};

			//node.Read(); // advance to next node

			ReadNodeGraph(ref state, ref shader, node.ReadSubtree());

			state.Scene.AddShader(shader);
			state.Scene.Background.Shader = shader;

		}

		private void ReadIntegrator(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();
			/* \todo AA sample stuff */
			var boolvar = false;
			var intvar = 0;
			var floatvar = 0.0f;
			var stringvar = "";

			read_bool(ref boolvar, node.GetAttribute("branched"));
			state.Scene.Integrator.IntegratorMethod = boolvar ? IntegratorMethod.BranchedPath : IntegratorMethod.Path;

			if (read_bool(ref boolvar, node.GetAttribute("sample_all_lights_direct")))
				state.Scene.Integrator.SampleAllLightsDirect = boolvar;
			if (read_bool(ref boolvar, node.GetAttribute("sample_all_lights_indirect")))
				state.Scene.Integrator.SampleAllLightsIndirect = boolvar;
			if (read_int(ref intvar, node.GetAttribute("diffuse_samples"))) state.Scene.Integrator.DiffuseSamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("glossy_samples"))) state.Scene.Integrator.GlossySamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("transmission_samples"))) state.Scene.Integrator.TransmissionSamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("ao_samples"))) state.Scene.Integrator.AoSamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("mesh_light_samples"))) state.Scene.Integrator.MeshLightSamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("subsurface_samples"))) state.Scene.Integrator.SubsurfaceSamples = intvar;
			if (read_int(ref intvar, node.GetAttribute("volume_samples"))) state.Scene.Integrator.VolumeSamples = intvar;

			if (read_int(ref intvar, node.GetAttribute("min_bounce"))) state.Scene.Integrator.MinBounce = intvar;
			if (read_int(ref intvar, node.GetAttribute("max_bounce"))) state.Scene.Integrator.MaxBounce = intvar;

			if (read_int(ref intvar, node.GetAttribute("max_diffuse_bounce"))) state.Scene.Integrator.MaxDiffuseBounce = intvar;
			if (read_int(ref intvar, node.GetAttribute("max_glossy_bounce"))) state.Scene.Integrator.MaxGlossyBounce = intvar;
			if (read_int(ref intvar, node.GetAttribute("max_transmission_bounce"))) state.Scene.Integrator.MaxTransmissionBounce = intvar;
			if (read_int(ref intvar, node.GetAttribute("max_volume_bounce"))) state.Scene.Integrator.MaxVolumeBounce = intvar;

			if (read_int(ref intvar, node.GetAttribute("transparent_min_bounce"))) state.Scene.Integrator.TransparentMinBounce = intvar;
			if (read_int(ref intvar, node.GetAttribute("transparent_max_bounce"))) state.Scene.Integrator.TransparentMaxBounce = intvar;
			if (read_bool(ref boolvar, node.GetAttribute("transparent_shadows"))) state.Scene.Integrator.TransparentShadows = boolvar;

			if (read_int(ref intvar, node.GetAttribute("volume_homogeneous_sampling"))) state.Scene.Integrator.VolumeHomogeneousSampling = intvar;
			if (read_float(ref floatvar, node.GetAttribute("volume_step_size"))) state.Scene.Integrator.VolumeStepSize = floatvar;
			if (read_int(ref intvar, node.GetAttribute("volume_max_steps"))) state.Scene.Integrator.VolumeMaxSteps = intvar;

			/* \todo wrap caustics form separation
			 * 
			if (read_bool(ref boolvar, node.GetAttribute("caustics_reflective"))) state.Scene.Integrator.DoCausticsReflective = boolvar;
			if (read_bool(ref boolvar, node.GetAttribute("caustics_refractive"))) state.Scene.Integrator.DoCausticsRefractive = boolvar;
			 */
			if (read_bool(ref boolvar, node.GetAttribute("no_caustics"))) state.Scene.Integrator.NoCaustics = boolvar;
			if (read_float(ref floatvar, node.GetAttribute("filter_glossy"))) state.Scene.Integrator.FilterGlossy = floatvar;

			if (read_int(ref intvar, node.GetAttribute("seed"))) state.Scene.Integrator.Seed = intvar;
			if (read_float(ref floatvar, node.GetAttribute("sample_clamp_direct"))) state.Scene.Integrator.SampleClampDirect = floatvar;
			if (read_float(ref floatvar, node.GetAttribute("sample_clamp_indirect"))) state.Scene.Integrator.SampleClampIndirect = floatvar;

			if (read_string(ref stringvar, node.GetAttribute("sampling_pattern")))
				state.Scene.Integrator.SamplingPattern = stringvar.Equals("sobol") ? SamplingPattern.Sobol : SamplingPattern.CMJ;

			state.Scene.Integrator.TagForUpdate();
		}

		/// <summary>
		/// Read a transform from XML.
		/// 
		/// If all are available then they are read and applied to transform according formula:
		/// 
		/// transform = ((matrix * translate) * rotate) * scale
		/// </summary>
		/// <param name="node"></param>
		/// <param name="transform"></param>
		private void ReadTransform(System.Xml.XmlReader node, ref Transform transform)
		{
			var mat = node.GetAttribute("matrix");
			if (!string.IsNullOrEmpty(mat))
			{
				var matrix = parse_floats(mat);
				if(matrix.Length==16)
				{
					var t = new Transform(matrix);
					transform = t;
				}
			}

			var rotate = node.GetAttribute("rotate");
			if (!string.IsNullOrEmpty(rotate))
			{
				var components = parse_floats(rotate);
				if (components.Length == 4)
				{
					var a = DegToRad(components[0]);
					var axis = new float4(components[1], components[2], components[3]);
					transform = transform*ccl.Transform.Rotate(a, axis);
				}
			}

			var trans = node.GetAttribute("translate");
			if (!string.IsNullOrEmpty(trans))
			{
				var components = parse_floats(trans);
				if(components.Length==3)
				{
					transform = transform*ccl.Transform.Translate(components[0], components[1], components[2]);
				}
			}

			var scale = node.GetAttribute("scale");
			if (!string.IsNullOrEmpty(scale))
			{
				var components = parse_floats(scale);
				if(components.Length == 3)
				{
					transform = transform*ccl.Transform.Scale(components[0], components[1], components[2]);
				}
			}
		}

		private void ReadState(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();

			var shader = node.GetAttribute("shader");
			var dicing_rate = node.GetAttribute("dicing_rate");
			var interpolation = node.GetAttribute("interpolation");
			var displacement_method = node.GetAttribute("displacement_method");

			if (!string.IsNullOrEmpty(shader))
			{
				state.Shader = state.Scene.ShaderWithName(shader);
			}

			if (!string.IsNullOrEmpty(dicing_rate)) state.DicingRate = float.Parse(dicing_rate, NumberFormatInfo);
			if (!string.IsNullOrEmpty(interpolation)) state.Smooth = interpolation.Equals("smooth", StringComparison.OrdinalIgnoreCase);

			if (!string.IsNullOrEmpty(displacement_method))
			{
				/* \todo wrap displacement method stuff */
			}
		}

		private float[] parse_floats(string floats)
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

		private bool read_bool(ref bool val, string booleanstring)
		{
			if (string.IsNullOrEmpty(booleanstring))
			{
				val = false;
				return false;
			}

			val = bool.Parse(booleanstring);
			return true;
		}

		private bool read_int(ref int val, string intstring)
		{
			if (string.IsNullOrEmpty(intstring)) return false;

			val = int.Parse(intstring);
			return true;
		}

		private bool read_string(ref string val, string stringstring)
		{
			if (string.IsNullOrEmpty(stringstring)) return false;

			val = stringstring;
			return true;
		}

		private bool read_float(ref float val, string floatstring)
		{
			if (string.IsNullOrEmpty(floatstring)) return false;

			val = float.Parse(floatstring, NumberFormatInfo);
			return true;
		}

		private void get_float(FloatSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = float.Parse(nr, NumberFormatInfo);
		}

		private void get_float4(Float4Socket socket, string floats)
		{
			if (string.IsNullOrEmpty(floats)) return;

			var vec = parse_floats(floats);
			socket.Value = new float4(vec[0], vec[1], vec[2]);
		}

		private float4 get_float4(string floats)
		{
			var vec = parse_floats(floats);
			return new float4(vec[0], vec[1], vec[2]);
		}

		private void get_int(IntSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = int.Parse(nr);
		}

		private int[] parse_ints(string ints)
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

		private void ReadMesh(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();

			var P = node.GetAttribute("P");
			var UV = node.GetAttribute("UV");
			var nverts = node.GetAttribute("nverts");
			var verts = node.GetAttribute("verts");
			Console.WriteLine("{0}", node);

			var has_uv = !string.IsNullOrEmpty(UV);

			float[] uvfloats = null;
			if (has_uv)
			{
				uvfloats = parse_floats(UV);
			}
			var pfloats = parse_floats(P);
			var nvertsints = parse_ints(nverts);
			var vertsints = parse_ints(verts);

			var ob = CSycles.scene_add_object(Client.Id, state.Scene.Id);
			CSycles.object_set_matrix(Client.Id, state.Scene.Id, ob, state.Transform);
			var me = CSycles.scene_add_mesh(Client.Id, state.Scene.Id, ob, state.Scene.ShaderSceneId(state.Shader));

			CSycles.mesh_set_verts(Client.Id, state.Scene.Id, me, ref pfloats, (uint) (pfloats.Length/3));

			var index_offset = 0;
			/* count triangles */
			var fc = nvertsints.Aggregate(0, (total, next) =>
																		next == 4 ? total + 2 : total + 1);

			float[] uvs = null;
			if(has_uv) uvs = new float[fc*3*2];
			var uvoffs = 0;
			foreach (var t in nvertsints)
			{
				for (var j = 0; j < t - 2; j++)
				{
					var v0 = vertsints[index_offset];
					var v1 = vertsints[index_offset + j + 1];
					var v2 = vertsints[index_offset + j + 2];

					if (has_uv)
					{
						uvs[uvoffs] = uvfloats[index_offset*2];
						uvs[uvoffs + 1] = uvfloats[index_offset*2 + 1];
						uvs[uvoffs + 2] = uvfloats[(index_offset + j + 1)*2];
						uvs[uvoffs + 3] = uvfloats[(index_offset + j + 1)*2 + 1];
						uvs[uvoffs + 4] = uvfloats[(index_offset + j + 2)*2];
						uvs[uvoffs + 5] = uvfloats[(index_offset + j + 2)*2 + 1];

						uvoffs += 6;
					}

					CSycles.mesh_add_triangle(Client.Id, state.Scene.Id, me, (uint)v0, (uint)v1, (uint)v2, state.Scene.ShaderSceneId(state.Shader), state.Smooth);
				}

				index_offset += t;
			}

			if (has_uv)
			{
				CSycles.mesh_set_uvs(Client.Id, state.Scene.Id, me, ref uvs, (uint) (uvs.Length/2));
			}
		}

		private void ReadScene(ref XmlReadState state, System.Xml.XmlReader node)
		{
			
			while (node.Read())
			{
				if (!node.IsStartElement()) continue;

				Console.WriteLine("XML node: {0}", node.Name);
				switch (node.Name)
				{
					case "camera":
						ReadCamera(ref state, node.ReadSubtree());
						break;
					case "background":
						ReadBackground(ref state, node.ReadSubtree());
						break;
					case "transform":
						var transform_substate = new XmlReadState(state);
						var t = transform_substate.Transform;
						ReadTransform(node, ref t);
						transform_substate.Transform = t;
						node.Read(); /* advance forward one, otherwise we'll end up in internal loop */
						ReadScene(ref transform_substate, node.ReadSubtree());
						break;
					case "state":
						var state_substate = new XmlReadState(state);
						ReadState(ref state_substate, node.ReadSubtree());
						node.Read(); /* advance one forward */
						ReadScene(ref state_substate, node.ReadSubtree());
						break;
					case "integrator":
						ReadIntegrator(ref state, node.ReadSubtree());
						break;
					case "shader":
						var shader_substate = new XmlReadState(state);
						ReadShader(ref shader_substate, node.ReadSubtree());
						break;
					case "mesh":
						ReadMesh(ref state, node.ReadSubtree());
						break;
					case "include":
						var src = node.GetAttribute("src");
						if (!string.IsNullOrEmpty(src))
						{
							ReadInclude(ref state, src);
						}
						break;
					default:
						Console.WriteLine("Uknown node {0}", node.Name);
						break;
				}
			}
		}

		public void ReadNodeGraph(ref XmlReadState state, ref Shader shader, System.Xml.XmlReader node)
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
							skynode.SunDirection = get_float4(sun_direction);
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

		public void ReadShader(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();
			var name = node.GetAttribute("name");
			Console.WriteLine("Shader: {0}", node.GetAttribute("name"));
			if (string.IsNullOrEmpty(name)) return;

			var shader = new Shader(Client, Shader.ShaderType.Material) {Name = name};

			//node.Read(); // advance to next node

			ReadNodeGraph(ref state, ref shader, node.ReadSubtree());

			state.Scene.AddShader(shader);
		}

		/// <summary>
		/// Read the scene description from the file in src
		/// </summary>
		/// <param name="state"></param>
		/// <param name="src"></param>
		public void ReadInclude(ref XmlReadState state, string src)
		{
			var path = System.IO.Path.Combine(state.BasePath, src);
			var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreComments = true, IgnoreProcessingInstructions = true, IgnoreWhitespace = true };
			var reader =  System.Xml.XmlReader.Create(path, settings);
			var substate = new XmlReadState(state) { BasePath = System.IO.Path.GetDirectoryName(path) };
			ReadScene(ref substate, reader);
			reader.Close();
		}

		/// <summary>
		/// Main access point for the XML reader. Reads
		/// the Scene description as given in Path
		/// </summary>
		public void Parse()
		{
			var state = new XmlReadState
			{
				BasePath = System.IO.Path.GetDirectoryName(Path),
				Scene = Client.Scene,
				Shader = Client.Scene.DefaultSurface,
				DicingRate = 0.1f,
				Smooth = false,
				Transform = ccl.Transform.Identity()
			};

			ReadInclude(ref state, System.IO.Path.GetFileName(Path));
		}
	}
}
