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
	/// Wrapper for creating and setting session parameters.
	/// 
	/// Note that this API is used only for setting parameters, not fetching them.
	/// </summary>
	public class SessionParameters
	{
		private Device _device;
		/// <summary>
		/// Get the ID for the session parameters.
		/// </summary>
		public uint Id { get; private set; }
		private Client Client { get; set; }
		/// <summary>
		/// Create session parameters using <c>Device</c>
		/// </summary>
		/// <param name="dev">The device to create session parameters for</param>
		public SessionParameters(Client client, Device dev)
		{
			Client = client;
			_device = dev;
			Id = CSycles.session_params_create(Client.Id, (uint)dev.Id);
		}

		/// <summary>
		/// Change session parameters to use given Device
		/// </summary>
		public Device SetDevice
		{
			set
			{
				_device = value;
				CSycles.session_params_set_device(Client.Id, Id, (uint)_device.Id);
			}
		}

		/// <summary>
		/// Set to true if background rendering is wanted
		/// </summary>
		public bool Background
		{
			set
			{
				CSycles.session_params_set_background(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if rendering should be progressively refined
		/// </summary>
		public bool ProgressiveRefine
		{
			set
			{
				CSycles.session_params_set_progressive_refine(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if rendering should be progressive
		/// </summary>
		public bool Progressive
		{
			set
			{
				CSycles.session_params_set_progressive(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the output path to which final render should be written
		/// </summary>
		public string OutputPath
		{
			set
			{
				CSycles.session_params_set_output_path(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if experimental shading features should be used
		/// </summary>
		public bool Experimental
		{
			set
			{
				CSycles.session_params_set_experimental(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the amount of samples to render
		/// </summary>
		public int Samples
		{
			set
			{
				CSycles.session_params_set_samples(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the Size of a tile used during rendering
		/// </summary>
		public Size TileSize
		{
			set
			{
				CSycles.session_params_set_tile_size(Client.Id, Id, (uint)value.Width, (uint)value.Height);
			}
		}

		/// <summary>
		/// Set the TileOrder in which tiles are rendered
		/// </summary>
		public TileOrder TileOrder
		{
			set
			{
				CSycles.session_params_set_tile_order(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the initial resolution at which updates should be pushed by the renderer.
		/// 
		/// Higher values means a more complete result is shown, but might take longer before
		/// anything visible is pushed.
		/// </summary>
		public int StartResolution
		{
			set
			{
				CSycles.session_params_set_start_resolution(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// The number of CPU threads to use to handle the rendering process.
		/// 
		/// 0 means automatic thread count based on available logic cores.
		/// </summary>
		public uint Threads
		{
			set
			{
				CSycles.session_params_set_threads(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set to true if the display buffer should be handled linearly
		/// </summary>
		public bool DisplayBufferLinear
		{
			set
			{
				CSycles.session_params_set_display_buffer_linear(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the cancel timeout
		/// </summary>
		public double CancelTimeout
		{
			set
			{
				CSycles.session_params_set_cancel_timeout(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the reset timeout
		/// </summary>
		public double ResetTimeout
		{
			set
			{
				CSycles.session_params_set_reset_timeout(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set the text timeout
		/// </summary>
		public double TextTimeout
		{
			set
			{
				CSycles.session_params_set_text_timeout(Client.Id, Id, value);
			}
		}

		/// <summary>
		/// Set which ShadingSystem should be used.
		/// 
		/// Note: only SVM supported currently.
		/// </summary>
		public ShadingSystem ShadingSystem
		{
			set
			{
				CSycles.session_params_set_shadingsystem(Client.Id, Id, value);
			}
		}
	}
}
