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

using Voxel_Engine.Utility;
using Voxel_Engine.GUI;
using Voxel_Engine.Rendering;

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
            window.UpdateFrame  += OnUpdate;
            window.RenderFrame  += OnRender;
            window.Load         += OnLoad;
            window.Resize       += OnResize;

            window.Run();
            window.Dispose();
        }

        private static void OnResize(ResizeEventArgs obj)
        {
            window.Size = obj.Size;
            Camera.Main?.FitToScreen();
        }

        private static void OnUpdate(FrameEventArgs e)
        {
            Input.Update();
        }
        static bool asd = true;
        private static void OnRender(FrameEventArgs e)
        {

            
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
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            if (asd)
            {
                asd = false;
            }
        }
        private static void OnLoad()
        {
            Utility.Debug.DebugTools.Enable();

            UI.Init(window);
            Input.Initialize(window);
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }
    }
}
