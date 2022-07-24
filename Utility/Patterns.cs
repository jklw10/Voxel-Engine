﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Utility
{
    public class Patterns
    {
        public static Color4 Rainbow(Vector2 pos)
        {
            return new Color4(Math.Abs((5 - (pos.X + pos.Y) % 10) / 10f), Math.Abs((5 - (pos.X + pos.Y+3) % 10) / 10f), Math.Abs((5 - (pos.X + pos.Y+6) % 10) / 10f), 1f);
        }
    }
    public static class NoiseGenerator
    {
        public static int Seed { get;  set; }

        public static int Octaves { get; set; }

        public static double Amplitude { get; set; }

        public static double Persistence { get; set; }

        public static double Frequency { get; set; }

        static NoiseGenerator()
        {
            Octaves = 8;
            Amplitude = 1;
            Frequency = 0.015;
            Persistence = 0.65;
        }
        
        public static float Noise(float x, float y)
        {
            //returns -1 to 1
            double total = 0.0;
            double freq = Frequency, amp = Amplitude;
            for (int i = 0; i < Octaves; ++i)
            {
                total += Smooth(x * freq, y * freq) * amp;
                freq *= 2;
                amp *= Persistence;
            }
            if (total < -2.4) total = -2.4;
            else if (total > 2.4) total = 2.4;

            return (float)(total / 2.4);
        }
        /// <summary>
        /// returns -1 to 1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Noise(int x, int y)
        {
            double total = 0.0;
            double freq = Frequency, amp = Amplitude;
            for (int i = 0; i < Octaves; ++i)
            {
                total += Smooth(x * freq, y * freq) * amp;
                freq *= 2;
                amp *= Persistence;
            }
            if (total < -2.4) total = -2.4;
            else if (total > 2.4) total = 2.4;

            return (total / 2.4);
        }

        public static double NoiseGeneration(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + Seed) & 0x7fffffff) / 1073741824.0);
        }
        private static double SmoothLerp(double x, double y, double a)
        {
            double value = (1 - Math.Cos(a * Math.PI)) * 0.5;
            return x * (1 - value) + y * value;
        }
        private static double Smooth(double x, double y)
        {
            double n1 = NoiseGeneration((int)x, (int)y);
            double n2 = NoiseGeneration((int)x + 1, (int)y);
            double n3 = NoiseGeneration((int)x, (int)y + 1);
            double n4 = NoiseGeneration((int)x + 1, (int)y + 1);

            double i1 = SmoothLerp(n1, n2, x - (int)x);
            double i2 = SmoothLerp(n3, n4, x - (int)x);

            return SmoothLerp(i1, i2, y - (int)y);
        }
        public static double NoiseGeneration(int x, int y, int z)
        {
            int n = x + y * 57+ z*2501;
            n = (n << 13) ^ n;

            return (1.0 - ((n * (n * n * 15731 + 789221) + Seed) & 0x7fffffff) / 1073741824.0);
        }
        private static double SmoothNoise3D(double x, double y, double z)
        {
            int xi = (int)x;
            int yi = (int)y;
            int zi = (int)z;

            int x1 = xi + 1;
            int y1 = yi + 1;
            int z1 = zi + 1;
            //smoothlerp from cube corner to opposite.
            double valuex = Math.Cos((x - x1) * Math.PI) * 0.5 + .5;
            double valuey = Math.Cos((y - y1) * Math.PI) * 0.5 + .5;
            double valuez = Math.Cos((z - z1) * Math.PI) * 0.5 + .5;
            double vx = 1 - valuex;
            double vy = 1 - valuey;

            return ((NoiseGeneration(xi, yi, zi) * vx + NoiseGeneration(x1, yi, zi) * valuex) * vy +
                    (NoiseGeneration(xi, y1, zi) * vx + NoiseGeneration(x1, y1, zi) * valuex) * valuey) * (1 - valuez) +
                   ((NoiseGeneration(xi, yi, z1) * vx + NoiseGeneration(x1, yi, z1) * valuex) * vy +
                    (NoiseGeneration(xi, y1, z1) * vx + NoiseGeneration(x1, y1, z1) * valuex) * valuey) * valuez;

            //return Lerp32(values, x - (int)x, y - (int)y, z - (int)z);
        }
        public static double Noise(float x, float y, float z)
        {
            //returns -1 to 1
            double total = 0.0;
            double freq = Frequency, amp = Amplitude;
            for (int i = 0; i < Octaves; ++i)
            {
                total += SmoothNoise3D(x * freq, y * freq, z * freq) * amp;
                freq *= 2;
                amp *= Persistence;
            }
            if (total < -2.4) total = -2.4;
            else if (total > 2.4) total = 2.4;

            return (total / 2.4);
        }
    }
}
