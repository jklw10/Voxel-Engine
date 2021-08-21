using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Voxel_Engine.DataHandling
{
    using Rendering;
    public class OctreeWorld : IWorld<Voxel, Vector3i>
    {
        
        Octree world;
        Vector3i[] positions = Array.Empty<Vector3i>();

        public int VoxelCount { get => positions.Length; }
        public float VoxelSize { get => 1; }
        public bool Dirty { get => world.Dirty; }
        public Voxel this[Vector3i i]
        {
            get => world[i];
            set => world[i] = value;
        }
        public OctreeWorld(int depth)
        {
            world = new(depth);
        }
        Vector3i[] UpdatePositions()
        {
            if (world.Dirty)
            {
                var list = world.PosList(true);

                positions = list.ToArray();
            }
            return positions;
        }
        public Span<Vector3i> GetPositionSpan()
        {
            if (world.Dirty)
            {
                UpdatePositions();
            }
            return positions;
        }
        public Span<Vector3i> GetDirtySpan()
        {
            if (world.Dirty)
            {
                var list = world.PosList(true,true);
                positions = list.ToArray();
            }
            return positions;
        }
        public bool IsEmpty()
        {
            return world.IsEmpty();
        }
        public bool Empty(Vector3i position)
        {
            return (!this[position].Exists());
        }
        public bool CleanUp()
        {
            if (Dirty)
            {
                return false;
            }
            positions = Array.Empty<Vector3i>();
            return true;
        }
    }
}
