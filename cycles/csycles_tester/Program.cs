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
using System;
using System.Drawing;
using System.IO;

namespace csycles_tester
{
	class Program
	{
		static Session Session { get; set; }
		static Client Client { get; set; }

		static public void StatusUpdateCallback(uint sessionId)
		{
			float progress;
			double total_time;

			CSycles.progress_get_progress(Client.Id, sessionId, out progress, out total_time);
			var status = CSycles.progress_get_status(Client.Id, sessionId);
			var substatus = CSycles.progress_get_substatus(Client.Id, sessionId);
			uint samples;
			uint num_samples;

			CSycles.tilemanager_get_sample_info(Client.Id, sessionId, out samples, out num_samples);

			if (status.Equals("Finished"))
			{
				Console.WriteLine("wohoo... :D");
			}

			status = "[" + status + "]";
			if (!substatus.Equals(string.Empty)) status = status + ": " + substatus;
			Console.WriteLine("C# status update: {0} {1} {2} {3} <|> {4:N}s {5:P}", CSycles.progress_get_sample(Client.Id, sessionId), status, samples, num_samples, total_time, progress);
		}

		static public void WriteRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
		{
			Console.WriteLine("C# Write Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		public static void UpdateRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
		{
			Console.WriteLine("C# Update Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		/// <summary>
		/// Callback for debug logging facility. Will be called only for Debug builds of ccycles.dll
		/// </summary>
		/// <param name="msg"></param>
		public static void LoggerCallback(string msg)
		{
			Console.WriteLine("DBG: {0}", msg);
		}

		public static int ColorClamp(int ch)
		{
			if (ch < 0) return 0;
			return ch > 255 ? 255 : ch;
		}

		public static float DegToRad(float ang)
		{
			return ang * (float)Math.PI / 180.0f;
		}

		private static CSycles.UpdateCallback g_update_callback;
		private static CSycles.RenderTileCallback g_update_render_tile_callback;
		private static CSycles.RenderTileCallback g_write_render_tile_callback;

		private static CSycles.LoggerCallback g_logger_callback;

		static void Main(string[] args)
		{
			var file = "";
			if (args.Length != 1)
			{
				Console.WriteLine("Missing parameter: csycles_tester file.xml");
				return;
			}
			var s = args[0];
			if (File.Exists(s))
			{
				file = Path.GetFullPath(s);
				Console.WriteLine("We get file path: {0}", file);
			}

			const uint samples = 5;
			g_update_callback = StatusUpdateCallback;
			g_update_render_tile_callback = UpdateRenderTileCallback;
			g_write_render_tile_callback = WriteRenderTileCallback;
			g_logger_callback = LoggerCallback;

			var client = new Client();
			Client = client;

			CSycles.set_logger(client.Id, g_logger_callback);

			CSycles.set_kernel_path("Plug-ins/RhinoCycles/lib");

			CSycles.initialise();
			var num_devices = Device.Count;
			var dev = Device.FirstCuda;
			Console.WriteLine("We have {0} device{1}", num_devices, num_devices != 1 ? "s" : "");

			var scene_params = new SceneParameters(client, ShadingSystem.SVM, BvhType.Dynamic, false, false, false, false);
			var scene = new Scene(client, scene_params, dev);

			client.Scene = scene;


#region background shader
			var background_shader = new Shader(client, Shader.ShaderType.World)
			{
				Name = "Background shader"
			};

			var bgnode = new BackgroundNode();
			bgnode.ins.Color.Value = new float4(0.7f);
			bgnode.ins.Strength.Value = 1.0f;

			background_shader.AddNode(bgnode);
			//background_shader.LastSocket(bgnode.outs.Background);
			bgnode.outs.Background.Connect(background_shader.Output.ins.Surface);
			background_shader.FinalizeGraph();

			scene.AddShader(background_shader);

			scene.Background.Shader = background_shader;
			scene.Background.AoDistance = 0.0f;
			scene.Background.AoFactor = 0.0f;
			scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
#endregion
#region diffuse shader

			var diffuse_shader = new Shader(client, Shader.ShaderType.Material)
			{
				Name = "Tester diffuse bsdf",
				UseMis = true,
				UseTransparentShadow = true,
				HeterogeneousVolume = false
			};
			var col_node = new ColorNode { Value = new float4(0.0f, 0.5f, 0.05f) };
			var diff_bsdf = new DiffuseBsdfNode();
			diff_bsdf.ins.Color.Value = new float4(0.0f, 0.0f, 1.0f);

			diffuse_shader.AddNode(col_node);
			diffuse_shader.AddNode(diff_bsdf);

			col_node.outs.Color.Connect(diff_bsdf.ins.Color);
			//diffuse_shader.LastSocket(diff_bsdf.outs.BSDF);

			diff_bsdf.outs.BSDF.Connect(diffuse_shader.Output.ins.Surface);

			diffuse_shader.FinalizeGraph();
			scene.AddShader(diffuse_shader);
			scene.DefaultSurface = diffuse_shader;
#endregion

#region point light shader

			var light_shader = new Shader(client, Shader.ShaderType.Material)
			{
				Name = "Tester light shader"
			};

			var emission_node = new EmissionNode();
			emission_node.ins.Color.Value = new float4(0.8f);
			emission_node.ins.Strength.Value = 10.0f;

			light_shader.AddNode(emission_node);
			emission_node.outs.Emission.Connect(light_shader.Output.ins.Surface);
			light_shader.FinalizeGraph();
			scene.AddShader(light_shader);
#endregion
			/*



			scene.AddShader(diffuse_shader);
			scene.AddShader(light_shader);

			scene.DefaultSurface = diffuse_shader;

			Console.WriteLine("Default shader is: {0}", scene.DefaultSurface);
			*/

			// update integrator settings
			/*Console.WriteLine("Update integrator settings");
			scene.Integrator.MaxBounce = 10;
			scene.Integrator.MinBounce = 10;

			scene.Integrator.NoCaustics = true;
			scene.Integrator.TransparentShadows = true;

			scene.Integrator.DiffuseSamples= 2;
			scene.Integrator.GlossySamples = 4;
			scene.Integrator.TransmissionSamples = 4;
			scene.Integrator.AoSamples = 1;
			scene.Integrator.MeshLightSamples = 1;
			scene.Integrator.SubsurfaceSamples = 1;
			scene.Integrator.VolumeSamples = 1;

			scene.Integrator.MaxDiffuseBounce = 2;
			scene.Integrator.MaxGlossyBounce = 4;
			scene.Integrator.MaxTransmissionBounce = 8;
			scene.Integrator.MaxVolumeBounce = 1;

			scene.Integrator.AaSamples = 0;

			scene.Integrator.TransparentMinBounce  = 8;
			scene.Integrator.TransparentMaxBounce = 8;

			scene.Integrator.FilterGlossy = 0.0f;

			scene.Integrator.IntegratorMethod = IntegratorMethod.Path;*/

			/*var l = new Light(client, scene, light_shader);
			l.Type = LightType.Point;
			l.SpotAngle = 1.0f;
			l.SpotSmooth = 0.5f;
			l.Location = new float4(2.0f, 10.0f, 10.0f);
			l.Direction = new float4(-0.3f, -1.0f, 0.0f);
			l.Size = 0.1f;*/

			var xml = new XmlReader(client, file);
			xml.Parse();
			var width = (uint) scene.Camera.Size.Width;
			var height = (uint) scene.Camera.Size.Height;

			var session_params = new SessionParameters(client, dev);
			//session_params.output_path = "test.png";
			//session_params.output_path = "";
			session_params.Experimental = false;
			session_params.Samples = (int)samples;
			session_params.TileSize = new Size(64, 64);
			session_params.StartResolution = 64;
			session_params.Threads = 0;
			//session_params.TileOrder = TileOrder.RightToLeft;
			session_params.ShadingSystem = ShadingSystem.SVM;
			//session_params.DisplayBufferLinear = false;
			session_params.Background = true;
			session_params.ProgressiveRefine = false;
			//session_params.Progressive = true;
			Session = new Session(client, session_params, scene);
			Session.Reset(width, height, samples);

			Session.UpdateCallback = g_update_callback;
			Session.UpdateTileCallback = g_update_render_tile_callback;
			Session.WriteTileCallback = g_write_render_tile_callback;


			Console.WriteLine("^^: {0}", CSycles.progress_get_status(client.Id, Session.Id));

			Session.Start();
			Session.Wait();

			uint bufsize;
			uint bufstride;
			CSycles.session_get_buffer_info(client.Id, Session.Id, out bufsize, out bufstride);
			var pixels = CSycles.session_copy_buffer(client.Id, Session.Id, bufsize);

			var bmp = new Bitmap((int)width, (int)height);
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var i = y*(int)width*4 + x*4;
					var r = ColorClamp((int)(pixels[i]*255.0f));
					var g = ColorClamp((int)(pixels[i+1]*255.0f));
					var b = ColorClamp((int)(pixels[i+2]*255.0f));
					var a = ColorClamp((int)(pixels[i+3]*255.0f));
					bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
				}
			}
			bmp.Save("test.bmp");

			Console.WriteLine("Done");
			Console.WriteLine("Cleaning up :)");

			CSycles.shutdown();

			Console.WriteLine("Bye bye! Press a key to end this program...");
			Console.ReadKey();
		}
	}
}
