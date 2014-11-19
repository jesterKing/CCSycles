using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ccl;
using ccl.ShaderNodes;

namespace HologramPrinter
{
    public static class Dynamic_Shader 
    {
        private static Client client;
        private static Device device;
        private static Scene scene;

        public static Client Client 
        {
            set
            {
                client = Client;
            }
            get
            {
                return client;
            }
        }
        public static Device Device 
        {
            set
            {
                device = Device;
            }
            get
            {
                return device;
            }
        }
        public static Scene Scene 
        {
            set
            {
                scene = Scene;
            }
            get
            {
                return scene;
            }
        }
        
        public static Shader Show(Client cl, Device dv, Scene sc, Shader.ShaderType st)
        {
            Client = cl;
            Device = dv;
            Scene = sc;

            #region background shader
            var background_shader = new Shader(cl, st)
            {
                Name = "Background shader"
            };

            var bgnode = new BackgroundNode();
            bgnode.ins.Color.Value = new float4(0.7f);
            bgnode.ins.Strength.Value = 1.0f;

            background_shader.AddNode(bgnode);
            bgnode.outs.Background.Connect(background_shader.Output.ins.Surface);
            background_shader.FinalizeGraph();

            sc.AddShader(background_shader);
            sc.Background.Shader = background_shader; 

            return background_shader;
            #endregion
        }
    }
}
