using Godot;
using System;

public class ChunkGenerator : Node
{
    public override void _Ready()
    {
        var size = 30;
        var scale = 0.4f;

        for (int x = 0; x < 2; x++)
        {
            for (int z = 0; z < 2; z++)
            {
                var chunk = new Chunk(size, scale, new Vector3(x * (size * scale - scale), 0, z * (size * scale - scale)));
                AddChild(chunk);
            }
        }
    }
}
