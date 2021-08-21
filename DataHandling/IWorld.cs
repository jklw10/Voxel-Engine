using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.DataHandling
{
    public interface IWorld<T, E>
    {
        public bool Dirty { get; }
        public int VoxelCount { get; }
        public float VoxelSize { get; }
        public bool IsEmpty();
        public Span<E> GetPositionSpan();
        public Span<E> GetDirtySpan();
        public bool CleanUp();
        public T this[E pos]
        {
            get;
            set;
        }
    }
}
