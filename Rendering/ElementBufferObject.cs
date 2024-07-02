namespace Voxel_Engine.Rendering;
public readonly struct ElementBufferObject
{
    public readonly BufferObject<uint> IBO;
    public readonly VertexBufferObject[] VBO;
    public readonly int VAO;
    public void Use()
    {
        GL.BindVertexArray(VAO);
    }
    public ElementBufferObject()
    {
        this = new(null);
    }
    public ElementBufferObject( uint[]? indices = null, params VertexBufferObject[] VBO)
    {
        VAO = GL.GenVertexArray();
        GL.BindVertexArray(VAO);
        Array.ForEach(VBO, x => x.Enable());
        indices ??= Array.Empty<uint>();

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


