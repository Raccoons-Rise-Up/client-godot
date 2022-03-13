using Godot;
using System;

public class ChunkGenerator : Node
{
    public override void _Ready()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int z = 0; z < 2; z++)
            {
                AddChild(new Chunk(new Vector3(x * 10, 0, z * 10)));
            }
        }
    }
}
