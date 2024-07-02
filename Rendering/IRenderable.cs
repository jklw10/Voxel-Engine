using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Linq;
using System;

namespace Voxel_Engine.Rendering;

public interface IRenderable
{
    public Renderer GetRenderable();
    public void Use() =>
        GetRenderable().Use();
    public void Render() =>
        GetRenderable().Render();
    public void SetUniform(Uniform assignment) =>
        GetRenderable().Program.SetUniform(assignment);
}   
public readonly struct Renderer
{
    public readonly Program Program;
    public readonly FrameBuffer OutputBuffer;
    public readonly ElementBufferObject EBO;
    public readonly int[] inputTextures;
    public Renderer(ElementBufferObject ebo, Program program, Texture[]? textures = null,  FrameBuffer? outputBuffer = null)
    {
        inputTextures = textures?.
            Select(x => x.handle).
            ToArray() ?? Array.Empty<int>();

        EBO = ebo;
        Program = program;
        OutputBuffer = outputBuffer ?? FrameBuffer.Default;

        for (int i = 0; i < (textures?.Length ?? 0); i++)
        {
            Program.SetUniform(new(textures![i].Target, new IGLType.Int1(i)));
        }
    }
    public void Use()
    {
        Program.Use();
        OutputBuffer.Use();
        EBO.Use();
        //what textures this pass will use
        if (inputTextures.Length > 0)
            GL.BindTextures(0, inputTextures.Length, inputTextures);
    }
    //todo: indirect rendering soon:tm:
    public void Render()
    {
        Use();
        GL.DrawElements(PrimitiveType.Triangles, EBO.IBO.DataCount, DrawElementsType.UnsignedInt, 0);
    }
}


