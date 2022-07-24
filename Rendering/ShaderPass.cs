using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Rendering
{
    public class RenderPassStack 
    {
        public void SetMatrix(string name, ref Matrix4 value)
        {
            foreach (var pass in All)
                pass.SetMatrix(name, ref value);
        }
        public void SetUniform1(string name, int value)
        {
            foreach (var pass in All)
                pass.SetUniform1(name, value);
        }
        public void SetUniform2(string name, Vector2 value)
        {
            foreach (var pass in All)
                pass.SetUniform2(name, value);
        }
        public void SetUniform3(string name, Vector3 value)
        {
            foreach (var pass in All)
                pass.SetUniform3(name, value);
        }
        public void SetUniform3i(string name, Vector3i value)
        {
            foreach (var pass in All)
                pass.SetUniform3i(name, value);
        }
        public void SetUniform4(string name, Vector4 value)
        {
            foreach (var pass in All)
                pass.SetUniform4(name, value);
        }
        public void Resize()
        {
            foreach (var pass in All) 
                pass.OutputBuffer.Resize();
        }
        public void Render()
        {
            foreach (var pass in All)
                pass.Render();
        }
        public DefaultRenderable[] All;
        public RenderPassStack(params DefaultRenderable[] passes) => 
            All = passes;
        
    }
    public class ShaderPass : DefaultRenderable
    {
        public readonly static VertexArrayObject VAO = new();
        public override void Render()
        {
            Use();
            VAO.Use();
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
        public ShaderPass(string shaderName, Texture[]? input = null, FrameBuffer? output = null)
            : base(input, new("Pass", shaderName), output) { }
    }
}
