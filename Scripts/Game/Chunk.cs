using Godot;
using System;
using System.Collections.Generic;

namespace Client.Game
{
    public class Chunk : MeshInstance
    {
        private static ChunkSettings ChunkSettings;
        private static Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");
        private int X, Z;

        private Chunk()
        {
            // Godot needs this
            // Private because we need to specify size, scale and pos
        }

        public Chunk(ChunkSettings chunkSettings, int x, int z)
        {
            ChunkSettings = chunkSettings;
            X = x;
            Z = z;
        }

        public override void _Ready()
        {
            var vertices = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
            var indices = new int[(ChunkSettings.Size - 1) * (ChunkSettings.Size - 1) * 6];
            var normals = new Vector3[ChunkSettings.Size * ChunkSettings.Size];
            var edges = new int[4, ChunkSettings.Size];
            var chunkOffset = new Vector3(X * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res), 0, Z * (ChunkSettings.Size * ChunkSettings.Res - ChunkSettings.Res));
            
            CalculateIndices(vertices, indices, edges);
            CalculateNoise(vertices, chunkOffset);
            CalculateNormals(vertices, indices, normals);
            GenerateMesh(vertices, indices, normals, chunkOffset);
        }

        private void CalculateIndices(Vector3[] vertices, int[] indices, int[,] edges)
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
                        edges[(int)Dir.South, edge1Index++] = vertexIndex;

                    if (x == ChunkSettings.Size - 1)
                        edges[(int)Dir.North, edge2Index++] = vertexIndex;

                    if (z == 0)
                        edges[(int)Dir.West, edge3Index++] = vertexIndex;

                    if (z == ChunkSettings.Size - 1)
                        edges[(int)Dir.East, edge4Index++] = vertexIndex;

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
        }

        private void CalculateNoise(Vector3[] vertices, Vector3 chunkOffset)
        {
            var strength = 3f;
            var simplexNoise = new OpenSimplexNoise();
            simplexNoise.Seed = 1234;
            simplexNoise.Octaves = 1;
            simplexNoise.Persistence = 1f;
            simplexNoise.Period = 4f;

            for (int i = 0; i < vertices.Length; i++)
                vertices[i] += new Vector3(0, simplexNoise.GetNoise2d(chunkOffset.x + vertices[i].x, chunkOffset.z + vertices[i].z) * strength, 0);
        }

        private void CalculateNormals(Vector3[] vertices, int[] indices, Vector3[] normals)
        {
            for (int i = 0; i < normals.Length; i++)
                normals[i] = Vector3.Zero;

            for (int i = 0; i < indices.Length; i += 3)
            {
                var vertexA = indices[i];
                var vertexB = indices[i + 2];
                var vertexC = indices[i + 1];

                var edgeAB = vertices[vertexB] - vertices[vertexA];
                var edgeAC = vertices[vertexC] - vertices[vertexA];

                var areaWeightedNormal = edgeAB.Cross(edgeAC);

                normals[vertexA] += areaWeightedNormal;
                normals[vertexB] += areaWeightedNormal;
                normals[vertexC] += areaWeightedNormal;
            }

            for (int i = 0; i < vertices.Length; i++)
                normals[i] = normals[i].Normalized();
        }

        private void GenerateMesh(Vector3[] vertices, int[] indices, Vector3[] normals, Vector3 chunkOffset)
        {
            Translate(chunkOffset);
            MaterialOverride = Material;

            Mesh = null;

            var arrMesh = new ArrayMesh();
            var arr = new Godot.Collections.Array();
            arr.Resize((int)ArrayMesh.ArrayType.Max);

            arr[(int)ArrayMesh.ArrayType.Vertex] = vertices;
            arr[(int)ArrayMesh.ArrayType.Index] = indices;
            arr[(int)ArrayMesh.ArrayType.Normal] = normals;

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
