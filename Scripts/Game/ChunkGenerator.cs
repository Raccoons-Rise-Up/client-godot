using Godot;
using System;
using System.Collections.Generic;

namespace Client.Game
{
    public class ChunkGenerator
    {
        public static int WorldSize = 100;
        public static int SpawnSize = 5;
        public static Chunk[,] ChunkData = new Chunk[WorldSize, WorldSize];
        public static List<Pos> LoadedChunks = new List<Pos>();
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
            for (int x = WorldSize / 2 - SpawnSize / 2; x <= WorldSize / 2 + SpawnSize / 2; x++)
            {
                for (int z = WorldSize / 2 - SpawnSize / 2; z <= WorldSize / 2 + SpawnSize / 2; z++)
                {
                    var chunk = new Chunk(ChunkSettings, x, z);
                    ChunkData[x, z] = chunk;
                    LoadedChunks.Add(new Pos { X = x, Z = z});
                    World.Instance.AddChild(chunk);
                }
            }

            foreach (var chunk in LoadedChunks) 
            {
                ChunkData[chunk.X, chunk.Z].SmoothEdgeNormals();
            }
                
        }
    }

    public struct Pos 
    {
        public int X { get; set; }
        public int Z { get; set; }
    }

    public struct ChunkSettings
    {
        public int Size { get; set; }
        public float Res { get; set; }
    }

}
