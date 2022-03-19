using Godot;

namespace Client.Game
{
    public class World : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath NodePathCamera;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static int WorldSize = 100;
        public static int SpawnSize = 3;

        public static Camera Camera;
        public static World Instance;

        public override void _Ready()
        {
            Camera = GetNode<Camera>(NodePathCamera);
            Instance = this;
            var gen = new ChunkGenerator();

            // center camera on spawn
            Camera.Translation += new Vector3((WorldSize * ChunkGenerator.ChunkLength) / 2, 0, (WorldSize * ChunkGenerator.ChunkLength) / 2);
        }

        public override void _Process(float delta)
        {
            var camPos = Camera.Translation;
            var settings = ChunkGenerator.ChunkSettings;

            var curX = (int)Mathf.Floor(camPos.x / ChunkGenerator.ChunkLength);
            var curZ = (int)Mathf.Floor(camPos.z / ChunkGenerator.ChunkLength);

            var range = 12;
            for (int x = curX - range / 2; x < curX + range / 2; x++)
                for (int z = curZ - range / 2; z < curZ + range / 2; z++)
                {
                    var posX = Mathf.Clamp(x, 0, WorldSize - 1);
                    var posZ = Mathf.Clamp(z, 0, WorldSize - 1);
                    var c = ChunkGenerator.ChunkData[posX, posZ];

                    if (c == null)
                        c = CreateChunkData(posX, posZ);

                    if (!c.Generated)
                        c.GenerateMesh();

                    var chunkPos = c.GetCenterPos();
                    var diff = (chunkPos - camPos);
                    diff.y = 0;

                    if (diff.LengthSquared() > 1000)
                        c.ClearMesh();
                }
        }

        private Chunk CreateChunkData(int x, int z)
        {
            var c = new Chunk(ChunkGenerator.ChunkSettings, x, z);
            ChunkGenerator.ChunkData[x, z] = c;
            AddChild(c);
            return c;
        }
    }
}