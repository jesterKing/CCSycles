using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ccl;
using ccl.ShaderNodes;
using System.Drawing;
using System.IO;

//using Microsoft.CSharp;
//using System.CodeDom.Compiler;
//using System.Reflection;
//using System.Text;

using System.IO;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;


//using System.Runtime.InteropServices;

namespace HologramPrinter
{
    public interface IUI
    {
        string getSceneFileName();
        string getMaterialFileName();
        string getOutputFolderName();
        void SetText(string text);
    }

    public class Engine
    {
        /*
        [DllImport("Kernel32.dll")]
        static extern Boolean AllocConsole();

         * */
        #region meshdata
        static float[] vert_floats =
            {
                -1.0f, -1.0f, 0.0f, -1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f, -1.0f, 0.0f
                 
            };
        readonly static int[] nverts =
            {
                4
            };
        readonly static int[] vertex_indices =
            {
                0, 2, 1, 0, 3, 2
            };
        #endregion
        private const uint width = 1024;
        private const uint height = 1024;
        private const uint samples = 1;
        static Session Session { get; set; }
        static Client Client { get; set; }
        static Device Device { get; set; }
        static Scene Scene { get; set; }

        public static bool Initialised { get; set; }
        
        static IUI _iui;
        public static IUI Iui
        {
            set { Engine._iui = value; }
        }

        static public Shader create_some_setup_shader()
        {
            var some_setup = new Shader(Client, Shader.ShaderType.Material)
            {
                Name = "some_setup",
                UseMis = false,
                UseTransparentShadow = true,
                HeterogeneousVolume = false
            };


            var brick_texture = new BrickTexture();
            brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Color1.Value = new float4(0.800f, 0.800f, 0.800f);
            brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
            brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Scale.Value = 1.000f;
            brick_texture.ins.MortarSize.Value = 0.020f;
            brick_texture.ins.Bias.Value = 0.000f;
            brick_texture.ins.BrickWidth.Value = 0.500f;
            brick_texture.ins.RowHeight.Value = 0.250f;

            var checker_texture = new CheckerTexture();
            checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            checker_texture.ins.Color1.Value = new float4(0.000f, 0.004f, 0.800f);
            checker_texture.ins.Color2.Value = new float4(0.200f, 0.000f, 0.007f);
            checker_texture.ins.Scale.Value = 5.000f;

            var diffuse_bsdf = new DiffuseBsdfNode();
            diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
            diffuse_bsdf.ins.Roughness.Value = 0.000f;
            diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);

            var texture_coordinate = new TextureCoordinateNode();


            some_setup.AddNode(brick_texture);
            some_setup.AddNode(checker_texture);
            some_setup.AddNode(diffuse_bsdf);
            some_setup.AddNode(texture_coordinate);

            brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
            checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
            texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
            texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

            diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);

            some_setup.FinalizeGraph();

