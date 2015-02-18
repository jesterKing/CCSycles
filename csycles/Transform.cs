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

namespace ccl
{
	public class Transform
	{
		static readonly public Transform RhinoToCyclesCam = new Transform(
			1.0f, 0.0f, 0.0f, 0.0f,
			0.0f, -1.0f, 0.0f, 0.0f,
			0.0f, 0.0f, -1.0f, 0.0f,
			0.0f, 0.0f, 0.0f, 1.0f
		);

		public float4 x;
		public float4 y;
		public float4 z;
		public float4 w;
		public Transform(
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l,
			float m, float n, float o, float p
		)
		{
			x = new float4(a, b, c, d);
			y = new float4(e, f, g, h);
			z = new float4(i, j, k, l);
			w = new float4(m, n, o, p);

		}
		public Transform()
			: this(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f)
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="old"></param>
		public Transform(Transform old)
			: this (
				old.x.x, old.x.y, old.x.z, old.x.w,
				old.y.x, old.y.y, old.y.z, old.y.w,
				old.z.x, old.z.y, old.z.z, old.z.w,
				old.w.x, old.w.y, old.w.z, old.w.w
			)
		{
		}

		/// <summary>
		/// Construct a Transform using a float array
		/// </summary>
		/// <param name="m">Array of 16 floats</param>
		public Transform(float[] m)
			: this(m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7], m[8], m[9], m[10], m[11], m[12], m[13], m[14], m[15])
		{
		}

		static public Transform Identity()
		{
			return Scale(1.0f, 1.0f, 1.0f);
		}

		static public Transform operator *(Transform a, Transform b)
		{
			var c = Transpose(b);
			return new Transform(
				float4.Dot(a.x, c.x), float4.Dot(a.x, c.y), float4.Dot(a.x, c.z), float4.Dot(a.x, c.w),
				float4.Dot(a.y, c.x), float4.Dot(a.y, c.y), float4.Dot(a.y, c.z), float4.Dot(a.y, c.w),
				float4.Dot(a.z, c.x), float4.Dot(a.z, c.y), float4.Dot(a.z, c.z), float4.Dot(a.z, c.w),
				float4.Dot(a.w, c.x), float4.Dot(a.w, c.y), float4.Dot(a.w, c.z), float4.Dot(a.w, c.w)
			);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="angle">Angle in radians</param>
		/// <param name="rotation_axis">axis around which the rotation is. w of float4 is unused</param>
		/// <returns></returns>
		static public Transform Rotate(float angle, float4 rotation_axis)
		{
			var axis = new float4(rotation_axis) { w = 0.0f };
			var s = (float)Math.Sin(angle);
			var c = (float)Math.Cos(angle);
			var t = 1.0f - c;

			axis = float4.Normalize(axis);

			return new Transform(
				axis.x * axis.x * t + c,
				axis.x * axis.y * t - s * axis.z,
				axis.x * axis.z * t + s * axis.y,
				0.0f,

				axis.y * axis.x * t + s * axis.z,
				axis.y * axis.y * t + c,
				axis.y * axis.z * t - s * axis.x,
				0.0f,

				axis.z * axis.x * t - s * axis.y,
				axis.z * axis.y * t + s * axis.x,
				axis.z * axis.z * t + c,
				0.0f,

				0.0f, 0.0f, 0.0f, 1.0f);

		}

		static public Transform Transpose(Transform a)
		{
			var t = new Transform
			{
				x = {x = a.x.x, y = a.y.x, z = a.z.x, w = a.w.x},
				y = {x = a.x.y, y = a.y.y, z = a.z.y, w = a.w.y},
				z = {x = a.x.z, y = a.y.z, z = a.z.z, w = a.w.z},
				w = {x = a.x.w, y = a.y.w, z = a.z.w, w = a.w.w}
			};

			return t;
		}

		static public Transform Translate(float4 t)
		{
			return new Transform(
				1, 0, 0, t.x,
				0, 1, 0, t.y,
				0, 0, 1, t.z,
				0, 0, 0, 1);
		}

		static public Transform Translate(float x, float y, float z)
		{
			return Translate(new float4(x, y, z));
		}

		static public Transform Scale(float x, float y, float z)
		{
			return new Transform(
				x, 0.0f, 0.0f, 0.0f,
				0.0f, y, 0.0f, 0.0f,
				0.0f, 0.0f, z, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f
				);
		}

		new public string ToString()
		{
			return string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}]", x.x, x.y, x.z, x.w, y.x, y.y, y.z, y.w, z.x, z.y, z.z, z.w, w.x, w.y, w.z, w.w);
		}
	}

}
