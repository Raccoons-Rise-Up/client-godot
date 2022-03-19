using Godot;
using System;
using System.Collections.Generic;

namespace Client.Game
{
    public class ChunkGenerator
    {
        public static Chunk[,] ChunkData = new Chunk[World.WorldSize, World.WorldSize];
        public static List<Pos> LoadedChunks = new List<Pos>();
        public static ChunkSettings ChunkSettings = new ChunkSettings
        {
            Size = 30,
            Res = 0.4f
        };
        public static float ChunkLength = ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res;

        public ChunkGenerator()
        {
            //InitSpawn();
        }

        private void InitSpawn()
        {
            for (int x = World.WorldSize / 2 - World.SpawnSize / 2; x <= World.WorldSize / 2 + World.SpawnSize / 2; x++)
            {
                for (int z = World.WorldSize / 2 - World.SpawnSize / 2; z <= World.WorldSize / 2 + World.SpawnSize / 2; z++)
                {
                    var chunk = new Chunk(ChunkSettings, x, z);
                    ChunkData[x, z] = chunk;
                    World.Instance.AddChild(chunk);
                }
            }

            foreach (var chunk in LoadedChunks) 
            {
                ChunkData[chunk.X, chunk.Z].GenerateMesh();
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
