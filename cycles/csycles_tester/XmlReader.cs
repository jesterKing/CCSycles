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
		public XmlReader(Client client, string path)
		{
			Client = client;
			Path = path;
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

			if (!string.IsNullOrEmpty(fov)) Client.Scene.Camera.Fov = float.Parse(fov);

			if (!string.IsNullOrEmpty(nearclip)) Client.Scene.Camera.NearClip = float.Parse(nearclip);

			if (!string.IsNullOrEmpty(farclip)) Client.Scene.Camera.FarClip = float.Parse(farclip);

			if (!string.IsNullOrEmpty(aperturesize)) Client.Scene.Camera.ApertureSize = float.Parse(aperturesize);

			if (!string.IsNullOrEmpty(focaldistance)) Client.Scene.Camera.FocalDistance = float.Parse(focaldistance);

			if (!string.IsNullOrEmpty(shuttertime)) Client.Scene.Camera.ShutterTime = float.Parse(shuttertime);

			if (!string.IsNullOrEmpty(fisheye_fov)) Client.Scene.Camera.FishEyeFov = float.Parse(fisheye_fov);

			if (!string.IsNullOrEmpty(fisheye_lens)) Client.Scene.Camera.FishEyeLens = float.Parse(fisheye_lens);

			if (!string.IsNullOrEmpty(sensorwidth)) Client.Scene.Camera.SensorWidth = float.Parse(sensorwidth);

			if (!string.IsNullOrEmpty(sensorheight)) Client.Scene.Camera.SensorHeight = float.Parse(sensorheight);

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

		/// <summary>
		/// Read a transform from XML.
		/// 
		/// If all are available then they are read and applied to transform according formula:
		/// 
		/// transform = transform * transpose(matrix)
		/// transform = ((transform * translate) * rotate) * scale
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
					transform = transform*ccl.Transform.Transpose(t);
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

			if (!string.IsNullOrEmpty(dicing_rate)) state.DicingRate = float.Parse(dicing_rate);
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
				realfloats[i] = float.Parse(fs[i]);
			}

			return realfloats;
		}

		private void get_float(FloatSocket socket, string nr)
		{
			if (string.IsNullOrEmpty(nr)) return;

			socket.Value = float.Parse(nr);
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
			var nverts = node.GetAttribute("nverts");
			var verts = node.GetAttribute("verts");
			Console.WriteLine("{0}", node);

			var pfloats = parse_floats(P);
			var nvertsints = parse_ints(nverts);
			var vertsints = parse_ints(verts);

			var ob = CSycles.scene_add_object(Client.Id, state.Scene.Id);
			CSycles.object_set_matrix(Client.Id, state.Scene.Id, ob, state.Transform);
			var me = CSycles.scene_add_mesh(Client.Id, state.Scene.Id, ob, state.Scene.ShaderSceneId(state.Shader));

			CSycles.mesh_set_verts(Client.Id, state.Scene.Id, me, ref pfloats, (uint)(pfloats.Length/3));

			var index_offset = 0;
			foreach (var t in nvertsints)
			{
				for (var j = 0; j < t - 2; j++)
				{
					var v0 = vertsints[index_offset];
					var v1 = vertsints[index_offset + j + 1];
					var v2 = vertsints[index_offset + j + 2];

					CSycles.mesh_add_triangle(Client.Id, state.Scene.Id, me, (uint)v0, (uint)v1, (uint)v2, state.Scene.ShaderSceneId(state.Shader), state.Smooth);
				}

				index_offset += t;
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
					case "shader":
						var shader_substate = new XmlReadState(state);
						ReadShader(ref shader_substate, node.ReadSubtree());
						break;
					case "mesh":
						ReadMesh(ref state, node.ReadSubtree());
						break;
					case "include":
						string src = node.GetAttribute("src");
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
						var offset = node.GetAttribute("offset");
						var offset_frequency = node.GetAttribute("offset_frequency");
						var squash = node.GetAttribute("squash");
						var squash_frequency = node.GetAttribute("squash_offset");
						if(!string.IsNullOrEmpty(offset)) bricktex.Offset = float.Parse(offset);
						if(!string.IsNullOrEmpty(offset_frequency)) bricktex.OffsetFrequency = int.Parse(offset_frequency);
						if(!string.IsNullOrEmpty(squash)) bricktex.Squash = float.Parse(squash);
						if(!string.IsNullOrEmpty(squash_frequency)) bricktex.SquashFrequency = int.Parse(squash_frequency);

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
							skynode.Turbidity = float.Parse(turbidity);
						}
						if (!string.IsNullOrEmpty(ground_albedo))
						{
							skynode.GroundAlbedo = float.Parse(ground_albedo);
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
