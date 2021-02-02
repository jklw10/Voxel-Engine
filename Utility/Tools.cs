using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace Voxel_Engine
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
