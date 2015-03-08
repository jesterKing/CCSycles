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

namespace ccl
{
	/// <summary>
	/// The background representation contains a background shader
	/// and settings to control Ambient Occlusion and path ray visibility
	/// </summary>
	public class Background
	{
		/// <summary>
		/// Reference to scene for which this the background is
		/// </summary>
		internal Scene Scene { get; set; }

		/// <summary>
		/// Create a background representation for scene. Generally this
		/// is handled by the Scene, therefor this is an internally visible
		/// constructor
		/// </summary>
		/// <param name="scene">Scene for which to create background</param>
		internal Background(Scene scene)
		{
			Scene = scene;
		}

		/// <summary>
		/// Get or set the background shader
		/// </summary>
		public Shader Shader
		{
			set
			{
				CSycles.scene_set_background_shader(Scene.Client.Id, Scene.Id, Scene.ShaderSceneId(value));
			}
			get
			{
				var shid = CSycles.scene_get_background_shader(Scene.Client.Id, Scene.Id);

				return Scene.ShaderFromSceneId(shid);
			}
		}

		/// <summary>
		/// Set the Ambient Occlusion distance
		/// </summary>
		public float AoDistance
		{
			set
			{
				CSycles.scene_set_background_ao_distance(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the Ambient Occlusion factor
		/// </summary>
		public float AoFactor
		{
			set
			{
				CSycles.scene_set_background_ao_factor(Scene.Client.Id, Scene.Id, value);
			}
		}

		/// <summary>
		/// Set the rays to which the background is responsive
		/// </summary>
		public PathRay Visibility
		{
			set
			{
				CSycles.scene_set_background_visibility(Scene.Client.Id, Scene.Id, value);
			}
		}
	}
}
