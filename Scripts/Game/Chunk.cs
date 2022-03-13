using Godot;
using System;

public class Chunk : MeshInstance
{
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

        Generate(4);
    }

    public static void Generate(float period) 
    {
        MeshInstance.Mesh = null;

        var size = 10;

        var mesh = new PlaneMesh();
        mesh.Size = new Vector2(size, size);
        mesh.SubdivideDepth = 50;
        mesh.SubdivideWidth = 50;

        var surfaceTool = new SurfaceTool();
        surfaceTool.CreateFrom(mesh, 0);

        var meshDataTool = new MeshDataTool();
        var arrayMesh = surfaceTool.Commit();
        meshDataTool.CreateFromSurface(arrayMesh, 0);

        var strength = 3f;
        var simplexNoise = new OpenSimplexNoise();
        simplexNoise.Seed = 1234;
        simplexNoise.Octaves = 1;
        simplexNoise.Persistence = 1f;
        simplexNoise.Period = period;

        for (int i = 0; i < meshDataTool.GetVertexCount(); i++)
        {
            var vertex = meshDataTool.GetVertex(i);
            vertex += new Vector3(vertex.x, vertex.y + simplexNoise.GetNoise2d(vertex.x, vertex.z) * strength, vertex.z);
            meshDataTool.SetVertexNormal(i, new Vector3(0, 1, 0));
            meshDataTool.SetVertex(i, vertex);
        }

        arrayMesh.SurfaceRemove(0);

        meshDataTool.CommitToSurface(arrayMesh);

        MeshInstance.Mesh = arrayMesh;
    }
}
