using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Utility
{
    using DataHandling;
    // https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
    /// <summary>
    /// Comparer for comparing two keys, handling equality as beeing greater
    /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey>
                    :
                 IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey? x, TKey? y)
        {
            int result = x?.CompareTo(y) ?? -1;

            if (result == 0)
                return 1; // Handle equality as being greater. Note: this will break Remove(key) or
            else          // IndexOfKey(key) since the comparer never returns 0 to signal key equality
                return result;
        }

        #endregion
    }
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct ChunkIterator
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public static readonly ChunkIterator MAX = new(15, 15, 15);
        public byte X;
        public byte Y;
        public byte Z;

        public ChunkIterator(byte x = 0, byte y = 0, byte z = 0) 
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static bool operator ==(ChunkIterator a, Vector3i b)
        {
            return a.X == b.X
                && a.Y == b.Y
                && a.Z == b.Z;
        }
        public static bool operator !=(ChunkIterator a, Vector3i b)
        {
            return !(a == b);
        }
        public ChunkIterator Next()
        {

            if (Z != Chunk.Size) { X++; }
            if (X == Chunk.Size) { Y++; X = 0; }
            if (Y == Chunk.Size) { Z++; Y = 0; }
            return this;
        }
        public static implicit operator int(ChunkIterator CI) => CI.X+ CI.Y*Chunk.Size+ CI.Z*Chunk.Size*Chunk.Size;
        public static implicit operator Vector3i(ChunkIterator CI) => new(CI.X, CI.Y, CI.Z);

    }

    static class Extensions
    {
        public static Vector3i Abs(this Vector3i a)
        {
            return new Vector3i(Math.Abs(a.X ), Math.Abs(a.Y ), Math.Abs(a.Z ));
        }
        public static Vector3i Bitshift(this Vector3i v, int number)
        {
            int x = v.X >> number;
            int y = v.Y >> number;
            int z = v.Z >> number;
            return new Vector3i(x, y, z);
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

    public class Tools
    {
        static readonly double goldenRatio = (1 + Math.Sqrt(5)) / 2;
        static readonly double angleIncrement = Math.Tau * goldenRatio;
        public static float[] SpherePoints(int n)
        {
            float[] ret = new float[n*3];
            for (int i = 0; i < n; i++)
            {
                double t = (double)i / n;
                double angle1 = Math.Acos(1 - 2 * t);
                double angle2 = angleIncrement * i;

                float x = (float)(Math.Sin(angle1) * Math.Cos(angle2));
                float y = (float)(Math.Sin(angle1) * Math.Sin(angle2));
                float z = (float)(Math.Cos(angle1));

                ret[i*3+0] = x;
                ret[i*3+1] = y;
                ret[i*3+2] = z;
            }
            return ret;
        }
        public readonly static Vector3i[] directions = new Vector3i[]
        {
            Vector3i.UnitX,
            -Vector3i.UnitX,
            Vector3i.UnitY,
            -Vector3i.UnitY,
            Vector3i.UnitZ,
            -Vector3i.UnitZ,
        };
        public readonly static Vector3i[] octPositions = new Vector3i[]
        {
           new(0),
           Vector3i.UnitX,
           Vector3i.UnitY,
           Vector3i.UnitY+Vector3i.UnitX,
           Vector3i.UnitZ,
           Vector3i.UnitZ+Vector3i.UnitX,
           Vector3i.UnitZ+Vector3i.UnitY,
           Vector3i.UnitZ+Vector3i.UnitY+Vector3i.UnitX
        };
        public static Action Once(Action action)
        {
            var context = new ContextCallOnlyOnce();
            void ret()
            {
                if (false == context.AlreadyCalled)
                {
                    action();
                    context.AlreadyCalled = true;
                }
            }

            return ret;
        }
        class ContextCallOnlyOnce
        {
            public bool AlreadyCalled;
        }

    }
}

namespace Voxel_Engine.Utility.Debug
{
    class DebugTools
    {

#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable IDE0044 // Add readonly modifier
        private static DebugProc _debugProcCallback = DebugCallback;
#pragma warning restore IDE0044 // Add readonly modifier
        private static GCHandle _debugProcCallbackHandle;
#pragma warning restore IDE0052 // Remove unread private members
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
