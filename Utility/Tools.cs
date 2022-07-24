using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Utility
{
    using DataHandling;
    using System.Collections;

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
        public static implicit operator int(ChunkIterator CI) => CI.X + (CI.Y + CI.Z * Chunk.Size) * Chunk.Size;
        public static implicit operator Vector3i(ChunkIterator CI) => new(CI.X, CI.Y, CI.Z);

    }
    public class Plane : IEnumerable
    {
        Vector3 center;
        Vector3i size;
        Vector3i dir;

        public Plane(Vector3 center, Vector3i size, Vector3i dir)
        {
            this.center = center;
            this.size = size;
            this.dir = dir;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public PlaneEnum GetEnumerator()
        {
            return new PlaneEnum(center,size,dir);
        }
    }
    public class PlaneEnum : IEnumerator
    {
        object IEnumerator.Current { get => current; }
        public Vector3 Current { get => current; }
        public Vector3 current;
        Vector3i hSize;
        Vector3 absD;
        Vector3 notdir;
        Vector3 planeCorner;
        int xmax;
        int ymax;

        int x;
        int y;

        public PlaneEnum(Vector3 center,Vector3i size, Vector3i dir)
        {
            hSize = (size / 2);
            absD = dir.Abs();
            notdir = new Vector3(1) - dir.Abs();
            planeCorner = center + notdir * -hSize;

            xmax = (int)(notdir.X * size.X + notdir.Y * size.Y * absD.X);

            ymax = (int)(notdir.Y * size.Y * absD.Z + //use Y side if X is iterated and Z is not
                         notdir.Z * size.Z * absD.X + //use Z if X isn't iterable (plane normal has X component)
                         notdir.Z * size.Z * absD.Y);
            
            x = -1;
            y = 0;
        }

        public bool MoveNext()
        {
            x++;
            if (x >= xmax) { y++; x = 0; }

            current = planeCorner;
            // determine what dimension the x and y iteration should increase int the plane
            // if plane isn't in X direction notdir.X is 0 which means it multiplies x by 0 becoming 0
            // thus making it not choose X dimension as an iteration target
            current.X += notdir.X * x;
            //rince repeat
            //Y dimension might be iterated by either x or y 
            current.Y += notdir.Y * x * absD.X; //if X is in use, can't use x to iterate Y
            current.Y += notdir.Y * y * absD.Z; //if Z is in use, can't use y to iterate Y

            //Z dimension is only iterated by y and when
            current.Z += notdir.Z * y * absD.X; //X and
            current.Z += notdir.Z * y * absD.Y; //Y aren't already iterate through
                                          
            return y < ymax;
        }
        public void Reset()
        {
            x = -1;
            y = 0;
        }
    }

    public static class Extensions
    {
        public static Vector3i Abs(this Vector3i a)
        {
            return new Vector3i(Math.Abs(a.X ), Math.Abs(a.Y ), Math.Abs(a.Z ));
        }
        public static Vector3 Abs(this Vector3 a)
        {
            return new Vector3(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z));
        }
        public static Vector4 Abs(this Vector4 a)
        {
            return new Vector4(Math.Abs(a.X), Math.Abs(a.Y), Math.Abs(a.Z), Math.Abs(a.W));
        }
        public static Vector3 Sign(this Vector3 a)
        {
            return new Vector3(Math.Sign(a.X), Math.Sign(a.Y), Math.Sign(a.Z));
        }
        public static Vector3i Bitshift(this Vector3i v, int number)
        {
            int x = v.X >> number;
            int y = v.Y >> number;
            int z = v.Z >> number;
            return new Vector3i(x, y, z);
        }
        public static Vector3i Mask(this Vector3i v, int number)
        {
            int x = v.X & number;
            int y = v.Y & number;
            int z = v.Z & number;
            return new Vector3i((int)x, (int)y, (int)z);
        }
        public static Vector3i Floor(this Vector3 a)
        {
            return new Vector3i((int)Math.Floor(a.X), (int)Math.Floor(a.Y), (int)Math.Floor(a.Z));
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
        public static double SmoothStep(double edge0, double edge1, double x)
        {
            // Scale, bias and saturate x to 0..1 range
            x = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            // Evaluate polynomial
            return x * x * (3 - 2 * x);
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
            if (severity == DebugSeverity.DebugSeverityNotification) return;

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
