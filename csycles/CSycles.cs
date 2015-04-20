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
	public partial class CSycles
	{
#region misc
		private static bool g_ccycles_loaded;

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string filename);

		/// <summary>
		/// Load the ccycles DLL.
		/// </summary>
		private static void LoadCCycles()
		{
			if (g_ccycles_loaded) return;

			var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
			var ccycles_dll = System.IO.Path.Combine(path, "ccycles.dll");
			LoadLibrary(ccycles_dll);
			g_ccycles_loaded = true;
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

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_path_init", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_path_init(string path, string userPath);
		/// <summary>
		/// Set the paths for Cycles to look for pre-compiled or cached kernels, or kernel code
		/// 
		/// Note: to have any effect needs to be called before <c>initialise</c>.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="userPath"></param>
		public static void path_init([MarshalAsAttribute(UnmanagedType.LPStr)] string path, [MarshalAsAttribute(UnmanagedType.LPStr)] string userPath)
		{
			LoadCCycles();
			cycles_path_init(path, userPath);
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
		public static void set_logger(uint clientId, LoggerCallback loggerCb)
		{
			var intptr_delegate = Marshal.GetFunctionPointerForDelegate(loggerCb);
			cycles_set_logger(clientId, intptr_delegate);
		}

		public static void remove_logger(uint clientId)
		{
			cycles_set_logger(clientId, IntPtr.Zero);
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

	}
}
