using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ccl;

namespace csycles_unittests
{
	[TestFixture]
	public class TestTransform
	{
		[TestCase(1.0f, 0.0f, 1.0f, 0.0f)]
		unsafe public void TestTransformInverse(float a, float b, float c, float d)
		{
			float4 f4 = new float4(a, b, c, d);
			_Transform _f4 = new _Transform(a, b, c, d, a, b, c, d, a, b, c, d, a, b, c, d);

			byte* addr = (byte*)&_f4;
			Console.WriteLine("Size:      {0}", sizeof(_Transform));
			Console.WriteLine("x Offset: {0}", (byte*)&_f4.x.x - addr);
			Console.WriteLine("y Offset: {0}", (byte*)&_f4.y.x - addr);
			Console.WriteLine("z Offset: {0}", (byte*)&_f4.z.x - addr);
			Console.WriteLine("w Offset: {0}", (byte*)&_f4.w.x - addr);

			Console.WriteLine("x.x:      {0}", _f4.x.x);
			Console.WriteLine("x.y:      {0}", _f4.x.y);
			Console.WriteLine("x.z:      {0}", _f4.x.z);
			Console.WriteLine("x.w:      {0}", _f4.x.w);


			_Transform _f4inv = _Transform.Inverse(_f4);

			Console.WriteLine("x.x:      {0}", _f4inv.x.x);
			Console.WriteLine("x.y:      {0}", _f4inv.x.y);
			Console.WriteLine("x.z:      {0}", _f4inv.x.z);
			Console.WriteLine("x.w:      {0}", _f4inv.x.w);
		}

	}
}
