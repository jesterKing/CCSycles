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
using System.Runtime.InteropServices;

/** \namespace ccl
 * \brief Namespace containing the low-level wrapping API of ccycles.dll and a set of higher-level classes.
 */
namespace ccl
{
	/// <summary>
	/// CSycles wraps the ccycles.dll, providing a very low-level API into the
	/// render engine Cycles.
	/// </summary>
	public class CSycles
	{
#region misc

		private static bool ccycles_loaded = false;

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string filename);

		private static void LoadCCycles()
		{
			if (!ccycles_loaded)
			{
				var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
				var ccycles_dll = System.IO.Path.Combine(path, "ccycles.dll");
				LoadLibrary(ccycles_dll);
				ccycles_loaded = true;
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_initialise", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_initialise();
		/// <summary>
		/// Initialise the Cycles render engine.
		/// 
		/// This will ensure ccycles.dll is loaded. The initialisation will also prepare
		/// the devices list for use with Cycles.
		/// 
		/// Note: call <c>set_kernel_path</c> before initialising CSycles, otherwise default
		/// kernel path "lib" will be used.
		/// </summary>
		/// <returns></returns>
		public static void initialise()
		{
			LoadCCycles();
			cycles_initialise();
		}

		/// <summary>
		/// Set the path location to look for CUDA kernels.
		/// 
		/// Note: to have any effect needs to be called before <c>initialise</c>.
		/// </summary>
		/// <param name="kernel_path"></param>
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_set_kernel_path", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_set_kernel_path(string kernel_path);

		public static void set_kernel_path([MarshalAsAttribute(UnmanagedType.LPStr)] string kernel_path)
		{
			LoadCCycles();
			cycles_set_kernel_path(kernel_path);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shutdown", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shutdown();
		/// <summary>
		/// Clean up CSycles, CCycles and Cycles.
		/// </summary>
		/// <returns></returns>
		public static void shutdown()
		{
			cycles_shutdown();
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		/** <summary>
		 * Signature for a logger callback.
		 * 
		 * CCycles will call logger callbacks only if built in Debug mode.
		 * </summary>
		 */
		public delegate void LoggerCallback([MarshalAsAttribute(UnmanagedType.LPStr)] string msg);

		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cycles_set_logger")]
		private static extern void cycles_set_logger(uint clientId, IntPtr loggerCb);
		/// <summary>
		/// Set the logger function to CCycles.
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="loggerCb">The logger callback function.</param>
		/// <param name="toStdOut">True if logged information should be printed to std::out as well.</param>
		public static void set_logger(uint clientId, LoggerCallback loggerCb)
		{
			var intptr_delegate = Marshal.GetFunctionPointerForDelegate(loggerCb);
			cycles_set_logger(clientId, intptr_delegate);
		}

		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "cycles_log_to_stdout")]
		private static extern void cycles_log_to_stdout(int stdout);
		/**
		 * Set to true if logger output should be sent to std::cout as well.
		 *
		 * Note that this is global to the logger.
		 */
		public static void log_to_stdout(bool stdOut)
		{
			cycles_log_to_stdout(stdOut ? 1 : 0);
		}

		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "cycles_new_client")]
		private static extern uint cycles_new_client();
		public static uint new_client()
		{
			return cycles_new_client();
		}

