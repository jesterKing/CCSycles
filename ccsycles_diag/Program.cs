using System;
using ccl;

namespace ccsycles_diag
{
	class Program
	{
		static void Main()
		{
			CSycles.set_kernel_path("lib");
			CSycles.initialise();

			var devices = Device.Devices;

			foreach (var dev in devices)
			{
				Console.WriteLine(dev);
			}
			
			Console.WriteLine("FirstCuda gives us: {0}", Device.FirstCuda);

			CSycles.shutdown();
		}
	}
}
