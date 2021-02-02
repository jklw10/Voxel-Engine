using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

using System.Drawing;
using OpenTK.Windowing.Common;

using ImGuiNET;

namespace Voxel_Engine
{
    public class Engine
    {
        static public GameWindow window = new GameWindow(new GameWindowSettings(), new NativeWindowSettings())
        {
            Size = new OpenTK.Mathematics.Vector2i(800, 600)
        };
        public static void CreateWindow()
        {
            window.UpdateFrame += OnUpdate;
            window.RenderFrame += OnRender;
            window.Load += OnLoad;


            window.Run();
            window.Dispose();
        }


        private static void OnUpdate(FrameEventArgs e)
        {
            Input.Update();
        }
        private static void OnRender(FrameEventArgs e)
        {

            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            if (Camera.Main is object)
            {
                Camera.Main.RenderCamera(true);
            }
            if (UI.IsInitialized && UI.IsActive)
            {
                UI.Render(e);
            }
            try
            {
                window.SwapBuffers();
                GL.Flush();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private static void OnLoad()
        {
            UI.Init(window);
            Input.Initialize(window);


            GL.ClearColor(Color.DarkGray);
        }
    }
}
