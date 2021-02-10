

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
        public static void RotateCamera(Vector2 dir)
        {
            if (dir.LengthSquared == 0 || Main is null) return;

            float MouseSensitivity = 0.2f;
            Main.Transform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-dir.X * MouseSensitivity));
            Main.Transform.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-dir.Y * MouseSensitivity));
        }

        /// <summary>
        /// updates the transformation matrices to reflect the new camera rotation/position
        /// </summary>
        public void RenderCamera(bool clear)
        {            

            if (subCamera   is object)  subCamera.RenderCamera(false); //don't clear when rendering a sub camera

            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (transform.Dirty) UpdateViewMatrix();

            GL.Enable(EnableCap.CullFace);

            GL.BindVertexArray(cubeVAO);
            GL.UseProgram(ProgramID);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0, DrawnVoxelCount);
            GL.BindVertexArray(0);
        }
        static void Initialize()
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
        int DrawnVoxelCount = 0;

        public void LoadWorld(World toLoad)
        {
            DrawnVoxelCount = toLoad.DrawnVoxelCount;
            GL.UseProgram(ProgramID);
            GL.BindVertexArray(cubeVAO);

            float[,] colors     = new float[toLoad.DrawnVoxelCount, 4];
            float[,] rotations  = new float[toLoad.DrawnVoxelCount, 4];
            float[,] positions  = new float[toLoad.DrawnVoxelCount, 3];


            GL.ProgramUniform1(ProgramID, GL.GetUniformLocation(ProgramID, "Scale"), toLoad.VoxelSize);

            int voxId = 0;
            foreach (Chunk c in toLoad)
            {
                foreach (Voxel v in c)
                {
                    if (v is object)
                    {
                        //color
                        colors[voxId, 0] = (v.Color.R);
                        colors[voxId, 1] = (v.Color.G);
                        colors[voxId, 2] = (v.Color.B);
                        colors[voxId, 3] = (v.Color.A);
                        //rotation
                        rotations[voxId, 0] = (v.Transform.Rotation.X);
                        rotations[voxId, 1] = (v.Transform.Rotation.Y);
                        rotations[voxId, 2] = (v.Transform.Rotation.Z);
                        rotations[voxId, 3] = (v.Transform.Rotation.W);

                        //position
                        positions[voxId, 0] = (v.Transform.Position.X);
                        positions[voxId, 1] = (v.Transform.Position.Y);
                        positions[voxId, 2] = (v.Transform.Position.Z);
                        voxId++;
                    }
                }
            }

            //voxel Color
            int VoxelColorBuffer = GL.GenBuffer();
            float[,] Colors = colors;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VoxelColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Colors.Length, Colors, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);
            GL.EnableVertexAttribArray(1);

            //voxel Rotations
            int RotationBuffer = GL.GenBuffer();
            float[,] Rotations = rotations;
            GL.BindBuffer(BufferTarget.ArrayBuffer, RotationBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Rotations.Length, Rotations, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, sizeof(float)*4, 0);
            GL.EnableVertexAttribArray(2);

            //voxel position
            int VoxelPos = GL.GenBuffer();
            float[,] Positions = positions;
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
                   0.5f,  0.5f,  0.5f, //top clockwise from top left
                  -0.5f,  0.5f,  0.5f,
                  -0.5f,  0.5f, -0.5f,
                   0.5f,  0.5f, -0.5f,
                                 
                   0.5f, -0.5f,  0.5f, //bottom
                  -0.5f, -0.5f,  0.5f,
                  -0.5f, -0.5f, -0.5f,
                   0.5f, -0.5f, -0.5f,
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

            int IndexObjectID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexObjectID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indices.Length), indices, BufferUsageHint.StaticDraw);
            
        }
    }
}
