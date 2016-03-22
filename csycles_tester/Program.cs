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

using System.Drawing.Imaging;
using ccl;
using ccl.ShaderNodes;
using System;
using System.Drawing;
using System.IO;

using ed = Eto.Drawing;
using ef = Eto.Forms;
using System.Collections.Generic;

namespace csycles_tester
{
	class Program
	{
		static Session Session { get; set; }
		static Client Client { get; set; }

		static public Shader create_some_setup_shader()
		{
			var some_setup = new Shader(Client, Shader.ShaderType.Material)
			{
				Name = "some_setup ",
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

		static public void StatusUpdateCallback(uint sessionId)
		{
			float progress;
			double total_time, render_time, tile_time;

			CSycles.progress_get_progress(Client.Id, sessionId, out progress, out total_time, out render_time, out tile_time);
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

		static public void WriteRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth, int startSample, int numSamples, int sample, int resolution)
		{
			Console.WriteLine("C# Write Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
		}

		public static void UpdateRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth, int startSample, int numSamples, int sample, int resolution)
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

		[STAThread]
		static void Main(string[] args)
		{
			var app = new Eto.Forms.Application();

			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
			var userpath = Path.Combine(path, "userpath");

			CSycles.path_init(path, userpath);
			CSycles.initialise();

			var csf = new CSyclesForm(path)
			{
				ClientSize = new ed.Size((int)1024, (int)768),
			};

			app.Run(csf);

			CSycles.shutdown();
		}
	}
}
