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
	/// <summary>
	/// Camera representation in a Cycles scene.
	/// </summary>
	public class Camera
	{
		/// <summary>
		/// Reference to the scene in which this camera is contained.
		/// </summary>
		internal Scene Scene { get; set; }

		/// <summary>
		/// Create a new camera representation for Scene.
		/// </summary>
		/// <param name="scene"></param>
		internal Camera(Scene scene)
		{
			Scene = scene;
		}

		/// <summary>
		/// Set the camera size. This corresponds to the final pixel resolution of the
		/// image to be rendered.
		/// </summary>
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

		/// <summary>
		/// Set the transformation matrix.
		/// </summary>
		public Transform Matrix
		{
			set
			{
				CSycles.camera_set_matrix(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set camera type.
		/// </summary>
		public CameraType Type
		{
			set
			{
				CSycles.camera_set_type(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set panorama type.
		/// </summary>
		public PanoramaType PanoramaType
		{
			set
			{
				CSycles.camera_set_panorama_type(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the Field of View.
		/// </summary>
		public float Fov
		{
			set
			{
				CSycles.camera_set_fov(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the sensor width.
		/// </summary>
		public float SensorWidth
		{
			set
			{
				CSycles.camera_set_sensor_width(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the sensor height.
		/// </summary>
		public float SensorHeight
		{
			set
			{
				CSycles.camera_set_sensor_height(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the near clip.
		/// </summary>
		public float NearClip
		{
			set
			{
				CSycles.camera_set_nearclip(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the far clip.
		/// </summary>
		public float FarClip
		{
			set
			{
				CSycles.camera_set_farclip(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the aperture size.
		/// </summary>
		public float ApertureSize
		{
			set
			{
				CSycles.camera_set_aperturesize(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the aperture ration.
		/// </summary>
		public float ApertureRatio
		{
			set
			{
				CSycles.camera_set_aperture_ratio(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the number of blades to use.
		/// </summary>
		public uint Blades
		{
			set
			{
				CSycles.camera_set_blades(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the blade rotation
		/// </summary>
		public float BladesRotation
		{
			set
			{
				CSycles.camera_set_bladesrotation(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the focal distance (for DoF)
		/// </summary>
		public float FocalDistance
		{
			set
			{
				CSycles.camera_set_focaldistance(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the shutter time.
		/// </summary>
		public float ShutterTime
		{
			set
			{
				CSycles.camera_set_shuttertime(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set Field of View, in case fish eye projection is used.
		/// </summary>
		public float FishEyeFov
		{
			set
			{
				CSycles.camera_set_fisheye_fov(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set lens length, in case fish eye projection is used.
		/// </summary>
		public float FishEyeLens
		{
			set
			{
				CSycles.camera_set_fisheye_lens(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Tag the camera for update.
		/// </summary>
		public void Update()
		{
			CSycles.camera_update(Scene.Client.Id, Scene.Id);
		}

		/// <summary>
		/// Compute auto view plane.
		/// </summary>
		public void ComputeAutoViewPlane()
		{
			CSycles.camera_compute_auto_viewplane(Scene.Client.Id, Scene.Id);
		}

		/// <summary>
		/// Set view plane
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="top"></param>
		/// <param name="bottom"></param>
		public void SetViewPlane(float left, float right, float top, float bottom)
		{
			CSycles.camera_set_viewplane(Scene.Client.Id, Scene.Id, left, right, top, bottom);
		}
	}
}
