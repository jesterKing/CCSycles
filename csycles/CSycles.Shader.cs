using System.Runtime.InteropServices;

namespace ccl
{
	public partial class CSycles
	{
#region shaders
		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_create_shader", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_create_shader(uint clientId);
		public static uint create_shader(uint clientId)
		{
			return cycles_create_shader(clientId);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_scene_tag_shader", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_tag_shader(uint clientId, uint sceneId, uint shaderId);
		public static uint scene_tag_shader(uint clientId, uint sceneId, uint shaderId)
		{
			return cycles_scene_tag_shader(clientId, sceneId, shaderId);
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

		public const uint DEFAULT_SURFACE_SHADER = 0;
		public const uint DEFAULT_LIGHT_SHADER = 1;
		public const uint DEFAULT_BACKGROUND_SHADER = 2;
		public const uint DEFAULT_EMPTY_SHADER = 3;


		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_add_shader_node", CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_add_shader_node(uint clientId, uint shaderId, uint shnType);
		public static uint add_shader_node(uint clientId, uint shaderId, ShaderNodeType shnType)
		{
			return cycles_add_shader_node(clientId, shaderId, (uint) shnType);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_texmapping_set_transformation", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_transformation(uint clientId, uint shaderId, uint shadernodeId, uint shnType, uint transformType, float x, float y, float z);
		public static void shadernode_texmapping_set_transformation(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType, uint transformType, float x, float y, float z)
		{
			cycles_shadernode_texmapping_set_transformation(clientId, shaderId, shadernodeId, (uint) shnType, transformType, x, y, z);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_texmapping_set_mapping", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_mapping(uint clientId, uint shaderId, uint shadernodeId, uint shnType, uint mappingx, uint mappingy, uint mappingz);
		public static void shadernode_texmapping_set_mapping(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType, uint mappingx, uint mappingy, uint mappingz)
		{
			cycles_shadernode_texmapping_set_mapping(clientId, shaderId, shadernodeId, (uint) shnType, mappingx, mappingy, mappingz);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_texmapping_set_projection", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_projection(uint clientId, uint shaderId, uint shadernodeId, uint shnType, uint projection);
		public static void shadernode_texmapping_set_projection(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType, uint projection)
		{
			cycles_shadernode_texmapping_set_projection(clientId, shaderId, shadernodeId, (uint) shnType, projection);
		}

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_texmapping_set_type", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_type(uint clientId, uint shaderId, uint shadernodeId, uint shnType, uint type);
		public static void shadernode_texmapping_set_type(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType, uint type)
		{
			cycles_shadernode_texmapping_set_type(clientId, shaderId, shadernodeId, (uint) shnType, type);
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

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shadernode_set_member_vec4_at_index", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_vec4_at_index(uint clientId, uint shaderId, uint shadernodeId, uint shnType, string name, float x, float y, float z, float w, int index);
		public static void shadernode_set_member_vec4_at_index(uint clientId, uint shaderId, uint shadernodeId, ShaderNodeType shnType,
			[MarshalAs(UnmanagedType.LPStr)] string name, float x, float y, float z, float w, int index)
		{
			cycles_shadernode_set_member_vec4_at_index(clientId, shaderId, shadernodeId, (uint)shnType, name, x, y, z, w, index);
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

		[DllImport("ccycles.dll", SetLastError = false, EntryPoint = "cycles_shader_new_graph",
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_new_graph(uint clientId, uint shaderId);
		public static void shader_new_graph(uint clientId, uint shaderId)
		{
			cycles_shader_new_graph(clientId, shaderId);
		}


#endregion
	}
}
