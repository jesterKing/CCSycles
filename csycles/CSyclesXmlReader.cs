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

using System.Globalization;
using System.Linq;
using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;

namespace ccl
{
	public class CSyclesXmlReader
	{
		private Client Client { get; set; }
		private string Path { get; set; }
		private NumberFormatInfo NumberFormatInfo { get; set; }
		public CSyclesXmlReader(Client client, string path)
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
			if (!state.Silent) Console.WriteLine("Background shader");

			var shader = new Shader(Client, Shader.ShaderType.World) { Name = Guid.NewGuid().ToString() };

			Utilities.Instance.ReadNodeGraph(ref shader, node.ReadSubtree());

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

			Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("branched"));
			state.Scene.Integrator.IntegratorMethod = boolvar ? IntegratorMethod.BranchedPath : IntegratorMethod.Path;

			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("sample_all_lights_direct")))
				state.Scene.Integrator.SampleAllLightsDirect = boolvar;
			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("sample_all_lights_indirect")))
				state.Scene.Integrator.SampleAllLightsIndirect = boolvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("diffuse_samples"))) state.Scene.Integrator.DiffuseSamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("glossy_samples"))) state.Scene.Integrator.GlossySamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("transmission_samples"))) state.Scene.Integrator.TransmissionSamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("ao_samples"))) state.Scene.Integrator.AoSamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("mesh_light_samples"))) state.Scene.Integrator.MeshLightSamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("subsurface_samples"))) state.Scene.Integrator.SubsurfaceSamples = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("volume_samples"))) state.Scene.Integrator.VolumeSamples = intvar;

			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("min_bounce"))) state.Scene.Integrator.MinBounce = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("max_bounce"))) state.Scene.Integrator.MaxBounce = intvar;

			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("max_diffuse_bounce"))) state.Scene.Integrator.MaxDiffuseBounce = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("max_glossy_bounce"))) state.Scene.Integrator.MaxGlossyBounce = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("max_transmission_bounce"))) state.Scene.Integrator.MaxTransmissionBounce = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("max_volume_bounce"))) state.Scene.Integrator.MaxVolumeBounce = intvar;

			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("transparent_min_bounce"))) state.Scene.Integrator.TransparentMinBounce = intvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("transparent_max_bounce"))) state.Scene.Integrator.TransparentMaxBounce = intvar;
			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("transparent_shadows"))) state.Scene.Integrator.TransparentShadows = boolvar;

			if (Utilities.Instance.get_float(ref floatvar, node.GetAttribute("volume_step_size"))) state.Scene.Integrator.VolumeStepSize = floatvar;
			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("volume_max_steps"))) state.Scene.Integrator.VolumeMaxSteps = intvar;

			/* \todo wrap caustics form separation
			 * 
			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("caustics_reflective"))) state.Scene.Integrator.DoCausticsReflective = boolvar;
			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("caustics_refractive"))) state.Scene.Integrator.DoCausticsRefractive = boolvar;
			 */
			if (Utilities.Instance.get_bool(ref boolvar, node.GetAttribute("no_caustics"))) state.Scene.Integrator.NoCaustics = boolvar;
			if (Utilities.Instance.get_float(ref floatvar, node.GetAttribute("filter_glossy"))) state.Scene.Integrator.FilterGlossy = floatvar;

			if (Utilities.Instance.get_int(ref intvar, node.GetAttribute("seed"))) state.Scene.Integrator.Seed = intvar;
			if (Utilities.Instance.get_float(ref floatvar, node.GetAttribute("sample_clamp_direct"))) state.Scene.Integrator.SampleClampDirect = floatvar;
			if (Utilities.Instance.get_float(ref floatvar, node.GetAttribute("sample_clamp_indirect"))) state.Scene.Integrator.SampleClampIndirect = floatvar;

			if (Utilities.Instance.read_string(ref stringvar, node.GetAttribute("sampling_pattern")))
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

			var f4 = new float4(0.0f);
			var t = new Transform();

			if (Utilities.Instance.get_transform(t, mat)) transform = t;

			var trans = node.GetAttribute("translate");
			if (Utilities.Instance.get_float4(f4, trans)) transform = transform * Transform.Translate(f4);

			var rotate = node.GetAttribute("rotate");
			if (Utilities.Instance.get_float4(f4, rotate))
			{
				var a = DegToRad(f4[0]);
				var axis = new float4(f4[1], f4[2], f4[3]);
				transform = transform * ccl.Transform.Rotate(a, axis);
			}

			var scale = node.GetAttribute("scale");
			if (!string.IsNullOrEmpty(scale))
			{
				var components = Utilities.Instance.parse_floats(scale);
				if (components.Length == 3)
				{
					transform = transform * ccl.Transform.Scale(components[0], components[1], components[2]);
				}
			}
		}

		/// <summary>
		/// Read a transform from the XML, using a look-at mechanism.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="transform"></param>
		private void ReadLookAt(System.Xml.XmlReader node, ref Transform transform)
		{

			var pos = new float4(0.0f);
			var look = new float4(0.0f);
			var up = new float4(0.0f);

			var posxml = node.GetAttribute("pos");
			var lookxml = node.GetAttribute("look");
			var upxml = node.GetAttribute("up");
			if (!Utilities.Instance.get_float4(pos, posxml) ||

				!Utilities.Instance.get_float4(look, lookxml) ||
				!Utilities.Instance.get_float4(up, upxml))
			{
				throw new MissingFieldException("lookat incomplete");
			}
			Transform res = Transform.LookAt(pos, look, up);
			transform.x = res.x;
			transform.y = res.y;
			transform.z = res.z;
			transform.w = res.w;
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

			bool boolval = false;

			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("is_shadow_catcher")))
			{
				state.IsShadowCatcher = boolval;
			}

			if (!string.IsNullOrEmpty(displacement_method))
			{
				/* \todo wrap displacement method stuff */
			}
		}

		Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();

		private void ReadMesh(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();

			var name = node.GetAttribute("name");
			if(!state.Silent) Console.WriteLine("Shader: {0}", node.GetAttribute("name"));
			if (string.IsNullOrEmpty(name))
			{
				throw new MissingFieldException("<mesh> is missing 'name'");
			}

			var P = node.GetAttribute("P");
			var UV = node.GetAttribute("UV");
			var nverts = node.GetAttribute("nverts");
			var verts = node.GetAttribute("verts");
			if(!state.Silent) Console.WriteLine("{0}", node);

			var has_uv = !string.IsNullOrEmpty(UV);

			float[] uvfloats = null;
			if (has_uv)
			{
				uvfloats = Utilities.Instance.parse_floats(UV);
			}
			var pfloats = Utilities.Instance.parse_floats(P);
			var nvertsints = Utilities.Instance.parse_ints(nverts);
			var vertsints = Utilities.Instance.parse_ints(verts);

			//var ob = new ccl.Object(Client) { Transform = state.Transform };
			var me = new Mesh(Client, state.Shader);

			//ob.Mesh = me;

			meshes.Add(name, me);

			me.SetVerts(ref pfloats);

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

					me.AddTri((uint)v0, (uint)v1, (uint)v2, state.Shader, state.Smooth);
				}

				index_offset += t;
			}

			if (has_uv)
			{
				me.SetUvs(ref uvs);
			}
		}
		private void ReadObject(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();

			var meshname = node.GetAttribute("mesh");
			var me = meshes[meshname];
			var ob = new ccl.Object(Client) { Transform = state.Transform, IsShadowCatcher = state.IsShadowCatcher };
			ob.Mesh = me;
		}

		private void ReadLight(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();

			Light light = new Light(Client, state.Scene, state.Shader);

			int intval = 0;
			float floatval = 0.0f;
			float4 float4val = new float4(0.0f);
			bool boolval = false;

			if (Utilities.Instance.get_int(ref intval, node.GetAttribute("type")))
			{
				light.Type = (LightType)intval;
			}
			if(Utilities.Instance.get_float(ref floatval, node.GetAttribute("spot_angle")))
			{
				light.SpotAngle = floatval;
			}
			if(Utilities.Instance.get_float(ref floatval, node.GetAttribute("spot_smooth")))
			{
				light.SpotSmooth = floatval;
			}

			if(Utilities.Instance.get_float(ref floatval, node.GetAttribute("sizeu")))
			{
				light.SizeU= floatval;
			}
			if(Utilities.Instance.get_float(ref floatval, node.GetAttribute("sizev")))
			{
				light.SizeV= floatval;
			}
			if(Utilities.Instance.get_float4(float4val, node.GetAttribute("axisu")))
			{
				light.AxisU= float4val;
			}
			if(Utilities.Instance.get_float4(float4val, node.GetAttribute("axisv")))
			{
				light.AxisV= float4val;
			}

			/* @todo
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("is_portal")))
			{
			}
			*/

			if(Utilities.Instance.get_float(ref floatval, node.GetAttribute("size")))
			{
				light.Size = floatval;
			}
			if(Utilities.Instance.get_float4(float4val, node.GetAttribute("dir")))
			{
				light.Direction = float4val;
			}
			if(Utilities.Instance.get_float4(float4val, node.GetAttribute("P")))
			{
				light.Location = state.Transform * float4val;
			}

			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("cast_shadow")))
			{
				light.CastShadow = boolval;
			}
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("use_mis")))
			{
				light.UseMis = boolval;
			}
			if(Utilities.Instance.get_int(ref intval, node.GetAttribute("samples")))
			{
				light.Samples = (uint)intval;
			}
			if(Utilities.Instance.get_int(ref intval, node.GetAttribute("max_bounces")))
			{
				light.MaxBounces = (uint)intval;
			}

			/* @todo add ray visibility to light API
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("use_diffuse")))
			{
				light.UseDiffuse = boolval;
			}
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("use_glossy")))
			{
				light.UseGlossy = boolval;
			}
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("use_transmission")))
			{
				light.UseTransmission = boolval;
			}
			if(Utilities.Instance.get_bool(ref boolval, node.GetAttribute("use_scatter")))
			{
				light.UseScatter = boolval;
			}
			*/

			light.TagUpdate();

		}

		private void ReadScene(ref XmlReadState state, System.Xml.XmlReader node)
		{
			
			while (node.Read())
			{
				if (!node.IsStartElement()) continue;

				if(!state.Silent) Console.WriteLine("XML node: {0}", node.Name);
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
					case "lookat":
						var lookat_substate= new XmlReadState(state);
						var lat = lookat_substate.Transform;
						ReadLookAt(node, ref lat);
						lookat_substate.Transform = lat;
						node.Read(); /* advance forward one, otherwise we'll end up in internal loop */
						ReadScene(ref lookat_substate, node.ReadSubtree());
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
					case "object":
						ReadObject(ref state, node.ReadSubtree());
						break;
					case "light":
						ReadLight(ref state, node.ReadSubtree());
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


		public void ReadShader(ref XmlReadState state, System.Xml.XmlReader node)
		{
			node.Read();
			var name = node.GetAttribute("name");
			if(!state.Silent) Console.WriteLine("Shader: {0}", node.GetAttribute("name"));
			if (string.IsNullOrEmpty(name)) return;

			var shader = new Shader(Client, Shader.ShaderType.Material) {Name = name};

			Utilities.Instance.ReadNodeGraph(ref shader, node.ReadSubtree());

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
		public void Parse(bool silent)
		{
			var state = new XmlReadState
			{
				BasePath = System.IO.Path.GetDirectoryName(Path),
				Scene = Client.Scene,
				Shader = Client.Scene.DefaultSurface,
				DicingRate = 0.1f,
				Smooth = false,
				Transform = ccl.Transform.Identity(),
				Silent = silent
			};

			ReadInclude(ref state, System.IO.Path.GetFileName(Path));
		}
	}
}
