using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;

namespace Voxel_Engine.DataHandling
{
    using Rendering;
    public class ChunkStreamer
    {
        public static ChunkStreamer? Main { get; private set; }
        readonly ITerrainGenerator worldGen;
        readonly IChunkConsumer chunkConsumer;
        Vector3i currentCenter;
        
        public void Use()
        {
            Main = this;
        }
        public ChunkStreamer(IChunkConsumer cc, ITerrainGenerator tg)
        {
            worldGen = tg;
            chunkConsumer = cc;
        }
        
        public void Load()
        {
            if (chunkConsumer is null || worldGen is null) return;

            Vector3i camPos = ChunkWorld.Chunkify((Vector3i)(Camera.Main.Transform.Position)).CC;
            Vector3i off = new(0);
            for (int i = 1; i <= 16; i++)
            {
                off.Y = i;
                chunkConsumer.Feed(camPos, worldGen.GetChunkPlane(camPos + off, 16, new(0,-1,0)));
            }
            currentCenter = camPos;

        }

        public static void OnUpdate()
        {
            Main?.Update();
        }
        internal void Update()
        {
            if (chunkConsumer is null || worldGen is null) return;

            Vector3i camPos = ChunkWorld.Chunkify((Vector3i)(Camera.Main.Transform.Position)).CC;
            
            var dir = camPos - currentCenter;
            if(dir != new Vector3i())
            {
                chunkConsumer.Feed(camPos, worldGen.GetChunkPlane(currentCenter, 16, dir));
                currentCenter = camPos;
            }
        }
    }
    public interface ITerrainGenerator
    {
        public bool TryGetChunk(Vector3i CC, out Chunk? chunk);

        public Chunk[] GetChunkPlane(Vector3i center, int size, Vector3i direction);
    }
    public interface IChunkConsumer
    {
        public void Feed(Vector3i center, Chunk[] chunks);
    }
}
