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
using ccl;

namespace csycles_tester
{
	public class XmlReadState : IDisposable
	{
		public Scene Scene { get; set; }
		public Transform Transform { get; set; }
		public bool Smooth { get; set; }
		public Shader Shader { get; set; }
		public string BasePath { get; set; }
		public float DicingRate { get; set; }
		// public DisplacementMethod DisplacementMethod {get; set; }

		public XmlReadState()
			: this(null, null, false, null, "", 0.0f)
		{
			
		}

		public XmlReadState(Scene scene, Transform transform, bool smooth, Shader shader, string basePath, float dicingRate)
		{
			Scene = scene;
			Transform = transform;
			Smooth = smooth;
			Shader = shader;
			BasePath = basePath;
			DicingRate = dicingRate;
		}

		public XmlReadState(XmlReadState old)
			:this(old.Scene, new Transform(old.Transform), old.Smooth, old.Shader, old.BasePath, old.DicingRate)
		{
		}

		public void Dispose()
		{
		}
	}
}
