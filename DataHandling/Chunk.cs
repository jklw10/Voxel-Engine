using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;



namespace Voxel_Engine.DataHandling
{
    using Rendering;
    class Chunk 
    {
        public Vector3i ChunkCoordinate;
        public const int Size = 16;
        public Voxel[,,] ChunkData = new Voxel[Size, Size, Size];
        public bool Empty = true;
        public bool Dirty = false;
        public Chunk(Vector3i CC)
        {
            ChunkCoordinate = CC;
        }
        public Voxel this[Vector3i c]
        {
            get { return this[c.X, c.Y, c.Z]; }
            set { this[c.X, c.Y, c.Z] = value; }
        }
        public Voxel this[int x, int y, int z]
        {
            get { return ChunkData[x, y, z]; }
            set { 
                if(ChunkData[x, y, z] != value)
                {
                    ChunkData[x, y, z] = value;
                    Dirty = true;
                    if (value.Exists())
                    {
                        Empty = false;
                    }
                }
            }
        }
    }
}
