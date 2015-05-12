using System;
using ccl;

namespace ccsycles_diag
{
	class Program
	{
		static void Main()
		{
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
