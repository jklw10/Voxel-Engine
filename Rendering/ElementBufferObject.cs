namespace Voxel_Engine.Rendering;
public readonly struct ElementBufferObject
{
    public readonly VertexArrayObject VAO;
    public readonly BufferObject<uint> IBO;
    public readonly VertexBufferObject[] VBO;
    public void Use() => VAO.Use();
    public ElementBufferObject(VertexArrayObject vao,uint[]? indices = null, params VertexBufferObject[] VBO)
    {
        VAO = vao;
        VAO.Use();
        Array.ForEach(VBO, x => x.Enable());
        indices ??= Array.Empty<uint>();

        //magic numbers for cube vertices (xyz)
        IBO = new(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw, indices);
        this.VBO = VBO;
    }
    public void SetIndexBufferData(uint[] data)
    {
        Use();
        IBO.SetData(data);
    }
    public void SetVertexBufferData(VertexBufferObject Handle, float[] data)
    {
        Use();
        foreach (var vb in VBO)
        {
            if (vb.Attribute == Handle.Attribute)
                vb.SetData(data);
        }
    }
}
public readonly struct VertexArrayObject
{
    public readonly int Handle = GL.GenVertexArray();
    public void Use() => GL.BindVertexArray(Handle);
    public VertexArrayObject() => Use();
}

