namespace Voxel_Engine;

using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

public record EventLoopRunnable
{
    public record OnLoad(Action Act) : EventLoopRunnable;
    public record OnRender(Action<FrameEventArgs> Act) : EventLoopRunnable;
    public record OnUpdate(Action<FrameEventArgs> Act) : EventLoopRunnable;
    public record OnResize(Action<ResizeEventArgs> Act) : EventLoopRunnable;
}

public class Engine
{
    
    static public readonly GameWindow Window = new(new(), new())
    {
        Size = new(800, 600)
    };
    /// <summary>
    /// Creates the game window
    /// </summary>
    public static void CreateWindow(params EventLoopRunnable[] items)
    {
        foreach (var item in items)
        {
            switch(item)
            {
                case EventLoopRunnable.OnLoad(Action act): 
                    Window.Load += act;
                    break;
                case EventLoopRunnable.OnRender(Action<FrameEventArgs> act):
                    Window.RenderFrame += act;
                    break;
                case EventLoopRunnable.OnUpdate(Action<FrameEventArgs> act):
                    Window.UpdateFrame += act;
                    break;
                case EventLoopRunnable.OnResize(Action<ResizeEventArgs> act):
                    Window.Resize += act;
                    break;
            }
        }
        Window.Load += OnLoad;
        Window.Resize += OnResize;
        Window.UpdateFrame += OnUpdate;
        Window.RenderFrame += OnRender;

        Window.Run();
        Window.Dispose();
    }

    static void OnResize(ResizeEventArgs e)
    {
        Console.WriteLine($"x:{e.Width} y:{e.Height} ");
        //Window.Size = e.Size;
        Camera.Main?.FitToScreen();
        UI.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    static void OnUpdate(FrameEventArgs e)
    {
        Time.PhysicsUpdate();
        Input.Update();

    }
    static void OnRender(FrameEventArgs e)
    {
        ChunkStreamer.Main?.Update();
        Time.Update();
        Camera.Render();
        UI.Render(e);
        Window.SwapBuffers();
    }

    static void OnLoad()
    {
        Utility.Debug.DebugTools.Enable();

        UI.Init(Window);
        Input.Initialize(Window);
        GL.ClearColor(new Color4(125, 125, 255, 255));
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }
}



