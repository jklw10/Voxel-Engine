using System;

using OpenTK.Graphics.OpenGL4;


namespace Voxel_Engine.DataHandling
{
    //TODO: INumber replace.
    public struct BufferObject<T> where T : unmanaged
    {
        public readonly int Handle = GL.GenBuffer();
        public readonly BufferTarget Target;
        public readonly BufferUsageHint Hint;
        public readonly int TypeSize;
        public int DataCount { get; private set; }
        public BufferObject(BufferTarget target, BufferUsageHint hint, 
            T[]? data = null)
        {
            DataCount = 0;
            unsafe { TypeSize = sizeof(T); }
            Target = target;
            Hint = hint;
            if (data is not null)
                SetData(data);
        }
        public void SetData(params T[] data)
        {
            Use();
            DataCount = data.Length;
            GL.BufferData(Target, TypeSize * data.Length, data, Hint); 
        }
        public void SubData(int start, params T[] data)
        {
            Use();
            GL.BufferSubData(Target,
                (IntPtr)(TypeSize * start), TypeSize * data.Length, data);
        }
        public void Use()
        {
            GL.BindBuffer(Target, Handle);
        }
    }
    public struct VertexBufferObject
    {
        readonly BufferObject<float> buffer;
        public int Handle { get => buffer.Handle; }
        public readonly int Attribute;
        readonly int Stride;
        public VertexBufferObject
            (BufferUsageHint hint, int attribute, int stride, params float[] data)
        {
            buffer = new(BufferTarget.ArrayBuffer, hint, data);
            Attribute = attribute;
            Stride = stride;
        }
        public void SetData(params float[] data)
        {
            buffer.SetData(data);
        }
        public void Enable()
        {
            buffer.Use();
            GL.VertexAttribPointer(Attribute, Stride, VertexAttribPointerType.Float, false, sizeof(float) * Stride, 0);
            GL.EnableVertexAttribArray(Attribute);
        }
    }
    public struct ShaderStorageBuffer<T> where T : unmanaged
    {
        public readonly BufferObject<T> buffer;
        public int Handle { get => buffer.Handle; }
        public int TypeSize { get => buffer.TypeSize; }
        public ShaderStorageBuffer(int bufferBase, BufferUsageHint hint, T[]? data = null)
        {
            buffer = new(BufferTarget.ShaderStorageBuffer, hint, data);
            BindTo(bufferBase);
        }
        public void BindTo(int bufferBase)
        {
            Use();
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 
                bufferBase, buffer.Handle);
        }
        public void SetData(params T[] data)
        {
            buffer.SetData(data);
        }
        public void SubData(int start,params T[] data)
        {
            buffer.SubData(start, data);
        }
        public void Use()
        {
            buffer.Use();
        }
    }
    public struct GrowingGPUBuffer<T> where T : unmanaged
    {
        public ShaderStorageBuffer<T> InternalBuffer { get; private set; }
        public ShaderStorageBuffer<T> CopyBuffer { get; private set; }
        readonly int bufferBase;
        public int Filled;
        public int Size;
        public int Handle { get => InternalBuffer.Handle; }
        public int TypeSize { get => InternalBuffer.TypeSize; }

        public GrowingGPUBuffer(int bufferBase, BufferUsageHint hint)
        {
            CopyBuffer = new(bufferBase, hint);
            InternalBuffer = new(bufferBase,hint);
            this.bufferBase = bufferBase;
            Size = 0;
            Filled = 0;
        }
        /// <summary>
        /// replace or append, grows the buffer when needed.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        public void Send(T[] data, int start)
        {
            if (Size == 0 && start == 0)
            {
                Size = data.Length;
                InternalBuffer.Use();
                InternalBuffer.SetData(data);
                InternalBuffer.BindTo(bufferBase);
                Filled = Size;
                return;
            }
            int end = data.Length + start;
            //if data spills, resize;
            if (end > Size) Grow(end); 
            InternalBuffer.SubData(start, data);
            Filled = Filled < end ? end : Filled;
        }
        /// <summary>
        /// grows the buffer to newSize's length
        /// </summary>
        /// <param name="newSize"></param>
        public void Grow(int newSize)
        {
            // no need to resize if it fits already
            if (newSize <= Size) return;

            //TODO: acquire brain
            if (newSize >= Size * 2) { newSize *= 2; }
            else { newSize = Size * 2; }

            GL.BindBuffer(BufferTarget.CopyReadBuffer, InternalBuffer.Handle); //select original buffer as where to read from.
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, CopyBuffer.Handle); //where to copy data into.
            //fill copyBuffer with an empty buffer with the needed size
            GL.BufferData(BufferTarget.CopyWriteBuffer, TypeSize * newSize, IntPtr.Zero,
                    BufferUsageHint.DynamicCopy);
            //this should copy it's data to another part of memory, with just a larger space
            GL.CopyBufferSubData(BufferTarget.CopyReadBuffer,
                BufferTarget.CopyWriteBuffer, 
                IntPtr.Zero, IntPtr.Zero, TypeSize * Size);
            
            Size = newSize;

            //swap handles
            (CopyBuffer, InternalBuffer) = (InternalBuffer, CopyBuffer);

            //rebind the ssbo to use the now larger buffer.
            InternalBuffer.BindTo(bufferBase);
        }

    }
}
