using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Linq;
using System;

namespace Voxel_Engine.Rendering
{
    public abstract class DefaultRenderable 
    {
        public Program Program { get; init; }
        public FrameBuffer OutputBuffer { get; set; }

        public readonly int[] inputTextures;
        protected DefaultRenderable(Texture[]? textures = null, Program? program = null, FrameBuffer? outputBuffer = null)
        {
            inputTextures = textures?.
                Select(x => x.handle).
                ToArray() ?? Array.Empty<int>();

            for (int i = 0; i < (textures?.Length ?? 0); i++)
            {
                SetUniform1(textures![i].Target, i);
            }
            Program = program ?? Program.Empty;
            OutputBuffer = outputBuffer ?? FrameBuffer.Default;
        }
        public void Use()
        {
            Program.Use();
            OutputBuffer.Use();
            //what textures this pass will use
            if (inputTextures.Length > 0)
                GL.BindTextures(0, inputTextures.Length, inputTextures);
        }
        public abstract void Render();
        public void SetMatrix(string name, ref Matrix4 value)=>
            Program.SetMatrix(name, ref value);
        public void SetUniform3(string name, Vector3 value) =>
            Program.SetUniform3(name, value);
        public void SetUniform3i(string name, Vector3i value) =>
            Program.SetUniform3i(name, value);
        public void SetUniform4(string name, Vector4 value) =>
            Program.SetUniform4(name, value);
        public void SetUniform2(string name, Vector2 value) =>
            Program.SetUniform2(name, value);
        public void SetUniform1(string name, int value) =>
            Program.SetUniform1(name, value);
    }

}
