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

using System.Drawing;

namespace ccl
{
	public class Camera
	{
		internal Scene Scene { get; set; }
		public Camera(Scene scene)
		{
			Scene = scene;
		}

		public Size Size
		{
			set
			{
				CSycles.camera_set_size(Scene.Client.Id, Scene.Id, (uint)value.Width, (uint)value.Height);
			}
			get
			{
				return new Size((int)CSycles.camera_get_width(Scene.Client.Id, Scene.Id), (int)CSycles.camera_get_height(Scene.Client.Id, Scene.Id));
			}
		}

		public Transform Matrix
		{
			set
			{
				CSycles.camera_set_matrix(Scene.Client.Id, Scene.Id, value);
			}
		}

		public CameraType Type
		{
			set
			{
				CSycles.camera_set_type(Scene.Client.Id, Scene.Id, value);
			}
		}

		public PanoramaType PanoramaType
		{
			set
			{
				CSycles.camera_set_panorama_type(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float Fov
		{
			set
			{
				CSycles.camera_set_fov(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float SensorWidth
		{
			set
			{
				CSycles.camera_set_sensor_width(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float SensorHeight
		{
			set
			{
				CSycles.camera_set_sensor_height(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float NearClip
		{
			set
			{
				CSycles.camera_set_nearclip(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float FarClip
		{
			set
			{
				CSycles.camera_set_farclip(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float ApertureSize
		{
			set
			{
				CSycles.camera_set_aperturesize(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float ApertureRatio
		{
			set
			{
				CSycles.camera_set_aperture_ratio(Scene.Client.Id, Scene.Id, value);
			}
		}

		public uint Blades
		{
			set
			{
				CSycles.camera_set_blades(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float BladesRotation
		{
			set
			{
				CSycles.camera_set_bladesrotation(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float FocalDistance
		{
			set
			{
				CSycles.camera_set_focaldistance(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float ShutterTime
		{
			set
			{
				CSycles.camera_set_shuttertime(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float FishEyeFov
		{
			set
			{
				CSycles.camera_set_fisheye_fov(Scene.Client.Id, Scene.Id, value);
			}
		}

		public float FishEyeLens
		{
			set
			{
				CSycles.camera_set_fisheye_lens(Scene.Client.Id, Scene.Id, value);
			}
		}

		public void Update()
		{
			CSycles.camera_update(Scene.Client.Id, Scene.Id);
		}

		public void ComputeAutoViewPlane()
		{
			CSycles.camera_compute_auto_viewplane(Scene.Client.Id, Scene.Id);
		}
	}
}