		[DllImport("ccycles.dll", SetLastError = false, CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "cycles_release_client")]
		private static extern void cycles_release_client(uint clientId);
		public static void release_client(uint clientId)
		{
			cycles_release_client(clientId);
		}
#endregion

#region devices
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_number_devices", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_devices();
		/// <summary>
		/// Get the number of available render devices.
		/// </summary>
		/// <returns>number of available render devices.</returns>
		public static uint number_devices()
		{
			return cycles_number_devices();
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_number_cuda_devices", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_number_cuda_devices();
		/// <summary>
		/// Get the number of available CUDA devices.
		/// </summary>
		/// <returns>number of available CUDA devices.</returns>
		public static uint number_cuda_devices()
		{
			return cycles_number_devices();
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_description", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_device_description(int i);
		/// <summary>
		/// Get the device description for specified device.
		/// </summary>
		/// <param name="i">Device ID to get description of.</param>
		/// <returns>The device description.</returns>
		public static string device_decription(int i)
		{
			return Marshal.PtrToStringAnsi(cycles_device_description(i));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_id", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_device_id(int i);
		/// <summary>
		/// Get the device ID string
		/// </summary>
		/// <param name="i">Device ID to get the device ID string for.</param>
		/// <returns>Device ID string.</returns>
		public static string DeviceId(int i)
		{
			return Marshal.PtrToStringAnsi(cycles_device_id(i));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_num", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_device_num(int i);
		/// <summary>
		/// Return device enumeration number
		/// </summary>
		/// <param name="i">Device ID to get the device enumeration number for.</param>
		/// <returns>Device enumeration number</returns>
		public static uint device_num(int i)
		{
			return cycles_device_num(i);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_device_type(int i);
		/// <summary>
		/// Get the <c>DeviceType</c> of th specified device
		/// </summary>
		/// <param name="i">Device ID to get the <c>DeviceType</c> for</param>
		/// <returns><c>DeviceType</c></returns>
		public static DeviceType device_type(int i)
		{
			return (DeviceType)cycles_device_type(i);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_advanced_shading", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_device_advanced_shading(int i);
		/// <summary>
		/// Query if device supports advanced shading.
		/// 
		/// \todo explain what advanced shading entails.
		/// </summary>
		/// <param name="i">Device ID to query</param>
		/// <returns>True if the device supports advanced shading.</returns>
		public static bool device_advanced_shading(int i)
		{
			return cycles_device_advanced_shading(i);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_display_device", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_device_display_device(int i);
		/// <summary>
		/// Query if device is used as display device.
		/// </summary>
		/// <param name="i">Device ID to query</param>
		/// <returns>True if the device is used as display device.</returns>
		public static bool device_display_device(int i)
		{
			return cycles_device_display_device(i);
		}


		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_device_pack_images", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_device_pack_images(int i);
		/// <summary>
		/// Query if device supports packed images.
		/// </summary>
		/// <param name="i">Device ID to query.</param>
		/// <returns>True if the device supports packed images.</returns>
		public static bool device_pack_images(int i)
		{
			return cycles_device_pack_images(i);
		}
#endregion

#region scene parameters
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_params_create(uint clientId, uint shadingsystem, uint bvhtype, uint bvhcache, uint bvhspatialsplit, uint qbvh, uint persistentdata);
		public static uint scene_params_create(uint clientId, ShadingSystem shadingSystem, BvhType bvhType, bool bvhCache, bool bvhSpatialSplit, bool qbvh, bool persistentData)
		{
			return cycles_scene_params_create(clientId, (uint)shadingSystem, (uint)bvhType, (uint)(bvhCache?1:0), (uint)(bvhSpatialSplit?1:0), (uint)(qbvh?1:0), (uint)(persistentData?1:0));
		}
		  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_type(uint clientId, uint sceneParamsId, uint type);
		public static void scene_params_set_bvh_type(uint clientId, uint sceneParamsId, BvhType type)
		{
			cycles_scene_params_set_bvh_type(clientId, sceneParamsId, (uint)type);
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_cache", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_cache(uint clientId, uint sceneParamsId, uint cache);
		public static void scene_params_set_bvh_cache(uint clientId, uint sceneParamsId, bool type)
		{
			cycles_scene_params_set_bvh_cache(clientId, sceneParamsId, (uint)(type?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_bvh_spatial_split", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_spatial_split(uint clientId, uint sceneParamsId, uint split);
		public static void scene_params_set_bvh_spatial_split(uint clientId, uint sceneParamsId, bool split)
		{
			cycles_scene_params_set_bvh_cache(clientId, sceneParamsId, (uint)(split?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_qbvh", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_qbvh(uint clientId, uint sceneParamsId, uint qbvh);
		public static void scene_params_set_qbvh(uint clientId, uint sceneParamsId, bool qbvh)
		{
			cycles_scene_params_set_qbvh(clientId, sceneParamsId, (uint)(qbvh?1:0));
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_shadingsystem", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_shadingsystem(uint clientId, uint sceneParamsId, uint shadingsystem);
		public static void scene_params_set_shadingsystem(uint clientId, uint sceneParamsId, ShadingSystem system)
		{
			cycles_scene_params_set_shadingsystem(clientId, sceneParamsId, (uint)system);
		}
  
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_params_set_persistent_data", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_persistent_data(uint clientId, uint sceneParamsId, uint persistent_data);
		public static void scene_params_set_persistent_data(uint clientId, uint sceneParamsId, bool persistent)
		{
			cycles_scene_params_set_persistent_data(clientId, sceneParamsId, (uint)(persistent?1:0));
		}
  
#endregion

#region scene
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_create", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_create(uint clientId, uint sceneParamsId, uint deviceid);
		public static uint scene_create(uint clientId, uint sceneParamsId, uint deviceid)
		{
			return cycles_scene_create(clientId, sceneParamsId, deviceid);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_add_object", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_object(uint clientId, uint sceneId);
		public static uint scene_add_object(uint clientId, uint sceneId)
		{
			return cycles_scene_add_object(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_add_mesh", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_mesh(uint clientId, uint sceneId, uint objectId, uint shaderId);
		public static uint scene_add_mesh(uint clientId, uint sceneId, uint objectId, uint shaderId)
		{
			return cycles_scene_add_mesh(clientId, sceneId, objectId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_shader(uint clientId, uint sceneId, uint shaderId);
		public static void scene_set_background_shader(uint clientId, uint sceneId, uint shaderId)
		{
			cycles_scene_set_background_shader(clientId, sceneId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_get_background_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_get_background_shader(uint clientId, uint sceneId);
		public static uint scene_get_background_shader(uint clientId, uint sceneId)
		{
			return cycles_scene_get_background_shader(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_default_surface_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_default_surface_shader(uint clientId, uint sceneId, uint shaderId);
		/// <summary>
		/// Set the default surface shader for sceneId to shaderId.
		/// 
		/// Note that this shaderId has to be the scene-specific shader id.
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="sceneId">Scene for which the default shader is set</param>
		/// <param name="shaderId">The shader to which the default shader is set</param>
		public static void scene_set_default_surface_shader(uint clientId, uint sceneId, uint shaderId)
		{
			cycles_scene_set_default_surface_shader(clientId, sceneId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_get_default_surface_shader",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_get_default_surface_shader(uint clientId, uint sceneId);
		/// <summary>
		/// Get the default surface shader id for sceneId
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="sceneId"></param>
		/// <returns></returns>
		public static uint scene_get_default_surface_shader(uint clientId, uint sceneId)
		{
			return cycles_scene_get_default_surface_shader(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_ao_factor", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_ao_factor(uint clientId, uint sceneId, float aoFactor);

		public static void scene_set_background_ao_factor(uint clientId, uint sceneId, float aoFactor)
		{
			cycles_scene_set_background_ao_factor(clientId, sceneId, aoFactor);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_ao_distance", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_ao_distance(uint clientId, uint sceneId, float aoDistance);

		public static void scene_set_background_ao_distance(uint clientId, uint sceneId, float aoDistance)
		{
			cycles_scene_set_background_ao_distance(clientId, sceneId, aoDistance);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_set_background_visibility", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_visibility(uint clientId, uint sceneId, int raypathFlag);

		public static void scene_set_background_visibility(uint clientId, uint sceneId, PathRay raypathFlag)
		{
			cycles_scene_set_background_visibility(clientId, sceneId, (int)raypathFlag);
		}
#endregion

#region integrator
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_min_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_min_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_min_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_min_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_diffuse_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_diffuse_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_diffuse_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_diffuse_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_glossy_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_glossy_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_glossy_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_glossy_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_transmission_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_transmission_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_transmission_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_transmission_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_max_volume_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_max_volume_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_max_volume_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_max_volume_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_max_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_max_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_transparent_max_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transparent_max_bounce(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_min_bounce", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_min_bounce(uint clientId, uint sceneId, int value);
		public static void integrator_set_transparent_min_bounce(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transparent_min_bounce(clientId, sceneId, value);
		}


		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_diffuse_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_diffuse_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_diffuse_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_diffuse_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_glossy_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_glossy_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_glossy_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_glossy_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transmission_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transmission_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_transmission_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_transmission_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_ao_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_ao_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_ao_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_ao_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_mesh_light_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_mesh_light_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_mesh_light_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_mesh_light_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_subsurface_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_subsurface_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_subsurface_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_subsurface_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_volume_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_volume_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_volume_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_volume_samples(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_aa_samples", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_aa_samples(uint clientId, uint sceneId, int value);
		public static void integrator_set_aa_samples(uint clientId, uint sceneId, int value)
		{
			cycles_integrator_set_aa_samples(clientId, sceneId, value);
		}


		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_no_caustics", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_no_caustics(uint clientId, uint sceneId, bool value);
		public static void integrator_set_no_caustics(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_no_caustics(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_transparent_shadows", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_transparent_shadows(uint clientId, uint sceneId, bool value);
		public static void integrator_set_transparent_shadows(uint clientId, uint sceneId, bool value)
		{
			cycles_integrator_set_transparent_shadows(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_filter_glossy", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_filter_glossy(uint clientId, uint sceneId, float value);
		public static void integrator_set_filter_glossy(uint clientId, uint sceneId, float value)
		{
			cycles_integrator_set_filter_glossy(clientId, sceneId, value);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_integrator_set_method", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_integrator_set_method(uint clientId, uint sceneId, int value);
		public static void integrator_set_method(uint clientId, uint sceneId, IntegratorMethod value)
		{
			cycles_integrator_set_method(clientId, sceneId, (int)value);
		}


#endregion

#region object
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_object_set_matrix", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_matrix(uint clientId, uint sceneId, uint objectId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l,
			float m, float n, float o, float p);
		public static void object_set_matrix(uint clientId, uint sceneId, uint objectId, Transform t)
		{
			cycles_scene_object_set_matrix(clientId, sceneId, objectId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w,
				t.w.x, t.w.y, t.w.z, t.w.w);
		}
#endregion

#region mesh
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_verts", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_verts(uint clientId, uint sceneId, uint meshId, float* verts, uint vcount);
		public static void mesh_set_verts(uint clientId, uint sceneId, uint meshId, ref float[] verts, uint vcount)
		{
			unsafe
			{
				fixed (float* pverts = verts)
				{
					cycles_mesh_set_verts(clientId, sceneId, meshId, pverts, vcount);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_uvs", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_uvs(uint clientId, uint sceneId, uint meshId, float* uvs, uint uvcount);
		public static void mesh_set_uvs(uint clientId, uint sceneId, uint meshId, ref float[] uvs, uint uvcount)
		{
			unsafe
			{
				fixed (float* puvs = uvs)
				{
					cycles_mesh_set_uvs(clientId, sceneId, meshId, puvs, uvcount);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_tris", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_tris(uint clientId, uint sceneId, uint meshId, int* faces, uint fcount, uint shaderId);
		public static void mesh_set_tris(uint clientId, uint sceneId, uint meshId, ref int[] tris, uint fcount, uint shaderId)
		{
			unsafe
			{
				fixed (int* ptris = tris)
				{
					cycles_mesh_set_tris(clientId, sceneId, meshId, ptris, fcount, shaderId);
				}
			}

		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_add_triangle", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_add_triangle(uint clientId, uint sceneId, uint meshId, uint v0, uint v1, uint v2, uint shaderId, uint smooth);

		public static void mesh_add_triangle(uint clientId, uint sceneId, uint meshId, uint v0, uint v1, uint v2,
			uint shaderId, bool smooth)
		{
			cycles_mesh_add_triangle(clientId, sceneId, meshId, v0, v1, v2, shaderId, (uint)(smooth ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_mesh_set_smooth", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_set_smooth(uint clientId, uint sceneId, uint meshId, uint smooth);

		public static void mesh_set_smooth(uint clientId, uint sceneId, uint meshId, bool smooth)
		{
			cycles_mesh_set_smooth(clientId, sceneId, meshId, (uint)(smooth ? 1 : 0));
		}

#endregion

#region shaders
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_create_shader", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_create_shader(uint clientId);
		public static uint create_shader(uint clientId)
		{
			return cycles_create_shader(clientId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_add_shader", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_shader(uint clientId, uint sceneId, uint shaderId);
		public static uint scene_add_shader(uint clientId, uint sceneId, uint shaderId)
		{
			return cycles_scene_add_shader(clientId, sceneId, shaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_shader_id", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_shader_id(uint clientId, uint sceneId, uint shaderId);
		public static uint scene_shader_id(uint clientId, uint sceneId, uint shaderId)
		{
			return cycles_scene_shader_id(clientId, sceneId, shaderId);
		}

		/// <summary>
		/// The output shader node ID for any graph is always 0.
		/// </summary>
		public const uint OUTPUT_SHADERNODE_ID = 0;

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_add_shader_node", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_add_shader_node(uint clientId, uint shaderId, uint shnType);
		public static uint add_shader_node(uint clientId, uint shaderId, ShaderNodeType shnType)
		{
			return cycles_add_shader_node(clientId, shaderId, (uint) shnType);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_enum", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_enum(uint clientId, uint shaderId, uint shadernodeId, uint shnType, [MarshalAs(UnmanagedType.LPStr)] string value);
		public static void shadernode_set_enum(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType, string value)
		{
			cycles_shadernode_set_enum(clientId, shaderId, shadernodeId, (uint) shnType, value);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_attribute_int", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_int(uint clientId, uint shaderId, uint shadernodeId, string name, int val);
		public static void shadernode_set_attribute_int(uint clientId, uint shaderId, uint shadernodeId,  [MarshalAs(UnmanagedType.LPStr)] string name, int val)
		{
			cycles_shadernode_set_attribute_int(clientId, shaderId, shadernodeId, name, val);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_attribute_float", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_float(uint clientId, uint shaderId, uint shadernodeId, string name, float val);
		public static void shadernode_set_attribute_float(uint clientId, uint shaderId, uint shadernodeId,  [MarshalAs(UnmanagedType.LPStr)] string name, float val)
		{
			cycles_shadernode_set_attribute_float(clientId, shaderId, shadernodeId, name, val);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_attribute_vec", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_vec(uint clientId, uint shaderId, uint shadernodeId, string name, float x, float y, float z);
		public static void shadernode_set_attribute_vec(uint clientId, uint shaderId, uint shadernodeId,  [MarshalAs(UnmanagedType.LPStr)] string name, float4 val)
		{
			cycles_shadernode_set_attribute_vec(clientId, shaderId, shadernodeId, name, val.x, val.y, val.z);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_attribute_string", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_string(uint clientId, uint shaderId, uint shadernodeId, string name, string val);
		public static void shadernode_set_attribute_string(uint clientId, uint shaderId, uint shadernodeId,  [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string val)
		{
			cycles_shadernode_set_attribute_string(clientId, shaderId, shadernodeId, name, val);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_float", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_float(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, float val);
		public static void shadernode_set_member_float(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, float val)
		{
			cycles_shadernode_set_member_float(clientId, shaderId, shadernodeId, (uint)shnType, name, val);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_int", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_int(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, int val);
		public static void shadernode_set_member_int(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, int val)
		{
			cycles_shadernode_set_member_int(clientId, shaderId, shadernodeId, (uint)shnType, name, val);
		}
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_bool", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_bool(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, bool val);
		public static void shadernode_set_member_bool(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, bool val)
		{
			cycles_shadernode_set_member_bool(clientId, shaderId, shadernodeId, (uint)shnType, name, val);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_vec", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_vec(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, float x, float y, float z);
		public static void shadernode_set_member_vec(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, float x, float y, float z)
		{
			cycles_shadernode_set_member_vec(clientId, shaderId, shadernodeId, (uint)shnType, name, x, y, z);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_float_img", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_shadernode_set_member_float_img(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, string  imgName, float* img, uint width, uint height, uint depth, uint channels);
		public static void shadernode_set_member_float_img(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string imgName, ref float[] img, uint width, uint height, uint depth, uint channels)
		{
			unsafe
			{
				fixed (float* pimg = img)
				{
					cycles_shadernode_set_member_float_img(clientId, shaderId, shadernodeId, (uint)shnType, name, imgName, pimg, width, height, depth, channels);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_byte_img", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_shadernode_set_member_byte_img(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, string  imgName, byte* img, uint width, uint height, uint depth, uint channels);
		public static void shadernode_set_member_byte_img(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string imgName, ref byte[] img, uint width, uint height, uint depth, uint channels)
		{
			unsafe
			{
				fixed (byte* pimg = img)
				{
					cycles_shadernode_set_member_byte_img(clientId, shaderId, shadernodeId, (uint)shnType, name, imgName, pimg, width, height, depth, channels);
				}
			}
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_connect_nodes", CharSet = CharSet.Ansi, 
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_connect_nodes(uint clientId, uint shaderId, uint fromId, string from, uint toId,
			string to);
		public static void shader_connect_nodes(uint clientId, uint shaderId, uint fromId, string from, uint toId, string to)
		{
			cycles_shader_connect_nodes(clientId, shaderId, fromId, from, toId, to);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_set_name", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_name(uint clientId, uint shaderId, [MarshalAs(UnmanagedType.LPStr)] string name);
		public static void shader_set_name(uint clientId, uint shaderId, string name)
		{
			cycles_shader_set_name(clientId, shaderId, name);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_set_use_mis",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_use_mis(uint clientId, uint shaderId, uint useMis);
		public static void shader_set_use_mis(uint clientId, uint shaderId, bool useMis)
		{
			cycles_shader_set_use_mis(clientId, shaderId, (uint) (useMis ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_set_use_transparent_shadow",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_use_transparent_shadow(uint clientId, uint shaderId, uint useTransparentShadow);
		public static void shader_set_use_transparent_shadow(uint clientId, uint shaderId, bool useTransparentShadow)
		{
			cycles_shader_set_use_transparent_shadow(clientId, shaderId, (uint) (useTransparentShadow ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_set_heterogeneous_volume",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_heterogeneous_volume(uint clientId, uint shaderId, uint heterogeneousVolume);
		public static void shader_set_heterogeneous_volume(uint clientId, uint shaderId, bool heterogeneousVolume)
		{
			cycles_shader_set_heterogeneous_volume(clientId, shaderId, (uint) (heterogeneousVolume ? 1 : 0));
		}


#endregion

#region camera
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_size", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_size(uint clientId, uint sceneId, uint width, uint height);
		public static void camera_set_size(uint clientId, uint sceneId, uint width, uint height)
		{
			cycles_camera_set_size(clientId, sceneId, width, height);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_get_width", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_camera_get_width(uint clientId, uint sceneId);
		public static uint camera_get_width(uint clientId, uint sceneId)
		{
			return cycles_camera_get_width(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_get_height", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_camera_get_height(uint clientId, uint sceneId);
		public static uint camera_get_height(uint clientId, uint sceneId)
		{
			return cycles_camera_get_height(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_matrix", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_matrix(uint clientId, uint sceneId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l,
			float m, float n, float o, float p);
		public static void camera_set_matrix(uint clientId, uint sceneId, Transform t)
		{
			cycles_camera_set_matrix(clientId, sceneId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w,
				t.w.x, t.w.y, t.w.z, t.w.w);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_type(uint clientId, uint sceneId, uint type);
		public static void camera_set_type(uint clientId, uint sceneId, CameraType type)
		{
			cycles_camera_set_type(clientId, sceneId, (uint)type);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_panorama_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_panorama_type(uint clientId, uint sceneId, uint type);
		public static void camera_set_panorama_type(uint clientId, uint sceneId, PanoramaType type)
		{
			cycles_camera_set_panorama_type(clientId, sceneId, (uint)type);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_compute_auto_viewplane", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_compute_auto_viewplane(uint clientId, uint sceneId);
		public static void camera_compute_auto_viewplane(uint clientId, uint sceneId)
		{
			cycles_camera_compute_auto_viewplane(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_update", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_update(uint clientId, uint sceneId);
		public static void camera_update(uint clientId, uint sceneId)
		{
			cycles_camera_update(clientId, sceneId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_fov", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fov(uint clientId, uint sceneId, float fov);
		public static void camera_set_fov(uint clientId, uint sceneId, float fov)
		{
			cycles_camera_set_fov(clientId, sceneId, fov);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_sensor_width", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_sensor_width(uint clientId, uint sceneId, float sensor_width);
		public static void camera_set_sensor_width(uint clientId, uint sceneId, float sensor_width)
		{
			cycles_camera_set_sensor_width(clientId, sceneId, sensor_width);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_sensor_height", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_sensor_height(uint clientId, uint sceneId, float sensor_height);
		public static void camera_set_sensor_height(uint clientId, uint sceneId, float sensor_height)
		{
			cycles_camera_set_sensor_height(clientId, sceneId, sensor_height);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_nearclip", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_nearclip(uint clientId, uint sceneId, float nearclip);
		public static void camera_set_nearclip(uint clientId, uint sceneId, float nearclip)
		{
			cycles_camera_set_nearclip(clientId, sceneId, nearclip);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_farclip", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_farclip(uint clientId, uint sceneId, float farclip);
		public static void camera_set_farclip(uint clientId, uint sceneId, float farclip)
		{
			cycles_camera_set_farclip(clientId, sceneId, farclip);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_aperturesize", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_aperturesize(uint clientId, uint sceneId, float aperturesize);
		public static void camera_set_aperturesize(uint clientId, uint sceneId, float aperturesize)
		{
			cycles_camera_set_aperturesize(clientId, sceneId, aperturesize);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_shuttertime", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_shuttertime(uint clientId, uint sceneId, float shuttertime);
		public static void camera_set_shuttertime(uint clientId, uint sceneId, float shuttertime)
		{
			cycles_camera_set_shuttertime(clientId, sceneId, shuttertime);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_focaldistance", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_focaldistance(uint clientId, uint sceneId, float focaldistance);
		public static void camera_set_focaldistance(uint clientId, uint sceneId, float focaldistance)
		{
			cycles_camera_set_focaldistance(clientId, sceneId, focaldistance);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_fisheye_fov", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fisheye_fov(uint clientId, uint sceneId, float fisheye_fov);
		public static void camera_set_fisheye_fov(uint clientId, uint sceneId, float fisheye_fov)
		{
			cycles_camera_set_fisheye_fov(clientId, sceneId, fisheye_fov);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_camera_set_fisheye_lens", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fisheye_lens(uint clientId, uint sceneId, float fisheye_lens);
		public static void camera_set_fisheye_lens(uint clientId, uint sceneId, float fisheye_lens)
		{
			cycles_camera_set_fisheye_lens(clientId, sceneId, fisheye_lens);
		}
#endregion

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

#region light

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_create_light", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_create_light(uint clientId, uint sceneId, uint lightShaderId);
		public static uint create_light(uint clientId, uint sceneId, uint lightShaderId)
		{
			return cycles_create_light(clientId, sceneId, lightShaderId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_type", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_type(uint clientId, uint sceneId, uint lightId, uint type);
		public static void light_set_type(uint clientId, uint sceneId, uint lightId, LightType type)
		{
			cycles_light_set_type(clientId, sceneId, lightId, (uint)type);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_spot_angle", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_spot_angle(uint clientId, uint sceneId, uint lightId, float spotAngle);
		public static void light_set_spot_angle(uint clientId, uint sceneId, uint lightId, float spotAngle)
		{
			cycles_light_set_spot_angle(clientId, sceneId, lightId, spotAngle);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_spot_smooth", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_spot_smooth(uint clientId, uint sceneId, uint lightId, float spotSmooth);
		public static void light_set_spot_smooth(uint clientId, uint sceneId, uint lightId, float spotSmooth)
		{
			cycles_light_set_spot_smooth(clientId, sceneId, lightId, spotSmooth);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_use_mis", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_use_mis(uint clientId, uint sceneId, uint lightId, uint useMis);
		public static void light_set_use_mis(uint clientId, uint sceneId, uint lightId, bool useMis)
		{
			cycles_light_set_use_mis(clientId, sceneId, lightId, (uint)(useMis ? 1 : 0));
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_sizeu", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_sizeu(uint clientId, uint sceneId, uint lightId, float sizeu);
		public static void light_set_sizeu(uint clientId, uint sceneId, uint lightId, float sizeu)
		{
			cycles_light_set_sizeu(clientId, sceneId, lightId, sizeu);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_sizev", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_sizev(uint clientId, uint sceneId, uint lightId, float sizev);
		public static void light_set_sizev(uint clientId, uint sceneId, uint lightId, float sizev)
		{
			cycles_light_set_sizev(clientId, sceneId, lightId, sizev);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_axisu", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_axisu(uint clientId, uint sceneId, uint lightId, float axisux, float axisuy, float axisuz);
		public static void light_set_axisu(uint clientId, uint sceneId, uint lightId, float axisux, float axisuy, float axisuz)
		{
			cycles_light_set_axisu(clientId, sceneId, lightId, axisux, axisuy, axisuz);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_axisv", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_axisv(uint clientId, uint sceneId, uint lightId, float axisvx, float axisvy, float axisvz);
		public static void light_set_axisv(uint clientId, uint sceneId, uint lightId, float axisvx, float axisvy, float axisvz)
		{
			cycles_light_set_axisv(clientId, sceneId, lightId, axisvx, axisvy, axisvz);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_size", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_size(uint clientId, uint sceneId, uint lightId, float size);
		public static void light_set_size(uint clientId, uint sceneId, uint lightId, float size)
		{
			cycles_light_set_size(clientId, sceneId, lightId, size);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_dir", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_dir(uint clientId, uint sceneId, uint lightId, float dirx, float diry, float dirz);
		public static void light_set_dir(uint clientId, uint sceneId, uint lightId, float dirx, float diry, float dirz)
		{
			cycles_light_set_dir(clientId, sceneId, lightId, dirx, diry, dirz);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_light_set_co", CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_co(uint clientId, uint sceneId, uint lightId, float cox, float coy, float coz);
		public static void light_set_co(uint clientId, uint sceneId, uint lightId, float cox, float coy, float coz)
		{
			cycles_light_set_co(clientId, sceneId, lightId, cox, coy, coz);
		}

#endregion
	}
}
