using Godot;
using System;

public class Chunk : MeshInstance
{
    private Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");
    private static MeshInstance MeshInstance;
    private static Vector3 ChunkOffset;

    public Chunk()
    {
        // Godot needs this
    }

    public Chunk(Vector3 pos)
    {
        ChunkOffset = pos;
    }

    public override void _Ready()
    {
        MeshInstance = this;
        Translate(ChunkOffset);
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
        surfaceTool.AddSmoothGroup(true);

        var vertices = new Vector3[size * size * 6];
        var v = 0;
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                var pos = new Vector3(x, 0, z);
                vertices[v] = pos + new Vector3(0, 0, 0);
                vertices[v + 1] = pos + new Vector3(1, 0, 0);
                vertices[v + 2] = pos + new Vector3(0, 0, 1);
                vertices[v + 3] = pos + new Vector3(0, 0, 1);
                vertices[v + 4] = pos + new Vector3(1, 0, 0);
                vertices[v + 5] = pos + new Vector3(1, 0, 1);

                v += 6;
            }
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(0, simplexNoise.GetNoise2d(ChunkOffset.x + vertices[i].x, ChunkOffset.z + vertices[i].z) * strength, 0);
            surfaceTool.AddVertex(vertices[i]);
        }

        surfaceTool.Index();
        surfaceTool.GenerateNormals();

        MeshInstance.Mesh = surfaceTool.Commit();
    }
}
