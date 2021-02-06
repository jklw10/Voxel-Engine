using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Linq;

namespace Voxel_Engine.Utility
{
    static class Extensions
    {
        public static Vector3i Modulo(this Vector3i a, Vector3i n)
        {
            return (Vector3i)(a - (n * Vector3.Divide(a, n).Floor()));
        }
        public static Vector3 Floor(this Vector3 a)
        {
            return new Vector3((int)a.X, (int)a.Y, (int)a.Z);
        }
        public static void Fill<T>(this T[] src, T value)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src[i] = value;
            }
        }

        public static void Fill<T>(this List<T> src, T value)
        {
            for (int i = 0; i < src.Count; i++)
            {
                src[i] = value;
            }
        }
        public static void ShiftLeft<T>(this T[] array)
        {
            Array.Copy(array, 1, array, 0, array.Length - 1);
        }
    }

    class Tools
    {
        public static Vector3i[] directions = new Vector3i[]
        {
            Vector3i.UnitX,
            -Vector3i.UnitX,
            Vector3i.UnitY,
            -Vector3i.UnitY,
            Vector3i.UnitZ,
            -Vector3i.UnitZ,
        };
        
    }
}

namespace Voxel_Engine.Utility.Debug
{
    class DebugTools
    {
        private static DebugProc _debugProcCallback = DebugCallback;
        private static GCHandle _debugProcCallbackHandle;
        private static void DebugCallback(DebugSource source,
                                  DebugType type,
                                  int id,
                                  DebugSeverity severity,
                                  int length,
                                  IntPtr message,
                                  IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(messageString);
            }
        }

        [Conditional("DEBUG")]
        public static void Enable(){
            _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

            GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DebugOutputSynchronous);
        }
    }
}
