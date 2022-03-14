using Godot;
using System;
using System.Collections.Generic;

public class Chunk : MeshInstance
{
    // Static
    private static ChunkSettings ChunkSettings;
    private static Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");

    // Instance
    private Vector3[] Normals;
    private int[,] Edges;
    private Vector3 ChunkOffset;

    private Chunk()
    {
        // Godot needs this
        // Private because we need to specify size, scale and pos
    }

    public Chunk(ChunkSettings chunkSettings, int x, int z)
    {
        ChunkSettings = chunkSettings;
        ChunkOffset = new Vector3(x * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res), 0, z * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res));
        Normals = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
        Edges = new int[4, ChunkSettings.Size];
    }

    public override void _Ready()
    {
        Generate(4);
    }

    public void Generate(float period) 
    {
        Translate(ChunkOffset);
        MaterialOverride = Material;

        Mesh = null;

        // Calculate vertices and indices
        var arrMesh = new ArrayMesh();
        var arr = new Godot.Collections.Array();
        arr.Resize((int)ArrayMesh.ArrayType.Max);

        var vertices = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
        var indices = new int[(ChunkSettings.Size - 1) * (ChunkSettings.Size - 1) * 6];
        
        var strength = 3f;
        var simplexNoise = new OpenSimplexNoise();
        simplexNoise.Seed = 1234;
        simplexNoise.Octaves = 1;
        simplexNoise.Persistence = 1f;
        simplexNoise.Period = period;
        
        var vertexIndex = 0;
        var triIndex = 0;
        
        var edge1Index = 0;
        var edge2Index = 0;
        var edge3Index = 0;
        var edge4Index = 0;

        for (int x = 0; x < ChunkSettings.Size; x++)
            for (int z = 0; z < ChunkSettings.Size; z++)
            {
                if (x == 0)
                {
                    Edges[0, edge1Index++] = vertexIndex; 
                }

                if (x == ChunkSettings.Size - 1)
                {
                    Edges[1, edge2Index++] = vertexIndex; 
                }

                if (z == 0)
                {
                    Edges[2, edge3Index++] = vertexIndex; 
                }

                if (z == ChunkSettings.Size - 1)
                {
                    Edges[3, edge4Index++] = vertexIndex; 
                }

                vertices[vertexIndex++] = new Vector3(x, 0, z) * ChunkSettings.Res;

                if (x == 0 || z == 0) continue;

                indices[triIndex] = (ChunkSettings.Size * x + z); //Top right
                indices[triIndex + 1] = (ChunkSettings.Size * (x - 1) + (z - 1)); //Bottom left - First triangle
                indices[triIndex + 2] = (ChunkSettings.Size * x + (z - 1)); //Bottom right
                indices[triIndex + 3] = (ChunkSettings.Size * (x - 1) + (z - 1)); //Bottom left 
                indices[triIndex + 4] = (ChunkSettings.Size * x + z); //Top right - Second triangle
                indices[triIndex + 5] = (ChunkSettings.Size * (x - 1) + z); //Top left

                triIndex += 6;
            }
                

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += new Vector3(0, simplexNoise.GetNoise2d(ChunkOffset.x + vertices[i].x, ChunkOffset.z + vertices[i].z) * strength, 0);
        }

        for (int i = 0; i < ChunkSettings.Size; i++)
        {
            AddChild(new DebugPoint(vertices[Edges[1, i]]));
        }
        
        // Calculate normals
        for (int i = 0; i < Normals.Length; i++)
        {
            Normals[i] = Vector3.Zero;
        }

        for (int i = 0; i < indices.Length; i+=3)
        {
            var vertexA = indices[i];
            var vertexB = indices[i + 2];
            var vertexC = indices[i + 1];

            var edgeAB = vertices[vertexB] - vertices[vertexA];
            var edgeAC = vertices[vertexC] - vertices[vertexA];

            var areaWeightedNormal = edgeAB.Cross(edgeAC);
            
            Normals[vertexA] += areaWeightedNormal;
            Normals[vertexB] += areaWeightedNormal;
            Normals[vertexC] += areaWeightedNormal;
        }

        for (int i = 0; i < vertices.Length; i++) 
        {
            Normals[i] = Normals[i].Normalized();
        }

        arr[(int)ArrayMesh.ArrayType.Vertex] = vertices;
        arr[(int)ArrayMesh.ArrayType.Index] = indices;
        arr[(int)ArrayMesh.ArrayType.Normal] = Normals;

        arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr);
        Mesh = arrMesh;
    }
}