            return some_setup;
        }

        /*
        public Shader create_material_hologramu_shader()
        {
	        var material_hologramu = new Shader(Client, Shader.ShaderType.Material);

	        material_hologramu.Name = "material_hologramu";
	        material_hologramu.UseMis = false;
	        material_hologramu.UseTransparentShadow = true;
	        material_hologramu.HeterogeneousVolume = false;
            
	        var aa_add = new MathNode();
	        aa_add.ins.Value2.Value =  0.500f;
	        aa_add.Operation = MathNode.Operations.Add;

	        var aa_add_001 = new MathNode();
	        aa_add_001.ins.Value2.Value =  0.500f;
	        aa_add_001.Operation = MathNode.Operations.Add;

	        var aa_combinergb = new CombineRGBNode();
	        aa_combinergb.ins.B.Value =  0.000f;

	        var aa_divide = new MathNode();
	        aa_divide.Operation = MathNode.Operations.Divide;

	        var aa_divide_001 = new MathNode();
	        aa_divide_001.Operation = MathNode.Operations.Divide;

	        var aa_multiply = new MathNode();
	        aa_multiply.Operation = MathNode.Operations.Multiply;

	        var aa_multiply_001 = new MathNode();
	        aa_multiply_001.Operation = MathNode.Operations.Multiply;

	        var aa_round = new MathNode();
	        aa_round.ins.Value2.Value =  1.000f;
	        aa_round.Operation = MathNode.Operations.Round;

	        var aa_round_001 = new MathNode();
	        aa_round_001.ins.Value2.Value =  1.000f;
	        aa_round_001.Operation = MathNode.Operations.Round;

	        var aa_separatergb = new SeparateRGBNode();

	        var aa_substract = new MathNode();
	        aa_substract.ins.Value2.Value =  0.500f;
	        aa_substract.Operation = MathNode.Operations.Subtract;

	        var aa_substract_001 = new MathNode();
	        aa_substract_001.ins.Value2.Value =  0.500f;
	        aa_substract_001.Operation = MathNode.Operations.Subtract;

	        var generated_cocka_absolute = new MathNode();
	        generated_cocka_absolute.ins.Value2.Value =  1.000f;
	        generated_cocka_absolute.Operation = MathNode.Operations.Absolute;

	        var generated_cocka_add = new MathNode();
	        generated_cocka_add.Operation = MathNode.Operations.Add;

	        var generated_cocka_combinergb = new CombineRGBNode();

	        var generated_cocka_divide = new MathNode();
	        generated_cocka_divide.ins.Value2.Value =  256.000f;
	        generated_cocka_divide.Operation = MathNode.Operations.Divide;

	        var generated_cocka_divide_001 = new MathNode();
	        generated_cocka_divide_001.ins.Value2.Value =  256.000f;
	        generated_cocka_divide_001.Operation = MathNode.Operations.Divide;

	        var generated_cocka_greaterthan = new MathNode();
	        generated_cocka_greaterthan.ins.Value2.Value =  0.004f;
	        generated_cocka_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var generated_cocka_multiply = new MathNode();
	        generated_cocka_multiply.ins.Value2.Value =  2.000f;
	        generated_cocka_multiply.Operation = MathNode.Operations.Multiply;

	        var generated_cocka_multiply_001 = new MathNode();
	        generated_cocka_multiply_001.ins.Value2.Value =  2.000f;
	        generated_cocka_multiply_001.Operation = MathNode.Operations.Multiply;

	        var generated_cocka_multiply_002 = new MathNode();
	        generated_cocka_multiply_002.Operation = MathNode.Operations.Multiply;

	        var generated_cocka_multiply_003 = new MathNode();
	        generated_cocka_multiply_003.Operation = MathNode.Operations.Multiply;

	        var generated_cocka_power = new MathNode();
	        generated_cocka_power.ins.Value2.Value =  2.000f;
	        generated_cocka_power.Operation = MathNode.Operations.Power;

	        var generated_cocka_power_001 = new MathNode();
	        generated_cocka_power_001.ins.Value2.Value =  2.000f;
	        generated_cocka_power_001.Operation = MathNode.Operations.Power;

	        var generated_cocka_power_002 = new MathNode();
	        generated_cocka_power_002.ins.Value2.Value =  0.500f;
	        generated_cocka_power_002.Operation = MathNode.Operations.Power;
	        generated_cocka_power_002.UseClamp = true;

	        var generated_cocka_separatergb = new SeparateRGBNode();

	        var generated_cocka_substract = new MathNode();
	        generated_cocka_substract.Operation = MathNode.Operations.Subtract;

	        var generated_cocka_substract_001 = new MathNode();
	        generated_cocka_substract_001.Operation = MathNode.Operations.Subtract;

	        var generated_cocka_substract_002 = new MathNode();
	        generated_cocka_substract_002.ins.Value2.Value =  1.000f;
	        generated_cocka_substract_002.Operation = MathNode.Operations.Subtract;

	        var hologram_material_achro_absolute = new MathNode();
	        hologram_material_achro_absolute.ins.Value2.Value =  0.000f;
	        hologram_material_achro_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_achro_add = new MathNode();
	        hologram_material_achro_add.Operation = MathNode.Operations.Add;

	        var hologram_material_achro_add_001 = new MathNode();
	        hologram_material_achro_add_001.ins.Value1.Value =  0.000f;
	        hologram_material_achro_add_001.Operation = MathNode.Operations.Add;

	        var hologram_material_achro_add_002 = new MathNode();
	        hologram_material_achro_add_002.Operation = MathNode.Operations.Add;

	        var hologram_material_achro_divide = new MathNode();
	        hologram_material_achro_divide.ins.Value2.Value =  100.000f;
	        hologram_material_achro_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_achro_divide_001 = new MathNode();
	        hologram_material_achro_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_achro_divide_002 = new MathNode();
	        hologram_material_achro_divide_002.ins.Value1.Value =  1.000f;
	        hologram_material_achro_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_achro_divide_003 = new MathNode();
	        hologram_material_achro_divide_003.Operation = MathNode.Operations.Divide;

	        var hologram_material_achro_greaterthan = new MathNode();
	        hologram_material_achro_greaterthan.ins.Value2.Value =  0.770f;
	        hologram_material_achro_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_achro_lessthan = new MathNode();
	        hologram_material_achro_lessthan.ins.Value2.Value =  0.790f;
	        hologram_material_achro_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_achro_modulo = new MathNode();
	        hologram_material_achro_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_achro_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_achro_modulo_001 = new MathNode();
	        hologram_material_achro_modulo_001.ins.Value2.Value =  1.000f;
	        hologram_material_achro_modulo_001.Operation = MathNode.Operations.Modulo;

	        var hologram_material_achro_multiply = new MathNode();
	        hologram_material_achro_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_001 = new MathNode();
	        hologram_material_achro_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_002 = new MathNode();
	        hologram_material_achro_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_003 = new MathNode();
	        hologram_material_achro_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_004 = new MathNode();
	        hologram_material_achro_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_005 = new MathNode();
	        hologram_material_achro_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_multiply_006 = new MathNode();
	        hologram_material_achro_multiply_006.Operation = MathNode.Operations.Multiply;

	        var hologram_material_achro_noisetex = new NoiseTextureNode();
	        hologram_material_achro_noisetex.ins.Distortion.Value =  0.000f;

	        var hologram_material_achro_separatergb = new SeparateRGBNode();

	        var hologram_material_achro_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_achro_substract = new MathNode();
	        hologram_material_achro_substract.ins.Value1.Value =  1.000f;
	        hologram_material_achro_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_achro_substract_001 = new MathNode();
	        hologram_material_achro_substract_001.ins.Value2.Value =  1.000f;
	        hologram_material_achro_substract_001.Operation = MathNode.Operations.Subtract;

	        var hologram_material_achro_substract_002 = new MathNode();
	        hologram_material_achro_substract_002.Operation = MathNode.Operations.Subtract;

	        var hologram_material_achro_substract_003 = new MathNode();
	        hologram_material_achro_substract_003.Operation = MathNode.Operations.Subtract;

	        var hologram_material_achro_substract_004 = new MathNode();
	        hologram_material_achro_substract_004.Operation = MathNode.Operations.Subtract;

	        var hologram_material_axicon_checkertex = new CheckerTexture();
	        hologram_material_axicon_checkertex.ins.Color1.Value = new float4( 1.000f,  1.000f,  1.000f);
	        hologram_material_axicon_checkertex.ins.Color2.Value = new float4( 0.000f,  0.000f,  0.000f);

	        var hologram_material_axicon_greaterthan = new MathNode();
	        hologram_material_axicon_greaterthan.ins.Value2.Value =  0.150f;
	        hologram_material_axicon_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_axicon_lessthan = new MathNode();
	        hologram_material_axicon_lessthan.ins.Value2.Value =  0.160f;
	        hologram_material_axicon_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_axicon_mapping = new MappingNode();
	        hologram_material_axicon_mapping.vector_type = MappingNode.vector_types.POINT;

	        var hologram_material_axicon_multiply = new MathNode();
	        hologram_material_axicon_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_axicon_multiply_001 = new MathNode();
	        hologram_material_axicon_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_axicon_separatergb = new SeparateRGBNode();

	        var hologram_material_axicon_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_cocka_add = new MathNode();
	        hologram_material_cocka_add.Operation = MathNode.Operations.Add;

	        var hologram_material_cocka_add_001 = new MathNode();
	        hologram_material_cocka_add_001.Operation = MathNode.Operations.Add;

	        var hologram_material_cocka_add_002 = new MathNode();
	        hologram_material_cocka_add_002.ins.Value2.Value =  1.000f;
	        hologram_material_cocka_add_002.Operation = MathNode.Operations.Add;

	        var hologram_material_cocka_divide = new MathNode();
	        hologram_material_cocka_divide.ins.Value1.Value =  1.000f;
	        hologram_material_cocka_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_cocka_divide_001 = new MathNode();
	        hologram_material_cocka_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_cocka_divide_002 = new MathNode();
	        hologram_material_cocka_divide_002.ins.Value2.Value =  100.000f;
	        hologram_material_cocka_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_cocka_divide_003 = new MathNode();
	        hologram_material_cocka_divide_003.Operation = MathNode.Operations.Divide;

	        var hologram_material_cocka_greaterthan = new MathNode();
	        hologram_material_cocka_greaterthan.ins.Value2.Value =  0.385f;
	        hologram_material_cocka_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_cocka_greaterthan_001 = new MathNode();
	        hologram_material_cocka_greaterthan_001.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_cocka_lessthan = new MathNode();
	        hologram_material_cocka_lessthan.ins.Value2.Value =  0.395f;
	        hologram_material_cocka_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_cocka_modulo = new MathNode();
	        hologram_material_cocka_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_cocka_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_cocka_multiply = new MathNode();
	        hologram_material_cocka_multiply.ins.Value2.Value = -1.000f;
	        hologram_material_cocka_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_cocka_multiply_001 = new MathNode();
	        hologram_material_cocka_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_cocka_multiply_002 = new MathNode();
	        hologram_material_cocka_multiply_002.ins.Value2.Value =  2.560f;
	        hologram_material_cocka_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_cocka_multiply_003 = new MathNode();
	        hologram_material_cocka_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_cocka_multiply_004 = new MathNode();
	        hologram_material_cocka_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_cocka_power = new MathNode();
	        hologram_material_cocka_power.ins.Value2.Value =  2.000f;
	        hologram_material_cocka_power.Operation = MathNode.Operations.Power;

	        var hologram_material_cocka_power_001 = new MathNode();
	        hologram_material_cocka_power_001.ins.Value2.Value =  0.500f;
	        hologram_material_cocka_power_001.Operation = MathNode.Operations.Power;

	        var hologram_material_cocka_separatergb = new SeparateRGBNode();

	        var hologram_material_cocka_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_cocka_substract = new MathNode();
	        hologram_material_cocka_substract.ins.Value1.Value =  0.000f;
	        hologram_material_cocka_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_cocka_substract_001 = new MathNode();
	        hologram_material_cocka_substract_001.ins.Value1.Value =  1.000f;
	        hologram_material_cocka_substract_001.Operation = MathNode.Operations.Subtract;

	        var hologram_material_e_direct_absolute = new MathNode();
	        hologram_material_e_direct_absolute.ins.Value2.Value =  1.000f;
	        hologram_material_e_direct_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_e_direct_add = new MathNode();
	        hologram_material_e_direct_add.Operation = MathNode.Operations.Add;

	        var hologram_material_e_direct_cosine = new MathNode();
	        hologram_material_e_direct_cosine.ins.Value2.Value =  0.000f;
	        hologram_material_e_direct_cosine.Operation = MathNode.Operations.Cosine;

	        var hologram_material_e_direct_divide = new MathNode();
	        hologram_material_e_direct_divide.ins.Value1.Value =  180.000f;
	        hologram_material_e_direct_divide.ins.Value2.Value =  3.142f;
	        hologram_material_e_direct_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_divide_001 = new MathNode();
	        hologram_material_e_direct_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_divide_002 = new MathNode();
	        hologram_material_e_direct_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_divide_003 = new MathNode();
	        hologram_material_e_direct_divide_003.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_divide_004 = new MathNode();
	        hologram_material_e_direct_divide_004.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_divide_005 = new MathNode();
	        hologram_material_e_direct_divide_005.ins.Value2.Value =  100.000f;
	        hologram_material_e_direct_divide_005.Operation = MathNode.Operations.Divide;

	        var hologram_material_e_direct_greaterthan = new MathNode();
	        hologram_material_e_direct_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_e_direct_lessthan = new MathNode();
	        hologram_material_e_direct_lessthan.ins.Value2.Value =  0.001f;
	        hologram_material_e_direct_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_e_direct_modulo = new MathNode();
	        hologram_material_e_direct_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_e_direct_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_e_direct_multiply = new MathNode();
	        hologram_material_e_direct_multiply.ins.Value2.Value =  256.000f;
	        hologram_material_e_direct_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_multiply_001 = new MathNode();
	        hologram_material_e_direct_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_multiply_002 = new MathNode();
	        hologram_material_e_direct_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_multiply_003 = new MathNode();
	        hologram_material_e_direct_multiply_003.ins.Value2.Value =  1000.000f;
	        hologram_material_e_direct_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_multiply_004 = new MathNode();
	        hologram_material_e_direct_multiply_004.ins.Value2.Value =  2.560f;
	        hologram_material_e_direct_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_multiply_005 = new MathNode();
	        hologram_material_e_direct_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_e_direct_separatergb = new SeparateRGBNode();

	        var hologram_material_e_direct_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_e_direct_sine = new MathNode();
	        hologram_material_e_direct_sine.ins.Value2.Value =  0.000f;
	        hologram_material_e_direct_sine.Operation = MathNode.Operations.Sine;

	        var hologram_material_e_direct_substract = new MathNode();
	        hologram_material_e_direct_substract.ins.Value2.Value =  0.502f;
	        hologram_material_e_direct_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_e_direct_substract_001 = new MathNode();
	        hologram_material_e_direct_substract_001.Operation = MathNode.Operations.Subtract;

	        var hologram_material_e_direct_value = new ValueNode();
	        hologram_material_e_direct_value.Value = 90.000f;

	        var hologram_material_emission = new EmissionNode();
	        hologram_material_emission.ins.Strength.Value =  1.000f;

	        var hologram_material_gridhelper_absolute = new MathNode();
	        hologram_material_gridhelper_absolute.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_gridhelper_add = new MathNode();
	        hologram_material_gridhelper_add.Operation = MathNode.Operations.Add;

	        var hologram_material_gridhelper_add_001 = new MathNode();
	        hologram_material_gridhelper_add_001.Operation = MathNode.Operations.Add;

	        var hologram_material_gridhelper_add_002 = new MathNode();
	        hologram_material_gridhelper_add_002.Operation = MathNode.Operations.Add;

	        var hologram_material_gridhelper_combinergb = new CombineRGBNode();
	        hologram_material_gridhelper_combinergb.ins.B.Value =  0.000f;

	        var hologram_material_gridhelper_divide = new MathNode();
	        hologram_material_gridhelper_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_gridhelper_divide_001 = new MathNode();
	        hologram_material_gridhelper_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_gridhelper_greaterthan = new MathNode();
	        hologram_material_gridhelper_greaterthan.ins.Value2.Value =  0.990f;
	        hologram_material_gridhelper_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_greaterthan_001 = new MathNode();
	        hologram_material_gridhelper_greaterthan_001.ins.Value2.Value =  0.980f;
	        hologram_material_gridhelper_greaterthan_001.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_greaterthan_002 = new MathNode();
	        hologram_material_gridhelper_greaterthan_002.ins.Value2.Value =  0.500f;
	        hologram_material_gridhelper_greaterthan_002.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_greaterthan_003 = new MathNode();
	        hologram_material_gridhelper_greaterthan_003.ins.Value2.Value =  0.990f;
	        hologram_material_gridhelper_greaterthan_003.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_greaterthan_004 = new MathNode();
	        hologram_material_gridhelper_greaterthan_004.ins.Value2.Value =  0.980f;
	        hologram_material_gridhelper_greaterthan_004.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_greaterthan_005 = new MathNode();
	        hologram_material_gridhelper_greaterthan_005.ins.Value2.Value =  0.500f;
	        hologram_material_gridhelper_greaterthan_005.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_gridhelper_modulo = new MathNode();
	        hologram_material_gridhelper_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_modulo_001 = new MathNode();
	        hologram_material_gridhelper_modulo_001.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo_001.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_modulo_002 = new MathNode();
	        hologram_material_gridhelper_modulo_002.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo_002.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_modulo_003 = new MathNode();
	        hologram_material_gridhelper_modulo_003.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo_003.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_modulo_004 = new MathNode();
	        hologram_material_gridhelper_modulo_004.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo_004.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_modulo_005 = new MathNode();
	        hologram_material_gridhelper_modulo_005.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_modulo_005.Operation = MathNode.Operations.Modulo;

	        var hologram_material_gridhelper_multiply = new MathNode();
	        hologram_material_gridhelper_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_001 = new MathNode();
	        hologram_material_gridhelper_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_002 = new MathNode();
	        hologram_material_gridhelper_multiply_002.ins.Value2.Value =  10.000f;
	        hologram_material_gridhelper_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_003 = new MathNode();
	        hologram_material_gridhelper_multiply_003.ins.Value2.Value =  10.000f;
	        hologram_material_gridhelper_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_004 = new MathNode();
	        hologram_material_gridhelper_multiply_004.ins.Value2.Value =  5.000f;
	        hologram_material_gridhelper_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_005 = new MathNode();
	        hologram_material_gridhelper_multiply_005.ins.Value2.Value =  5.000f;
	        hologram_material_gridhelper_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_006 = new MathNode();
	        hologram_material_gridhelper_multiply_006.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_007 = new MathNode();
	        hologram_material_gridhelper_multiply_007.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_008 = new MathNode();
	        hologram_material_gridhelper_multiply_008.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_multiply_009 = new MathNode();
	        hologram_material_gridhelper_multiply_009.Operation = MathNode.Operations.Multiply;

	        var hologram_material_gridhelper_separatergb = new SeparateRGBNode();

	        var hologram_material_gridhelper_substract = new MathNode();
	        hologram_material_gridhelper_substract.ins.Value2.Value =  1.000f;
	        hologram_material_gridhelper_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_kinetic3d_absolute = new MathNode();
	        hologram_material_kinetic3d_absolute.ins.Value2.Value =  0.500f;
	        hologram_material_kinetic3d_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_kinetic3d_divide = new MathNode();
	        hologram_material_kinetic3d_divide.ins.Value2.Value =  57.296f;
	        hologram_material_kinetic3d_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_kinetic3d_divide_001 = new MathNode();
	        hologram_material_kinetic3d_divide_001.ins.Value2.Value =  100.000f;
	        hologram_material_kinetic3d_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_kinetic3d_divide_002 = new MathNode();
	        hologram_material_kinetic3d_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_kinetic3d_greaterthan = new MathNode();
	        hologram_material_kinetic3d_greaterthan.ins.Value2.Value =  0.996f;
	        hologram_material_kinetic3d_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_kinetic3d_greaterthan_001 = new MathNode();
	        hologram_material_kinetic3d_greaterthan_001.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_kinetic3d_modulo = new MathNode();
	        hologram_material_kinetic3d_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_kinetic3d_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_kinetic3d_multiply = new MathNode();
	        hologram_material_kinetic3d_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_kinetic3d_multiply_001 = new MathNode();
	        hologram_material_kinetic3d_multiply_001.ins.Value2.Value =  2.560f;
	        hologram_material_kinetic3d_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_kinetic3d_multiply_002 = new MathNode();
	        hologram_material_kinetic3d_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_kinetic3d_multiply_003 = new MathNode();
	        hologram_material_kinetic3d_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_kinetic3d_multiply_004 = new MathNode();
	        hologram_material_kinetic3d_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_kinetic3d_separatergb = new SeparateRGBNode();

	        var hologram_material_kinetic3d_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_kinetic3d_separatergb_002 = new SeparateRGBNode();

	        var hologram_material_kinetic3d_sine = new MathNode();
	        hologram_material_kinetic3d_sine.ins.Value2.Value =  0.000f;
	        hologram_material_kinetic3d_sine.Operation = MathNode.Operations.Sine;

	        var hologram_material_kinetic3d_substract = new MathNode();
	        hologram_material_kinetic3d_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_krizovamrizka_divide = new MathNode();
	        hologram_material_krizovamrizka_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_krizovamrizka_divide_001 = new MathNode();
	        hologram_material_krizovamrizka_divide_001.ins.Value2.Value =  100.000f;
	        hologram_material_krizovamrizka_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_krizovamrizka_divide_002 = new MathNode();
	        hologram_material_krizovamrizka_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_krizovamrizka_lessthan = new MathNode();
	        hologram_material_krizovamrizka_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_krizovamrizka_lessthan_001 = new MathNode();
	        hologram_material_krizovamrizka_lessthan_001.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_krizovamrizka_modulo = new MathNode();
	        hologram_material_krizovamrizka_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_krizovamrizka_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_krizovamrizka_modulo_001 = new MathNode();
	        hologram_material_krizovamrizka_modulo_001.ins.Value2.Value =  1.000f;
	        hologram_material_krizovamrizka_modulo_001.Operation = MathNode.Operations.Modulo;

	        var hologram_material_krizovamrizka_multiply = new MathNode();
	        hologram_material_krizovamrizka_multiply.ins.Value2.Value =  2.560f;
	        hologram_material_krizovamrizka_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_001 = new MathNode();
	        hologram_material_krizovamrizka_multiply_001.ins.Value2.Value =  1000.000f;
	        hologram_material_krizovamrizka_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_002 = new MathNode();
	        hologram_material_krizovamrizka_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_003 = new MathNode();
	        hologram_material_krizovamrizka_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_004 = new MathNode();
	        hologram_material_krizovamrizka_multiply_004.ins.Value2.Value =  1000.000f;
	        hologram_material_krizovamrizka_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_005 = new MathNode();
	        hologram_material_krizovamrizka_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_006 = new MathNode();
	        hologram_material_krizovamrizka_multiply_006.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_multiply_007 = new MathNode();
	        hologram_material_krizovamrizka_multiply_007.Operation = MathNode.Operations.Multiply;

	        var hologram_material_krizovamrizka_separatergb = new SeparateRGBNode();

	        var hologram_material_krizovamrizka_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_krizovamrizka_separatergb_002 = new SeparateRGBNode();

	        var hologram_material_mix = new MixNode();

	        var hologram_material_mix_001 = new MixNode();

	        var hologram_material_mix_002 = new MixNode();

	        var hologram_material_mix_003 = new MixNode();

	        var hologram_material_mix_004 = new MixNode();

	        var hologram_material_mix_005 = new MixNode();

	        var hologram_material_mix_006 = new MixNode();

	        var hologram_material_mix_007 = new MixNode();

	        var hologram_material_mix_008 = new MixNode();

	        var hologram_material_rgb = new ColorNode();
	        hologram_material_rgb.Value = new float4(0.608f, 0.608f, 0.608f);

	        var hologram_material_sterbina_absolute = new MathNode();
	        hologram_material_sterbina_absolute.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_sterbina_add = new MathNode();
	        hologram_material_sterbina_add.ins.Value2.Value =  0.000f;
	        hologram_material_sterbina_add.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_add_001 = new MathNode();
	        hologram_material_sterbina_add_001.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_add_002 = new MathNode();
	        hologram_material_sterbina_add_002.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_add_003 = new MathNode();
	        hologram_material_sterbina_add_003.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_add_004 = new MathNode();
	        hologram_material_sterbina_add_004.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_add_005 = new MathNode();
	        hologram_material_sterbina_add_005.Operation = MathNode.Operations.Add;

	        var hologram_material_sterbina_divide = new MathNode();
	        hologram_material_sterbina_divide.ins.Value2.Value =  57.296f;
	        hologram_material_sterbina_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_001 = new MathNode();
	        hologram_material_sterbina_divide_001.ins.Value1.Value =  1.000f;
	        hologram_material_sterbina_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_002 = new MathNode();
	        hologram_material_sterbina_divide_002.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_003 = new MathNode();
	        hologram_material_sterbina_divide_003.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_004 = new MathNode();
	        hologram_material_sterbina_divide_004.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_005 = new MathNode();
	        hologram_material_sterbina_divide_005.ins.Value2.Value =  100.000f;
	        hologram_material_sterbina_divide_005.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_divide_006 = new MathNode();
	        hologram_material_sterbina_divide_006.ins.Value2.Value =  100.000f;
	        hologram_material_sterbina_divide_006.Operation = MathNode.Operations.Divide;

	        var hologram_material_sterbina_greaterthan = new MathNode();
	        hologram_material_sterbina_greaterthan.ins.Value2.Value =  0.330f;
	        hologram_material_sterbina_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_sterbina_greaterthan_001 = new MathNode();
	        hologram_material_sterbina_greaterthan_001.ins.Value2.Value =  0.312f;
	        hologram_material_sterbina_greaterthan_001.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_sterbina_greaterthan_002 = new MathNode();
	        hologram_material_sterbina_greaterthan_002.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_sterbina_greaterthan_003 = new MathNode();
	        hologram_material_sterbina_greaterthan_003.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_sterbina_lessthan = new MathNode();
	        hologram_material_sterbina_lessthan.ins.Value2.Value =  0.335f;
	        hologram_material_sterbina_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_sterbina_lessthan_001 = new MathNode();
	        hologram_material_sterbina_lessthan_001.ins.Value2.Value =  0.314f;
	        hologram_material_sterbina_lessthan_001.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_sterbina_modulo = new MathNode();
	        hologram_material_sterbina_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_modulo_001 = new MathNode();
	        hologram_material_sterbina_modulo_001.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo_001.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_modulo_002 = new MathNode();
	        hologram_material_sterbina_modulo_002.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo_002.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_modulo_003 = new MathNode();
	        hologram_material_sterbina_modulo_003.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo_003.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_modulo_004 = new MathNode();
	        hologram_material_sterbina_modulo_004.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo_004.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_modulo_005 = new MathNode();
	        hologram_material_sterbina_modulo_005.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_modulo_005.Operation = MathNode.Operations.Modulo;

	        var hologram_material_sterbina_multiply = new MathNode();
	        hologram_material_sterbina_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_001 = new MathNode();
	        hologram_material_sterbina_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_002 = new MathNode();
	        hologram_material_sterbina_multiply_002.ins.Value2.Value =  256.000f;
	        hologram_material_sterbina_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_003 = new MathNode();
	        hologram_material_sterbina_multiply_003.ins.Value2.Value =  1000.000f;
	        hologram_material_sterbina_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_004 = new MathNode();
	        hologram_material_sterbina_multiply_004.ins.Value2.Value =  2.560f;
	        hologram_material_sterbina_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_005 = new MathNode();
	        hologram_material_sterbina_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_006 = new MathNode();
	        hologram_material_sterbina_multiply_006.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_007 = new MathNode();
	        hologram_material_sterbina_multiply_007.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_008 = new MathNode();
	        hologram_material_sterbina_multiply_008.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_009 = new MathNode();
	        hologram_material_sterbina_multiply_009.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_010 = new MathNode();
	        hologram_material_sterbina_multiply_010.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_011 = new MathNode();
	        hologram_material_sterbina_multiply_011.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_multiply_012 = new MathNode();
	        hologram_material_sterbina_multiply_012.Operation = MathNode.Operations.Multiply;

	        var hologram_material_sterbina_power = new MathNode();
	        hologram_material_sterbina_power.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_power.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_power_001 = new MathNode();
	        hologram_material_sterbina_power_001.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_power_001.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_power_002 = new MathNode();
	        hologram_material_sterbina_power_002.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_power_002.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_power_003 = new MathNode();
	        hologram_material_sterbina_power_003.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_power_003.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_power_004 = new MathNode();
	        hologram_material_sterbina_power_004.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_power_004.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_power_005 = new MathNode();
	        hologram_material_sterbina_power_005.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_power_005.Operation = MathNode.Operations.Power;

	        var hologram_material_sterbina_round = new MathNode();
	        hologram_material_sterbina_round.ins.Value2.Value =  1.000f;
	        hologram_material_sterbina_round.Operation = MathNode.Operations.Round;

	        var hologram_material_sterbina_separatergb = new SeparateRGBNode();

	        var hologram_material_sterbina_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_sterbina_sine = new MathNode();
	        hologram_material_sterbina_sine.ins.Value2.Value =  2.000f;
	        hologram_material_sterbina_sine.Operation = MathNode.Operations.Sine;

	        var hologram_material_sterbina_substract = new MathNode();
	        hologram_material_sterbina_substract.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_001 = new MathNode();
	        hologram_material_sterbina_substract_001.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_substract_001.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_002 = new MathNode();
	        hologram_material_sterbina_substract_002.ins.Value1.Value =  0.000f;
	        hologram_material_sterbina_substract_002.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_003 = new MathNode();
	        hologram_material_sterbina_substract_003.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_substract_003.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_004 = new MathNode();
	        hologram_material_sterbina_substract_004.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_005 = new MathNode();
	        hologram_material_sterbina_substract_005.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_substract_005.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_006 = new MathNode();
	        hologram_material_sterbina_substract_006.ins.Value1.Value =  0.000f;
	        hologram_material_sterbina_substract_006.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_007 = new MathNode();
	        hologram_material_sterbina_substract_007.ins.Value2.Value =  0.500f;
	        hologram_material_sterbina_substract_007.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_008 = new MathNode();
	        hologram_material_sterbina_substract_008.Operation = MathNode.Operations.Subtract;

	        var hologram_material_sterbina_substract_009 = new MathNode();
	        hologram_material_sterbina_substract_009.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_absolute = new MathNode();
	        hologram_material_whitekinetic_absolute.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_absolute.Operation = MathNode.Operations.Absolute;

	        var hologram_material_whitekinetic_add = new MathNode();
	        hologram_material_whitekinetic_add.Operation = MathNode.Operations.Add;

	        var hologram_material_whitekinetic_add_001 = new MathNode();
	        hologram_material_whitekinetic_add_001.Operation = MathNode.Operations.Add;

	        var hologram_material_whitekinetic_add_002 = new MathNode();
	        hologram_material_whitekinetic_add_002.Operation = MathNode.Operations.Add;

	        var hologram_material_whitekinetic_cosine = new MathNode();
	        hologram_material_whitekinetic_cosine.ins.Value2.Value =  0.000f;
	        hologram_material_whitekinetic_cosine.Operation = MathNode.Operations.Cosine;

	        var hologram_material_whitekinetic_divide = new MathNode();
	        hologram_material_whitekinetic_divide.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_divide.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_001 = new MathNode();
	        hologram_material_whitekinetic_divide_001.ins.Value1.Value =  180.000f;
	        hologram_material_whitekinetic_divide_001.ins.Value2.Value =  3.142f;
	        hologram_material_whitekinetic_divide_001.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_002 = new MathNode();
	        hologram_material_whitekinetic_divide_002.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_003 = new MathNode();
	        hologram_material_whitekinetic_divide_003.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_004 = new MathNode();
	        hologram_material_whitekinetic_divide_004.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_005 = new MathNode();
	        hologram_material_whitekinetic_divide_005.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_006 = new MathNode();
	        hologram_material_whitekinetic_divide_006.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_007 = new MathNode();
	        hologram_material_whitekinetic_divide_007.ins.Value1.Value =  1.000f;
	        hologram_material_whitekinetic_divide_007.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_divide_008 = new MathNode();
	        hologram_material_whitekinetic_divide_008.Operation = MathNode.Operations.Divide;

	        var hologram_material_whitekinetic_greaterthan = new MathNode();
	        hologram_material_whitekinetic_greaterthan.ins.Value2.Value =  0.038f;
	        hologram_material_whitekinetic_greaterthan.Operation = MathNode.Operations.Greater_Than;

	        var hologram_material_whitekinetic_lessthan = new MathNode();
	        hologram_material_whitekinetic_lessthan.ins.Value2.Value =  0.040f;
	        hologram_material_whitekinetic_lessthan.Operation = MathNode.Operations.Less_Than;

	        var hologram_material_whitekinetic_modulo = new MathNode();
	        hologram_material_whitekinetic_modulo.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_modulo.Operation = MathNode.Operations.Modulo;

	        var hologram_material_whitekinetic_modulo_001 = new MathNode();
	        hologram_material_whitekinetic_modulo_001.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_modulo_001.Operation = MathNode.Operations.Modulo;

	        var hologram_material_whitekinetic_multiply = new MathNode();
	        hologram_material_whitekinetic_multiply.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_001 = new MathNode();
	        hologram_material_whitekinetic_multiply_001.ins.Value2.Value =  256.000f;
	        hologram_material_whitekinetic_multiply_001.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_002 = new MathNode();
	        hologram_material_whitekinetic_multiply_002.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_003 = new MathNode();
	        hologram_material_whitekinetic_multiply_003.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_004 = new MathNode();
	        hologram_material_whitekinetic_multiply_004.ins.Value2.Value =  1000.000f;
	        hologram_material_whitekinetic_multiply_004.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_005 = new MathNode();
	        hologram_material_whitekinetic_multiply_005.ins.Value2.Value =  2.560f;
	        hologram_material_whitekinetic_multiply_005.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_006 = new MathNode();
	        hologram_material_whitekinetic_multiply_006.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_007 = new MathNode();
	        hologram_material_whitekinetic_multiply_007.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_008 = new MathNode();
	        hologram_material_whitekinetic_multiply_008.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_009 = new MathNode();
	        hologram_material_whitekinetic_multiply_009.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_010 = new MathNode();
	        hologram_material_whitekinetic_multiply_010.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_multiply_011 = new MathNode();
	        hologram_material_whitekinetic_multiply_011.Operation = MathNode.Operations.Multiply;

	        var hologram_material_whitekinetic_noisetex = new NoiseTextureNode();
	        hologram_material_whitekinetic_noisetex.ins.Distortion.Value =  0.000f;

	        var hologram_material_whitekinetic_separatergb = new SeparateRGBNode();

	        var hologram_material_whitekinetic_separatergb_001 = new SeparateRGBNode();

	        var hologram_material_whitekinetic_sine = new MathNode();
	        hologram_material_whitekinetic_sine.ins.Value2.Value =  0.000f;
	        hologram_material_whitekinetic_sine.Operation = MathNode.Operations.Sine;

	        var hologram_material_whitekinetic_substract = new MathNode();
	        hologram_material_whitekinetic_substract.ins.Value2.Value =  0.502f;
	        hologram_material_whitekinetic_substract.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_001 = new MathNode();
	        hologram_material_whitekinetic_substract_001.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_002 = new MathNode();
	        hologram_material_whitekinetic_substract_002.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_substract_002.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_003 = new MathNode();
	        hologram_material_whitekinetic_substract_003.ins.Value1.Value =  1.000f;
	        hologram_material_whitekinetic_substract_003.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_004 = new MathNode();
	        hologram_material_whitekinetic_substract_004.ins.Value2.Value =  1.000f;
	        hologram_material_whitekinetic_substract_004.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_005 = new MathNode();
	        hologram_material_whitekinetic_substract_005.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_substract_006 = new MathNode();
	        hologram_material_whitekinetic_substract_006.Operation = MathNode.Operations.Subtract;

	        var hologram_material_whitekinetic_value = new ValueNode();
	        hologram_material_whitekinetic_value.Value = 90.000f;

	        var image_texture_with_aa = new ImageTextureNode();

	        var image_texture_without_aa = new ImageTextureNode();

	        var input = new TextureCoordinateNode();

	        var mapping = new MappingNode();
	        mapping.vector_type = MappingNode.vector_types.TEXTURE;

	        var math_014 = new MathNode();
	        math_014.ins.Value2.Value =  0.240f;
	        math_014.Operation = MathNode.Operations.Less_Than;

	        var math_015 = new MathNode();
	        math_015.ins.Value2.Value =  0.230f;
	        math_015.Operation = MathNode.Operations.Greater_Than;

	        var math_016 = new MathNode();
	        math_016.Operation = MathNode.Operations.Multiply;




            var aa_input_RozmerbitmapyPrechodu = new ValueNode();
            aa_input_RozmerbitmapyPrechodu.Value = 7500.0f;



            var generated_cocka_input_HodnotaGkanalupro = new ValueNode();
            generated_cocka_input_HodnotaGkanalupro.Value = 100.0f;

            var generated_cocka_input_KoeficientPeriody0_256 = new ValueNode();
            generated_cocka_input_KoeficientPeriody0_256.Value = 100.0f;

            var generated_cocka_input_StredX = new ValueNode();
            generated_cocka_input_StredX.Value = 0.5f;

            var generated_cocka_input_StredY = new ValueNode();
            generated_cocka_input_StredY.Value = 0.5f;



            var hologram_material_input_CameraTextureCoordinates = new ValueNode();
            hologram_material_input_CameraTextureCoordinates.Value = 0.0f;
            
            var hologram_material_input_Data_ACHROInverzesmeruschodu = new ValueNode();
            hologram_material_input_Data_ACHROInverzesmeruschodu.Value = 0.0f;
            
            var hologram_material_input_Data_ACHRONoiseDetail = new ValueNode();
            hologram_material_input_Data_ACHRONoiseDetail.Value = 0.0f;
            
            var hologram_material_input_Data_ACHRONoiseScale = new ValueNode();
            hologram_material_input_Data_ACHRONoiseScale.Value = 0.0f;
            
            var hologram_material_input_Data_ACHRONoiseStrength = new ValueNode();
            hologram_material_input_Data_ACHRONoiseStrength.Value = 0.0f;
                        
            var hologram_material_input_Data_ACHROPoceturovni = new ValueNode();
            hologram_material_input_Data_ACHROPoceturovni.Value = 0.0f;
            
            var hologram_material_input_Data_ACHROScale = new ValueNode();
            hologram_material_input_Data_ACHROScale.Value = 0.0f;

            var hologram_material_input_Data_AXICONScale = new ValueNode();
            hologram_material_input_Data_AXICONScale.Value = 0.0f;
            
            var hologram_material_input_Data_BILA_Pocetschodu = new ValueNode();
            hologram_material_input_Data_BILA_Pocetschodu.Value = 0.0f;
            
            var hologram_material_input_Data_BILA_Scalenasobekperiody = new ValueNode();
            hologram_material_input_Data_BILA_Scalenasobekperiody.Value = 0.0f;
            
            var hologram_material_input_Data_BILANoiseDetail = new ValueNode();
            hologram_material_input_Data_BILANoiseDetail.Value = 0.0f;
            
            var hologram_material_input_Data_BILANoiseScale = new ValueNode();
            hologram_material_input_Data_BILANoiseScale.Value = 0.0f;
            
            var hologram_material_input_Data_BILANoiseStrength = new ValueNode();
            hologram_material_input_Data_BILANoiseStrength.Value = 0.0f;
            
            var hologram_material_input_Data_COCKAKoeficientplochosti = new ValueNode();
            hologram_material_input_Data_COCKAKoeficientplochosti.Value = 0.0f;
            
            var hologram_material_input_Data_COCKAScalemrizky = new ValueNode();
            hologram_material_input_Data_COCKAScalemrizky.Value = 0.0f;

            
            var hologram_material_input_Data_EDIRECTScale = new ValueNode();
            hologram_material_input_Data_EDIRECTScale.Value = 0.0f;
            
            var hologram_material_input_Data_KINETIC3D_KOMPENZACEkolikratprvekdorozmeruholo = new ValueNode();
            hologram_material_input_Data_KINETIC3D_KOMPENZACEkolikratprvekdorozmeruholo.Value = 0.0f;
            
            var hologram_material_input_Data_KINETIC3DScale = new ValueNode();
            hologram_material_input_Data_KINETIC3DScale.Value = 0.0f;
            
            var hologram_material_input_Data_KINETIC3DUhelmrizkyvestupnich0 = new ValueNode();
            hologram_material_input_Data_KINETIC3DUhelmrizkyvestupnich0.Value = 0.0f;
            
            var hologram_material_input_Data_KINETIC3DZMapHeight = new ValueNode();
            hologram_material_input_Data_KINETIC3DZMapHeight.Value = 0.0f;
            
            var hologram_material_input_Data_Pomercernekubilevprocentech = new ValueNode();
            hologram_material_input_Data_Pomercernekubilevprocentech.Value = 0.0f;
            
            var hologram_material_input_Data_STERBINAXperioda = new ValueNode();
            hologram_material_input_Data_STERBINAXperioda.Value = 0.0f;
            
            var hologram_material_input_GRIDAlpha = new ValueNode();
            hologram_material_input_GRIDAlpha.Value = 0.0f;
            
            var hologram_material_input_GRIDScale = new ValueNode();
            hologram_material_input_GRIDScale.Value = 0.0f;
            
            var hologram_material_input_GeneratedTextureCoordinates = new ValueNode();
            hologram_material_input_GeneratedTextureCoordinates.Value = 0.0f;
            
            var hologram_material_input_HologramRozmerX = new ValueNode();
            hologram_material_input_HologramRozmerX.Value = 0.0f;
            
            var hologram_material_input_HologramRozmerY = new ValueNode();
            hologram_material_input_HologramRozmerY.Value = 0.0f;
            
            var hologram_material_input_MASTER = new ColorNode();
            hologram_material_input_MASTER.Value = new float4(0.608f, 0.608f, 0.608f);

            var hologram_material_input_MASTERAA = new ColorNode();
            hologram_material_input_MASTERAA.Value = new float4(0.608f, 0.608f, 0.608f);
            
            
            
            var hologram_material_krizovamrizka_input_provetsinupripadu1 = new ValueNode();
            hologram_material_krizovamrizka_input_provetsinupripadu1.Value = 0.0f;
            
            


            material_hologramu.AddNode(aa_input_RozmerbitmapyPrechodu);

            material_hologramu.AddNode(generated_cocka_input_HodnotaGkanalupro);
            material_hologramu.AddNode(generated_cocka_input_KoeficientPeriody0_256);
            material_hologramu.AddNode(generated_cocka_input_StredX);
            material_hologramu.AddNode(generated_cocka_input_StredY);

            material_hologramu.AddNode(hologram_material_input_CameraTextureCoordinates);
            material_hologramu.AddNode(hologram_material_input_Data_ACHROInverzesmeruschodu);
            material_hologramu.AddNode(hologram_material_input_Data_ACHRONoiseDetail);
            material_hologramu.AddNode(hologram_material_input_Data_ACHRONoiseScale);
            material_hologramu.AddNode(hologram_material_input_Data_ACHRONoiseStrength);
            material_hologramu.AddNode(hologram_material_input_Data_ACHROPoceturovni);
            material_hologramu.AddNode(hologram_material_input_Data_ACHROScale);
            material_hologramu.AddNode(hologram_material_input_Data_AXICONScale);
            material_hologramu.AddNode(hologram_material_input_Data_BILA_Pocetschodu);
            material_hologramu.AddNode(hologram_material_input_Data_BILA_Scalenasobekperiody);
            material_hologramu.AddNode(hologram_material_input_Data_BILANoiseDetail);
            material_hologramu.AddNode(hologram_material_input_Data_BILANoiseScale);
            material_hologramu.AddNode(hologram_material_input_Data_BILANoiseStrength);
            material_hologramu.AddNode(hologram_material_input_Data_COCKAKoeficientplochosti);
            material_hologramu.AddNode(hologram_material_input_Data_COCKAScalemrizky);

            material_hologramu.AddNode(hologram_material_input_Data_EDIRECTScale);
            material_hologramu.AddNode(hologram_material_input_Data_KINETIC3D_KOMPENZACEkolikratprvekdorozmeruholo);
            material_hologramu.AddNode(hologram_material_input_Data_KINETIC3DScale);
            material_hologramu.AddNode(hologram_material_input_Data_KINETIC3DUhelmrizkyvestupnich0);
            material_hologramu.AddNode(hologram_material_input_Data_KINETIC3DZMapHeight);
            material_hologramu.AddNode(hologram_material_input_Data_Pomercernekubilevprocentech);
            material_hologramu.AddNode(hologram_material_input_Data_STERBINAXperioda);
            material_hologramu.AddNode(hologram_material_input_GRIDAlpha);
            material_hologramu.AddNode(hologram_material_input_GRIDScale);
            material_hologramu.AddNode(hologram_material_input_GeneratedTextureCoordinates);
            material_hologramu.AddNode(hologram_material_input_HologramRozmerX);
            material_hologramu.AddNode(hologram_material_input_HologramRozmerY);
            material_hologramu.AddNode(hologram_material_input_MASTER);
            material_hologramu.AddNode(hologram_material_input_MASTERAA);

            material_hologramu.AddNode(hologram_material_krizovamrizka_input_provetsinupripadu1);
            
	        material_hologramu.AddNode(aa_add);
	        material_hologramu.AddNode(aa_add_001);
	        material_hologramu.AddNode(aa_combinergb);
	        material_hologramu.AddNode(aa_divide);
	        material_hologramu.AddNode(aa_divide_001);
	        material_hologramu.AddNode(aa_multiply);
	        material_hologramu.AddNode(aa_multiply_001);
	        material_hologramu.AddNode(aa_round);
	        material_hologramu.AddNode(aa_round_001);
	        material_hologramu.AddNode(aa_separatergb);
	        material_hologramu.AddNode(aa_substract);
	        material_hologramu.AddNode(aa_substract_001);
	        material_hologramu.AddNode(generated_cocka_absolute);
	        material_hologramu.AddNode(generated_cocka_add);
	        material_hologramu.AddNode(generated_cocka_combinergb);
	        material_hologramu.AddNode(generated_cocka_divide);
	        material_hologramu.AddNode(generated_cocka_divide_001);
	        material_hologramu.AddNode(generated_cocka_greaterthan);
	        material_hologramu.AddNode(generated_cocka_multiply);
	        material_hologramu.AddNode(generated_cocka_multiply_001);
	        material_hologramu.AddNode(generated_cocka_multiply_002);
	        material_hologramu.AddNode(generated_cocka_multiply_003);
	        material_hologramu.AddNode(generated_cocka_power);
	        material_hologramu.AddNode(generated_cocka_power_001);
	        material_hologramu.AddNode(generated_cocka_power_002);
	        material_hologramu.AddNode(generated_cocka_separatergb);
	        material_hologramu.AddNode(generated_cocka_substract);
	        material_hologramu.AddNode(generated_cocka_substract_001);
	        material_hologramu.AddNode(generated_cocka_substract_002);
	        material_hologramu.AddNode(hologram_material_achro_absolute);
	        material_hologramu.AddNode(hologram_material_achro_add);
	        material_hologramu.AddNode(hologram_material_achro_add_001);
	        material_hologramu.AddNode(hologram_material_achro_add_002);
	        material_hologramu.AddNode(hologram_material_achro_divide);
	        material_hologramu.AddNode(hologram_material_achro_divide_001);
	        material_hologramu.AddNode(hologram_material_achro_divide_002);
	        material_hologramu.AddNode(hologram_material_achro_divide_003);
	        material_hologramu.AddNode(hologram_material_achro_greaterthan);
	        material_hologramu.AddNode(hologram_material_achro_lessthan);
	        material_hologramu.AddNode(hologram_material_achro_modulo);
	        material_hologramu.AddNode(hologram_material_achro_modulo_001);
	        material_hologramu.AddNode(hologram_material_achro_multiply);
	        material_hologramu.AddNode(hologram_material_achro_multiply_001);
	        material_hologramu.AddNode(hologram_material_achro_multiply_002);
	        material_hologramu.AddNode(hologram_material_achro_multiply_003);
	        material_hologramu.AddNode(hologram_material_achro_multiply_004);
	        material_hologramu.AddNode(hologram_material_achro_multiply_005);
	        material_hologramu.AddNode(hologram_material_achro_multiply_006);
	        material_hologramu.AddNode(hologram_material_achro_noisetex);
	        material_hologramu.AddNode(hologram_material_achro_separatergb);
	        material_hologramu.AddNode(hologram_material_achro_separatergb_001);
	        material_hologramu.AddNode(hologram_material_achro_substract);
	        material_hologramu.AddNode(hologram_material_achro_substract_001);
	        material_hologramu.AddNode(hologram_material_achro_substract_002);
	        material_hologramu.AddNode(hologram_material_achro_substract_003);
	        material_hologramu.AddNode(hologram_material_achro_substract_004);
	        material_hologramu.AddNode(hologram_material_axicon_checkertex);
	        material_hologramu.AddNode(hologram_material_axicon_greaterthan);
	        material_hologramu.AddNode(hologram_material_axicon_lessthan);
	        material_hologramu.AddNode(hologram_material_axicon_mapping);
	        material_hologramu.AddNode(hologram_material_axicon_multiply);
	        material_hologramu.AddNode(hologram_material_axicon_multiply_001);
	        material_hologramu.AddNode(hologram_material_axicon_separatergb);
	        material_hologramu.AddNode(hologram_material_axicon_separatergb_001);
	        material_hologramu.AddNode(hologram_material_cocka_add);
	        material_hologramu.AddNode(hologram_material_cocka_add_001);
	        material_hologramu.AddNode(hologram_material_cocka_add_002);
	        material_hologramu.AddNode(hologram_material_cocka_divide);
	        material_hologramu.AddNode(hologram_material_cocka_divide_001);
	        material_hologramu.AddNode(hologram_material_cocka_divide_002);
	        material_hologramu.AddNode(hologram_material_cocka_divide_003);
	        material_hologramu.AddNode(hologram_material_cocka_greaterthan);
	        material_hologramu.AddNode(hologram_material_cocka_greaterthan_001);
	        material_hologramu.AddNode(hologram_material_cocka_lessthan);
	        material_hologramu.AddNode(hologram_material_cocka_modulo);
	        material_hologramu.AddNode(hologram_material_cocka_multiply);
	        material_hologramu.AddNode(hologram_material_cocka_multiply_001);
	        material_hologramu.AddNode(hologram_material_cocka_multiply_002);
	        material_hologramu.AddNode(hologram_material_cocka_multiply_003);
	        material_hologramu.AddNode(hologram_material_cocka_multiply_004);
	        material_hologramu.AddNode(hologram_material_cocka_power);
	        material_hologramu.AddNode(hologram_material_cocka_power_001);
	        material_hologramu.AddNode(hologram_material_cocka_separatergb);
	        material_hologramu.AddNode(hologram_material_cocka_separatergb_001);
	        material_hologramu.AddNode(hologram_material_cocka_substract);
	        material_hologramu.AddNode(hologram_material_cocka_substract_001);
	        material_hologramu.AddNode(hologram_material_e_direct_absolute);
	        material_hologramu.AddNode(hologram_material_e_direct_add);
	        material_hologramu.AddNode(hologram_material_e_direct_cosine);
	        material_hologramu.AddNode(hologram_material_e_direct_divide);
	        material_hologramu.AddNode(hologram_material_e_direct_divide_001);
	        material_hologramu.AddNode(hologram_material_e_direct_divide_002);
	        material_hologramu.AddNode(hologram_material_e_direct_divide_003);
	        material_hologramu.AddNode(hologram_material_e_direct_divide_004);
	        material_hologramu.AddNode(hologram_material_e_direct_divide_005);
	        material_hologramu.AddNode(hologram_material_e_direct_greaterthan);
	        material_hologramu.AddNode(hologram_material_e_direct_lessthan);
	        material_hologramu.AddNode(hologram_material_e_direct_modulo);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply_001);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply_002);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply_003);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply_004);
	        material_hologramu.AddNode(hologram_material_e_direct_multiply_005);
	        material_hologramu.AddNode(hologram_material_e_direct_separatergb);
	        material_hologramu.AddNode(hologram_material_e_direct_separatergb_001);
	        material_hologramu.AddNode(hologram_material_e_direct_sine);
	        material_hologramu.AddNode(hologram_material_e_direct_substract);
	        material_hologramu.AddNode(hologram_material_e_direct_substract_001);
	        material_hologramu.AddNode(hologram_material_e_direct_value);
	        material_hologramu.AddNode(hologram_material_emission);
	        material_hologramu.AddNode(hologram_material_gridhelper_absolute);
	        material_hologramu.AddNode(hologram_material_gridhelper_add);
	        material_hologramu.AddNode(hologram_material_gridhelper_add_001);
	        material_hologramu.AddNode(hologram_material_gridhelper_add_002);
	        material_hologramu.AddNode(hologram_material_gridhelper_combinergb);
	        material_hologramu.AddNode(hologram_material_gridhelper_divide);
	        material_hologramu.AddNode(hologram_material_gridhelper_divide_001);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan_001);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan_002);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan_003);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan_004);
	        material_hologramu.AddNode(hologram_material_gridhelper_greaterthan_005);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo_001);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo_002);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo_003);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo_004);
	        material_hologramu.AddNode(hologram_material_gridhelper_modulo_005);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_001);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_002);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_003);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_004);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_005);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_006);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_007);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_008);
	        material_hologramu.AddNode(hologram_material_gridhelper_multiply_009);
	        material_hologramu.AddNode(hologram_material_gridhelper_separatergb);
	        material_hologramu.AddNode(hologram_material_gridhelper_substract);
	        material_hologramu.AddNode(hologram_material_kinetic3d_absolute);
	        material_hologramu.AddNode(hologram_material_kinetic3d_divide);
	        material_hologramu.AddNode(hologram_material_kinetic3d_divide_001);
	        material_hologramu.AddNode(hologram_material_kinetic3d_divide_002);
	        material_hologramu.AddNode(hologram_material_kinetic3d_greaterthan);
	        material_hologramu.AddNode(hologram_material_kinetic3d_greaterthan_001);
	        material_hologramu.AddNode(hologram_material_kinetic3d_modulo);
	        material_hologramu.AddNode(hologram_material_kinetic3d_multiply);
	        material_hologramu.AddNode(hologram_material_kinetic3d_multiply_001);
	        material_hologramu.AddNode(hologram_material_kinetic3d_multiply_002);
	        material_hologramu.AddNode(hologram_material_kinetic3d_multiply_003);
	        material_hologramu.AddNode(hologram_material_kinetic3d_multiply_004);
	        material_hologramu.AddNode(hologram_material_kinetic3d_separatergb);
	        material_hologramu.AddNode(hologram_material_kinetic3d_separatergb_001);
	        material_hologramu.AddNode(hologram_material_kinetic3d_separatergb_002);
	        material_hologramu.AddNode(hologram_material_kinetic3d_sine);
	        material_hologramu.AddNode(hologram_material_kinetic3d_substract);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_divide);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_divide_001);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_divide_002);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_lessthan);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_lessthan_001);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_modulo);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_modulo_001);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_001);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_002);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_003);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_004);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_005);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_006);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_multiply_007);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_separatergb);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_separatergb_001);
	        material_hologramu.AddNode(hologram_material_krizovamrizka_separatergb_002);
	        material_hologramu.AddNode(hologram_material_mix);
	        material_hologramu.AddNode(hologram_material_mix_001);
	        material_hologramu.AddNode(hologram_material_mix_002);
	        material_hologramu.AddNode(hologram_material_mix_003);
	        material_hologramu.AddNode(hologram_material_mix_004);
	        material_hologramu.AddNode(hologram_material_mix_005);
	        material_hologramu.AddNode(hologram_material_mix_006);
	        material_hologramu.AddNode(hologram_material_mix_007);
	        material_hologramu.AddNode(hologram_material_mix_008);
	        material_hologramu.AddNode(hologram_material_rgb);
	        material_hologramu.AddNode(hologram_material_sterbina_absolute);
	        material_hologramu.AddNode(hologram_material_sterbina_add);
	        material_hologramu.AddNode(hologram_material_sterbina_add_001);
	        material_hologramu.AddNode(hologram_material_sterbina_add_002);
	        material_hologramu.AddNode(hologram_material_sterbina_add_003);
	        material_hologramu.AddNode(hologram_material_sterbina_add_004);
	        material_hologramu.AddNode(hologram_material_sterbina_add_005);
	        material_hologramu.AddNode(hologram_material_sterbina_divide);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_001);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_002);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_003);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_004);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_005);
	        material_hologramu.AddNode(hologram_material_sterbina_divide_006);
	        material_hologramu.AddNode(hologram_material_sterbina_greaterthan);
	        material_hologramu.AddNode(hologram_material_sterbina_greaterthan_001);
	        material_hologramu.AddNode(hologram_material_sterbina_greaterthan_002);
	        material_hologramu.AddNode(hologram_material_sterbina_greaterthan_003);
	        material_hologramu.AddNode(hologram_material_sterbina_lessthan);
	        material_hologramu.AddNode(hologram_material_sterbina_lessthan_001);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo_001);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo_002);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo_003);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo_004);
	        material_hologramu.AddNode(hologram_material_sterbina_modulo_005);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_001);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_002);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_003);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_004);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_005);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_006);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_007);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_008);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_009);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_010);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_011);
	        material_hologramu.AddNode(hologram_material_sterbina_multiply_012);
	        material_hologramu.AddNode(hologram_material_sterbina_power);
	        material_hologramu.AddNode(hologram_material_sterbina_power_001);
	        material_hologramu.AddNode(hologram_material_sterbina_power_002);
	        material_hologramu.AddNode(hologram_material_sterbina_power_003);
	        material_hologramu.AddNode(hologram_material_sterbina_power_004);
	        material_hologramu.AddNode(hologram_material_sterbina_power_005);
	        material_hologramu.AddNode(hologram_material_sterbina_round);
	        material_hologramu.AddNode(hologram_material_sterbina_separatergb);
	        material_hologramu.AddNode(hologram_material_sterbina_separatergb_001);
	        material_hologramu.AddNode(hologram_material_sterbina_sine);
	        material_hologramu.AddNode(hologram_material_sterbina_substract);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_001);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_002);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_003);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_004);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_005);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_006);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_007);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_008);
	        material_hologramu.AddNode(hologram_material_sterbina_substract_009);
	        material_hologramu.AddNode(hologram_material_whitekinetic_absolute);
	        material_hologramu.AddNode(hologram_material_whitekinetic_add);
	        material_hologramu.AddNode(hologram_material_whitekinetic_add_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_add_002);
	        material_hologramu.AddNode(hologram_material_whitekinetic_cosine);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_002);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_003);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_004);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_005);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_006);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_007);
	        material_hologramu.AddNode(hologram_material_whitekinetic_divide_008);
	        material_hologramu.AddNode(hologram_material_whitekinetic_greaterthan);
	        material_hologramu.AddNode(hologram_material_whitekinetic_lessthan);
	        material_hologramu.AddNode(hologram_material_whitekinetic_modulo);
	        material_hologramu.AddNode(hologram_material_whitekinetic_modulo_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_002);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_003);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_004);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_005);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_006);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_007);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_008);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_009);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_010);
	        material_hologramu.AddNode(hologram_material_whitekinetic_multiply_011);
	        material_hologramu.AddNode(hologram_material_whitekinetic_noisetex);
	        material_hologramu.AddNode(hologram_material_whitekinetic_separatergb);
	        material_hologramu.AddNode(hologram_material_whitekinetic_separatergb_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_sine);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_001);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_002);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_003);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_004);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_005);
	        material_hologramu.AddNode(hologram_material_whitekinetic_substract_006);
	        material_hologramu.AddNode(hologram_material_whitekinetic_value);
	        material_hologramu.AddNode(image_texture_with_aa);
	        material_hologramu.AddNode(image_texture_without_aa);
	        material_hologramu.AddNode(input);
	        material_hologramu.AddNode(mapping);
	        material_hologramu.AddNode(math_014);
	        material_hologramu.AddNode(math_015);
	        material_hologramu.AddNode(math_016);

	        aa_add_001.outs.Value.Connect(aa_round_001.ins.Value1);
	        aa_add.outs.Value.Connect(aa_round.ins.Value1);
	        aa_combinergb.outs.Image.Connect(image_texture_without_aa.ins.Vector);
	        aa_divide_001.outs.Value.Connect(aa_combinergb.ins.G);
	        aa_divide.outs.Value.Connect(aa_combinergb.ins.R);
	        aa_input_RozmerbitmapyPrechodu.outs.Value.Connect(aa_divide_001.ins.Value2);
	        aa_input_RozmerbitmapyPrechodu.outs.Value.Connect(aa_divide.ins.Value2);
	        aa_input_RozmerbitmapyPrechodu.outs.Value.Connect(aa_multiply_001.ins.Value2);
	        aa_input_RozmerbitmapyPrechodu.outs.Value.Connect(aa_multiply.ins.Value2);
	        aa_multiply_001.outs.Value.Connect(aa_add_001.ins.Value1);
	        aa_multiply.outs.Value.Connect(aa_add.ins.Value1);
	        aa_round_001.outs.Value.Connect(aa_substract_001.ins.Value1);
	        aa_round.outs.Value.Connect(aa_substract.ins.Value1);
	        aa_separatergb.outs.G.Connect(aa_multiply_001.ins.Value1);
	        aa_separatergb.outs.R.Connect(aa_multiply.ins.Value1);
	        aa_substract_001.outs.Value.Connect(aa_divide_001.ins.Value1);
	        aa_substract.outs.Value.Connect(aa_divide.ins.Value1);
	        generated_cocka_absolute.outs.Value.Connect(generated_cocka_combinergb.ins.R);
	        generated_cocka_absolute.outs.Value.Connect(generated_cocka_greaterthan.ins.Value1);
	        generated_cocka_add.outs.Value.Connect(generated_cocka_power_002.ins.Value1);
	        generated_cocka_divide_001.outs.Value.Connect(generated_cocka_multiply_003.ins.Value2);
	        generated_cocka_divide.outs.Value.Connect(generated_cocka_multiply_002.ins.Value2);
	        generated_cocka_greaterthan.outs.Value.Connect(generated_cocka_multiply_002.ins.Value1);
	        generated_cocka_greaterthan.outs.Value.Connect(generated_cocka_multiply_003.ins.Value1);
	        generated_cocka_input_HodnotaGkanalupro.outs.Value.Connect(generated_cocka_divide_001.ins.Value1);
	        generated_cocka_input_KoeficientPeriody0_256.outs.Value.Connect(generated_cocka_divide.ins.Value1);
	        generated_cocka_input_StredX.outs.Value.Connect(generated_cocka_substract.ins.Value2);
	        generated_cocka_input_StredY.outs.Value.Connect(generated_cocka_substract_001.ins.Value2);
	        generated_cocka_multiply_001.outs.Value.Connect(generated_cocka_power_001.ins.Value1);
	        generated_cocka_multiply_002.outs.Value.Connect(generated_cocka_combinergb.ins.G);
	        generated_cocka_multiply_003.outs.Value.Connect(generated_cocka_combinergb.ins.B);
	        generated_cocka_multiply.outs.Value.Connect(generated_cocka_power.ins.Value1);
	        generated_cocka_power_001.outs.Value.Connect(generated_cocka_add.ins.Value2);
	        generated_cocka_power_002.outs.Value.Connect(generated_cocka_substract_002.ins.Value1);
	        generated_cocka_power.outs.Value.Connect(generated_cocka_add.ins.Value1);
	        generated_cocka_separatergb.outs.G.Connect(generated_cocka_substract_001.ins.Value1);
	        generated_cocka_separatergb.outs.R.Connect(generated_cocka_substract.ins.Value1);
	        generated_cocka_substract_001.outs.Value.Connect(generated_cocka_multiply_001.ins.Value1);
	        generated_cocka_substract_002.outs.Value.Connect(generated_cocka_absolute.ins.Value1);
	        generated_cocka_substract.outs.Value.Connect(generated_cocka_multiply.ins.Value1);
	        hologram_material_achro_absolute.outs.Value.Connect(hologram_material_achro_multiply_006.ins.Value1);
	        hologram_material_achro_add_001.outs.Value.Connect(hologram_material_achro_add_002.ins.Value1);
	        hologram_material_achro_add_001.outs.Value.Connect(hologram_material_achro_multiply_002.ins.Value1);
	        hologram_material_achro_add_002.outs.Value.Connect(hologram_material_achro_multiply_004.ins.Value1);
	        hologram_material_achro_add_002.outs.Value.Connect(hologram_material_achro_substract_003.ins.Value1);
	        hologram_material_achro_add.outs.Value.Connect(hologram_material_achro_multiply_001.ins.Value1);
	        hologram_material_achro_divide_001.outs.Value.Connect(hologram_material_achro_add_002.ins.Value2);
	        hologram_material_achro_divide_002.outs.Value.Connect(hologram_material_achro_divide_003.ins.Value1);
	        hologram_material_achro_divide_003.outs.Value.Connect(hologram_material_achro_multiply_005.ins.Value2);
	        hologram_material_achro_divide.outs.Value.Connect(hologram_material_achro_multiply.ins.Value2);
	        hologram_material_achro_greaterthan.outs.Value.Connect(hologram_material_achro_multiply_003.ins.Value2);
	        hologram_material_achro_lessthan.outs.Value.Connect(hologram_material_achro_multiply_003.ins.Value1);
	        hologram_material_achro_modulo_001.outs.Value.Connect(hologram_material_achro_substract.ins.Value2);
	        hologram_material_achro_modulo.outs.Value.Connect(hologram_material_achro_add_001.ins.Value2);
	        hologram_material_achro_multiply_001.outs.Value.Connect(hologram_material_achro_modulo.ins.Value1);
	        hologram_material_achro_multiply_002.outs.Value.Connect(hologram_material_achro_modulo_001.ins.Value1);
	        hologram_material_achro_multiply_003.outs.Value.Connect(hologram_material_mix.ins.Fac);
	        hologram_material_achro_multiply_003.outs.Value.Connect(hologram_material_achro_multiply_006.ins.Value2);
	        hologram_material_achro_multiply_004.outs.Value.Connect(hologram_material_achro_substract_002.ins.Value2);
	        hologram_material_achro_multiply_005.outs.Value.Connect(hologram_material_achro_substract_003.ins.Value2);
	        hologram_material_achro_multiply_006.outs.Value.Connect(hologram_material_mix.ins.Color2);
	        hologram_material_achro_multiply.outs.Value.Connect(hologram_material_achro_add.ins.Value2);
	        hologram_material_achro_noisetex.outs.Fac.Connect(hologram_material_achro_multiply.ins.Value1);
	        hologram_material_achro_separatergb_001.outs.B.Connect(hologram_material_achro_greaterthan.ins.Value1);
	        hologram_material_achro_separatergb_001.outs.B.Connect(hologram_material_achro_lessthan.ins.Value1);
	        hologram_material_achro_separatergb.outs.R.Connect(hologram_material_achro_add.ins.Value1);
	        hologram_material_achro_substract_001.outs.Value.Connect(hologram_material_achro_divide_003.ins.Value2);
	        hologram_material_achro_substract_002.outs.Value.Connect(hologram_material_achro_multiply_005.ins.Value1);
	        hologram_material_achro_substract_003.outs.Value.Connect(hologram_material_achro_substract_004.ins.Value2);
	        hologram_material_achro_substract_004.outs.Value.Connect(hologram_material_achro_absolute.ins.Value1);
	        hologram_material_achro_substract.outs.Value.Connect(hologram_material_achro_divide_001.ins.Value1);
	        hologram_material_axicon_checkertex.outs.Color.Connect(hologram_material_axicon_multiply_001.ins.Value2);
	        hologram_material_axicon_greaterthan.outs.Value.Connect(hologram_material_axicon_multiply.ins.Value1);
	        hologram_material_axicon_lessthan.outs.Value.Connect(hologram_material_axicon_multiply.ins.Value2);
	        hologram_material_axicon_mapping.outs.Vector.Connect(hologram_material_axicon_checkertex.ins.Vector);
	        hologram_material_axicon_multiply_001.outs.Value.Connect(hologram_material_mix_002.ins.Color2);
	        hologram_material_axicon_multiply.outs.Value.Connect(hologram_material_mix_002.ins.Fac);
	        hologram_material_axicon_multiply.outs.Value.Connect(hologram_material_axicon_multiply_001.ins.Value1);
	        hologram_material_axicon_separatergb_001.outs.B.Connect(hologram_material_axicon_greaterthan.ins.Value1);
	        hologram_material_axicon_separatergb_001.outs.B.Connect(hologram_material_axicon_lessthan.ins.Value1);
	        hologram_material_axicon_separatergb.outs.R.Connect(hologram_material_axicon_mapping.ins.Vector);
	        hologram_material_cocka_add_001.outs.Value.Connect(hologram_material_cocka_substract_001.ins.Value2);
	        hologram_material_cocka_add_002.outs.Value.Connect(hologram_material_cocka_power_001.ins.Value1);
	        hologram_material_cocka_add.outs.Value.Connect(hologram_material_cocka_add_001.ins.Value2);
	        hologram_material_cocka_divide_001.outs.Value.Connect(hologram_material_cocka_substract.ins.Value2);
	        hologram_material_cocka_divide_002.outs.Value.Connect(hologram_material_cocka_greaterthan_001.ins.Value2);
	        hologram_material_cocka_divide_003.outs.Value.Connect(hologram_material_cocka_multiply_003.ins.Value1);
	        hologram_material_cocka_divide.outs.Value.Connect(hologram_material_cocka_divide_001.ins.Value2);
	        hologram_material_cocka_greaterthan_001.outs.Value.Connect(hologram_material_cocka_multiply_004.ins.Value2);
	        hologram_material_cocka_greaterthan.outs.Value.Connect(hologram_material_cocka_multiply_001.ins.Value2);
	        hologram_material_cocka_lessthan.outs.Value.Connect(hologram_material_cocka_multiply_001.ins.Value1);
	        hologram_material_cocka_modulo.outs.Value.Connect(hologram_material_cocka_greaterthan_001.ins.Value1);
	        hologram_material_cocka_multiply_001.outs.Value.Connect(hologram_material_mix_006.ins.Fac);
	        hologram_material_cocka_multiply_001.outs.Value.Connect(hologram_material_cocka_multiply_004.ins.Value1);
	        hologram_material_cocka_multiply_002.outs.Value.Connect(hologram_material_cocka_divide_003.ins.Value2);
	        hologram_material_cocka_multiply_003.outs.Value.Connect(hologram_material_cocka_modulo.ins.Value1);
	        hologram_material_cocka_multiply_004.outs.Value.Connect(hologram_material_mix_006.ins.Color2);
	        hologram_material_cocka_multiply.outs.Value.Connect(hologram_material_cocka_add_002.ins.Value1);
	        hologram_material_cocka_power_001.outs.Value.Connect(hologram_material_cocka_divide_003.ins.Value1);
	        hologram_material_cocka_power.outs.Value.Connect(hologram_material_cocka_multiply.ins.Value1);
	        hologram_material_cocka_separatergb_001.outs.B.Connect(hologram_material_cocka_greaterthan.ins.Value1);
	        hologram_material_cocka_separatergb_001.outs.B.Connect(hologram_material_cocka_lessthan.ins.Value1);
	        hologram_material_cocka_separatergb.outs.G.Connect(hologram_material_cocka_multiply_002.ins.Value1);
	        hologram_material_cocka_separatergb.outs.R.Connect(hologram_material_cocka_add_001.ins.Value1);
	        hologram_material_cocka_separatergb.outs.R.Connect(hologram_material_cocka_divide_001.ins.Value1);
	        hologram_material_cocka_substract_001.outs.Value.Connect(hologram_material_cocka_power.ins.Value1);
	        hologram_material_cocka_substract.outs.Value.Connect(hologram_material_cocka_add.ins.Value1);
	        hologram_material_e_direct_absolute.outs.Value.Connect(hologram_material_e_direct_multiply_005.ins.Value1);
	        hologram_material_e_direct_add.outs.Value.Connect(hologram_material_e_direct_cosine.ins.Value1);
	        hologram_material_e_direct_add.outs.Value.Connect(hologram_material_e_direct_sine.ins.Value1);
	        hologram_material_e_direct_cosine.outs.Value.Connect(hologram_material_e_direct_multiply_001.ins.Value2);
	        hologram_material_e_direct_divide_001.outs.Value.Connect(hologram_material_e_direct_add.ins.Value1);
	        hologram_material_e_direct_divide_002.outs.Value.Connect(hologram_material_e_direct_add.ins.Value2);
	        hologram_material_e_direct_divide_003.outs.Value.Connect(hologram_material_e_direct_divide_004.ins.Value1);
	        hologram_material_e_direct_divide_004.outs.Value.Connect(hologram_material_e_direct_multiply_005.ins.Value2);
	        hologram_material_e_direct_divide_005.outs.Value.Connect(hologram_material_e_direct_greaterthan.ins.Value2);
	        hologram_material_e_direct_divide.outs.Value.Connect(hologram_material_e_direct_divide_001.ins.Value2);
	        hologram_material_e_direct_divide.outs.Value.Connect(hologram_material_e_direct_divide_002.ins.Value2);
	        hologram_material_e_direct_greaterthan.outs.Value.Connect(hologram_material_mix_003.ins.Color2);
	        hologram_material_e_direct_lessthan.outs.Value.Connect(hologram_material_mix_003.ins.Fac);
	        hologram_material_e_direct_modulo.outs.Value.Connect(hologram_material_e_direct_greaterthan.ins.Value1);
	        hologram_material_e_direct_multiply_001.outs.Value.Connect(hologram_material_e_direct_substract_001.ins.Value1);
	        hologram_material_e_direct_multiply_002.outs.Value.Connect(hologram_material_e_direct_substract_001.ins.Value2);
	        hologram_material_e_direct_multiply_003.outs.Value.Connect(hologram_material_e_direct_divide_003.ins.Value1);
	        hologram_material_e_direct_multiply_004.outs.Value.Connect(hologram_material_e_direct_divide_003.ins.Value2);
	        hologram_material_e_direct_multiply_005.outs.Value.Connect(hologram_material_e_direct_modulo.ins.Value1);
	        hologram_material_e_direct_multiply.outs.Value.Connect(hologram_material_e_direct_divide_001.ins.Value1);
	        hologram_material_e_direct_separatergb_001.outs.G.Connect(hologram_material_e_direct_multiply_002.ins.Value1);
	        hologram_material_e_direct_separatergb_001.outs.R.Connect(hologram_material_e_direct_multiply_001.ins.Value1);
	        hologram_material_e_direct_separatergb.outs.B.Connect(hologram_material_e_direct_lessthan.ins.Value1);
	        hologram_material_e_direct_separatergb.outs.G.Connect(hologram_material_e_direct_multiply_004.ins.Value1);
	        hologram_material_e_direct_separatergb.outs.R.Connect(hologram_material_e_direct_substract.ins.Value1);
	        hologram_material_e_direct_sine.outs.Value.Connect(hologram_material_e_direct_multiply_002.ins.Value2);
	        hologram_material_e_direct_substract_001.outs.Value.Connect(hologram_material_e_direct_absolute.ins.Value1);
	        hologram_material_e_direct_substract.outs.Value.Connect(hologram_material_e_direct_multiply.ins.Value1);
	        hologram_material_e_direct_value.outs.Value.Connect(hologram_material_e_direct_divide_002.ins.Value1);
	        hologram_material_gridhelper_absolute.outs.Value.Connect(hologram_material_gridhelper_multiply_008.ins.Value1);
	        hologram_material_gridhelper_add_001.outs.Value.Connect(hologram_material_gridhelper_multiply_007.ins.Value1);
	        hologram_material_gridhelper_add_002.outs.Value.Connect(hologram_material_gridhelper_multiply_009.ins.Value2);
	        hologram_material_gridhelper_add.outs.Value.Connect(hologram_material_gridhelper_combinergb.ins.R);
	        hologram_material_gridhelper_add.outs.Value.Connect(hologram_material_gridhelper_add_002.ins.Value1);
	        hologram_material_gridhelper_add.outs.Value.Connect(hologram_material_gridhelper_substract.ins.Value1);
	        hologram_material_gridhelper_combinergb.outs.Image.Connect(hologram_material_mix_008.ins.Color2);
	        hologram_material_gridhelper_divide_001.outs.Value.Connect(hologram_material_gridhelper_multiply_001.ins.Value2);
	        hologram_material_gridhelper_divide.outs.Value.Connect(hologram_material_gridhelper_multiply.ins.Value2);
	        hologram_material_gridhelper_greaterthan_001.outs.Value.Connect(hologram_material_gridhelper_add_001.ins.Value1);
	        hologram_material_gridhelper_greaterthan_002.outs.Value.Connect(hologram_material_gridhelper_multiply_006.ins.Value1);
	        hologram_material_gridhelper_greaterthan_003.outs.Value.Connect(hologram_material_gridhelper_add.ins.Value2);
	        hologram_material_gridhelper_greaterthan_004.outs.Value.Connect(hologram_material_gridhelper_add_001.ins.Value2);
	        hologram_material_gridhelper_greaterthan_005.outs.Value.Connect(hologram_material_gridhelper_multiply_006.ins.Value2);
	        hologram_material_gridhelper_greaterthan.outs.Value.Connect(hologram_material_gridhelper_add.ins.Value1);
	        hologram_material_gridhelper_modulo_001.outs.Value.Connect(hologram_material_gridhelper_greaterthan_001.ins.Value1);
	        hologram_material_gridhelper_modulo_002.outs.Value.Connect(hologram_material_gridhelper_greaterthan_002.ins.Value1);
	        hologram_material_gridhelper_modulo_003.outs.Value.Connect(hologram_material_gridhelper_greaterthan_003.ins.Value1);
	        hologram_material_gridhelper_modulo_004.outs.Value.Connect(hologram_material_gridhelper_greaterthan_004.ins.Value1);
	        hologram_material_gridhelper_modulo_005.outs.Value.Connect(hologram_material_gridhelper_greaterthan_005.ins.Value1);
	        hologram_material_gridhelper_modulo.outs.Value.Connect(hologram_material_gridhelper_greaterthan.ins.Value1);
	        hologram_material_gridhelper_multiply_001.outs.Value.Connect(hologram_material_gridhelper_modulo_003.ins.Value1);
	        hologram_material_gridhelper_multiply_001.outs.Value.Connect(hologram_material_gridhelper_multiply_003.ins.Value1);
	        hologram_material_gridhelper_multiply_002.outs.Value.Connect(hologram_material_gridhelper_modulo_001.ins.Value1);
	        hologram_material_gridhelper_multiply_002.outs.Value.Connect(hologram_material_gridhelper_multiply_004.ins.Value1);
	        hologram_material_gridhelper_multiply_003.outs.Value.Connect(hologram_material_gridhelper_modulo_004.ins.Value1);
	        hologram_material_gridhelper_multiply_003.outs.Value.Connect(hologram_material_gridhelper_multiply_005.ins.Value1);
	        hologram_material_gridhelper_multiply_004.outs.Value.Connect(hologram_material_gridhelper_modulo_002.ins.Value1);
	        hologram_material_gridhelper_multiply_005.outs.Value.Connect(hologram_material_gridhelper_modulo_005.ins.Value1);
	        hologram_material_gridhelper_multiply_006.outs.Value.Connect(hologram_material_gridhelper_multiply_007.ins.Value2);
	        hologram_material_gridhelper_multiply_007.outs.Value.Connect(hologram_material_gridhelper_multiply_008.ins.Value2);
	        hologram_material_gridhelper_multiply_008.outs.Value.Connect(hologram_material_gridhelper_combinergb.ins.G);
	        hologram_material_gridhelper_multiply_008.outs.Value.Connect(hologram_material_gridhelper_add_002.ins.Value2);
	        hologram_material_gridhelper_multiply_009.outs.Value.Connect(hologram_material_mix_008.ins.Fac);
	        hologram_material_gridhelper_multiply.outs.Value.Connect(hologram_material_gridhelper_modulo.ins.Value1);
	        hologram_material_gridhelper_multiply.outs.Value.Connect(hologram_material_gridhelper_multiply_002.ins.Value1);
	        hologram_material_gridhelper_separatergb.outs.G.Connect(hologram_material_gridhelper_multiply_001.ins.Value1);
	        hologram_material_gridhelper_separatergb.outs.R.Connect(hologram_material_gridhelper_multiply.ins.Value1);
	        hologram_material_gridhelper_substract.outs.Value.Connect(hologram_material_gridhelper_absolute.ins.Value1);
	        hologram_material_input_CameraTextureCoordinates.outs.Value.Connect(hologram_material_achro_noisetex.ins.Vector);
	        hologram_material_input_CameraTextureCoordinates.outs.Value.Connect(hologram_material_whitekinetic_noisetex.ins.Vector);
	        hologram_material_input_Data_ACHROInverzesmeruschodu.outs.Value.Connect(hologram_material_achro_substract_004.ins.Value1);
	        hologram_material_input_Data_ACHRONoiseDetail.outs.Value.Connect(hologram_material_achro_noisetex.ins.Detail);
	        hologram_material_input_Data_ACHRONoiseScale.outs.Value.Connect(hologram_material_achro_noisetex.ins.Scale);
	        hologram_material_input_Data_ACHRONoiseStrength.outs.Value.Connect(hologram_material_achro_divide.ins.Value1);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_divide_001.ins.Value2);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_divide_002.ins.Value2);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_multiply_002.ins.Value2);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_multiply_004.ins.Value2);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_substract_001.ins.Value1);
	        hologram_material_input_Data_ACHROPoceturovni.outs.Value.Connect(hologram_material_achro_substract_002.ins.Value1);
	        hologram_material_input_Data_ACHROScale.outs.Value.Connect(hologram_material_achro_multiply_001.ins.Value2);
	        hologram_material_input_Data_AXICONScale.outs.Value.Connect(hologram_material_axicon_checkertex.ins.Scale);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_divide_006.ins.Value2);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_divide_007.ins.Value2);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_multiply_007.ins.Value2);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_multiply_008.ins.Value2);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_substract_004.ins.Value1);
	        hologram_material_input_Data_BILA_Pocetschodu.outs.Value.Connect(hologram_material_whitekinetic_substract_005.ins.Value1);
	        hologram_material_input_Data_BILA_Scalenasobekperiody.outs.Value.Connect(hologram_material_whitekinetic_divide_005.ins.Value2);
	        hologram_material_input_Data_BILANoiseDetail.outs.Value.Connect(hologram_material_whitekinetic_noisetex.ins.Detail);
	        hologram_material_input_Data_BILANoiseScale.outs.Value.Connect(hologram_material_whitekinetic_noisetex.ins.Scale);
	        hologram_material_input_Data_BILANoiseStrength.outs.Value.Connect(hologram_material_whitekinetic_divide.ins.Value1);
	        hologram_material_input_Data_COCKAKoeficientplochosti.outs.Value.Connect(hologram_material_cocka_add.ins.Value2);
	        hologram_material_input_Data_COCKAKoeficientplochosti.outs.Value.Connect(hologram_material_cocka_divide.ins.Value2);
	        hologram_material_input_Data_COCKAScalemrizky.outs.Value.Connect(hologram_material_cocka_multiply_003.ins.Value2);
	        hologram_material_input_Data_EDIRECTScale.outs.Value.Connect(hologram_material_e_direct_divide_004.ins.Value2);
	        hologram_material_input_Data_KINETIC3D_KOMPENZACEkolikratprvekdorozmeruholo.outs.Value.Connect(hologram_material_kinetic3d_multiply_003.ins.Value2);
	        hologram_material_input_Data_KINETIC3DScale.outs.Value.Connect(hologram_material_kinetic3d_multiply_004.ins.Value2);
	        hologram_material_input_Data_KINETIC3DUhelmrizkyvestupnich0.outs.Value.Connect(hologram_material_kinetic3d_divide.ins.Value1);
	        hologram_material_input_Data_KINETIC3DZMapHeight.outs.Value.Connect(hologram_material_kinetic3d_multiply_002.ins.Value2);
	        hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_cocka_divide_002.ins.Value1);
	        hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_e_direct_divide_005.ins.Value1);
	        hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_kinetic3d_divide_001.ins.Value1);
	        hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_krizovamrizka_divide_001.ins.Value1);
	        hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_sterbina_divide_005.ins.Value1);
            hologram_material_input_Data_Pomercernekubilevprocentech.outs.Value.Connect(hologram_material_sterbina_divide_006.ins.Value1);
	        hologram_material_input_Data_STERBINAXperioda.outs.Value.Connect(hologram_material_sterbina_divide_003.ins.Value1);
	        hologram_material_input_Data_STERBINAXperioda.outs.Value.Connect(hologram_material_sterbina_divide_004.ins.Value2);
	        hologram_material_input_GRIDAlpha.outs.Value.Connect(hologram_material_gridhelper_multiply_009.ins.Value1);
	        hologram_material_input_GRIDScale.outs.Value.Connect(hologram_material_gridhelper_divide_001.ins.Value2);
	        hologram_material_input_GRIDScale.outs.Value.Connect(hologram_material_gridhelper_divide.ins.Value2);
	        hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_e_direct_separatergb_001.ins.Image);
            hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_gridhelper_separatergb.ins.Image);
            hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_kinetic3d_separatergb.ins.Image);
            hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_krizovamrizka_separatergb_002.ins.Image);
            hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_sterbina_separatergb_001.ins.Image);
            hologram_material_input_GeneratedTextureCoordinates.outs.Value.Connect(hologram_material_whitekinetic_separatergb_001.ins.Image);
	        hologram_material_input_HologramRozmerX.outs.Value.Connect(hologram_material_gridhelper_divide.ins.Value1);
            hologram_material_input_HologramRozmerX.outs.Value.Connect(hologram_material_krizovamrizka_multiply_001.ins.Value1);
            hologram_material_input_HologramRozmerX.outs.Value.Connect(hologram_material_sterbina_multiply_003.ins.Value1);
            hologram_material_input_HologramRozmerY.outs.Value.Connect(hologram_material_e_direct_multiply_003.ins.Value1);
            hologram_material_input_HologramRozmerY.outs.Value.Connect(hologram_material_gridhelper_divide_001.ins.Value1);
            hologram_material_input_HologramRozmerY.outs.Value.Connect(hologram_material_krizovamrizka_multiply_004.ins.Value1);
            hologram_material_input_HologramRozmerY.outs.Value.Connect(hologram_material_whitekinetic_multiply_004.ins.Value1);
	        hologram_material_input_MASTER.outs.Color.Connect(hologram_material_achro_separatergb_001.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_axicon_separatergb_001.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_cocka_separatergb_001.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_e_direct_separatergb.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_kinetic3d_separatergb_002.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_krizovamrizka_separatergb_001.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_sterbina_separatergb.ins.Image);
            hologram_material_input_MASTER.outs.Color.Connect(hologram_material_whitekinetic_separatergb.ins.Image);
            hologram_material_input_MASTERAA.outs.Color.Connect(hologram_material_achro_separatergb.ins.Image);
            hologram_material_input_MASTERAA.outs.Color.Connect(hologram_material_axicon_separatergb.ins.Image);
            hologram_material_input_MASTERAA.outs.Color.Connect(hologram_material_cocka_separatergb.ins.Image);
            hologram_material_input_MASTERAA.outs.Color.Connect(hologram_material_kinetic3d_separatergb_001.ins.Image);
            hologram_material_input_MASTERAA.outs.Color.Connect(hologram_material_krizovamrizka_separatergb.ins.Image);
	        hologram_material_kinetic3d_absolute.outs.Value.Connect(hologram_material_kinetic3d_multiply_004.ins.Value1);
	        hologram_material_kinetic3d_divide_001.outs.Value.Connect(hologram_material_kinetic3d_greaterthan_001.ins.Value2);
	        hologram_material_kinetic3d_divide_002.outs.Value.Connect(hologram_material_kinetic3d_absolute.ins.Value1);
	        hologram_material_kinetic3d_divide.outs.Value.Connect(hologram_material_kinetic3d_sine.ins.Value1);
	        hologram_material_kinetic3d_greaterthan_001.outs.Value.Connect(hologram_material_mix_001.ins.Color2);
	        hologram_material_kinetic3d_greaterthan.outs.Value.Connect(hologram_material_mix_001.ins.Fac);
	        hologram_material_kinetic3d_modulo.outs.Value.Connect(hologram_material_kinetic3d_greaterthan_001.ins.Value1);
	        hologram_material_kinetic3d_multiply_001.outs.Value.Connect(hologram_material_kinetic3d_divide_002.ins.Value2);
	        hologram_material_kinetic3d_multiply_002.outs.Value.Connect(hologram_material_kinetic3d_multiply.ins.Value1);
	        hologram_material_kinetic3d_multiply_003.outs.Value.Connect(hologram_material_kinetic3d_substract.ins.Value2);
	        hologram_material_kinetic3d_multiply_004.outs.Value.Connect(hologram_material_kinetic3d_modulo.ins.Value1);
	        hologram_material_kinetic3d_multiply.outs.Value.Connect(hologram_material_kinetic3d_substract.ins.Value1);
	        hologram_material_kinetic3d_separatergb_001.outs.G.Connect(hologram_material_kinetic3d_multiply_001.ins.Value1);
	        hologram_material_kinetic3d_separatergb_001.outs.R.Connect(hologram_material_kinetic3d_multiply_002.ins.Value1);
	        hologram_material_kinetic3d_separatergb_002.outs.B.Connect(hologram_material_kinetic3d_greaterthan.ins.Value1);
	        hologram_material_kinetic3d_separatergb.outs.G.Connect(hologram_material_kinetic3d_multiply_003.ins.Value1);
	        hologram_material_kinetic3d_sine.outs.Value.Connect(hologram_material_kinetic3d_multiply.ins.Value2);
	        hologram_material_kinetic3d_substract.outs.Value.Connect(hologram_material_kinetic3d_divide_002.ins.Value1);
	        hologram_material_krizovamrizka_divide_001.outs.Value.Connect(hologram_material_krizovamrizka_lessthan_001.ins.Value2);
	        hologram_material_krizovamrizka_divide_001.outs.Value.Connect(hologram_material_krizovamrizka_lessthan.ins.Value2);
	        hologram_material_krizovamrizka_divide_002.outs.Value.Connect(hologram_material_krizovamrizka_multiply_005.ins.Value2);
	        hologram_material_krizovamrizka_divide.outs.Value.Connect(hologram_material_krizovamrizka_multiply_002.ins.Value2);
	        hologram_material_krizovamrizka_input_provetsinupripadu1.outs.Value.Connect(hologram_material_krizovamrizka_multiply_002.ins.Value1);
	        hologram_material_krizovamrizka_input_provetsinupripadu1.outs.Value.Connect(hologram_material_krizovamrizka_multiply_005.ins.Value1);
	        hologram_material_krizovamrizka_lessthan_001.outs.Value.Connect(hologram_material_krizovamrizka_multiply_007.ins.Value2);
	        hologram_material_krizovamrizka_lessthan.outs.Value.Connect(hologram_material_krizovamrizka_multiply_007.ins.Value1);
	        hologram_material_krizovamrizka_modulo_001.outs.Value.Connect(hologram_material_krizovamrizka_lessthan_001.ins.Value1);
	        hologram_material_krizovamrizka_modulo.outs.Value.Connect(hologram_material_krizovamrizka_lessthan.ins.Value1);
	        hologram_material_krizovamrizka_multiply_001.outs.Value.Connect(hologram_material_krizovamrizka_divide.ins.Value1);
	        hologram_material_krizovamrizka_multiply_002.outs.Value.Connect(hologram_material_krizovamrizka_multiply_003.ins.Value2);
	        hologram_material_krizovamrizka_multiply_003.outs.Value.Connect(hologram_material_krizovamrizka_modulo.ins.Value1);
	        hologram_material_krizovamrizka_multiply_004.outs.Value.Connect(hologram_material_krizovamrizka_divide_002.ins.Value1);
	        hologram_material_krizovamrizka_multiply_005.outs.Value.Connect(hologram_material_krizovamrizka_multiply_006.ins.Value2);
	        hologram_material_krizovamrizka_multiply_006.outs.Value.Connect(hologram_material_krizovamrizka_modulo_001.ins.Value1);
	        hologram_material_krizovamrizka_multiply_007.outs.Value.Connect(hologram_material_mix_007.ins.Color2);
	        hologram_material_krizovamrizka_multiply.outs.Value.Connect(hologram_material_krizovamrizka_divide_002.ins.Value2);
	        hologram_material_krizovamrizka_multiply.outs.Value.Connect(hologram_material_krizovamrizka_divide.ins.Value2);
	        hologram_material_krizovamrizka_separatergb_001.outs.B.Connect(math_014.ins.Value1);
	        hologram_material_krizovamrizka_separatergb_001.outs.B.Connect(math_015.ins.Value1);
	        hologram_material_krizovamrizka_separatergb_002.outs.G.Connect(hologram_material_krizovamrizka_multiply_006.ins.Value1);
	        hologram_material_krizovamrizka_separatergb_002.outs.R.Connect(hologram_material_krizovamrizka_multiply_003.ins.Value1);
	        hologram_material_krizovamrizka_separatergb.outs.G.Connect(hologram_material_krizovamrizka_multiply.ins.Value1);
	        hologram_material_mix_001.outs.Color.Connect(hologram_material_mix.ins.Color1);
	        hologram_material_mix_002.outs.Color.Connect(hologram_material_mix_001.ins.Color1);
	        hologram_material_mix_003.outs.Color.Connect(hologram_material_mix_002.ins.Color1);
	        hologram_material_mix_004.outs.Color.Connect(hologram_material_mix_003.ins.Color1);
	        hologram_material_mix_005.outs.Color.Connect(hologram_material_mix_004.ins.Color1);
	        hologram_material_mix_006.outs.Color.Connect(hologram_material_mix_005.ins.Color1);
	        hologram_material_mix_007.outs.Color.Connect(hologram_material_mix_006.ins.Color1);
	        hologram_material_mix_008.outs.Color.Connect(hologram_material_emission.ins.Color);
	        hologram_material_mix.outs.Color.Connect(hologram_material_mix_008.ins.Color1);
	        hologram_material_rgb.outs.Color.Connect(hologram_material_mix_007.ins.Color1);
	        hologram_material_sterbina_absolute.outs.Value.Connect(hologram_material_sterbina_divide.ins.Value1);
	        hologram_material_sterbina_add_001.outs.Value.Connect(hologram_material_sterbina_power_002.ins.Value1);
	        hologram_material_sterbina_add_002.outs.Value.Connect(hologram_material_sterbina_power_005.ins.Value1);
	        hologram_material_sterbina_add_003.outs.Value.Connect(hologram_material_sterbina_modulo_003.ins.Value1);
	        hologram_material_sterbina_add_004.outs.Value.Connect(hologram_material_mix_005.ins.Color2);
	        hologram_material_sterbina_add_005.outs.Value.Connect(hologram_material_mix_005.ins.Fac);
	        hologram_material_sterbina_add.outs.Value.Connect(hologram_material_sterbina_multiply_005.ins.Value2);
	        hologram_material_sterbina_add.outs.Value.Connect(hologram_material_sterbina_multiply_006.ins.Value2);
	        hologram_material_sterbina_add.outs.Value.Connect(hologram_material_sterbina_multiply_007.ins.Value2);
	        hologram_material_sterbina_add.outs.Value.Connect(hologram_material_sterbina_multiply_008.ins.Value2);
	        hologram_material_sterbina_divide_001.outs.Value.Connect(hologram_material_sterbina_divide_002.ins.Value1);
	        hologram_material_sterbina_divide_002.outs.Value.Connect(hologram_material_sterbina_power_001.ins.Value1);
	        hologram_material_sterbina_divide_002.outs.Value.Connect(hologram_material_sterbina_power_004.ins.Value1);
	        hologram_material_sterbina_divide_002.outs.Value.Connect(hologram_material_sterbina_substract_003.ins.Value1);
	        hologram_material_sterbina_divide_002.outs.Value.Connect(hologram_material_sterbina_substract_007.ins.Value1);
	        hologram_material_sterbina_divide_003.outs.Value.Connect(hologram_material_sterbina_round.ins.Value1);
	        hologram_material_sterbina_divide_004.outs.Value.Connect(hologram_material_sterbina_add.ins.Value1);
	        hologram_material_sterbina_divide_005.outs.Value.Connect(hologram_material_sterbina_greaterthan_002.ins.Value2);
	        hologram_material_sterbina_divide_006.outs.Value.Connect(hologram_material_sterbina_greaterthan_003.ins.Value2);
	        hologram_material_sterbina_divide.outs.Value.Connect(hologram_material_sterbina_sine.ins.Value1);
	        hologram_material_sterbina_greaterthan_001.outs.Value.Connect(hologram_material_sterbina_multiply_001.ins.Value2);
	        hologram_material_sterbina_greaterthan_002.outs.Value.Connect(hologram_material_sterbina_multiply_011.ins.Value2);
	        hologram_material_sterbina_greaterthan_003.outs.Value.Connect(hologram_material_sterbina_multiply_012.ins.Value1);
	        hologram_material_sterbina_greaterthan.outs.Value.Connect(hologram_material_sterbina_multiply.ins.Value2);
	        hologram_material_sterbina_lessthan_001.outs.Value.Connect(hologram_material_sterbina_multiply_001.ins.Value1);
	        hologram_material_sterbina_lessthan.outs.Value.Connect(hologram_material_sterbina_multiply.ins.Value1);
	        hologram_material_sterbina_modulo_001.outs.Value.Connect(hologram_material_sterbina_substract_005.ins.Value1);
	        hologram_material_sterbina_modulo_002.outs.Value.Connect(hologram_material_sterbina_multiply_009.ins.Value1);
	        hologram_material_sterbina_modulo_003.outs.Value.Connect(hologram_material_sterbina_multiply_010.ins.Value1);
	        hologram_material_sterbina_modulo_004.outs.Value.Connect(hologram_material_sterbina_greaterthan_002.ins.Value1);
	        hologram_material_sterbina_modulo_005.outs.Value.Connect(hologram_material_sterbina_greaterthan_003.ins.Value1);
	        hologram_material_sterbina_modulo.outs.Value.Connect(hologram_material_sterbina_substract_001.ins.Value1);
	        hologram_material_sterbina_multiply_001.outs.Value.Connect(hologram_material_sterbina_add_005.ins.Value1);
	        hologram_material_sterbina_multiply_001.outs.Value.Connect(hologram_material_sterbina_multiply_011.ins.Value1);
	        hologram_material_sterbina_multiply_002.outs.Value.Connect(hologram_material_sterbina_absolute.ins.Value1);
	        hologram_material_sterbina_multiply_003.outs.Value.Connect(hologram_material_sterbina_divide_004.ins.Value1);
	        hologram_material_sterbina_multiply_004.outs.Value.Connect(hologram_material_sterbina_divide_003.ins.Value2);
	        hologram_material_sterbina_multiply_005.outs.Value.Connect(hologram_material_sterbina_modulo.ins.Value1);
	        hologram_material_sterbina_multiply_006.outs.Value.Connect(hologram_material_sterbina_substract_009.ins.Value1);
	        hologram_material_sterbina_multiply_007.outs.Value.Connect(hologram_material_sterbina_add_003.ins.Value1);
	        hologram_material_sterbina_multiply_008.outs.Value.Connect(hologram_material_sterbina_modulo_001.ins.Value1);
	        hologram_material_sterbina_multiply_009.outs.Value.Connect(hologram_material_sterbina_modulo_004.ins.Value1);
	        hologram_material_sterbina_multiply_010.outs.Value.Connect(hologram_material_sterbina_modulo_005.ins.Value1);
	        hologram_material_sterbina_multiply_011.outs.Value.Connect(hologram_material_sterbina_add_004.ins.Value1);
	        hologram_material_sterbina_multiply_012.outs.Value.Connect(hologram_material_sterbina_add_004.ins.Value2);
	        hologram_material_sterbina_multiply.outs.Value.Connect(hologram_material_sterbina_add_005.ins.Value2);
	        hologram_material_sterbina_multiply.outs.Value.Connect(hologram_material_sterbina_multiply_012.ins.Value2);
	        hologram_material_sterbina_power_001.outs.Value.Connect(hologram_material_sterbina_add_001.ins.Value2);
	        hologram_material_sterbina_power_002.outs.Value.Connect(hologram_material_sterbina_substract_004.ins.Value1);
	        hologram_material_sterbina_power_003.outs.Value.Connect(hologram_material_sterbina_substract_006.ins.Value2);
	        hologram_material_sterbina_power_004.outs.Value.Connect(hologram_material_sterbina_add_002.ins.Value2);
	        hologram_material_sterbina_power_005.outs.Value.Connect(hologram_material_sterbina_substract_008.ins.Value1);
	        hologram_material_sterbina_power.outs.Value.Connect(hologram_material_sterbina_substract_002.ins.Value2);
	        hologram_material_sterbina_round.outs.Value.Connect(hologram_material_sterbina_multiply_009.ins.Value2);
	        hologram_material_sterbina_round.outs.Value.Connect(hologram_material_sterbina_multiply_010.ins.Value2);
	        hologram_material_sterbina_separatergb_001.outs.G.Connect(hologram_material_sterbina_multiply_006.ins.Value1);
	        hologram_material_sterbina_separatergb_001.outs.G.Connect(hologram_material_sterbina_multiply_008.ins.Value1);
	        hologram_material_sterbina_separatergb_001.outs.R.Connect(hologram_material_sterbina_multiply_005.ins.Value1);
	        hologram_material_sterbina_separatergb_001.outs.R.Connect(hologram_material_sterbina_multiply_007.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.B.Connect(hologram_material_sterbina_greaterthan_001.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.B.Connect(hologram_material_sterbina_greaterthan.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.B.Connect(hologram_material_sterbina_lessthan_001.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.B.Connect(hologram_material_sterbina_lessthan.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.G.Connect(hologram_material_sterbina_multiply_004.ins.Value1);
	        hologram_material_sterbina_separatergb.outs.R.Connect(hologram_material_sterbina_substract.ins.Value1);
	        hologram_material_sterbina_sine.outs.Value.Connect(hologram_material_sterbina_divide_001.ins.Value2);
	        hologram_material_sterbina_substract_001.outs.Value.Connect(hologram_material_sterbina_power.ins.Value1);
	        hologram_material_sterbina_substract_002.outs.Value.Connect(hologram_material_sterbina_add_001.ins.Value1);
	        hologram_material_sterbina_substract_003.outs.Value.Connect(hologram_material_sterbina_substract_004.ins.Value2);
	        hologram_material_sterbina_substract_004.outs.Value.Connect(hologram_material_sterbina_substract_009.ins.Value2);
	        hologram_material_sterbina_substract_005.outs.Value.Connect(hologram_material_sterbina_power_003.ins.Value1);
	        hologram_material_sterbina_substract_006.outs.Value.Connect(hologram_material_sterbina_add_002.ins.Value1);
	        hologram_material_sterbina_substract_007.outs.Value.Connect(hologram_material_sterbina_substract_008.ins.Value2);
	        hologram_material_sterbina_substract_008.outs.Value.Connect(hologram_material_sterbina_add_003.ins.Value2);
	        hologram_material_sterbina_substract_009.outs.Value.Connect(hologram_material_sterbina_modulo_002.ins.Value1);
	        hologram_material_sterbina_substract.outs.Value.Connect(hologram_material_sterbina_multiply_002.ins.Value1);
	        hologram_material_whitekinetic_absolute.outs.Value.Connect(hologram_material_whitekinetic_multiply_006.ins.Value1);
	        hologram_material_whitekinetic_add_001.outs.Value.Connect(hologram_material_whitekinetic_modulo.ins.Value1);
	        hologram_material_whitekinetic_add_002.outs.Value.Connect(hologram_material_whitekinetic_multiply_008.ins.Value1);
	        hologram_material_whitekinetic_add_002.outs.Value.Connect(hologram_material_whitekinetic_substract_006.ins.Value1);
	        hologram_material_whitekinetic_add.outs.Value.Connect(hologram_material_whitekinetic_cosine.ins.Value1);
	        hologram_material_whitekinetic_add.outs.Value.Connect(hologram_material_whitekinetic_sine.ins.Value1);
	        hologram_material_whitekinetic_cosine.outs.Value.Connect(hologram_material_whitekinetic_multiply_002.ins.Value2);
	        hologram_material_whitekinetic_divide_001.outs.Value.Connect(hologram_material_whitekinetic_divide_002.ins.Value2);
	        hologram_material_whitekinetic_divide_001.outs.Value.Connect(hologram_material_whitekinetic_divide_003.ins.Value2);
	        hologram_material_whitekinetic_divide_002.outs.Value.Connect(hologram_material_whitekinetic_add.ins.Value1);
	        hologram_material_whitekinetic_divide_003.outs.Value.Connect(hologram_material_whitekinetic_add.ins.Value2);
	        hologram_material_whitekinetic_divide_004.outs.Value.Connect(hologram_material_whitekinetic_divide_005.ins.Value1);
	        hologram_material_whitekinetic_divide_005.outs.Value.Connect(hologram_material_whitekinetic_multiply_006.ins.Value2);
	        hologram_material_whitekinetic_divide_006.outs.Value.Connect(hologram_material_whitekinetic_add_002.ins.Value2);
	        hologram_material_whitekinetic_divide_007.outs.Value.Connect(hologram_material_whitekinetic_divide_008.ins.Value1);
	        hologram_material_whitekinetic_divide_008.outs.Value.Connect(hologram_material_whitekinetic_multiply_009.ins.Value2);
	        hologram_material_whitekinetic_divide.outs.Value.Connect(hologram_material_whitekinetic_multiply.ins.Value2);
	        hologram_material_whitekinetic_greaterthan.outs.Value.Connect(hologram_material_whitekinetic_multiply_010.ins.Value2);
	        hologram_material_whitekinetic_lessthan.outs.Value.Connect(hologram_material_whitekinetic_multiply_010.ins.Value1);
	        hologram_material_whitekinetic_modulo_001.outs.Value.Connect(hologram_material_whitekinetic_substract_003.ins.Value2);
	        hologram_material_whitekinetic_modulo.outs.Value.Connect(hologram_material_whitekinetic_add_002.ins.Value1);
	        hologram_material_whitekinetic_modulo.outs.Value.Connect(hologram_material_whitekinetic_multiply_007.ins.Value1);
	        hologram_material_whitekinetic_multiply_001.outs.Value.Connect(hologram_material_whitekinetic_divide_002.ins.Value1);
	        hologram_material_whitekinetic_multiply_002.outs.Value.Connect(hologram_material_whitekinetic_substract_001.ins.Value1);
	        hologram_material_whitekinetic_multiply_003.outs.Value.Connect(hologram_material_whitekinetic_substract_001.ins.Value2);
	        hologram_material_whitekinetic_multiply_004.outs.Value.Connect(hologram_material_whitekinetic_divide_004.ins.Value1);
	        hologram_material_whitekinetic_multiply_005.outs.Value.Connect(hologram_material_whitekinetic_divide_004.ins.Value2);
	        hologram_material_whitekinetic_multiply_006.outs.Value.Connect(hologram_material_whitekinetic_add_001.ins.Value2);
	        hologram_material_whitekinetic_multiply_007.outs.Value.Connect(hologram_material_whitekinetic_modulo_001.ins.Value1);
	        hologram_material_whitekinetic_multiply_008.outs.Value.Connect(hologram_material_whitekinetic_substract_005.ins.Value2);
	        hologram_material_whitekinetic_multiply_009.outs.Value.Connect(hologram_material_whitekinetic_substract_006.ins.Value2);
	        hologram_material_whitekinetic_multiply_010.outs.Value.Connect(hologram_material_mix_004.ins.Fac);
	        hologram_material_whitekinetic_multiply_010.outs.Value.Connect(hologram_material_whitekinetic_multiply_011.ins.Value2);
	        hologram_material_whitekinetic_multiply_011.outs.Value.Connect(hologram_material_mix_004.ins.Color2);
	        hologram_material_whitekinetic_multiply.outs.Value.Connect(hologram_material_whitekinetic_add_001.ins.Value1);
	        hologram_material_whitekinetic_noisetex.outs.Fac.Connect(hologram_material_whitekinetic_multiply.ins.Value1);
	        hologram_material_whitekinetic_separatergb_001.outs.G.Connect(hologram_material_whitekinetic_multiply_003.ins.Value1);
	        hologram_material_whitekinetic_separatergb_001.outs.R.Connect(hologram_material_whitekinetic_multiply_002.ins.Value1);
	        hologram_material_whitekinetic_separatergb.outs.B.Connect(hologram_material_whitekinetic_greaterthan.ins.Value1);
	        hologram_material_whitekinetic_separatergb.outs.B.Connect(hologram_material_whitekinetic_lessthan.ins.Value1);
	        hologram_material_whitekinetic_separatergb.outs.G.Connect(hologram_material_whitekinetic_multiply_005.ins.Value1);
	        hologram_material_whitekinetic_separatergb.outs.R.Connect(hologram_material_whitekinetic_substract.ins.Value1);
	        hologram_material_whitekinetic_sine.outs.Value.Connect(hologram_material_whitekinetic_multiply_003.ins.Value2);
	        hologram_material_whitekinetic_substract_001.outs.Value.Connect(hologram_material_whitekinetic_substract_002.ins.Value1);
	        hologram_material_whitekinetic_substract_002.outs.Value.Connect(hologram_material_whitekinetic_absolute.ins.Value1);
	        hologram_material_whitekinetic_substract_003.outs.Value.Connect(hologram_material_whitekinetic_divide_006.ins.Value1);
	        hologram_material_whitekinetic_substract_004.outs.Value.Connect(hologram_material_whitekinetic_divide_008.ins.Value2);
	        hologram_material_whitekinetic_substract_005.outs.Value.Connect(hologram_material_whitekinetic_multiply_009.ins.Value1);
	        hologram_material_whitekinetic_substract_006.outs.Value.Connect(hologram_material_whitekinetic_multiply_011.ins.Value1);
	        hologram_material_whitekinetic_substract.outs.Value.Connect(hologram_material_whitekinetic_multiply_001.ins.Value1);
	        hologram_material_whitekinetic_value.outs.Value.Connect(hologram_material_whitekinetic_divide_003.ins.Value1);
	        input.outs.Generated.Connect(generated_cocka_separatergb.ins.Image);
	        input.outs.UV.Connect(mapping.ins.Vector);
	        mapping.outs.Vector.Connect(aa_separatergb.ins.Image);
	        mapping.outs.Vector.Connect(image_texture_with_aa.ins.Vector);
	        math_014.outs.Value.Connect(math_016.ins.Value1);
	        math_015.outs.Value.Connect(math_016.ins.Value2);
	        math_016.outs.Value.Connect(hologram_material_mix_007.ins.Fac);

	        hologram_material_emission.outs.Emission.Connect(material_hologramu.Output.ins.Surface);

	        material_hologramu.FinalizeGraph();

	        return material_hologramu;
        }
        */

        static public void CompileMaterial()
        {
            CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
            string Output = "Out.exe";

            SetMessage("Compiling material");

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, _iui.getMaterialFileName());

            string errmsg="";
            if (results.Errors.HasErrors)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    errmsg = errmsg +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine;
                    SetMessage(errmsg);
                }
            }
            else
            {
                Assembly assembly = results.CompiledAssembly;
                MethodInfo mi = assembly.EntryPoint;
                //Type program = assembly.GetType("HologramPrinter.Test");
                //MethodInfo main = program.GetMethod("Main");
                object o = assembly.CreateInstance(mi.Name);
                mi.Invoke(o, null);

                //Successful Compile
                SetMessage("Success!");
                //If we clicked run then launch our EXE
                //Process.Start(Output);
                //main.Invoke(null, null);
            }
        }

        /*
        static public Shader read_shader_from_file(string filename)
        {
            var xml = new XmlReader(Client, filename);
            xml.Parse();

            var some_setup = new Shader(Client, Shader.ShaderType.Material)
            {
                Name = "read_from_file"
            };


            var brick_texture = new BrickTexture();
            brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Color1.Value = new float4(0.800f, 0.800f, 0.800f);
            brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
            brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Scale.Value = 1.000f;
            brick_texture.ins.MortarSize.Value = 0.020f;
            brick_texture.ins.Bias.Value = 0.000f;
            brick_texture.ins.BrickWidth.Value = 0.500f;
            brick_texture.ins.RowHeight.Value = 0.250f;

            var checker_texture = new CheckerTexture();
            checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            checker_texture.ins.Color1.Value = new float4(0.000f, 0.004f, 0.800f);
            checker_texture.ins.Color2.Value = new float4(0.200f, 0.000f, 0.007f);
            checker_texture.ins.Scale.Value = 5.000f;

            var diffuse_bsdf = new DiffuseBsdfNode();
            diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
            diffuse_bsdf.ins.Roughness.Value = 0.000f;
            diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);

            var texture_coordinate = new TextureCoordinateNode();


            some_setup.AddNode(brick_texture);
            some_setup.AddNode(checker_texture);
            some_setup.AddNode(diffuse_bsdf);
            some_setup.AddNode(texture_coordinate);

            brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
            checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
            texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
            texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

            diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);

            some_setup.FinalizeGraph();

            return some_setup;
        }
         * */

        public static bool CompileMaterial(String sourceName)
        {
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            SetMessage("Compiling material: " + sourceName);

            // Select the code provider based on the input file extension.
            if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            {
                provider = CodeDomProvider.CreateProvider("CSharp");
            }
            else 
            {
                Console.WriteLine("Source file must have a .cs or .vb extension");
            }

            if (provider != null)
            {

                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.

                String exeName = String.Format(@"{0}\{1}.exe", 
                    System.Environment.CurrentDirectory, 
                    sourceFile.Name.Replace(".", "_"));

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of 
                // a class library.
                cp.GenerateExecutable = false;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = true;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                // Add the reference of csycles to the to-be-compiled material
                cp.ReferencedAssemblies.Add("csycles.dll");

                // Invoke compilation of the source file.
                CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

                if(cr.Errors.Count > 0)
                {
                    // Display compilation errors.
                    SetMessage("Errors building " + sourceName + "into " + cr.PathToAssembly);
                    //Console.WriteLine("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
                    foreach(CompilerError ce in cr.Errors)
                    {
                        SetMessage("Error: " + ce.ToString());
                        //Console.WriteLine("  {0}", ce.ToString());
                        //Console.WriteLine();
                    }
                }
                else
                {
                    SetMessage("Source " + sourceName + "built into " + cr.PathToAssembly + " succesfully.");
                    // Display a successful compilation message.
                    //Console.WriteLine("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);

                    
                    var cls = cr.CompiledAssembly.GetType("HologramPrinter.Dynamic_Shader");
                    /*
                    var method = cls.GetMethod("CreateDynamicShader", BindingFlags.Static | BindingFlags.Public);   
                    var returned_value = method.Invoke(null, null);
                    returned_value = Activator.CreateInstance(Type.GetType("ccl.Shader"));
                    */

                    Object[] parameters;
                    parameters = new Object[4];
                    parameters[0] = Client;
                    parameters[1] = Device;
                    parameters[2] = Scene;
                    parameters[3] = Shader.ShaderType.World;

                    MethodInfo methodInformation = cls.GetMethod("Show");
                    //object assemblyInstance = cr.CompiledAssembly.CreateInstance("Test Shader", false);
                    var returned_value = methodInformation.Invoke(null , parameters);
                    SetMessage("Set background shader");
                    /*
                    returned_value = Activator.CreateInstance(Type.GetType("ccl.Shader"));
                    
                    if(returned_value is Shader)
                    {
                        SetMessage("Set background shader");
                        Shader dynamic_shader = returned_value as Shader;
                        Scene.AddShader(dynamic_shader);
                        Scene.Background.Shader = dynamic_shader; 
                    }
                    else
                    {
                        SetMessage("Unable to set background shader");
                    }
                     * */

                    /*
                    try
                    {
                        Shader dynamic_shader = (Shader)Convert.ChangeType(returned_value, typeof(Shader));
                        Scene.AddShader(dynamic_shader);
                        Scene.Background.Shader = dynamic_shader;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine("Cannot convert an object to a Shader");
                    }
                     * */

                    /*
                    //Shader dynamic_shader = (Shader)returned_value;
                    Scene.Background.AoDistance = 0.0f;
                    Scene.Background.AoFactor = 0.0f;
                    Scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
                    */

                    //String ClassName = "ccl.Shader";
                    //Shader dynamic_shader = (Shader)Activator.CreateInstance(cr.CompiledAssembly, ClassName))

                    /*
                    Shader dynamic_shader = new Shader(Client, Shader.ShaderType.World)
                    {
                        Name = "Dynamic shader"
                    };
                    */

                    /*
                    Type t = typeof(Shader);
                    int val;

                    // Get constructor info. 
                    ConstructorInfo[] ci = t.GetConstructors();

                    Console.WriteLine("Available constructors: ");
                    foreach (ConstructorInfo c in ci)
                    {
                        // Display return type and name. 
                        SetMessage("   " + t.Name + "(");

                        // Display parameters. 
                        ParameterInfo[] pi = c.GetParameters();

                        for (int i = 0; i < pi.Length; i++)
                        {
                            Console.Write(pi[i].ParameterType.Name +
                                          " " + pi[i].Name);
                            if (i + 1 < pi.Length) Console.Write(", ");
                        }

                        SetMessage(")");
                    }
                    SetMessage("");

                    // Find matching constructor. 
                    int x;

                    for (x = 0; x < ci.Length; x++)
                    {
                        ParameterInfo[] pi = ci[x].GetParameters();
                        if (pi.Length == 2) break;
                    }

                    if (x == ci.Length)
                    {
                        SetMessage("No matching constructor found.");
                    }
                    else
                        SetMessage("Two-parameter constructor found.\n");

                    // Construct the object.   
                    object[] consargs = new object[2];
                    consargs[0] = Client;
                    consargs[1] = Shader.ShaderType.World;
                    object reflectOb = ci[x].Invoke(consargs);

                    SetMessage("\nInvoking methods on reflectOb.");
                    SetMessage("");
                    MethodInfo[] mi = t.GetMethods();

                    foreach (MethodInfo m in mi)
                    {
                         if (m.Name.CompareTo("set") == 0)
                         {

                         }
                    }
                     * */

                    /*
                    // Invoke each method. 
                    foreach (MethodInfo m in mi)
                    {
                        // Get the parameters 
                        ParameterInfo[] pi = m.GetParameters();

                        if (m.Name.CompareTo("set") == 0 &&
                           pi[0].ParameterType == typeof(int))
                        {
                            // This is set(int, int). 
                            object[] args = new object[2];
                            args[0] = 9;
                            args[1] = 18;
                            m.Invoke(reflectOb, args);
                        }
                        else if (m.Name.CompareTo("set") == 0 &&
                                pi[0].ParameterType == typeof(double))
                        {
                            // This is set(double, double). 
                            object[] args = new object[2];
                            args[0] = 1.12;
                            args[1] = 23.4;
                            m.Invoke(reflectOb, args);
                        }
                        else if (m.Name.CompareTo("sum") == 0)
                        {
                            val = (int)m.Invoke(reflectOb, null);
                            SetMessage("sum is " + val);
                        }
                        else if (m.Name.CompareTo("isBetween") == 0)
                        {
                            object[] args = new object[1];
                            args[0] = 14;
                            if ((bool)m.Invoke(reflectOb, args))
                                SetMessage("14 is between x and y");
                        }
                        else if (m.Name.CompareTo("show") == 0)
                        {
                            m.Invoke(reflectOb, null);
                        }
                    } 
                    */
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else 
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }

        static public void StatusUpdateCallback(uint sessionId)
        {
            float progress;
            double total_time;

            CSycles.progress_get_progress(Client.Id, sessionId, out progress, out total_time);
            var status = CSycles.progress_get_status(Client.Id, sessionId);
            var substatus = CSycles.progress_get_substatus(Client.Id, sessionId);
            uint samples;
            uint num_samples;

            CSycles.tilemanager_get_sample_info(Client.Id, sessionId, out samples, out num_samples);

            if (status.Equals("Finished"))
            {
                Console.WriteLine("wohoo... :D");
            }

            status = "[" + status + "]";
            if (!substatus.Equals(string.Empty)) status = status + ": " + substatus;
            Console.WriteLine("C# status update: {0} {1} {2} {3} <|> {4:N}s {5:P}", CSycles.progress_get_sample(Client.Id, sessionId), status, samples, num_samples, total_time, progress);
        }

        static public void WriteRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
        {
            Console.WriteLine("C# Write Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
        }

        public static void UpdateRenderTileCallback(uint sessionId, uint x, uint y, uint w, uint h, uint depth)
        {
            Console.WriteLine("C# Update Render Tile for session {0} at ({1},{2}) [{3}]", sessionId, x, y, depth);
        }

        /// <summary>
        /// Callback for debug logging facility. Will be called only for Debug builds of ccycles.dll
        /// </summary>
        /// <param name="msg"></param>
        public static void LoggerCallback(string msg)
        {
            Console.WriteLine("DBG: {0}", msg);
        }

        public static int ColorClamp(int ch)
        {
            if (ch < 0) return 0;
            return ch > 255 ? 255 : ch;
        }

        public static float DegToRad(float ang)
        {
            return ang * (float)Math.PI / 180.0f;
        }

        private static CSycles.UpdateCallback g_update_callback;
        private static CSycles.RenderTileCallback g_update_render_tile_callback;
        private static CSycles.RenderTileCallback g_write_render_tile_callback;

        private static CSycles.LoggerCallback g_logger_callback;

        public static void SetMessage(string msg)
        {
            _iui.SetText(msg);
        }

        public static void Initiate()
        {
            /*
            if (!AllocConsole())
                MessageBox.Show("Failed");
            */

            SetMessage("Reading input...");

            string materialFile = _iui.getMaterialFileName();//Path.GetFullPath(_iui.getMaterialFileName());
            string sceneFile = _iui.getSceneFileName();//Path.GetFullPath(_iui.getSceneFileName());

            CSycles.set_kernel_path("lib");
            CSycles.initialise();

            SetMessage("Initialising callbacks...");

            g_update_callback = StatusUpdateCallback;
            g_update_render_tile_callback = UpdateRenderTileCallback;
            g_write_render_tile_callback = WriteRenderTileCallback;
            g_logger_callback = LoggerCallback;

            SetMessage("Initialising client...");

            var client = new Client();
            Client = client;
            CSycles.set_logger(client.Id, g_logger_callback);


            SetMessage("Available devices-> Device 0: " + Device.GetDevice(0).Name + ", Device 1: " + Device.GetDevice(1).Name + ", Device 2: " + Device.GetDevice(2).Name + ", Cuda available: " + Device.CudaAvailable() + ", Cuda device: " + Device.FirstCuda.Name);


            SetMessage("Selecting devices...");
            var dev = Device.FirstCuda;
            Device = dev;

            SetMessage("Using device " + dev.Name);

            SetMessage("Creating default scene...");

            #region Create default scene

            var scene_params = new SceneParameters(client, ShadingSystem.SVM, BvhType.Static, true, false, false, false);
            var scene = new Scene(client, scene_params, dev);

            SetMessage("Setup Camera...");
            /* move the scene camera a bit, so we can see our render result. */
            var t = Transform.Identity();
            float angle = 90.0f;
            t = t * Transform.Rotate(angle * (float)Math.PI / 180.0f, new float4(0.0f, 1.0f, 0.0f));
            t = t * Transform.Translate(0.0f, 0.0f, 4.0f);

            scene.Camera.Matrix = t;
            /* set also the camera size = render resolution in pixels. Also do some extra settings. */
            scene.Camera.Size = new Size((int)width, (int)height);
            scene.Camera.Type = CameraType.Perspective;
            scene.Camera.ApertureSize = 0.0f;
            scene.Camera.Fov = 0.661f;
            //scene.Camera.FocalDistance = 0.0f;
            scene.Camera.SensorWidth = 32.0f;
            scene.Camera.SensorHeight = 18.0f;

            if (string.IsNullOrEmpty(materialFile))
            {
                
            }
            else
            {
                
            }

            #endregion

            SetMessage("Creating default shader...");

            #region default shader

            /* Create a simple surface shader and make it the default surface shader */
            var background = new Shader(Client, Shader.ShaderType.World)
            {
                Name = "SmallCSycles material"
            };

            var bgnode = new BackgroundNode();
            bgnode.ins.Color.Value = new float4(1.0f, 0.64f, 0.0f); //255-165-0
            //bgnode.ins.Strength.Value = 2.0f;

            background.AddNode(bgnode);
            bgnode.outs.Background.Connect(background.Output.ins.Surface);

            background.FinalizeGraph();

            scene.AddShader(background);
            scene.Background.Shader = background;
            scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
            
            if (string.IsNullOrEmpty(materialFile))
            {
                
            }
            else
            {
                /*
                var diffuse_shader = read_shader_from_file(materialFile);
                scene.AddShader(diffuse_shader);
                scene.DefaultSurface = diffuse_shader;
                 * */
            }
            #endregion

            SetMessage("Creating background shader...");

            #region background shader
            /*
            var background = new Shader(Client, Shader.ShaderType.World)
            {
                Name = "SmallCSycles world",
                UseMis = false,
                UseTransparentShadow = true,
                HeterogeneousVolume = false
            };

            var bgnode = new BackgroundNode();
            bgnode.ins.Color.Value = new float4(0.2f, 0.4f, 0.8f);
            //bgnode.ins.Strength.Value = 2.0f;

            background.AddNode(bgnode);
            bgnode.outs.Background.Connect(background.Output.ins.Surface);

            background.FinalizeGraph();

            scene.AddShader(background);
            scene.Background.Shader = background;
            scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;

             * */

            var some_setup = new Shader(Client, Shader.ShaderType.Material)
            {
                Name = "some_setup"
            };


            var brick_texture = new BrickTexture();
            brick_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Color1.Value = new float4(0.800f, 0.800f, 0.800f);
            brick_texture.ins.Color2.Value = new float4(0.200f, 0.200f, 0.200f);
            brick_texture.ins.Mortar.Value = new float4(0.000f, 0.000f, 0.000f);
            brick_texture.ins.Scale.Value = 1.000f;
            brick_texture.ins.MortarSize.Value = 0.020f;
            brick_texture.ins.Bias.Value = 0.000f;
            brick_texture.ins.BrickWidth.Value = 0.500f;
            brick_texture.ins.RowHeight.Value = 0.250f;

            var checker_texture = new CheckerTexture();
            checker_texture.ins.Vector.Value = new float4(0.000f, 0.000f, 0.000f);
            checker_texture.ins.Color1.Value = new float4(0.000f, 0.004f, 0.800f);
            checker_texture.ins.Color2.Value = new float4(0.200f, 0.000f, 0.007f);
            checker_texture.ins.Scale.Value = 5.000f;

            var diffuse_bsdf = new DiffuseBsdfNode();
            diffuse_bsdf.ins.Color.Value = new float4(0.800f, 0.800f, 0.800f);
            diffuse_bsdf.ins.Roughness.Value = 0.000f;
            diffuse_bsdf.ins.Normal.Value = new float4(0.000f, 0.000f, 0.000f);

            var texture_coordinate = new TextureCoordinateNode();


            some_setup.AddNode(brick_texture);
            some_setup.AddNode(checker_texture);
            some_setup.AddNode(diffuse_bsdf);
            some_setup.AddNode(texture_coordinate);

            brick_texture.outs.Color.Connect(diffuse_bsdf.ins.Color);
            checker_texture.outs.Color.Connect(brick_texture.ins.Mortar);
            texture_coordinate.outs.Normal.Connect(checker_texture.ins.Vector);
            texture_coordinate.outs.UV.Connect(brick_texture.ins.Vector);

            diffuse_bsdf.outs.BSDF.Connect(some_setup.Output.ins.Surface);

            some_setup.FinalizeGraph();

            scene.AddShader(some_setup);
            scene.DefaultSurface = some_setup;
            //scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;


            #endregion

            /* get scene-specific default shader ID */
            var default_shader = scene.ShaderSceneId(scene.DefaultSurface);

            SetMessage("Set integrator settings...");
            /* Set integrator settings */
            scene.Integrator.IntegratorMethod = IntegratorMethod.Path;
            scene.Integrator.MaxBounce = 1;
            scene.Integrator.MinBounce = 1;
            scene.Integrator.NoCaustics = true;
            scene.Integrator.MaxDiffuseBounce = 1;
            scene.Integrator.MaxGlossyBounce = 1;
            scene.Integrator.Seed = 1;
            scene.Integrator.SamplingPattern = SamplingPattern.Sobol;
            scene.Integrator.FilterGlossy = 0.0f;

            SetMessage("Add geometry to scene...");
            /* Add a bit of geometry and move camera around so we can see what we're rendering.
             * First off we need an object, we put it at the origo
             */
            var ob = CSycles.scene_add_object(Client.Id, scene.Id);
            CSycles.object_set_matrix(Client.Id, scene.Id, ob, Transform.Identity());
            /* Now we can create a mesh, attached to the object */
            var mesh = CSycles.scene_add_mesh(Client.Id, scene.Id, ob, default_shader);
            
            /* populate mesh with geometry */
            
            CSycles.mesh_set_verts(Client.Id, scene.Id, mesh, ref vert_floats, (uint)(vert_floats.Length / 3));
            var index_offset = 0;
            foreach (var face in nverts)
            {
                for (var j = 0; j < face - 2; j++)
                {
                    var v0 = (uint)vertex_indices[index_offset];
                    var v1 = (uint)vertex_indices[index_offset + j + 1];
                    var v2 = (uint)vertex_indices[index_offset + j + 2];

                    CSycles.mesh_add_triangle(Client.Id, scene.Id, mesh, v0, v1, v2, default_shader, false);
                }

                index_offset += face;
            }
            


            #region point light shader

            var light_shader = new Shader(client, Shader.ShaderType.Material)
            {
                Name = "Tester light shader"
            };

            var emission_node = new EmissionNode();
            emission_node.ins.Color.Value = new float4(0.8f);
            emission_node.ins.Strength.Value = 10.0f;

            light_shader.AddNode(emission_node);
            emission_node.outs.Emission.Connect(light_shader.Output.ins.Surface);
            light_shader.FinalizeGraph();
            scene.AddShader(light_shader);
            #endregion
            
            
            Scene = scene;

            CSycles.shutdown();

            Initialised = true;

            //CSycles.shutdown();

            /*

            #region background shader
            var background_shader = new Shader(client, Shader.ShaderType.World)
            {
                Name = "Background shader"
            };

            var bgnode = new BackgroundNode();
            bgnode.ins.Color.Value = new float4(0.0f);
            bgnode.ins.Strength.Value = 1.0f;

            background_shader.AddNode(bgnode);
            bgnode.outs.Background.Connect(background_shader.Output.ins.Surface);
            background_shader.FinalizeGraph();

            scene.AddShader(background_shader);

            scene.Background.Shader = background_shader;
            scene.Background.AoDistance = 0.0f;
            scene.Background.AoFactor = 0.0f;
            scene.Background.Visibility = PathRay.PATH_RAY_ALL_VISIBILITY;
            #endregion
            #region diffuse shader

            var diffuse_shader = create_some_setup_shader();
            scene.AddShader(diffuse_shader);
            scene.DefaultSurface = diffuse_shader;
            #endregion

            

            var xml = new XmlReader(client, sceneFile);
            xml.Parse();
            var width = (uint)scene.Camera.Size.Width;
            var height = (uint)scene.Camera.Size.Height;
            */

            /* move the scene camera a bit, so we can see our render result. */
            /*
            var t = Transform.Identity();
            t = t * Transform.Rotate(75.0f * (float)Math.PI / 180.0f, new float4(0.0f, 1.0f, 1.0f));
            
            t = t * Transform.Translate(50.0f, 2.0f * (float)tile, -10f);//2.0f * (float)tile
            //scene.Camera.Matrix.x = new float4((float)(2.0f * (float)tile), (float)0.0f, (float)0.0f);

             * */

            /*
            var transform = Transform.Identity();
            //rotate
            var angle = DegToRad(75.0f);
            var axis = new float4(0.0f, 1.0f, 1.0f);
            transform = transform * ccl.Transform.Rotate(angle, axis);
            //translate
            float tr = 2.0f * (float)tile;
            _iui.SetText("Moving camera: " + tr);
            transform = transform * ccl.Transform.Translate(0.000f, -2.302f+ tr, 0.617f);
            scene.Camera.Matrix = transform;
            */

        }

        public static void Render(int tile)
        {
            CSycles.set_kernel_path("lib");
            CSycles.initialise();

            /*
            g_update_callback = StatusUpdateCallback;
            g_update_render_tile_callback = UpdateRenderTileCallback;
            g_write_render_tile_callback = WriteRenderTileCallback;
            g_logger_callback = LoggerCallback;
             * */

            string outFolder = _iui.getOutputFolderName();//Path.GetFullPath(_iui.getOutputFolderName());
            
            _iui.SetText("Setting up session parameters...");

            var session_params = new SessionParameters(Client, Device)
            {
                Experimental = false,
                Samples = (int)samples,
                TileSize = new Size(64, 64),
                StartResolution = 64,
                Threads = 1,
                ShadingSystem = ShadingSystem.SVM,
                Background = true,
                ProgressiveRefine = false
            };
            Session = new Session(Client, session_params, Scene);
            Session.Reset(width, height, samples);

            Session.UpdateCallback = g_update_callback;
            Session.UpdateTileCallback = g_update_render_tile_callback;
            Session.WriteTileCallback = g_write_render_tile_callback;
            
            
            _iui.SetText("Rendering tile " + tile + ": " + CSycles.progress_get_status(Client.Id, Session.Id));

            //_iui.SetText("Devices: " + Device.GetDevice(2).Name);
            //_iui.SetText("Device 0: " + Device.GetDevice(0).Name + ", Device 1: " + Device.GetDevice(1).Name + ", Device 2: " + Device.GetDevice(2).Name + ", Cuda available: " + Device.CudaAvailable() + ", Cuda device: " + Device.FirstCuda.Name);
            
            Session.Start();
            Session.Wait();

            uint bufsize;
            uint bufstride;
            CSycles.session_get_buffer_info(Client.Id, Session.Id, out bufsize, out bufstride);
            var pixels = CSycles.session_copy_buffer(Client.Id, Session.Id, bufsize);

            _iui.SetText("Creating bitmap...");

            var bmp = new Bitmap((int)width, (int)height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var i = y * (int)width * 4 + x * 4;
                    var r = ColorClamp((int)(pixels[i] * 255.0f));
                    var g = ColorClamp((int)(pixels[i + 1] * 255.0f));
                    var b = ColorClamp((int)(pixels[i + 2] * 255.0f));
                    var a = ColorClamp((int)(pixels[i + 3] * 255.0f));
                    bmp.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }
            _iui.SetText("Saving bitmap...");

            if (string.IsNullOrEmpty(outFolder))
                bmp.Save("test"+tile+".bmp");
            else
                bmp.Save(outFolder+"\\test"+tile+".bmp");

            _iui.SetText("Done");

            CSycles.shutdown();
        }
    }

    static class Program
    {        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Form form = new Form1();
            Application.Run(new Form1());
        }
    }
}
