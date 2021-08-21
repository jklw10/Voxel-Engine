

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
        private static Camera? main;
        public static Camera Main 
        {
            get
            {
                if (main is null) main = new(
                    new ShaderPassStack());
                return main;
            }
            private set
            {
                main = value;
            } 
        }
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
                passes.SetUniform2("Resolution",screenSize);
                
                CubeShader.SetScreenSize(screenSize);
                UpdateProjectionMatrix();
            }
        }
        public Vector2 DisplayPos 
        { 
            get => displayPos; 
            set 
            {
                displayPos = value;
                CubeShader.SetScreenPos(displayPos);
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
        private Transform transform = new(Vector3.Zero);
        private Vector2 screenSize = Engine.window.Size;
        private Vector2 displayPos = new(0);
#nullable enable

        public void FitToScreen()
        {
            ScreenSize = Engine.window.Size;
        }

        public float FOV = (float)Math.PI / 3;


        public Camera(ShaderPassStack passes)
        {
            this.passes = passes;
            UpdateViewMatrix();
            UpdateProjectionMatrix();

            _ = CubeShader.Indices ?? throw new ApplicationException("Voxel object mesh index creation failed");
        }

        readonly ShaderPassStack passes;

        Matrix4 ViewMatrix;
        Matrix4 ProjectionMatrix;

        /// <summary>
        /// call when camera is moved to update the data inside shaders.
        /// </summary>
        public void UpdateViewMatrix()
        {
            ViewMatrix = transform.TransformMatrix.Inverted();

            passes.SetUniform3("ViewPos",transform.Position);
            passes.SetViewMatrix(ref ViewMatrix);
            CubeShader.SetViewMatrix(ref ViewMatrix);
            transform.Dirty = false;
        }

        /// <summary>
        /// call when fov or resolution is changed to update the data inside shaders.
        /// </summary>
        public void UpdateProjectionMatrix()
        {
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI - FOV, (float)(screenSize.X / screenSize.Y), 0.1f, 1000);
            
            passes.SetProjectionMatrix(ref ProjectionMatrix);
            CubeShader.SetProjectionMatric(ref ProjectionMatrix);
        }
       
        /// <summary>
        /// x,y,tilt
        /// </summary>
        /// <param name="dir"></param>
        public static void RotateCamera(Vector3 dir)
        {
            if (dir.LengthSquared == 0 || Main is null) return;

            float MouseSensitivity = 0.2f;
            Quaternion pitch = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-dir.Y * MouseSensitivity));
            Quaternion yaw = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-dir.X * MouseSensitivity));
            Main.Transform.Rotation = Main.Transform.Rotation * pitch * yaw;
        }

        readonly int VoxelColorBuffer = GL.GenBuffer();
        readonly int VoxelPosBuffer = GL.GenBuffer();

        static readonly FrameBuffer RenderBuffer = new(
            new(TextureType.Color),
            new(TextureType.Depth) 
            );



        int DrawnVoxelCount = 0;
        /// <summary>
        /// Draws the loaded world on screen ! 
        /// </summary>
        public void Render(bool clear)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, RenderBuffer.handle);
            {
                if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                if (subCamera is object) subCamera.Render(false); //don't clear when rendering a sub camera

                if (transform.Dirty) UpdateViewMatrix();


                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);

                CubeShader.Use();

                //GL.Disable(EnableCap.FramebufferSrgb);
                if (!(loaded?.Dirty ?? true))
                {
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, CubeShader.Indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0, DrawnVoxelCount);
                }
                else
                {
                    GL.DrawElementsInstanced(PrimitiveType.Triangles, CubeShader.Indices.Length, DrawElementsType.UnsignedInt, (IntPtr)0, 0);
                }
                //GL.Enable(EnableCap.FramebufferSrgb);
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.CullFace);
            passes.Draw(RenderBuffer, clear);

            GL.BindVertexArray(0);
        }

        IWorld<Voxel, Vector3i>? loaded;
        public void LoadWorld(IWorld<Voxel, Vector3i> toLoad)
        {
            loaded = toLoad;
            DrawnVoxelCount = toLoad.VoxelCount;

            CubeShader.Use();

            //GL.ProgramUniform1(Cube.Program, GL.GetUniformLocation(Cube.Program, "Scale"), toLoad.VoxelSize/40);
            //float[] Positions = Tools.SpherePoints(DrawnVoxelCount);
            CubeShader.SetScale(toLoad.VoxelSize);
            CubeShader.SetRotation(new(0, 0, 0, 1));

            int voxId = 0;
            var span = toLoad.GetPositionSpan();
            float[,] colors    = new float[toLoad.VoxelCount, 4];
            float[,] positions = new float[toLoad.VoxelCount, 3];
            foreach (Vector3i pos in span)
            {
                Voxel v = toLoad[pos];
                if (v.Exists())
                {
                    //color
                    colors[voxId, 0] = (v.R) / 255f;
                    colors[voxId, 1] = (v.G) / 255f;
                    colors[voxId, 2] = (v.B) / 255f;
                    colors[voxId, 3] = (v.A) / 255f;
                    //position
                    positions[voxId, 0] = ((pos.X) * toLoad.VoxelSize);
                    positions[voxId, 1] = ((pos.Y) * toLoad.VoxelSize);
                    positions[voxId, 2] = ((pos.Z) * toLoad.VoxelSize);
                    voxId++;
                }
            }
            DrawnVoxelCount = voxId;

            //voxel Color
            GL.BindBuffer(BufferTarget.ArrayBuffer, VoxelColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * colors.Length, colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, 0);

            //voxel position
            GL.BindBuffer(BufferTarget.ArrayBuffer, VoxelPosBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);

            //per voxel aka per instance =1 per vertex = 0
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);

            toLoad.CleanUp();
        }
    }
}
