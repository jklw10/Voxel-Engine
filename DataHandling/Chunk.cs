using System;
using System.Collections.Generic;
using System.Text;

using OpenTK;
using OpenTK.Mathematics;



namespace Voxel_Engine.DataHandling
{ 
    public class Chunk
    {
        public Vector3i ChunkCoordinate { get; set; }


        public static int Size = 16;
        public IComponent?[,,] ChunkData = new IComponent[Size, Size, Size];

        public Chunk(Vector3i chunkCoordinate)
        {
            ChunkCoordinate = chunkCoordinate;
        }
        public Chunk(int x, int y, int z)
        {
            ChunkCoordinate = new Vector3i(x, y, z);
        }
        public void Write(Vector3i c, IComponent? entity)
        {
            ChunkData[c.X, c.Y, c.Z] = entity;
        }
        public IComponent? Read(Vector3i c)
        {
            return ChunkData[c.X, c.Y, c.Z];
        }
        public IComponent? this[int x, int y, int z] 
        {
            get { return ChunkData[x, y, z]; }
            set { ChunkData[x, y, z] = value; }
        }
    }
}
