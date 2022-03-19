using Godot;

namespace Client.Game
{
    public class World : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath NodePathCamera;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static Camera Camera;
        public static World Instance;

        public override void _Ready()
        {
            Camera = GetNode<Camera>(NodePathCamera);
            Instance = this;
            var gen = new ChunkGenerator();
        }

        public override void _Process(float delta)
        {
            var camPos = Camera.Translation;
            var settings = ChunkGenerator.ChunkSettings;

            var chunkLength = settings.Size * settings.Res - settings.Res;
            var curX = (int)Mathf.Floor(camPos.x / chunkLength);
            var curZ = (int)Mathf.Floor(camPos.z / chunkLength);

            var chunkDataRange = 12;
            for (int x = curX - chunkDataRange / 2; x < curX + chunkDataRange / 2; x++)
            {
                for (int z = curZ - chunkDataRange / 2; z < curZ + chunkDataRange / 2; z++)
                {
                    AddChunkData(Mathf.Clamp(x, 0, ChunkGenerator.WorldSize - 1), Mathf.Clamp(z, 0, ChunkGenerator.WorldSize - 1));
                }
            }

            var meshDataRange = 6;
            for (int x = curX - meshDataRange / 2; x < curX + meshDataRange / 2; x++)
            {
                for (int z = curZ - meshDataRange / 2; z < curZ + meshDataRange / 2; z++)
                {
                    var c = ChunkGenerator.ChunkData[Mathf.Clamp(x, 0, ChunkGenerator.WorldSize - 1), Mathf.Clamp(z, 0, ChunkGenerator.WorldSize - 1)];
                    
                    if (!c.Generated) 
                    {
                        c.SmoothEdgeNormals(); // also generates mesh
                    }
                }
            }
        }

        private void AddChunkData(int x, int z)
        {
            var c = ChunkGenerator.ChunkData[x, z];

            if (c == null)
            {
                c = new Chunk(ChunkGenerator.ChunkSettings, x, z);
                ChunkGenerator.ChunkData[x, z] = c;
                AddChild(c);
            }
        }
    }
}