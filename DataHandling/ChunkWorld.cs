using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using OpenTK.Mathematics;

using Voxel_Engine.Rendering;
using Voxel_Engine.Utility;

namespace Voxel_Engine.DataHandling
{
    public class ChunkWorld : IWorld<Voxel, Vector3i>
    {
        public static ChunkWorld? Current { get; private set; }

        public int VoxelCount { get; private set; }


        Dictionary<(int,int,int), Chunk> ToDraw = new();
        public float VoxelSize { get => 1;}

        public readonly static int ChunkBitWidth = (int)Math.Sqrt(Chunk.Size);


        public ChunkWorld()
        {
            //ChunkBitWidth = (int)Math.Sqrt(Chunk.Size);
            Select();
        }
        public void ThrowAway()
        {
            ToDraw = new();
            VoxelCount = 0;
        }

        public void RemoveAt(Vector3i pos)
        {
            this[pos] = Voxel.EMPTY;
        }
        public void Write(Vector3i pos, Voxel c)
        {
            this[pos] = c;
        }
        bool TargetExists(Vector3i CC, Vector3i CR, out Chunk? chunk)
        {
            ToDraw.TryGetValue((CC.X, CC.Y, CC.Z), out chunk);
            return chunk?[CR].Exists() ?? false;
        }
        bool TargetExists(Vector3i pos, out Chunk? chunk)
        {
            var (CC, CR) = Chunkify(pos);
            return TargetExists(CC,CR, out chunk);
        }
        bool TargetExists(Vector3i pos)
        {
            return TargetExists(pos, out _);
        }
        public Voxel this[Vector3i pos] 
        { 
            get 
            {
                var (CC, CR) = Chunkify(pos);
                ToDraw.TryGetValue((CC.X, CC.Y, CC.Z), out var chunk);
                return chunk?[CR] ?? Voxel.EMPTY;
            }
            set
            {
                Dirty = true;
                var (CC, CR) = Chunkify(pos);
                bool targetExists = TargetExists(CC, CR, out Chunk? chunk);
                if (value.Exists())
                {
                    if (!targetExists)
                    {
                        VoxelCount++;
                    }
                    if (chunk is null)
                    {
                        var Written = new Chunk(CC);
                        Written[CR] = value;
                        ToDraw.Add((CC.X, CC.Y, CC.Z), Written);
                        return;
                    }
                    chunk[CR] = value;
                }else if(targetExists)
                {
                    VoxelCount--;
                    chunk![CR] = value;
                }
            }
        }
        public bool IsVisible(Vector3i pos)
        {
            //TODO check if inside chunk or in border for slight speedup;
            foreach (Vector3i dir in Tools.directions)
            {
                if (!TargetExists(pos+dir))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// returns the Chunk Coordinate and Chunk Relative coordinates
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static (Vector3i CC, Vector3i CR) Chunkify(Vector3i pos)
        {
            //chunk in map coordinate
            Vector3i CC = pos.Bitshift(ChunkBitWidth);
            //chunk  relative to itself coordinate
            Vector3i CR = (pos - CC* Chunk.Size).Abs();     
            
            return (CC, CR);
        }
        

        public void Select()
        {
            Current = this;
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)ToDraw.Values).GetEnumerator();
        }

        public bool IsEmpty()
        {
            return ToDraw.Count == 0;
        }
        Vector3i[] positions = Array.Empty<Vector3i>();

        public bool Dirty { get; private set; }
        public Span<Vector3i> GetPositionSpan()
        {
            if (Dirty)
            {
                VoxelCount = 0;
                List<Vector3i> Positions = new();
                foreach (var c in ToDraw)
                {
                    if (!c.Value.Empty)
                    {
                        Vector3i CC = c.Value.ChunkCoordinate * Chunk.Size;
                        for (ChunkIterator pos = new(); pos < ChunkIterator.MAX; pos.Next())
                        {
                            //var v = c.Value[pos.X, pos.Y, pos.Z];
                            if (c.Value[pos.X, pos.Y, pos.Z].Exists())
                            {
                                Positions.Add(CC + pos);
                                VoxelCount++;
                            }
                        }
                    }
                }
                positions = Positions.ToArray();
                Dirty = false;
            }
            return positions.AsSpan();
        }
        public Span<Vector3i> GetDirtySpan()
        {
            if (Dirty)
            {
                VoxelCount = 0;
                List<Vector3i> Positions = new();
                foreach (var c in ToDraw)
                {
                    if (!c.Value.Empty && !c.Value.Dirty)
                    {
                        Vector3i CC = c.Value.ChunkCoordinate * Chunk.Size;
                        for (ChunkIterator pos = new(); pos < ChunkIterator.MAX; pos.Next())
                        {
                            //var v = c.Value[pos.X, pos.Y, pos.Z];
                            if (c.Value[pos.X, pos.Y, pos.Z].Exists())
                            {
                                Positions.Add(CC + pos);
                                VoxelCount++;
                            }
                        }
                    }
                }
                positions = Positions.ToArray();
                Dirty = false;
            }
            return positions.AsSpan();
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
