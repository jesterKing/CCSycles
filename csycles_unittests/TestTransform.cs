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
		private void PrintTransform(_Transform xfm)
		{
			Console.WriteLine(" {0} {1} {2} {3}", xfm[0][0], xfm[0][1], xfm[0][2], xfm[0][3]);
			Console.WriteLine(" {0} {1} {2} {3}", xfm[1][0], xfm[1][1], xfm[1][2], xfm[1][3]);
			Console.WriteLine(" {0} {1} {2} {3}", xfm[2][0], xfm[2][1], xfm[2][2], xfm[2][3]);
			Console.WriteLine(" {0} {1} {2} {3}", xfm[3][0], xfm[3][1], xfm[3][2], xfm[3][3]);
		}

		[TestCase(1.0f, 0.0f, 1.0f, 0.0f)]
		unsafe public void TestTransformInverse(float a, float b, float c, float d)
		{
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

		//[TestCase(0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f)]
		//[TestCase(0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f)]
		//[TestCase(0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f)]
		[TestCase(0.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f)]
		public void TestTransformLookAt(float px, float py, float pz, float lx, float ly, float lz, float ux, float uy, float uz)
		{
			_float4 pos = new _float4 { x = px, y = py, z = pz, w = 0.0f };
			_float4 look = new _float4 { x = lx, y = ly, z = lz, w = 0.0f };
			_float4 up = new _float4 { x = ux, y = uy, z = uz, w = 0.0f };

			_Transform laxfm = _Transform.LookAt(pos, look, up);

			PrintTransform(laxfm);
		}

		[TestCase(90.0f, 1.0f, 1.0f, 1.0f)]
		[TestCase(86.0f, 1.0f, 1.0f, 1.0f)]
		[TestCase(13.0f, 1.0f, 0.0f, 0.0f)]
		[TestCase(19.0f, 0.0f, 0.0f, -1.0f)]
		public void TestTransformRotateAroundAxis(float angle, float x, float y, float z)
		{
			_Transform _t = _Transform.RotateAroundAxis(angle, new _float4(x, y, z, 0.0f));
			Transform t = Transform.Rotate(angle, new float4(x, y, z));

			Assert.AreEqual(_t[0][0], t[0][0]);
			Assert.AreEqual(_t[1][0], t[1][0]);
			Assert.AreEqual(_t[2][0], t[2][0]);
			Assert.AreEqual(_t[3][0], t[3][0]);

			Assert.AreEqual(_t[0][1], t[0][1]);
			Assert.AreEqual(_t[1][1], t[1][1]);
			Assert.AreEqual(_t[2][1], t[2][1]);
			Assert.AreEqual(_t[3][1], t[3][1]);

			Assert.AreEqual(_t[0][2], t[0][2]);
			Assert.AreEqual(_t[1][2], t[1][2]);
			Assert.AreEqual(_t[2][2], t[2][2]);
			Assert.AreEqual(_t[3][2], t[3][2]);

			Assert.AreEqual(_t[0][3], t[0][3]);
			Assert.AreEqual(_t[1][3], t[1][3]);
			Assert.AreEqual(_t[2][3], t[2][3]);
			Assert.AreEqual(_t[3][3], t[3][3]);

			PrintTransform(_t);
			Console.WriteLine("---");
			PrintTransform((_Transform)t);

		}

	}
}
