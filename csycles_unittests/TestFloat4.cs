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
	public class TestFloat4
	{
		[TestCase(0.0f, 0.0f, 0.0f, ExpectedResult = 0.0f)]
		[TestCase(1.0f, 0.0f, 0.0f, ExpectedResult = 1.0f)]
		public float TestInitFloat4GetX(float a, float b, float c)
		{
			float4 f4 = new float4(a, b, c);

			return f4.x;
		}

		[TestCase(0.0f, 1.0f, 0.0f, ExpectedResult = 1.0f)]
		[TestCase(1.0f, 0.0f, 0.0f, ExpectedResult = 0.0f)]
		public float TestInitFloat4GetY(float a, float b, float c)
		{
			float4 f4 = new float4(a, b, c);

			return f4.y;
		}

		[TestCase(0.0f, 1.0f, 0.0f, ExpectedResult = 0.0f)]
		[TestCase(1.0f, 0.0f, 1.0f, ExpectedResult = 1.0f)]
		public float TestInitFloat4GetZ(float a, float b, float c)
		{
			float4 f4 = new float4(a, b, c);

			return f4.z;
		}

		[TestCase(0.0f, 1.0f, 0.0f, 1.0f)]
		[TestCase(1.0f, 0.0f, 1.0f, 0.0f)]
		unsafe public void TestInitFloat4_Float4(float a, float b, float c, float d)
		{
			float4 f4 = new float4(a, b, c, d);
			_float4 _f4 = new _float4(a, b, c, d);

			byte* addr = (byte*)&_f4;
			Console.WriteLine("Size:      {0}", sizeof(_float4));
			Console.WriteLine("x Offset: {0}", (byte*)&_f4.x - addr);
			Console.WriteLine("y Offset: {0}", (byte*)&_f4.y - addr);
			Console.WriteLine("z Offset: {0}", (byte*)&_f4.z - addr);
			Console.WriteLine("w Offset: {0}", (byte*)&_f4.w - addr);

			Console.WriteLine("morjeeens");

			Assert.AreEqual(f4.x, _f4.x);
			Assert.AreEqual(f4.y, _f4.y);
			Assert.AreEqual(f4.z, _f4.z);
			Assert.AreEqual(f4.w, _f4.w);

		}

		[TestCase(0.0f, 1.0f, 0.0f)]
		[TestCase(1.0f, 0.0f, 1.0f)]
		[TestCase(1.0f, 2.0f, 3.0f)]
		[TestCase(1.1f, 2.2f, 3.3f)]
		public void TestF4Add(float a, float b, float c)
		{

			_float4 _f1 = new _float4(a, b, c, 0.0f);
			float4 f1 = new float4(a, b, c, 0.0f);
			_float4 _f2 = new _float4(a, b, c, 0.0f);
			float4 f2 = new float4(a, b, c, 0.0f);

			_float4 _f3 = _f1 + _f2;
			float4 f3 = f1 + f2;


			Assert.AreEqual(f3.x, _f3.x);
			Assert.AreEqual(f3.y, _f3.y);
			Assert.AreEqual(f3.z, _f3.z);
			Assert.AreEqual(f3.w, _f3.w);
		}

		[TestCase(0.0f, 1.0f, 0.0f)]
		[TestCase(1.0f, 0.0f, 1.0f)]
		[TestCase(1.0f, 2.0f, 3.0f)]
		[TestCase(1.1f, 2.2f, 3.3f)]
		public void TestF4Div(float a, float b, float c)
		{

			_float4 _f1 = new _float4(a, b, c, 0.0f);
			float4 f1 = new float4(a, b, c, 0.0f);
			_float4 _f2 = new _float4(a, b, c, 0.0f);
			float4 f2 = new float4(a, b, c, 0.0f);

			_float4 _f3 = _f1 + _f2;
			float4 f3 = f1 + f2;


			Assert.AreEqual(f3.x, _f3.x);
			Assert.AreEqual(f3.y, _f3.y);
			Assert.AreEqual(f3.z, _f3.z);
			Assert.AreEqual(f3.w, _f3.w);
		}
	}
}
