namespace Voxel_Engine.Controls;

using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;

public static class Input
{
    private static readonly HashSet<Keys> keysDown = new();
    private static readonly HashSet<MouseButton> buttonsDown = new();
    private static HashSet<Keys> keysDownLast = new();
    private static HashSet<MouseButton> buttonsDownLast = new();

    public static void Initialize(GameWindow game)
    {
        game.MouseDown += MouseDown;
        game.MouseUp += MouseUp;
        game.KeyDown += KeyDown;
        game.KeyUp += KeyUp;
    }

    static void KeyDown(KeyboardKeyEventArgs e)
    {
        if (!keysDown.Contains(e.Key))
            keysDown.Add(e.Key);
    }
    static void KeyUp(KeyboardKeyEventArgs e)
    {
        if (keysDown.Contains(e.Key))
            keysDown.Remove(e.Key);
    }

    static void MouseDown(MouseButtonEventArgs e)
    {
        if (!buttonsDown.Contains(e.Button))
            buttonsDown.Add(e.Button);
    }
    static void MouseUp(MouseButtonEventArgs e)
    {
        if (buttonsDown.Contains(e.Button))
            buttonsDown.Remove(e.Button);
    }

    public static void Update()
    {
        keysDownLast = new(keysDown);
        buttonsDownLast = new(buttonsDown);
    }

    public static bool KeyPress(Keys key)
    {
        return keysDown.Contains(key) && !keysDownLast.Contains(key);
    }
    public static bool KeyRelease(Keys key)
    {
        return !keysDown.Contains(key) && keysDownLast.Contains(key);
    }
    public static bool KeyDown(Keys key)
    {
        return keysDown.Contains(key);
    }

    public static bool MousePress(MouseButton button)
    {
        return buttonsDown.Contains(button) && !buttonsDownLast.Contains(button);
    }
    public static bool MouseRelease(MouseButton button)
    {
        return !buttonsDown.Contains(button) && buttonsDownLast.Contains(button);
    }
    public static bool MouseDown(MouseButton button)
    {
        return buttonsDown.Contains(button);
    }
}


