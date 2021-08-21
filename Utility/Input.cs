using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;


namespace Voxel_Engine.Utility
{
	public static class Input
	{
		private static readonly List<Keys>			keysDown		= new();
		private static readonly List<MouseButton>	buttonsDown		= new();
		private static List<Keys>			keysDownLast	= new();
		private static List<MouseButton>	buttonsDownLast	= new();

		public static void Initialize(GameWindow game)
		{
            game.MouseDown	+= Game_MouseDown;
			game.MouseUp	+= Game_MouseUp;
			game.KeyDown	+= Game_KeyDown;
			game.KeyUp		+= Game_KeyUp;
		}


        static void Game_KeyDown( KeyboardKeyEventArgs e)
		{
			if (!keysDown.Contains(e.Key))
				keysDown.Add(e.Key);
		}
		static void Game_KeyUp( KeyboardKeyEventArgs e)
		{
			while (keysDown.Contains(e.Key))
				keysDown.Remove(e.Key);
		}

		static void Game_MouseDown( MouseButtonEventArgs e)
		{
			if (!buttonsDown.Contains(e.Button))
				buttonsDown.Add(e.Button);
		}
		static void Game_MouseUp( MouseButtonEventArgs e)
		{
			while (buttonsDown.Contains(e.Button))
				buttonsDown.Remove(e.Button);
		}

		public static void Update()
		{
			keysDownLast = new List<Keys>(keysDown);
			buttonsDownLast = new List<MouseButton>(buttonsDown);
		}

		public static bool KeyPress(Keys key)
		{
			return (keysDown.Contains(key) && !keysDownLast.Contains(key));
		}
		public static bool KeyRelease(Keys key)
		{
			return (!keysDown.Contains(key) && keysDownLast.Contains(key));
		}
		public static bool KeyDown(Keys key)
		{
			return (keysDown.Contains(key));
		}

		public static bool MousePress(MouseButton button)
		{
			return (buttonsDown.Contains(button) && !buttonsDownLast.Contains(button));
		}
		public static bool MouseRelease(MouseButton button)
		{
			return (!buttonsDown.Contains(button) && buttonsDownLast.Contains(button));
		}
		public static bool MouseDown(MouseButton button)
		{
			return (buttonsDown.Contains(button));
		}
	}
}
