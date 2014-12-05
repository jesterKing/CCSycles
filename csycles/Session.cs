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
	/// The Cycles session representation.
	/// 
	/// The Session is used to manage the render process at the highest level. Here one can set
	/// the different callbacks to retrieve render results and updates and statistics during a render.
	/// 
	/// A session is created using SessionParameters and a Scene.
	/// 
	/// Through Session the render process is started and ended.
	/// </summary>
	public class Session
	{
		/// <summary>
		/// True if the session has already been destroyed.
		/// </summary>
		private bool Destroyed { get; set; }
		/// <summary>
		/// Get the Scene used for this Session
		/// </summary>
		public Scene Scene { get; private set; }
		/// <summary>
		/// Get the SessionParams used for this Session
		/// </summary>
		public SessionParameters SessionParams { get; private set; }
		/// <summary>
		/// Get the ID for this session
		/// </summary>
		public uint Id { get; private set; }

		private Client Client { get; set; }
		/// <summary>
		/// Create a new session using the given SessionParameters and Scene
		/// </summary>
		/// <param name="sessionParams">Previously created SessionParameters to create Session with</param>
		/// <param name="scene">Previously created Scene to create Session with</param>
		public Session(Client client, SessionParameters sessionParams, Scene scene)
		{
			Client = client;
			SessionParams = sessionParams;
			Scene = scene;
			Id = CSycles.session_create(Client.Id, sessionParams.Id, scene.Id);
		}

		CSycles.UpdateCallback updateCallback;

		/// <summary>
		/// Set the CSycles.UpdateCallback to Cycles to receive progress updates
		/// </summary>
		public CSycles.UpdateCallback UpdateCallback
		{
			set
			{
				if (Destroyed) return;
				updateCallback = value;
				CSycles.session_set_update_callback(Client.Id, Id, value);
			}
		}

		CSycles.RenderTileCallback updateTileCallback;

		/// <summary>
		/// Set the CSycles.RenderTileCallback to use for render tile updates
		/// </summary>
		public CSycles.RenderTileCallback UpdateTileCallback
		{
			set
			{
				if (Destroyed) return;
				updateTileCallback = value;
				CSycles.session_set_update_tile_callback(Client.Id, Id, value);
			}
		}

		CSycles.RenderTileCallback writeTileCallback;
		/// <summary>
		/// Set the CSycles.RenderTileCallback to use for render tile writes
		/// </summary>
		public CSycles.RenderTileCallback WriteTileCallback
		{
			set
			{
				if (Destroyed) return;
				writeTileCallback = value;
				CSycles.session_set_write_tile_callback(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Start the rendering session
		/// </summary>
		public void Start()
		{
			if (Destroyed) return;
			CSycles.progress_reset(Client.Id, Id);
			CSycles.session_start(Client.Id, Id);
		}

		/// <summary>
		/// Wait for the rendering session to complete
		/// </summary>
		public void Wait()
		{
			if (Destroyed) return;
			CSycles.session_wait(Client.Id, Id);
		}

		/// <summary>
		/// Cancel the currently in progress render
		/// </summary>
		/// <param name="cancelMessage"></param>
		public void Cancel(string cancelMessage)
		{
			if (Destroyed) return;
			CSycles.session_cancel(Client.Id, Id, cancelMessage);
		}

		/// <summary>
		/// Destroy the session and all related.
		/// </summary>
		public void Destroy()
		{
			if (Destroyed) return;
			CSycles.session_destroy(Client.Id, Id);
			Destroyed = true;
		}

		public void Draw()
		{
			if (Destroyed) return;
			CSycles.session_draw(Client.Id, Id);
		}

		/// <summary>
		/// Copy the ccycles API level session buffer through CSycles into this Session.
		/// 
		/// TODO: implement a way to do partial updates
		/// </summary>
		/// <returns></returns>
		public float[] CopyBuffer()
		{
			if (Destroyed) return null;
			uint bufStride = 0;
			uint bufSize = 0;

			BufferInfo(out bufSize, out bufStride);

			return CSycles.session_copy_buffer(Client.Id, Id, bufSize);
		}

		/// <summary>
		/// Retrieve the buffer information for this Session
		/// </summary>
		/// <param name="bufferSize">Contains the buffer size in floats</param>
		/// <param name="bufferStride">Contains the stride to use in the buffer</param>
		public void BufferInfo(out uint bufferSize, out uint bufferStride) 
		{
			if (Destroyed)
			{
				bufferSize = 0;
				bufferStride = 0;
				return;
			}
			CSycles.session_get_buffer_info(Client.Id, Id, out bufferSize, out bufferStride);
		}

		/// <summary>
		/// Reset a Session
		/// </summary>
		/// <param name="width">Width of the resolution to reset with</param>
		/// <param name="height">Height of the resolutin to reset with</param>
		/// <param name="samples">The amount of samples to reset with</param>
		public void Reset(uint width, uint height, uint samples)
		{
			if (Destroyed) return;
			CSycles.session_reset(Client.Id, Id, width, height, samples);
		}
	}
}
