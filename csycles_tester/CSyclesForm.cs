using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ef = Eto.Forms;
using ed = Eto.Drawing;
using ccl;
using System.Drawing;
using System.IO;

namespace csycles_tester
{

	internal class RendererModel : INotifyPropertyChanged
	{
		public Client Client { get; private set; }
		public RendererModel()
		{
			Client = new Client();
		}

		ed.Bitmap bitmap;
		public ed.Bitmap Result
		{
			get { return bitmap; }
			set
			{
				if(bitmap != value)
				{
					bitmap = value;
					OnPropertyChanged();
				}
			}
		}

		void OnPropertyChanged([CallerMemberName] string memberName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
		public event PropertyChangedEventHandler PropertyChanged;

		const uint samples = 50;

		public void RenderScene(string scenename)
		{
			var dev = Device.FirstGpu;
			Console.WriteLine("Using device {0} {1}", dev.Name, dev.Description);

			var scene_params = new SceneParameters(Client, ShadingSystem.SVM, BvhType.Static, false, false, false);
			var scene = new Scene(Client, scene_params, dev);

			var xml = new CSyclesXmlReader(Client, scenename);
			xml.Parse(true);
			var width = (uint)scene.Camera.Size.Width;
			var height = (uint)scene.Camera.Size.Height;

			var session_params = new SessionParameters(Client, dev)
			{
				Experimental = false,
				Samples = (int) samples,
				TileSize = new Size(64, 64),
				StartResolution = 64,
				Threads = (uint)(dev.IsCpu ? 0 : 0),
				ShadingSystem = ShadingSystem.SVM,
				Background = true,
				ProgressiveRefine = false,
				Progressive = false,
				TileOrder = TileOrder.HilbertSpiral
			};
			var Session = new Session(Client, session_params, scene);
			Session.Reset(width, height, samples);

			/*if (!silent)
			{
				Session.UpdateCallback = g_update_callback;
				Session.UpdateTileCallback = g_update_render_tile_callback;
				Session.WriteTileCallback = g_write_render_tile_callback;
			}
			CSycles.set_logger(Client.Id, g_logger_callback);
			*/

			Session.Start();
			Session.Wait();

			uint bufsize;
			uint bufstride;
			CSycles.session_get_buffer_info(Client.Id, Session.Id, out bufsize, out bufstride);
			var pixels = CSycles.session_copy_buffer(Client.Id, Session.Id, bufsize);

			var bmp = new ed.Bitmap((int)width, (int)height, Eto.Drawing.PixelFormat.Format32bppRgba);
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					var i = y * (int)width * 4 + x * 4;
					/*var r = ColorClamp((int)(pixels[i] * 255.0f));
					var g = ColorClamp((int)(pixels[i + 1] * 255.0f));
					var b = ColorClamp((int)(pixels[i + 2] * 255.0f));
					var a = ColorClamp((int)(pixels[i + 3] * 255.0f));*/
					bmp.SetPixel(x, y, new ed.Color(pixels[i], pixels[i + 1], pixels[i + 2], pixels[i + 3]));
				}
			}
			bmp.Save("test.png", Eto.Drawing.ImageFormat.Png);

			Result = bmp;

			Session.Destroy();

			Console.WriteLine("Cleaning up :)");

		}
	}
	public class CSyclesForm : ef.Form
	{

		public ef.ImageView Image { get; set; }

		public string Path { get; set; }

		public CSyclesForm(string path)
		{
			ClientSize = new Eto.Drawing.Size(500, 500);
			Title = "CSycles Tester";
			Path = path;

			Image = new ef.ImageView();
			var layout = new ef.TableLayout();
			layout.Rows.Add(
				new ef.TableRow(
					Image
					)
				);

			var scenes = Directory.EnumerateFiles(path, "scene*.xml");
			Menu = new ef.MenuBar();
			var scenesmenu = Menu.Items.GetSubmenu("scenes");
			foreach(var sf in scenes)
			{
				scenesmenu.Items.Add(new RenderModalCommand(this, sf));
			}

			Content = layout;

			var m = new RendererModel();
			DataContext = m;
		}
	}
}
