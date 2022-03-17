using Godot;
using System;

public class ChunkGenerator : Node
{
    private static int WorldSize = 10;
    public Chunk[,] Chunks = new Chunk[WorldSize, WorldSize];
    private ChunkSettings ChunkSettings = new ChunkSettings
    {
        Size = 30,
        Res = 0.4f
    };

    public override void _Ready()
    {
        InitSpawn();
        FixNormals();
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
    }

    private void FixNormals()
    {
        // Fix normals on seams (take average of 2)
        // Start with a chunk
        var chunk1 = Chunks[0, 0];
        var chunk1EdgeNormals = new Vector3[ChunkSettings.Size];

        for (int i = 0; i < ChunkSettings.Size; i++)
        {
            chunk1EdgeNormals[i] = chunk1.Normals[chunk1.Edges[(int)Dir.North, i]];

            var point = chunk1.ChunkOffset + chunk1.Vertices[chunk1.Edges[(int)Dir.North, i]];
            //AddChild(new DebugPoint(point, Colors.Red));
        }

        // Get chunk neighbor
        var chunk2 = Chunks[1, 0];
        var chunk2EdgeNormals = new Vector3[ChunkSettings.Size];

        for (int i = 0; i < ChunkSettings.Size; i++)
        {
            chunk2EdgeNormals[i] = chunk2.Normals[chunk2.Edges[(int)Dir.South, i]];

            var point = chunk2.ChunkOffset + chunk2.Vertices[chunk2.Edges[(int)Dir.South, i]] + new Vector3(0, 1, 0);
            //AddChild(new DebugPoint(point, Colors.Blue));
        }

        // Take average of normals
        for (int i = 0; i < ChunkSettings.Size; i++)
        {
            var average = (chunk1EdgeNormals[i] + chunk2EdgeNormals[i]) / 2;
            chunk1.Normals[chunk1.Edges[(int)Dir.North, i]] = average.Normalized();
            chunk2.Normals[chunk2.Edges[(int)Dir.South, i]] = average.Normalized();
        }

        // Reapply and recalculate the normals
        var arrMesh = new ArrayMesh();
        var arr = new Godot.Collections.Array();
        arr.Resize((int)ArrayMesh.ArrayType.Max);

        arr[(int)ArrayMesh.ArrayType.Vertex] = chunk1.Vertices;
        arr[(int)ArrayMesh.ArrayType.Index] = chunk1.Indices;
        arr[(int)ArrayMesh.ArrayType.Normal] = chunk1.Normals;

        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr);
        chunk1.Mesh = arrMesh;


        var arrMesh2 = new ArrayMesh();
        var arr2 = new Godot.Collections.Array();
        arr2.Resize((int)ArrayMesh.ArrayType.Max);

        arr2[(int)ArrayMesh.ArrayType.Vertex] = chunk2.Vertices;
        arr2[(int)ArrayMesh.ArrayType.Index] = chunk2.Indices;
        arr2[(int)ArrayMesh.ArrayType.Normal] = chunk2.Normals;

        arrMesh2.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr2);
        chunk2.Mesh = arrMesh2;
    }
}

public struct ChunkSettings
{
    public int Size { get; set; }
    public float Res { get; set; }
}
