using System;
using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
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
			return cycles_number_cuda_devices();
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
	}
}
