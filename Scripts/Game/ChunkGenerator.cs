using Godot;
using System;

namespace Client.Game
{
    public class ChunkGenerator : Node
    {
        public static int WorldSize = 3;
        public static Chunk[,] Chunks = new Chunk[WorldSize, WorldSize];
        private static ChunkSettings ChunkSettings = new ChunkSettings
        {
            Size = 30,
            Res = 0.4f
        };

        public override void _Ready()
        {
            InitSpawn();
        }

        private void InitSpawn()
        {
            for (int x = 0; x < WorldSize; x++)
            {
                for (int z = 0; z < WorldSize; z++)
                {
                    var chunk = new Chunk(ChunkSettings, x, z);
                    Chunks[x, z] = chunk;
                    AddChild(chunk);
                }
            }

            foreach (var chunk in Chunks)
                chunk.SmoothEdgeNormals();
        }
    }

    public struct ChunkSettings
    {
        public int Size { get; set; }
        public float Res { get; set; }
    }

}
