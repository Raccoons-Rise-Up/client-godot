using Godot;
using System;

public class Chunk : MeshInstance
{
    private Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");
    private static MeshInstance MeshInstance;
    private Vector3 Position;

    public Chunk()
    {
        // Godot needs this
    }

    public Chunk(Vector3 position)
    {
        Position = position;
    }

    public override void _Ready()
    {
        MeshInstance = this;
        Translate(Position);
        MaterialOverride = Material;

        Generate(4);
    }

    public static void Generate(float period) 
    {
        MeshInstance.Mesh = null;

        var size = 10;

        var strength = 3f;
        var simplexNoise = new OpenSimplexNoise();
        simplexNoise.Seed = 1234;
        simplexNoise.Octaves = 1;
        simplexNoise.Persistence = 1f;
        simplexNoise.Period = period;

        var surfaceTool = new SurfaceTool();
        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        int vertexIndex = 0;

        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                var noise1 = simplexNoise.GetNoise2d(x, z);
                var noise2 = simplexNoise.GetNoise2d(x + 1, z);
                var noise3 = simplexNoise.GetNoise2d(x, z + 1);
                var noise4 = simplexNoise.GetNoise2d(x + 1, z + 1);
                var pos = new Vector3(x, 0, z);

                surfaceTool.AddVertex(pos + new Vector3(0, noise1, 0));
                surfaceTool.AddVertex(pos + new Vector3(1, noise2, 0));
                surfaceTool.AddVertex(pos + new Vector3(0, noise3, 1));
                surfaceTool.AddVertex(pos + new Vector3(1, noise4, 1));

                surfaceTool.AddIndex(vertexIndex);
                surfaceTool.AddIndex(vertexIndex + 1);
                surfaceTool.AddIndex(vertexIndex + 2);

                surfaceTool.AddIndex(vertexIndex + 2);
                surfaceTool.AddIndex(vertexIndex + 1);
                surfaceTool.AddIndex(vertexIndex + 3);

                vertexIndex += 4;
            }
        }

        surfaceTool.GenerateNormals();

        MeshInstance.Mesh = surfaceTool.Commit();
    }
}
