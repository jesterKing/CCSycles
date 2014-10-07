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

using System.Windows.Forms;
using ccl;
using System;
using System.IO;

namespace csycles_tester
{
	class Program
	{

		static void Main(string[] args)
		{
			CSycles.set_kernel_path("Plug-ins/RhinoCycles/lib");
			CSycles.initialise();

			var cst = new CSyclesTester();

			if (args.Length != 1)
			{
				Console.WriteLine("Missing parameter: csycles_tester file.xml");
				return;
			}
			var s = args[0];
			if (!File.Exists(s))
			{
				Console.WriteLine("File {0} doesn't exist.", s);
				return;
			}

			var file = Path.GetFullPath(s);
			Console.WriteLine("We get file path: {0}", file);

			cst.Renderer.RenderFile = file;


			Application.Run(cst);

			CSycles.shutdown();
		}

	}
}
