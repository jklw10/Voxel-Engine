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
    public Renderer backing;
    public Renderer GetRenderable() => backing;
    public void Render()
    {
        backing.Use();
        //GL.DrawArrays(PrimitiveType.Triangles,0,3);
        GL.DrawElements(PrimitiveType.Triangles,3,DrawElementsType.UnsignedInt,(IntPtr)0);
    }
    public ShaderPass(string shaderName, Texture[]? input = null, FrameBuffer? output = null)
    {
        ElementBufferObject ebo = new(new uint[] { 0, 1, 2 });
        backing = new(ebo, new("Pass", shaderName), input, output);
    }
}

