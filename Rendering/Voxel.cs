using System;
using System.Drawing;

using OpenTK.Mathematics;

using Voxel_Engine.DataHandling;

namespace Voxel_Engine.Rendering
{
    public struct Voxel : IComponent
    {
        public static readonly Voxel EMPTY = new(0);
        public uint colors;
        public byte R { get => (byte)((colors & 0x000000FF) ); }
        public byte G { get => (byte)((colors & 0x0000FF00) >> 8); }
        public byte B { get => (byte)((colors & 0x00FF0000) >> 16); }
        public byte A { get => (byte)((colors & 0xFF000000) >> 24); }
        public Voxel(byte A, byte R, byte G, byte B)
        {
            colors = BitConverter.ToUInt32(new byte[] {R,G,B,A });
        }
        public Voxel(Color4 v)
        {
            colors = BitConverter.ToUInt32(new byte[] { (byte)(v.R * 255), (byte)( v.G*255), (byte)(v.B * 255), (byte)(v.A * 255) });
        }
        public Voxel(uint color)
        {
            colors = color;
        }
        public bool Exists()
        {
            return A != 0;
        }
        public override string ToString()
        {
            return $"({R},{G},{B},{A})";
        }
        public static bool operator ==(Voxel a, Voxel b)
        {
            return a.R == b.R
                && a.G == b.G
                && a.B == b.B
                && a.A == b.A;
        }
        public static bool operator !=(Voxel a, Voxel b)
        {
            return !(a == b);
        }
        public static Voxel operator -(Voxel a, Voxel b)
        {
            return new Voxel(
            (byte)(a.R - b.R),
            (byte)(a.G - b.G),
            (byte)(a.B - b.B),
            (byte)(a.A - b.A));
            
        }
        public static Voxel operator +(Voxel a, Voxel b)
        {
            return new Voxel(
            (byte)(a.R + b.R),
            (byte)(a.G + b.G),
            (byte)(a.B + b.B),
            (byte)(a.A + b.A));
        }
        public static Voxel operator *(Voxel a, Voxel b)
        {
            return new Voxel(
            (byte)(a.R * b.R),
            (byte)(a.G * b.G),
            (byte)(a.B * b.B),
            (byte)(a.A * b.A));
        }
        public static Voxel operator /(Voxel a, Voxel b)
        {
            return new Voxel(
            (byte)(a.R / b.R),
            (byte)(a.G / b.G),
            (byte)(a.B / b.B),
            (byte)(a.A / b.A));
        }

        public override bool Equals(object? obj)
        {
            return obj?.Equals(this) ?? false;
        }
        public override int GetHashCode()
        {
            return (int)colors;
        }
        public static implicit operator uint(Voxel a)
        {
            return a.colors;
        }
    }
}
