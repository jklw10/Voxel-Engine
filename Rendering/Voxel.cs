using System;
using System.Drawing;

using OpenTK.Mathematics;

using Voxel_Engine.DataHandling;

namespace Voxel_Engine.Rendering
{
    public struct Voxel : IComponent
    {
        public static readonly Voxel EMPTY = new(0,0,0,0);
        public byte A, R, G, B;
        public Voxel(Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }
        public Voxel(byte A, byte R, byte G, byte B)
        {
            this.A = A;
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public bool Exists()
        {
            return A != 0;
        }
        public override string ToString()
        {
            return $"({A},{R},{G},{B})";
        }
        public static bool operator ==(Voxel a, Voxel b)
        {
            return a.A == b.A
                && a.R == b.R
                && a.G == b.G
                && a.B == b.B;
        }
        public static bool operator !=(Voxel a, Voxel b)
        {
            return !(a == b);
        }
        public static Voxel operator -(Voxel a, Voxel b)
        {
            a.A -= b.A;
            a.R -= b.R;
            a.G -= b.G;
            a.B -= b.B;
            return a;
        }
        public static Voxel operator +(Voxel a, Voxel b)
        {
            a.A += b.A;
            a.R += b.R;
            a.G += b.G;
            a.B += b.B;
            return a;
        }
        public static Voxel operator *(Voxel a, Voxel b)
        {
            a.A *= b.A;
            a.R *= b.R;
            a.G *= b.G;
            a.B *= b.B;
            return a;
        }
        public static Voxel operator /(Voxel a, Voxel b)
        {
            a.A /= b.A;
            a.R /= b.R;
            a.G /= b.G;
            a.B /= b.B;
            return a;
        }

        public override bool Equals(object? obj)
        {
            return obj?.Equals(this) ?? false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(A,R,G,B);
        }
    }
}
