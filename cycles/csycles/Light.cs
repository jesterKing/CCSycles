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
	public class Light
	{
		public uint Id { get; internal set; }
		public Scene Scene { get; internal set; }
		public Client Client { get; internal set; }
		public Light(Client client, Scene scene, Shader lightShader)
		{
			Client = client;
			Scene = scene;
			Id = CSycles.create_light(Client.Id, Scene.Id, scene.ShaderSceneId(lightShader));
		}

		public LightType Type
		{
			set
			{
				CSycles.light_set_type(Client.Id, Scene.Id, Id, value);
			}
		}

		public float Size
		{
			set
			{
				CSycles.light_set_size(Client.Id, Scene.Id, Id, value);
			}
		}

		public float4 Location
		{
			set
			{
				CSycles.light_set_co(Client.Id, Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		public float4 Direction
		{
			set
			{
				CSycles.light_set_dir(Client.Id, Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		public float SizeU
		{
			set
			{
				CSycles.light_set_sizeu(Client.Id, Scene.Id, Id, value);
			}
		}

		public float SizeV
		{
			set
			{
				CSycles.light_set_sizev(Client.Id, Scene.Id, Id, value);
			}
		}

		public float4 AxisU
		{
			set
			{
				CSycles.light_set_axisu(Client.Id, Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		public float4 AxisV
		{
			set
			{
				CSycles.light_set_axisv(Client.Id, Scene.Id, Id, value.x, value.y, value.z);
			}
		}

		public float SpotAngle
		{
			set
			{
				CSycles.light_set_spot_angle(Client.Id, Scene.Id, Id, value);
			}
		}

		public float SpotSmooth
		{
			set
			{
				CSycles.light_set_spot_smooth(Client.Id, Scene.Id, Id, value);
			}
		}
	}
}
