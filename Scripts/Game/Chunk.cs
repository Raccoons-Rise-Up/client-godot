using Godot;
using System;
using System.Collections.Generic;

public class Chunk : MeshInstance
{
    private Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");
    private static MeshInstance MeshInstance;
    private static Vector3 ChunkOffset;
    private static Chunk Instance;
    private static int Size;
    private static float ChunkScale;
    private static Vector3[] Vertices;
    private static int[] Indices;
    private static Vector3[] Normals;

    public Chunk()
    {
        // Godot needs this
    }

    public Chunk(int size, float scale, Vector3 pos)
    {
        ChunkOffset = pos;
        Size = size;
        ChunkScale = scale;
        Instance = this;
        Vertices = new Vector3[Size * Size];
        Indices = new int[(Size - 1) * (Size - 1) * 6];
        Normals = new Vector3[Vertices.Length];
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

        var arrMesh = new ArrayMesh();
        var arr = new Godot.Collections.Array();
        arr.Resize((int)ArrayMesh.ArrayType.Max);
        
        CalculateVerticesAndIndices(period);
        CalculateNormals();

        arr[(int)ArrayMesh.ArrayType.Vertex] = Vertices;
        arr[(int)ArrayMesh.ArrayType.Index] = Indices;
        arr[(int)ArrayMesh.ArrayType.Normal] = Normals;

        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr);
        MeshInstance.Mesh = arrMesh;
    }

    private static void CalculateVerticesAndIndices(float period)
    {
        var strength = 3f;
        var simplexNoise = new OpenSimplexNoise();
        simplexNoise.Seed = 1234;
        simplexNoise.Octaves = 1;
        simplexNoise.Persistence = 1f;
        simplexNoise.Period = period;

        var vertexIndex = 0;
        var triIndex = 0;

        for (int x = 0; x < Size; x++)
            for (int z = 0; z < Size; z++)
            {
                Vertices[vertexIndex++] = new Vector3(x, 0, z) * ChunkScale;

                if (x == 0 || z == 0) continue;

                Indices[triIndex] = (Size * x + z); //Top right
                Indices[triIndex + 1] = (Size * (x - 1) + (z - 1)); //Bottom left - First triangle
                Indices[triIndex + 2] = (Size * x + (z - 1)); //Bottom right
                Indices[triIndex + 3] = (Size * (x - 1) + (z - 1)); //Bottom left 
                Indices[triIndex + 4] = (Size * x + z); //Top right - Second triangle
                Indices[triIndex + 5] = (Size * (x - 1) + z); //Top left

                triIndex += 6;
            }
                

        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] += new Vector3(0, simplexNoise.GetNoise2d(ChunkOffset.x + Vertices[i].x, ChunkOffset.z + Vertices[i].z) * strength, 0);
        }
    }

    private static void CalculateNormals()
    {
        for (int i = 0; i < Normals.Length; i++)
        {
            Normals[i] = Vector3.Zero;
        }

        for (int i = 0; i < Indices.Length; i+=3)
        {
            var vertexA = Indices[i];
            var vertexB = Indices[i + 2];
            var vertexC = Indices[i + 1];

            var edgeAB = Vertices[vertexB] - Vertices[vertexA];
            var edgeAC = Vertices[vertexC] - Vertices[vertexA];

            var areaWeightedNormal = edgeAB.Cross(edgeAC);
            
            Normals[vertexA] += areaWeightedNormal;
            Normals[vertexB] += areaWeightedNormal;
            Normals[vertexC] += areaWeightedNormal;
        }

        for (int i = 0; i < Vertices.Length; i++) 
        {
            Normals[i] = Normals[i].Normalized();
        }
    }
}
