﻿

using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


using Voxel_Engine.Utility;
using Voxel_Engine.DataHandling;

namespace Voxel_Engine.Rendering
{
    public class Camera
    {
        public static Camera? Main;
        public void Select()
        {
            Main = this;
        }

        public Camera? subCamera;
        

        public Vector2 ScreenSize
        {
            get => screenSize;
            set
            {
                screenSize = value;
                GL.ProgramUniform2(ProgramID, GL.GetUniformLocation(ProgramID, "Resolution"), ref screenSize);
                UpdateProjectionMatrix();
            }
        }
        public Vector2 DisplayPos 
        { 
            get => displayPos; 
            set 
            {
                displayPos = value;
                GL.ProgramUniform2(ProgramID, GL.GetUniformLocation(ProgramID, "ScreenPos"), ref displayPos);
            } 
        }
        public Transform Transform 
        {
            get => transform;
            set
            {
                transform = value;
                UpdateViewMatrix();
            }
        }


#nullable disable
        private Transform transform;
        private Vector2 screenSize;
        private Vector2 displayPos;
        private static uint[] indices; //what order to use vertices in.
#nullable enable

        public void FitToScreen()
        {
            ScreenSize = Engine.window.Size;
        }


        public float FOV = (float)Math.PI / 3;

        private static int ProgramID;

        private static int cubeVAO; //cube vertex data
        public Camera(Vector2 screenSize, Vector2? displayPos = null, Transform? transform = null)
        {
            Initialize();

            ScreenSize = screenSize;
            DisplayPos = displayPos ?? new Vector2(0, 0);

            Transform = transform ?? new Transform(Vector3.Zero);

            UpdateViewMatrix();
            UpdateProjectionMatrix();
            _ = indices ?? throw new ApplicationException("Voxel object mesh index creation failed");
        }

        Matrix4 ViewMatrix;
        Matrix4 ProjectionMatrix;

        /// <summary>
        /// call when camera is moved for it to take effect.
        /// </summary>
        public void UpdateViewMatrix()
        {
            GL.UseProgram(ProgramID);
            ViewMatrix = Transform.TransformMatrix.Inverted();
            GL.ProgramUniformMatrix4(ProgramID, GL.GetUniformLocation(ProgramID, "ViewMatrix"), true, ref ViewMatrix);
            transform.Dirty = false;
        }

        /// <summary>
        /// call when fov or resolution is changed for it to take effect.
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            GL.UseProgram(ProgramID);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI - FOV, (float)(screenSize.X / screenSize.Y), 0.1f, 1000);
            GL.ProgramUniformMatrix4(ProgramID, GL.GetUniformLocation(ProgramID, "ProjMatrix"), true, ref ProjectionMatrix);

        }

        /// <summary>
        /// updates the transformation matrices to reflect the new camera rotation/position
        /// </summary>
        public void RenderCamera(bool clear)
        {            

            if (subCamera   is object)  subCamera.RenderCamera(false); //don't clear when rendering a sub camera

            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (transform.Dirty) UpdateViewMatrix();

            GL.BindVertexArray(cubeVAO);
            GL.UseProgram(ProgramID);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0, 1);
            GL.BindVertexArray(0);
        }
        public static void Initialize()
        {
            CreateVisuals();
            CreateCube();
        }
        static void CreateVisuals()
        {
            int VS = Shaders.LoadResource("vertex_shader", ShaderType.VertexShader);
            int FS = Shaders.LoadResource("fragment_shader", ShaderType.FragmentShader);

            ProgramID = Shaders.CreateProgram(VS, FS);

            GL.UseProgram(ProgramID);
        }


        public static void LoadWorld(World toLoad)
        {
            GL.UseProgram(ProgramID);
            GL.BindVertexArray(cubeVAO);

            List<float> colors = new List<float>();
            List<float> rotations = new List<float>();
            List<float> positions = new List<float>();


            GL.ProgramUniform1(ProgramID, GL.GetUniformLocation(ProgramID, "Scale"), toLoad.VoxelSize);
            

            foreach (Chunk c in toLoad.ToDraw)
            {
                foreach (Voxel? v in c.ChunkData)
                {
                    if (v is object)
                    {
                        //color
                        colors.Add(v.Color.A);
                        colors.Add(v.Color.R);
                        colors.Add(v.Color.G);
                        colors.Add(v.Color.B);

                        //rotation
                        rotations.Add(v.Transform.Rotation.X);
                        rotations.Add(v.Transform.Rotation.Y);
                        rotations.Add(v.Transform.Rotation.Z);
                        rotations.Add(v.Transform.Rotation.W);

                        //position
                        positions.Add(v.Transform.Position.X);
                        positions.Add(v.Transform.Position.Y);
                        positions.Add(v.Transform.Position.Z);

                    }
                }
            }

            //voxel Color
            int VoxelColorBuffer = GL.GenBuffer();
            float[] Colors = colors.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VoxelColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Colors.Length, Colors, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
            GL.EnableVertexAttribArray(1);

            //voxel Rotations
            int RotationBuffer = GL.GenBuffer();
            float[] Rotations = rotations.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, RotationBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Rotations.Length, Rotations, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, sizeof(float)*4, 0);
            GL.EnableVertexAttribArray(2);

            //voxel position
            int VoxelPos = GL.GenBuffer();
            float[] Positions = positions.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VoxelPos);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Positions.Length, Positions, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(3);

            //per voxel aka per instance =1 per vertex = 0
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);
            GL.VertexAttribDivisor(3, 1);
        }


        static void CreateCube()
        {
            cubeVAO = GL.GenVertexArray();
            GL.BindVertexArray(cubeVAO);
            //magic numbers for cube vertices (xyz)
            int VID = GL.GenBuffer();
            float[] Vertices =
            {
                   -0.5f,  0.5f,  0.5f,
                    0.5f,  0.5f,  0.5f,
                   -0.5f, -0.5f,  0.5f,

                    0.5f, -0.5f,  0.5f,
                   -0.5f, -0.5f, -0.5f,
                    0.5f, -0.5f, -0.5f,

                   -0.5f,  0.5f, -0.5f,
                    0.5f,  0.5f, -0.5f,
                   -0.5f,  0.5f,  0.5f,

                   -0.5f,  0.5f, -0.5f,
                    0.5f,  0.5f,  0.5f,
                    0.5f,  0.5f, -0.5f
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, VID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Vertices.Length, Vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float)*3, 0);
            //GL.VertexPointer(3, VertexAttribPointerType.Float, Vector3.SizeInBytes, 0); 
            //GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableVertexAttribArray(0);

            //magic numbers for cube vertex indices ids (XYZ)
            indices = new uint[]
            {
                0, 2, 1,
                2, 3, 1,
                8, 9, 2,
                9, 4, 2,
                2, 4, 3,
                4, 5, 3,
                3, 5,10,
                5,11,10,
                4, 6, 5,
                6, 7, 5,
                6, 0, 7,
                0, 1, 7
            };

            int IndexObjectID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indices.Length), indices, BufferUsageHint.StaticDraw);
            
        }
    }//*/
}
