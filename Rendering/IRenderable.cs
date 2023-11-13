using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Linq;
using System;

namespace Voxel_Engine.Rendering;

public interface IRenderable
{
    public RenderObject GetRenderable();
    public void Use() =>
        GetRenderable().Use();
    public void Render() =>
        GetRenderable().Render();
    public void SetUniform(Uniform assignment) =>
        GetRenderable().Program.SetUniform(assignment);
}   
public readonly struct RenderObject
{
    public readonly Program Program;
    public readonly FrameBuffer OutputBuffer;
    public readonly ElementBufferObject EBO;
    public readonly int[] inputTextures;
    public RenderObject(ElementBufferObject ebo, Texture[]? textures = null, Program? program = null, FrameBuffer? outputBuffer = null)
    {
        inputTextures = textures?.
            Select(x => x.handle).
            ToArray() ?? Array.Empty<int>();

        EBO = ebo;
        Program = program ?? Program.Empty;
        OutputBuffer = outputBuffer ?? FrameBuffer.Default;

        for (int i = 0; i < (textures?.Length ?? 0); i++)
        {
            Program.SetUniform(Uniform.TextureTarget(textures![i].Target, new(i)));
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
    public void Render()
    {
        Use();
        GL.DrawElements(PrimitiveType.Triangles, EBO.IBO.DataCount, DrawElementsType.UnsignedInt, 0);
    }
}


