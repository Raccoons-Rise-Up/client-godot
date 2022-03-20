using Godot;
using Client.Utilities;
using System;
using System.Collections.Generic;

namespace Client.Game
{
    public class Chunk : MeshInstance
    {
        private static ChunkSettings ChunkSettings;
        private static Material Material = ResourceLoader.Load<Material>("res://Materials/Grass.tres");

        private Vector3[] Normals;
        public int[,] Edges;
        public bool Generated;
        
        private Vector3[] Vertices;
        private int[] Indices;
        private Vector3 ChunkOffset;
        private Vector3 ChunkLength;
        private int X, Z;

        private Chunk()
        {
            // Godot needs this
        }

        public Chunk(ChunkSettings chunkSettings, int x, int z)
        {
            ChunkSettings = chunkSettings;
            X = x;
            Z = z;
            ChunkGenerator.LoadedChunks.Add(new Pos { X = X, Z = Z });

            var chunkSize = ChunkSettings.Size;
            var res = ChunkSettings.Res;

            Normals = new Vector3[chunkSize * chunkSize];
            Edges = new int[4, chunkSize];

            Vertices = new Vector3[chunkSize * chunkSize];
            Indices = new int[(chunkSize - 1) * (chunkSize - 1) * 6];

            ChunkLength = new Vector3(chunkSize * res - res, 0, chunkSize * res - res);
            ChunkOffset = new Vector3(X, 0, Z) * ChunkLength;

            Translate(ChunkOffset);
            MaterialOverride = Material;
            
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
            // fix normal chunk edge seams

            // check not to go outside boundaries of array
            if (X + 1 > World.WorldSize - 1)
                return;
            if (X - 1 < 0)
                return;
            if (Z + 1 > World.WorldSize - 1)
                return;
            if (Z - 1 < 0)
                return;

            // n is for neighbor
            var n1 = ChunkGenerator.ChunkData[X + 1, Z];
            var n2 = ChunkGenerator.ChunkData[X - 1, Z];
            var n3 = ChunkGenerator.ChunkData[X, Z + 1];
            var n4 = ChunkGenerator.ChunkData[X, Z - 1];
            
            if (n1 == null || n2 == null || n3 == null || n4 == null)
                return;

            var n0 = this;
            var n0EdgeNorth = GetEdgeNormals(n0, Dir.North);
            var n0EdgeEast  = GetEdgeNormals(n0, Dir.East);
            var n0EdgeWest  = GetEdgeNormals(n0, Dir.West);

            var n1EdgeSouth = GetEdgeNormals(n1, Dir.South);
            var n3EdgeWest  = GetEdgeNormals(n3, Dir.West);
            var n4EdgeEast  = GetEdgeNormals(n4, Dir.East);

            // Take average of normals
            for (int i = 0; i < ChunkSettings.Size; i++)
            {
                var avgNS = (n0EdgeNorth[i] + n1EdgeSouth[i]) / 2;
                n0.Normals[n0.Edges[(int)Dir.North, i]] = avgNS;
                n1.Normals[n1.Edges[(int)Dir.South, i]] = avgNS;

                var avgEW = (n0EdgeEast[i] + n3EdgeWest[i]) / 2;
                n0.Normals[n0.Edges[(int)Dir.East, i]] = avgEW;
                n3.Normals[n3.Edges[(int)Dir.West, i]] = avgEW;

                var avgWE = (n0EdgeWest[i] + n4EdgeEast[i]) / 2;
                n0.Normals[n0.Edges[(int)Dir.West, i]] = avgWE;
                n4.Normals[n4.Edges[(int)Dir.East, i]] = avgWE;
            }

            GenMesh();
        }

        private void GenMesh()
        {
            // generate mesh
            Generated = true;
            
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

        private Vector3[] GetEdgeNormals(Chunk chunk, Dir direction)
        {
            var arr = new Vector3[ChunkSettings.Size];
            for (int i = 0; i < ChunkSettings.Size; i++)
                arr[i] = chunk.Normals[chunk.Edges[(int)direction, i]];
            
            return arr;
        }

        public void ClearMesh()
        {
            //var loadedChunks = ChunkGenerator.LoadedChunks;
            //loadedChunks.Remove(new Pos{X = X, Z = Z});
            Generated = false;
            Mesh = null;
        }

        public Vector3 GetCenterPos() => ChunkOffset + ChunkLength / 2;

        public int GetVertexCount() => ChunkSettings.Size * ChunkSettings.Size;
    }

    public enum Dir
    {
        South,
        North,
        West,
        East
    }
}
