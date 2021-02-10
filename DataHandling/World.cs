using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OpenTK.Mathematics;

using Voxel_Engine.Rendering;
using Voxel_Engine.Utility;

namespace Voxel_Engine.DataHandling
{
    public class World : IEnumerable
    {
        public static World? Current;
        List<Chunk> ToDraw = new List<Chunk>();
        public float VoxelSize = 1;

        public int ChunkBitWidth = (int)Math.Sqrt(Chunk.Size);

        public int DrawnVoxelCount = 1;

        public World()
        {
            ChunkBitWidth = (int)Math.Sqrt(Chunk.Size);
            Select();
        }

        public void RemoveAt(Vector3i pos)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);
            if (index != -1)
            {
                ToDraw[index].Write((CR.X, CR.Y, CR.Z), null);
            }
            foreach (Vector3 v in Tools.directions)
            {

            }
        }
        public void Write(Vector3i pos, Voxel c)
        {
            var (CC, CR) = Chunkify(pos);
            int index = ToDraw.FindIndex(X => X.ChunkCoordinate == CC);
            c.visible = IsVisible(pos);

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
            if (c.visible)
            {
                DrawnVoxelCount++;
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
            Vector3i CC = pos.Bitshift(ChunkBitWidth);        
            //chunk  relative to itself coordinate
            Vector3i CR = pos.Modulo(Chunk.Size);      
            
            return (CC, CR);
        }
        

        public void Select()
        {
            Current = this;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)ToDraw).GetEnumerator();
        }
    }
}
