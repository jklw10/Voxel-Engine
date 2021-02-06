using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

using Voxel_Engine.Rendering;
using Voxel_Engine.Utility;

namespace Voxel_Engine.DataHandling
{
    public class World
    {
        public static World? Current;
        public List<Chunk> ToDraw = new List<Chunk>();
        public float VoxelSize = 1;

        public int DrawnVoxelCount = 1;

        public void RemoveAt(Vector3i pos)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);
            if (index != -1)
            {
                ToDraw[index].Write((CR.X, CR.Y, CR.Z), null);
            }
        }
        public void Write(Vector3i pos, Voxel c)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);

            if (index == -1)
            {
                Chunk Written = new Chunk(CC);
                Written.Write((CR.X, CR.Y, CR.Z), c);
                ToDraw.Add(Written);
            }
            else
            {
                ToDraw[index].Write((CR.X, CR.Y, CR.Z), c);
            }

        }
        public bool IsVisible(Vector3i pos)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);

            foreach (Vector3i dir in Tools.directions)
            {
                if (!Exists(pos + dir))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Exists(Vector3i pos)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);
            if (index == -1)
            {
                return false;
            }
            else
            {
                return ((ToDraw[index]?.Read(CR) as Voxel)?.Color.A ?? 0) > 0;
            }
        }
        /// <summary>
        /// returns the Chunk Coordinate and Chunk Relative coordinates
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public (Vector3i CC, Vector3i CR) Chunkify(Vector3i pos)
        {
            //TODO: bitshift and bitmanipulate to get faster?
            //chunk in map coordinate
            Vector3i CC = new Vector3i(pos.X / Chunk.Size, pos.Y / Chunk.Size, pos.Z / Chunk.Size);        
            //chunk  relative to itself coordinate
            Vector3i CR = new Vector3i(pos.X-1, pos.Y-1, pos.Z-1).Modulo(new Vector3i(Chunk.Size, Chunk.Size, Chunk.Size));      
            return (CC, CR);

        }

        public void Select()
        {
            Current = this;
        }
    }
}
