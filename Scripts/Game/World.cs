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

            foreach (var chunk in ChunkGenerator.Chunks)
            {
                var chunkLength = settings.Size * settings.Res - settings.Res;
                var x = (int)Mathf.Clamp(Mathf.Floor(camPos.x / chunkLength), 0, ChunkGenerator.WorldSize - 1);
                var z = (int)Mathf.Clamp(Mathf.Floor(camPos.z / chunkLength), 0, ChunkGenerator.WorldSize - 1);

                var c = ChunkGenerator.Chunks[x, z];
                if (c == null)
                {
                    c = new Chunk(settings, x, z);
                    ChunkGenerator.Chunks[x, z] = c;
                    AddChild(c);
                }

                if (!c.Generated)
                {
                    c.GenerateMesh();
                }
            }
        }
    }
}