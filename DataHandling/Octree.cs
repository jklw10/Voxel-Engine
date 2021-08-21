using OpenTK.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voxel_Engine.DataHandling
{
    using Rendering;
    using Utility;
    struct Octree 
    {
        Voxel estimate;
        Octree[] container;
        readonly int depth;
        readonly int depthBit;
        public bool Dirty { get; private set; }
        public Octree(int depth = 0)
        {
            this.depth = depth;
            if (depth > 31) throw new ArgumentException("octree can't be deeper than 31 because of int limit.");
            container  = Array.Empty<Octree>();
            estimate   = Voxel.EMPTY;
            depthBit   = (int)(1u << depth);
            Dirty = true;
        }
        public Voxel this[Vector3i pos] 
        {
            get
            {
                if (depth == -1)
                {
                    return estimate;
                }
                if (container.Length == 0)
                {
                    return estimate;
                }
                uint x = ((uint)(pos.X & depthBit)) >> (depth);
                uint y = ((uint)(pos.Y & depthBit)) >> (depth);
                uint z = ((uint)(pos.Z & depthBit)) >> (depth);
                uint index = x + y * 2 + z * 4; 
                return container[index][pos];
            }
            set
            {
                if (depth == -1)
                {
                    if (estimate != value) Dirty = true;
                    estimate = value;
                    return;
                }
                if (container.Length == 0)
                {
                    container = new Octree[8];
                    Array.Fill(container, new Octree(depth-1));
                }
                uint x = ((uint)(pos.X & depthBit)) >> (depth);
                uint y = ((uint)(pos.Y & depthBit)) >> (depth);
                uint z = ((uint)(pos.Z & depthBit)) >> (depth);
                uint index = x + y * 2 + z * 4;
                container[index][pos] = value;
                if (container[index].Dirty)
                {
                    Dirty = true;
                }
            }
        }
        public List<Vector3i> PosList(bool resetDirty, bool dirtyOnly = false, Vector3i? pos = null)
        {
            Vector3i position = pos ?? new(0);
            List<Vector3i> voxels = new();
            if(resetDirty) Dirty = false;
            if (container.Length != 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    var v = container[i];
                    var posi = position + (Tools.octPositions[i] * depthBit);
                    if (!dirtyOnly || v.Dirty)
                    {
                        if (v.depth != -1)
                        {
                            voxels.AddRange(v.PosList(resetDirty, dirtyOnly, posi));
                        }
                        else
                        {
                            voxels.Add(posi);
                        }
                    }
                }
            }
            return voxels;
        }
        public bool IsEmpty() => container.Length != 0;
    }
}
