using Godot;
using System;

namespace Client.Game
{
    public class ChunkGenerator
    {
        public static int WorldSize = 5;
        public static Chunk[,] Chunks = new Chunk[WorldSize, WorldSize];
        public static ChunkSettings ChunkSettings = new ChunkSettings
        {
            Size = 30,
            Res = 0.4f
        };

        public ChunkGenerator()
        {
            InitSpawn();
        }

        private void InitSpawn()
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    var chunk = new Chunk(ChunkSettings, x, z);
                    Chunks[x, z] = chunk;
                    World.Instance.AddChild(chunk);
                }
            }

            foreach (var chunk in Chunks) 
            {
                if (chunk != null)
                    chunk.SmoothEdgeNormals();
            }
                
        }
    }

    public struct ChunkSettings
    {
        public int Size { get; set; }
        public float Res { get; set; }
    }

}
