using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voxel_Engine.Utility;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxel_Engine.Rendering
{
    public static class CubeShader
    {
#nullable disable
        public readonly static int VAO = GL.GenVertexArray();
        public static uint[] Indices { get; private set; }
        private static int Program;
#nullable enable

        public static void Use()
        {
            GL.UseProgram(Program);
            GL.BindVertexArray(VAO);
        }
        public static void SetScale(float scale)
        {
            GL.ProgramUniform1(Program, GL.GetUniformLocation(Program, "Scale"), scale);
        }
        public static void SetRotation(Quaternion scale)
        {
            GL.ProgramUniform4(Program, GL.GetUniformLocation(Program, "Rotation"), scale);
        }
        public static void SetProjectionMatric(ref Matrix4 projectionMatrix)
        {
            GL.ProgramUniformMatrix4(Program, GL.GetUniformLocation(Program, "ProjMatrix"), true, ref projectionMatrix);
        }
        public static void SetViewMatrix(ref Matrix4 viewMatrix)
        {
            GL.ProgramUniformMatrix4(Program, GL.GetUniformLocation(Program, "ViewMatrix"), true, ref viewMatrix);
        }
        public static void SetScreenSize(Vector2 size)
        {
            GL.ProgramUniform2(Program, GL.GetUniformLocation(Program, "Resolution"), ref size);
        }
        public static void SetScreenPos(Vector2 pos)
        {
            GL.ProgramUniform2(Program, GL.GetUniformLocation(Program, "ScreenPos"), ref pos);
        }
        static void CreateProgram()
        {
            int VS = Loader.LoadShader("Base.vert", ShaderType.VertexShader);
            int FS = Loader.LoadShader("Base.frag", ShaderType.FragmentShader);
            Program = Shader.CreateProgram(VS, FS);
        }
        [InitFunction]
        static void Init()
        {
            CreateProgram();
            Use();

            //magic numbers for cube vertices (xyz)
            int VBO = GL.GenBuffer();
            float[] Vertices =
            {
                   0.5f,  0.5f,  0.5f, //top clockwise from top left
                  -0.5f,  0.5f,  0.5f,
                  -0.5f,  0.5f, -0.5f,
                   0.5f,  0.5f, -0.5f,

                   0.5f, -0.5f,  0.5f, //bottom
                  -0.5f, -0.5f,  0.5f,
                  -0.5f, -0.5f, -0.5f,
                   0.5f, -0.5f, -0.5f,
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            //GL.VertexPointer(3, VertexAttribPointerType.Float, Vector3.SizeInBytes, 0); 
            //GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableVertexAttribArray(0);
            //magic numbers for cube vertex indices ids (XYZ)
            Indices = new uint[]
            {
                //right hand rule, thumb = normal.
                2, 1, 0, //top
                3, 2, 0,

                4, 5, 6, //bottom
                4, 6, 7,

                0, 1, 4, //back
                4, 1, 5,

                2, 3, 6, //front
                6, 3, 7,

                1, 2, 5, //left 1256
                2, 6, 5,

                0, 4, 3, //right 0347
                3, 4, 7,
            };

            int IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * Indices.Length), Indices, BufferUsageHint.StaticDraw);
            return;
        }
    }
}
