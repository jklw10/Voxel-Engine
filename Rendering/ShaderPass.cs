using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Voxel_Engine.Rendering
{
    public class ShaderPassStack
    {
        public ShaderPass[] All;
        public void SetProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            foreach (var pass in All)
            {
                pass.SetMatrix("ProjMatrix",ref projectionMatrix);
            }
        }
        public void SetViewMatrix(ref Matrix4 viewMatrix)
        {
            foreach (var pass in All)
            {
                pass.SetMatrix("ViewMatrix", ref viewMatrix);
            }
        }
        public void SetUniform1(string name, int value)
        {
            foreach (var pass in All)
            {
                pass.SetUniform1(name, value);
            }
        }
        public void SetUniform2(string name, Vector2 vector)
        {
            foreach (var pass in All)
            {
                pass.SetUniform2(name, vector);
            }
        }
        public void SetUniform3(string name, Vector3 vector)
        {
            foreach (var pass in All)
            {
                pass.SetUniform3(name, vector);
            }
        }
        public ShaderPassStack(params ShaderPass[] passes)
        {
            All = passes;
        }
        public void Draw(FrameBuffer input, bool clear)
        {
            FrameBuffer previous = All[0].Draw(input,clear);
            foreach (ShaderPass pass in All[1..])
            {
                previous = pass.Draw(previous,clear);
            }
        }
    }
    public class ShaderPass
    {
        public readonly int Program;
        public void Use()
        {
            GL.UseProgram(Program);
        }
        public void SetMatrix(string name,ref Matrix4 projectionMatrix)
        {
            GL.ProgramUniformMatrix4(Program, GL.GetUniformLocation(Program, name), true, ref projectionMatrix);
        }
        public void SetUniform3(string name,Vector3 position)
        {
            GL.ProgramUniform3(Program, GL.GetUniformLocation(Program, name), position);
        }
        public void SetUniform2(string name, Vector2 size)
        {
            GL.ProgramUniform2(Program, GL.GetUniformLocation(Program, name), size);
        }
        public void SetUniform1(string name, int value)
        {
            GL.ProgramUniform1(Program, GL.GetUniformLocation(Program, name), value);
        }
        static int CreateProgram(string shaderName)
        {
            int VS = Loader.LoadShader("Pass.vert", ShaderType.VertexShader);
            int FS = Loader.LoadShader(shaderName+".frag", ShaderType.FragmentShader);
            return Shader.CreateProgram(VS, FS);
        }
        public FrameBuffer Draw(FrameBuffer input, bool clear)
        {
            Use();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Buffer.handle);
            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTextures(0, input.output.Length, input.output);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            return Buffer;
        }
        public readonly FrameBuffer Buffer;

        public ShaderPass(string shaderName, Texture[]? input = null, Texture[]? output = null)
        {
            Program = CreateProgram(shaderName);
            Use();
            for (int i = 0; i < (input?.Length ?? 0); i++)
            {
                GL.Uniform1(GL.GetUniformLocation(Program, input![i].Target), i);
            }

            if (output is  null)
            {
                Buffer = new FrameBuffer(0);
            }
            else
            {
                Buffer = new FrameBuffer(output);
            }
        }
    }
}
