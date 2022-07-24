﻿namespace Voxel_Engine.Rendering;

using Voxel_Engine.Utility;
using Voxel_Engine.DataHandling;

public class Camera
{
    static Camera? main;

    

    /// <summary>
    /// finds a main camera if one has been selected, or creates a new empty one if not.
    /// </summary>
    public static Camera Main 
    {
        get
        {
            if (main is null) main = new(
                new RenderPassStack());
            return main;
        }
        private set
        {
            main = value;
        } 
    }
    /// <summary>
    /// selects a camera that can be found via Camera.Main
    /// </summary>
    public void Use()
    {
        Main = this;
    }
    private Vector2 screenSize = Engine.Window.Size;
    public Vector2 ScreenSize
    {
        get => screenSize;
        set
        {
            screenSize = value;
            passes.SetUniform2("Resolution",screenSize);
            UpdateProjectionMatrix();
            passes.Resize();
        }
    }
    Transform transform = new(Vector3.Zero);
    public Transform Transform 
    {
        get => transform;
        set
        {
            transform = value;
            UpdateViewMatrix();
        }
    }

    public void FitToScreen()
    {
        ScreenSize = Engine.Window.Size;
    }

    public float FOV = (float)Math.PI / 3;
    readonly RenderPassStack passes;
    public Camera(RenderPassStack passes)
    {
        this.passes = passes;
        UpdateViewMatrix();
        UpdateProjectionMatrix();
    }

    internal static void Render()
    {
        main?.Render();
    }
    /// <summary>
    /// Draws the loaded world on screen ! 
    /// </summary>
    public void Render(bool clear = false)
    {
        if (clear) GL.Clear(
            ClearBufferMask.DepthBufferBit | 
            ClearBufferMask.ColorBufferBit | 
            ClearBufferMask.StencilBufferBit);

        GL.Disable(EnableCap.FramebufferSrgb);
        GL.Disable(EnableCap.CullFace);
        // passes.Draw(RenderBuffer);
        passes.Render();

        GL.BindVertexArray(0);
    }

    Matrix4 ViewMatrix;
    Matrix4 ProjectionMatrix;

    /// <summary>
    /// call when camera is moved to update the data inside shaders.
    /// </summary>
    void UpdateViewMatrix()
    {
        ViewMatrix = transform.TransformMatrix().Inverted();
        Quaternion rot = transform.Rotation;
        passes.SetUniform4("CamRot", new Vector4(rot.Xyz,rot.W));
        passes.SetUniform3("ViewPos",transform.Position);
        passes.SetMatrix("ViewMatrix", ref ViewMatrix);
    }

    /// <summary>
    /// call when fov or resolution is changed to update the data inside shaders.
    /// </summary>
    void UpdateProjectionMatrix()
    {
        ProjectionMatrix = 
            Matrix4.CreatePerspectiveFieldOfView((float)Math.PI - FOV, 
            (float)(screenSize.X / screenSize.Y), 0.1f, 1000);

        passes.SetMatrix("ProjMatrix", ref ProjectionMatrix);
    }
   
    /// <summary>
    /// x,y,tilt
    /// </summary>
    /// <param name="dir"></param>
    public void Rotate(Vector3 dir)
    {
        if (dir.LengthSquared == 0 || Main is null) return;

        float MouseSensitivity = 0.2f;
        Quaternion pitch = Quaternion.FromAxisAngle(
            -Vector3.UnitZ, MathHelper.DegreesToRadians(dir.Y * MouseSensitivity));
        Quaternion yaw = Quaternion.FromAxisAngle(
            -Vector3.UnitY, MathHelper.DegreesToRadians(dir.X * MouseSensitivity));
        Transform next = Transform;
        next.Rotation = yaw * Transform.Rotation *  pitch ;
        Transform = next;
    }
}
