namespace Voxel_Engine.Rendering;
public class RenderPassStack
{
    public void SetUniform(Uniform assignment)
    {
        foreach (IRenderable pass in All)
            pass.SetUniform(assignment);
    }
    public void Resize()
    {
        foreach (IRenderable pass in All)
            pass.GetRenderable().OutputBuffer.Resize();
    }
    public void Render()
    {
        foreach (var pass in All)
            pass.Render();
    }
    public IRenderable[] All;
    public RenderPassStack(params IRenderable[] passes) =>
        All = passes;

}
public class ShaderPass : IRenderable
{
    static VertexArrayObject VAO = new();
    public RenderObject backing;
    public RenderObject GetRenderable() => backing;
    public void Render()
    {
        backing.Use();
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }
    public ShaderPass(string shaderName, Texture[]? input = null, FrameBuffer? output = null)
    {
        backing = new(new(VAO), input, new("Pass", shaderName), output);
    }
    public void Use() =>
        backing.Use();
}

