using Godot;
using System;
using System.Collections.Generic;

namespace Client.Game
{
    public class Chunk : MeshInstance
    {
        // Static
        private static ChunkSettings ChunkSettings;
        private static Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");

        // Instance
        public int[,] Edges;
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public int[] Indices;
        public Vector3 ChunkOffset;

        private Chunk()
        {
            // Godot needs this
            // Private because we need to specify size, scale and pos
        }

        public Chunk(ChunkSettings chunkSettings, int x, int z)
        {
            ChunkSettings = chunkSettings;
            ChunkOffset = new Vector3(x * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res), 0, z * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res));
            Vertices = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
            Normals = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
            Indices = new int[(ChunkSettings.Size - 1) * (ChunkSettings.Size - 1) * 6];
            Edges = new int[4, ChunkSettings.Size];
        }

        public override void _Ready()
        {
            InitData();
            GenerateMesh();
        }

        public void InitData()
        {
            CalculateIndices();
            CalculateNoise();
            CalculateNormals();
        }

        private void CalculateIndices()
        {
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
                        Edges[(int)Dir.South, edge1Index++] = vertexIndex;

                    if (x == ChunkSettings.Size - 1)
                        Edges[(int)Dir.North, edge2Index++] = vertexIndex;

                    if (z == 0)
                        Edges[(int)Dir.West, edge3Index++] = vertexIndex;

                    if (z == ChunkSettings.Size - 1)
                        Edges[(int)Dir.East, edge4Index++] = vertexIndex;

                    Vertices[vertexIndex++] = new Vector3(x, 0, z) * ChunkSettings.Res;

                    if (x == 0 || z == 0) continue;

                    Indices[triIndex] = (ChunkSettings.Size * x + z); //Top right
                    Indices[triIndex + 1] = (ChunkSettings.Size * (x - 1) + (z - 1)); //Bottom left - First triangle
                    Indices[triIndex + 2] = (ChunkSettings.Size * x + (z - 1)); //Bottom right
                    Indices[triIndex + 3] = (ChunkSettings.Size * (x - 1) + (z - 1)); //Bottom left 
                    Indices[triIndex + 4] = (ChunkSettings.Size * x + z); //Top right - Second triangle
                    Indices[triIndex + 5] = (ChunkSettings.Size * (x - 1) + z); //Top left

                    triIndex += 6;
                }
        }

        private void CalculateNoise()
        {
            var strength = 3f;
            var simplexNoise = new OpenSimplexNoise();
            simplexNoise.Seed = 1234;
            simplexNoise.Octaves = 1;
            simplexNoise.Persistence = 1f;
            simplexNoise.Period = 4f;

            for (int i = 0; i < Vertices.Length; i++)
                Vertices[i] += new Vector3(0, simplexNoise.GetNoise2d(ChunkOffset.x + Vertices[i].x, ChunkOffset.z + Vertices[i].z) * strength, 0);
        }

        private void CalculateNormals()
        {
            // to calculate the normals, the data for vertices and indices is needed
            for (int i = 0; i < Normals.Length; i++)
                Normals[i] = Vector3.Zero;

            for (int i = 0; i < Indices.Length; i += 3)
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
                Normals[i] = Normals[i].Normalized();
        }

        public void GenerateMesh()
        {
            Translate(ChunkOffset);
            MaterialOverride = Material;

            Mesh = null;

            var arrMesh = new ArrayMesh();
            var arr = new Godot.Collections.Array();
            arr.Resize((int)ArrayMesh.ArrayType.Max);

            arr[(int)ArrayMesh.ArrayType.Vertex] = Vertices;
            arr[(int)ArrayMesh.ArrayType.Index] = Indices;
            arr[(int)ArrayMesh.ArrayType.Normal] = Normals;

            arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arr);
            Mesh = arrMesh;
        }
    }

    public enum Dir
    {
        South,
        North,
        West,
        East
    }
}
