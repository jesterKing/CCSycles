using System;
using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region session
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_reset", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_reset(uint clientId, uint sessionId, uint width, uint height, uint samples);
		public static void session_reset(uint clientId, uint sessionId, uint width, uint height, uint samples)
		{
			cycles_session_reset(clientId, sessionId, width, height, samples);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_session_create(uint clientId, uint sessionParamsId, uint sceneId);
		public static uint session_create(uint clientId, uint sessionParamsId, uint sceneId)
		{
			return cycles_session_create(clientId, sessionParamsId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_destroy", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_session_destroy(uint clientId, uint sceneId);
		public static uint session_destroy(uint clientId, uint sceneId)
		{
			return cycles_session_destroy(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_copy_buffer", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_copy_buffer(uint clientId, uint sessionId, [In, Out] IntPtr buffer);
		public static float[] session_copy_buffer(uint clientId, uint sessionId, uint bufferSize)
		{
			var alloc_mem = Marshal.AllocHGlobal((int)bufferSize * sizeof(float));
			var to_return = new float[bufferSize];
			cycles_session_copy_buffer(clientId, sessionId, alloc_mem);
			Marshal.Copy(alloc_mem, to_return, 0, (int)bufferSize);

			Marshal.FreeHGlobal(alloc_mem);
			return to_return;
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_get_buffer", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_session_get_buffer(uint clientId, uint sessionId);
		public static IntPtr session_get_buffer(uint clientId, uint sessionId)
		{
			return cycles_session_get_buffer(clientId, sessionId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_get_buffer_info", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_get_buffer_info(uint clientId, uint sessionId, [Out] out uint bufferSize, [Out] out uint bufferStride);
		public static void session_get_buffer_info(uint clientId, uint sessionId, out uint bufferSize, out uint bufferStride)
		{
			cycles_session_get_buffer_info(clientId, sessionId, out bufferSize, out bufferStride);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void UpdateCallback(uint sid);
		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cycles_session_set_update_callback")]
		private static extern void cycles_session_set_update_callback(uint clientId, uint sessionId, UpdateCallback update);
		public static void session_set_update_callback(uint clientId, uint sessionId, UpdateCallback update)
		{
			cycles_session_set_update_callback(clientId, sessionId, update);
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void RenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth);
		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cycles_session_set_update_tile_callback")]
		private static extern void cycles_session_set_update_tile_callback(uint clientId, uint sessionId, RenderTileCallback renderTileCb);
		public static void session_set_update_tile_callback(uint clientId, uint sessionId, RenderTileCallback renderTileCb)
		{
			cycles_session_set_update_tile_callback(clientId, sessionId, renderTileCb);
		}
		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cycles_session_set_write_tile_callback")]
		private static extern void cycles_session_set_write_tile_callback(uint clientId, uint sessionId, RenderTileCallback renderTileCb);
		public static void session_set_write_tile_callback(uint clientId, uint sessionId, RenderTileCallback renderTileCb)
		{
			cycles_session_set_write_tile_callback(clientId, sessionId, renderTileCb);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_start", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_start(uint clientId, uint sessionId);
		public static void session_start(uint clientId, uint sessionId)
		{
			cycles_session_start(clientId, sessionId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_wait", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_wait(uint clientId, uint sessionId);
		public static void session_wait(uint clientId, uint sessionId)
		{
			cycles_session_wait(clientId, sessionId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_cancel", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_cancel(uint clientId, uint sessionId, [MarshalAs(UnmanagedType.LPStr)] string cancelMessage);
		public static void session_cancel(uint clientId, uint sessionId, string cancelMessage)
		{
			cycles_session_cancel(clientId, sessionId, cancelMessage);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_draw", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_draw(uint clientId, uint sessionId);
		public static void session_draw(uint clientId, uint sessionId)
		{
			cycles_session_draw(clientId, sessionId);
		}
#endregion

#region session parameters
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_session_params_create(uint clientId, uint deviceId);
		public static uint session_params_create(uint clientId, uint deviceId)
		{
			return cycles_session_params_create(clientId, deviceId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_device", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_device(uint clientId, uint sessionParamsId, uint deviceId);
		public static void session_params_set_device(uint clientId, uint sessionParamsId, uint deviceId)
		{
			cycles_session_params_set_device(clientId, sessionParamsId, deviceId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_background", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_background(uint clientId, uint sessionParamsId, uint background);
		public static void session_params_set_background(uint clientId, uint sessionParamsId, bool background)
		{
			cycles_session_params_set_background(clientId, sessionParamsId, (uint)(background ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_progressive_refine", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_progressive_refine(uint clientId, uint sessionParamsId, uint progressiveRefine);
		public static void session_params_set_progressive_refine(uint clientId, uint sessionParamsId, bool progressiveRefine)
		{
			cycles_session_params_set_progressive_refine(clientId, sessionParamsId, (uint)(progressiveRefine?1:0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_output_path", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_output_path(uint clientId, uint sessionParamsId, [MarshalAs(UnmanagedType.LPStr)] string outputPath);
		public static void session_params_set_output_path(uint clientId, uint sessionParamsId, string outputPath)
		{
			cycles_session_params_set_output_path(clientId, sessionParamsId, outputPath);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_progressive", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_progressive(uint clientId, uint sessionParamsId, uint progressive);
		public static void session_params_set_progressive(uint clientId, uint sessionParamsId, bool progressive)
		{
			cycles_session_params_set_progressive(clientId, sessionParamsId, (uint)(progressive?1:0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_experimental", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_experimental(uint clientId, uint sessionParamsId, uint experimental);
		public static void session_params_set_experimental(uint clientId, uint sessionParamsId, bool experimental)
		{
			cycles_session_params_set_experimental(clientId, sessionParamsId, (uint)(experimental?1:0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_samples(uint clientId, uint sessionParamsId, int samples);
		public static void session_params_set_samples(uint clientId, uint sessionParamsId, int samples)
		{
			cycles_session_params_set_samples(clientId, sessionParamsId, samples);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_tile_size", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_tile_size(uint clientId, uint sessionParamsId, uint x, uint y);
		public static void session_params_set_tile_size(uint clientId, uint sessionParamsId, uint x, uint y)
		{
			cycles_session_params_set_tile_size(clientId, sessionParamsId, x, y);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_tile_order", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_tile_order(uint clientId, uint sessionParamsId, uint tileOrder);
		public static void session_params_set_tile_order(uint clientId, uint sessionParamsId, TileOrder tileOrder)
		{
			cycles_session_params_set_tile_order(clientId, sessionParamsId, (uint)tileOrder);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_start_resolution", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_start_resolution(uint clientId, uint sessionParamsId, int startResolution);
		public static void session_params_set_start_resolution(uint clientId, uint sessionParamsId, int startResolution)
		{
			cycles_session_params_set_start_resolution(clientId, sessionParamsId, startResolution);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_threads", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_threads(uint clientId, uint sessionParamsId, uint threads);
		public static void session_params_set_threads(uint clientId, uint sessionParamsId, uint threads)
		{
			cycles_session_params_set_threads(clientId, sessionParamsId, threads);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_display_buffer_linear", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_display_buffer_linear(uint clientId, uint sessionParamsId, uint displayBufferLinear);
		public static void session_params_set_display_buffer_linear(uint clientId, uint sessionParamsId, bool displayBufferLinear)
		{
			cycles_session_params_set_display_buffer_linear(clientId, sessionParamsId, (uint)(displayBufferLinear ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_cancel_timeout", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_cancel_timeout(uint clientId, uint sessionParamsId, double cancelTimeout);
		public static void session_params_set_cancel_timeout(uint clientId, uint sessionParamsId, double cancelTimeout)
		{
			cycles_session_params_set_cancel_timeout(clientId, sessionParamsId, cancelTimeout);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_reset_timeout", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_reset_timeout(uint clientId, uint sessionParamsId, double resetTimeout);
		public static void session_params_set_reset_timeout(uint clientId, uint sessionParamsId, double resetTimeout)
		{
			cycles_session_params_set_reset_timeout(clientId, sessionParamsId, resetTimeout);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_text_timeout", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_text_timeout(uint clientId, uint sessionParamsId, double textTimeout);
		public static void session_params_set_text_timeout(uint clientId, uint sessionParamsId, double textTimeout)
		{
			cycles_session_params_set_text_timeout(clientId, sessionParamsId, textTimeout);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_session_params_set_shadingsystem", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_session_params_set_shadingsystem(uint clientId, uint sessionParamsId, uint shadingsystem);
		public static void session_params_set_shadingsystem(uint clientId, uint sessionParamsId, ShadingSystem shadingSystem)
		{
			cycles_session_params_set_shadingsystem(clientId, sessionParamsId, (uint)shadingSystem);
		}
#endregion

#region progress
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_reset", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_progress_reset(uint clientId, uint sessionId);
		public static void progress_reset(uint clientId, uint sessionId)
		{
			cycles_progress_reset(clientId, sessionId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_get_sample", CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_progress_get_sample(uint clientId, uint sessionId);
		public static int progress_get_sample(uint clientId, uint sessionId)
		{
			return cycles_progress_get_sample(clientId, sessionId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_get_tile", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_progress_get_tile(uint clientId, uint sessionId, out int tile, out double totalTime, out double sampleTime);
		public static void progress_get_tile(uint clientId, uint sessionId, out int tile, out double totalTime, out double sampleTime)
		{
			cycles_progress_get_tile(clientId, sessionId, out tile, out totalTime, out sampleTime);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_get_progress", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_progress_get_progress(uint clientId, uint sessionId, out float progress, out double totalTime);
		public static void progress_get_progress(uint clientId, uint sessionId, out float progress, out double totalTime)
		{
			cycles_progress_get_progress(clientId, sessionId, out progress, out totalTime);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_tilemanager_get_sample_info", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_tilemanager_get_sample_info(uint clientId, uint sessionId, out uint samples, out uint numSamples);
		public static void tilemanager_get_sample_info(uint clientId, uint sessionId, out uint samples, out uint numSamples)
		{
			cycles_tilemanager_get_sample_info(clientId, sessionId, out samples, out numSamples);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_get_status", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_progress_get_status(uint clientId, uint sessionId);
		public static string progress_get_status(uint clientId, uint sessionId)
		{
			return Marshal.PtrToStringAnsi(cycles_progress_get_status(clientId, sessionId));
		}

		
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_progress_get_substatus", CharSet = CharSet.Ansi, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr cycles_progress_get_substatus(uint clientId, uint sessionId);
		public static string progress_get_substatus(uint clientId, uint sessionId)
		{
			return Marshal.PtrToStringAnsi(cycles_progress_get_substatus(clientId, sessionId));
		}

#endregion
	}
}
