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
	public class float4
	{
		public float x;
		public float y;
		public float z;
		public float w;
		public float4() : this(0.0f, 0.0f, 0.0f, 0.0f) { }
		/// <summary>
		/// Create float4 with all members set to x_
		/// </summary>
		/// <param name="x_"></param>
		public float4(float x_) : this(x_, x_, x_, x_) { }
		public float4(float x_, float y_, float z_) : this(x_, y_, z_, 0.0f) { }
		public float4(float x_, float y_, float z_, float w_)
		{
			x = x_;
			y = y_;
			z = z_;
			w = w_;
		}
		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="old">float4 to copy</param>
		public float4(float4 old) : this(old.x, old.y, old.z, old.w) { }

		public static float4 operator /(float4 a, float4 b)
		{
			return new float4(a.x/b.x, a.y/b.y, a.z/b.z, a.w/b.w);
		}

		public static float4 operator /(float4 a, float b)
		{
			var inv = 1.0f/b;
			return new float4(a.x*inv, a.y*inv, a.z*inv, a.w*inv);
		}

		public static float4 operator /(float a, float4 b)
		{
			return new float4(a/b.x, a/b.y, a/b.z, a/b.w);
		}

		public float Length()
		{
			return (float)Math.Sqrt(Dot(this, this));
		}

		public static float Dot(float4 a, float4 b)
		{
			return (a.x * b.x + a.y * b.y) + (a.z * b.z + a.w * b.w);
		}

		public static float4 Normalize(float4 a)
		{
			return a / a.Length();
		}

		public bool IsZero()
		{
			return Math.Abs(x) < 0.00001f && Math.Abs(y) < 0.00001f
			    && Math.Abs(z) < 0.00001f && Math.Abs(w) < 0.00001f;
		}
	}

}
