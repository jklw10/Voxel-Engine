using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Windowing.Common;
using ImGuiNET;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Voxel_Engine.GUI
{
    public static class UI
    {
        static ImGuiController? controller;
        static GameWindow? Window;
        public static bool IsInitialized    { get; private set; }
        public static bool IsActive         { get; private set; }
        public static Action? ImGuiAction   { get; set; }

        public static void Init(GameWindow window)
        {
            Window = window;

            Window.Resize       += OnResize;
            Window.TextInput    += OnTextInput;
            Window.MouseWheel   += OnMouseWheel;

            controller = new ImGuiController(1,1);
            IsInitialized = true;
            IsActive = true;

        }
        /// <summary>
        /// makes cursor visible, and uncontained by screen
        /// </summary>
        public static void Detach()
        {
            Engine.Window.CursorState = CursorState.Normal;
        }
        public static void ToggleMouseAttachment()
        {
            if (Engine.Window.CursorState != CursorState.Normal)
            {
                Detach();
            }
            else
            {
                Attach();
            }
        }
        /// <summary>
        /// makes cursor invisible, and contained to screen
        /// </summary>
        public static void Attach()
        {
            Engine.Window.CursorState = CursorState.Grabbed;
        }
        public static void OnResize(ResizeEventArgs obj)
        {
            // Tell ImGui of the new size
            controller?.WindowResized(Window?.ClientSize.X ?? 0, Window?.ClientSize.Y ?? 0);
        }
        public static void Render(FrameEventArgs e)
        {
            if (IsInitialized && IsActive)
            {
                if (Window is null)throw new ApplicationException("what? UI render was called without a window");
                
                controller?.Update(Window, (float)e.Time);
                ImGuiAction?.Invoke();
                controller?.Render();

                Util.CheckGLError("End of imgui frame");
            }
        }
        public static void OnTextInput(TextInputEventArgs e)
        {
            controller?.PressChar((char)e.Unicode);
        }
        public static void OnMouseWheel(MouseWheelEventArgs e)
        {
            controller?.MouseScroll(e.Offset);
        }
        public static void Style()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.TabRounding, 0);
        }
    }
}
