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
using System.Drawing;
using System.Drawing.Imaging;
using ccl;
using ccl.ShaderNodes;

namespace csycles_tester
{
	public class CSyclesRender
	{
		public CSyclesTester Cst { get; set; }
		public Session Session { get; set; }
		public Client Client { get; set; }
		public string RenderFile { get; set; }
		public bool SkipCopy { get; set; }

		public static Bitmap FinalBitmap { get; set; }

		public static CSyclesRender Engine;

		public static void Renderer(object renderer)
		{
			var start = DateTime.Now;
			var engine = (CSyclesRender) renderer;
			Engine = engine;

			const uint samples = 5;
			g_update_callback = StatusUpdateCallback;
			g_update_render_tile_callback = UpdateRenderTileCallback;
			g_write_render_tile_callback = WriteRenderTileCallback;
			g_logger_callback = LoggerCallback;

			var client = new Client();
			engine.Client = client;
			CSycles.set_logger(client.Id, g_logger_callback);

			var dev = Device.FirstCuda;
			Console.WriteLine("Using device {0}", dev.Name);

			var scene_params = new SceneParameters(client, ShadingSystem.SVM, BvhType.Dynamic, false, false, false, false);
			var scene = new Scene(client, scene_params, dev);

			#region background shader

			var background_shader = new Shader(client, Shader.ShaderType.World)
			{
				Name = "Background shader"
			};

			var bgnode = new BackgroundNode();
			bgnode.ins.Color.Value = new float4(0.7f);
			bgnode.ins.Strength.Value = 1.0f;

			background_shader.AddNode(bgnode);
			bgnode.outs.Background.Connect(background_shader.Output.ins.Surface);
			background_shader.FinalizeGraph();

			scene.AddShader(background_shader);

			scene.Background.Shader = background_shader;
			scene.Background.AoDistance = 0.0f;
			scene.Background.AoFactor = 0.0f;
			scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;

			#endregion

			#region diffuse shader

			var diffuse_shader = create_some_setup_shader(engine);
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

			var xml = new XmlReader(client, engine.RenderFile);
			xml.Parse();
			var width = (uint)scene.Camera.Size.Width;
			var height = (uint)scene.Camera.Size.Height;

			FinalBitmap = new Bitmap(scene.Camera.Size.Width, scene.Camera.Size.Height, PixelFormat.Format32bppRgb);

			var update_winsize = new object[]
			{
				engine,
				width,
				height
			};

			engine.Cst.BeginInvoke(new UpdateWindowSizeDelegate(UpdateWindowSize), update_winsize);

			var session_params = new SessionParameters(client, dev)
			{
				Experimental = false,
				Samples = (int)samples,
				TileSize = new Size(64, 64),
				StartResolution = 64,
				Threads = 0,
				ShadingSystem = ShadingSystem.SVM,
				Background = true,
				ProgressiveRefine = false
			};
			engine.Session = new Session(client, session_params, scene);
			engine.Session.Reset(width, height, samples);

			engine.Session.UpdateCallback = g_update_callback;
			engine.Session.UpdateTileCallback = g_update_render_tile_callback;
			engine.Session.WriteTileCallback = g_write_render_tile_callback;


			Console.WriteLine("^^: {0}", CSycles.progress_get_status(client.Id, engine.Session.Id));

			engine.Session.Start();
			engine.Session.Wait();

			uint bufsize;
			uint bufstride;
			CSycles.session_get_buffer_info(client.Id, engine.Session.Id, out bufsize, out bufstride);
			var pixels = CSycles.session_copy_buffer(client.Id, engine.Session.Id, bufsize);

			var bmp = new Bitmap((int)width, (int)height);
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var i = y * (int)width * 4 + x * 4;
					var r = ColorClamp((int)(pixels[i] * 255.0f));
					var g = ColorClamp((int)(pixels[i + 1] * 255.0f));
					var b = ColorClamp((int)(pixels[i + 2] * 255.0f));
					var a = ColorClamp((int)(pixels[i + 3] * 255.0f));
					bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
				}
			}
			bmp.Save("test.bmp");

			Console.WriteLine("Done");
			Console.WriteLine("Cleaning up :)");

			var end = (DateTime.Now - start).TotalMilliseconds/1000;

			Console.WriteLine("{0:F3}s", end);

