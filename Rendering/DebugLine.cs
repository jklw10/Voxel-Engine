using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.Rendering
{
    public class DebugLine
    {
        static readonly List<Vector3> LineData = new();
        public static void Draw(Vector3 a, Vector3 b)
        {
            LineData.Add(a);
            LineData.Add(b);
            UpdateBuffer();
        }
        static readonly int vbo = GL.GenBuffer();
        static void UpdateBuffer()
        {
            Vector3[] LD = LineData.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, LD.Length * 3 * sizeof(float), LD, BufferUsageHint.StaticDraw);
        }
        
        internal static void Draw()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.EnableVertexAttribArray(vbo);
            GL.DrawArrays(PrimitiveType.Lines, 0, LineData.Count);
            GL.DisableVertexAttribArray(vbo);
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
