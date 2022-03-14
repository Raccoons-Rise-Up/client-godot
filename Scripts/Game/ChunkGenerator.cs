using Godot;
using System;

public class ChunkGenerator : Node
{
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
