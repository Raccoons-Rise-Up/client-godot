using Godot;
using System;

public class ChunkGenerator : Node
{
    public Chunk[,] Chunks = new Chunk[128, 128];

    public override void _Ready()
    {
        var chunkSettings = new ChunkSettings {
            Size = 30,
            Res = 0.4f
        };

        for (int x = 0; x < 2; x++)
        {
            for (int z = 0; z < 1; z++)
            {
                var chunk = new Chunk(chunkSettings, x, z);
                Chunks[x, z] = chunk;
                AddChild(chunk);
            }
        }
    }
}

public struct ChunkSettings 
{
    public int Size { get; set; }
    public float Res { get; set; }
}