			engine.Session.Destroy();
			engine.Cst.EnableButtons();
		}

		public delegate void UpdateWindowSizeDelegate(CSyclesRender engine, uint width, uint height);
		public static void UpdateWindowSize(CSyclesRender engine, uint width, uint height)
		{
			engine.Cst.Size = new Size((int) width+11, (int) height+81);
			engine.Cst.Invalidate();
			engine.Cst.Update();
		}

		static public Shader create_some_setup_shader(CSyclesRender engine)
		{
			var some_setup = new Shader(engine.Client, Shader.ShaderType.Material)
			{
				Name = "some_setup",
				UseMis = false,
				UseTransparentShadow = true,
				HeterogeneousVolume = false
			};


			var brick_texture = new BrickTexture();
			brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
			brick_texture.ins.Color1.Value = new float4(0.800f, 0.800f, 0.800f);
			brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
			brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
			brick_texture.ins.Scale.Value = 1.000f;
			brick_texture.ins.MortarSize.Value = 0.020f;
			brick_texture.ins.Bias.Value = 0.000f;
			brick_texture.ins.BrickWidth.Value = 0.500f;
			brick_texture.ins.RowHeight.Value = 0.250f;

			var checker_texture = new CheckerTexture();
			checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
			checker_texture.ins.Color1.Value = new float4(0.000f, 0.004f, 0.800f);
			checker_texture.ins.Color2.Value = new float4(0.200f, 0.000f, 0.007f);
			checker_texture.ins.Scale.Value = 5.000f;

			var diffuse_bsdf = new DiffuseBsdfNode();
			diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
			diffuse_bsdf.ins.Roughness.Value = 0.000f;
			diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);

			var texture_coordinate = new TextureCoordinateNode();


			some_setup.AddNode(brick_texture);
			some_setup.AddNode(checker_texture);
			some_setup.AddNode(diffuse_bsdf);
			some_setup.AddNode(texture_coordinate);

			brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
			checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
			texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
			texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

			diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);

			some_setup.FinalizeGraph();

			return some_setup;
		}

		public delegate void StatusUpdateDelegate(CSyclesRender engine, string s);

		public static void InvokeStatusUpdate(CSyclesRender engine, string s)
		{
			if (String.IsNullOrEmpty(s)) return;

			engine.Cst.toolRenderStatus.Text = s;
			engine.Cst.Invalidate();
			engine.Cst.Update();
		}

		static public void StatusUpdateCallback(uint sessionId)
		{
			float progress;
			double total_time;

			CSycles.progress_get_progress(Engine.Client.Id, sessionId, out progress, out total_time);
			var status = CSycles.progress_get_status(Engine.Client.Id, sessionId);
			var substatus = CSycles.progress_get_substatus(Engine.Client.Id, sessionId);
			uint samples;
			uint num_samples;

			CSycles.tilemanager_get_sample_info(Engine.Client.Id, sessionId, out samples, out num_samples);

			if (status.Equals("Finished"))
			{
				Console.WriteLine("wohoo... :D");
			}

			status = "[" + status + "]";
			if (!substatus.Equals(string.Empty)) status = status + ": " + substatus;
			var finalstatus = String.Format("C# status update: {0} {1} {2} {3} <|> {4:N}s {5:P}",
				CSycles.progress_get_sample(Engine.Client.Id, sessionId), status, samples, num_samples, total_time, progress);
			Console.WriteLine(finalstatus);

			var update_args = new[]
			{
				Engine,
				(object) finalstatus
			};
			Engine.Cst.BeginInvoke(new StatusUpdateDelegate(InvokeStatusUpdate),
				update_args);
		}

		public delegate void DrawRenderResultDelegate(CSyclesRender engine, Bitmap bmp);
		public static void DrawRenderResult(CSyclesRender engine, Bitmap bmp)
		{
			engine.Cst.renderResult.Image = bmp;
			engine.Cst.renderResult.Invalidate();
			engine.Cst.renderResult.Update();
		}

		public static Object RenderLock = new object();

		static public void WriteRenderTileCallback(uint sessionId, uint tx, uint ty, uint tw, uint th, uint depth)
		{
			DisplayBuffer(tx, ty, tw, th);
			Console.WriteLine("C# Write Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, tx, ty, depth);
		}

		private static void DisplayBuffer(uint tx, uint ty, uint tw, uint th)
		{
			var res = FinalBitmap;

			uint bufsize;
			uint bufstride;
			CSycles.session_get_buffer_info(Engine.Client.Id, Engine.Session.Id, out bufsize, out bufstride);

			var pixels = Engine.SkipCopy ? new float[bufsize] : CSycles.session_copy_buffer(Engine.Client.Id, Engine.Session.Id, bufsize);

			for (var x = (int) tx; x < tw; x++)
			{
				for (var y = (int) ty; y < th; y++)
				{
					var i = y*(int) tw*4 + x*4;
					var r = ColorClamp((int) (pixels[i]*255.0f));
					var g = ColorClamp((int) (pixels[i + 1]*255.0f));
					var b = ColorClamp((int) (pixels[i + 2]*255.0f));
					var a = ColorClamp((int) (pixels[i + 3]*255.0f));
					res.SetPixel(x, y, Color.FromArgb(a, r, g, b));
				}
			}

			var render_args = new object[]
			{
				Engine,
				res
			};

			Engine.Cst.BeginInvoke(new DrawRenderResultDelegate(DrawRenderResult), render_args);
		}

		public static void UpdateRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
		{
			DisplayBuffer(x, y, w, h);
			Console.WriteLine("C# Update Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		/// <summary>
		/// Callback for debug logging facility. Will be called only for Debug builds of ccycles.dll
		/// </summary>
		/// <param name="msg"></param>
		public static void LoggerCallback(string msg)
		{
			var update_args = new[]
			{
				Engine,
				(object) msg
			};
			Engine.Cst.renderStatus.BeginInvoke(new StatusUpdateDelegate(InvokeStatusUpdate),
				update_args);
			Console.WriteLine("DBG: {0}", msg);
			/*Cst.renderStatus.Text = msg;
			Cst.Update();*/
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
	}
}
