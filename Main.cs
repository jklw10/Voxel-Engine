using System;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

using Voxel_Engine.Utility;
using Voxel_Engine.GUI;
using Voxel_Engine.Rendering;


namespace Voxel_Engine
{
    public class Engine
    {
        static public readonly GameWindow window = new(new GameWindowSettings(), new NativeWindowSettings())
        {
            Size = new(800, 600)
        };
        /// <summary>
        /// Creates the game window
        /// </summary>
        public static void CreateWindow()
        {
            Reflector.Init();
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
            UI.OnResize(obj);
            GL.Viewport(0,0,obj.Width,obj.Height);
        }

        private static void OnUpdate(FrameEventArgs e)
        {
            Time.PhysicsUpdate();
            Input.Update();
        }
        private static void OnRender(FrameEventArgs e)
        {
            Time.Update(); 
            Camera.Main?.Render(true);
            UI.Render(e);
            //if(Camera.Debug is object)Camera.DontTestDepth();
            //Camera.Debug?.Render(false);
            TrySwapBuffer();
        }
        public static void TrySwapBuffer()
        {
            try { 
                window.SwapBuffers(); 
            }catch (Exception ex) {
                Console.WriteLine(ex);
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
