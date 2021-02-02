using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Windowing.Common;
using ImGuiNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Voxel_Engine
{
    public static class UI
    {
        static ImGuiController? controller;
        static GameWindow? Window;
        public static bool IsInitialized { get; private set; }
        public static bool IsActive;
        public static Action? ImGuiAction;
        public static void Init(GameWindow window)
        {
            Window = window;

            Window.Resize       += OnResize;
            Window.TextInput    += OnTextInput;
            Window.MouseWheel   += OnMouseWheel;

            controller = new ImGuiController(1,1);
            IsInitialized = true;
            Vector3 a = (Vector3)new Vector3d(2, 2,2);
            
        }
        public static void OnResize(ResizeEventArgs obj)
        {
            // Tell ImGui of the new size
            controller?.WindowResized(Window?.ClientSize.X ?? 0, Window?.ClientSize.Y ?? 0);
        }
        public static void Render(FrameEventArgs e)
        {
            if (Window is null)
            {
                throw new Exception("what? UI render was called without a window attachment");
            }
            controller?.Update(Window, (float)e.Time);
            if (ImGuiAction is object)
            {
                ImGuiAction.Invoke();
            }
            else
            {
               // ImGui.ShowDemoWindow();
            }

            controller?.Render();

            Util.CheckGLError("End of frame");
        }
        public static void OnTextInput(TextInputEventArgs e)
        {
            controller?.PressChar((char)e.Unicode);
        }
        public static void OnMouseWheel(MouseWheelEventArgs e)
        {
            controller?.MouseScroll(e.Offset);
        }
    }
}
