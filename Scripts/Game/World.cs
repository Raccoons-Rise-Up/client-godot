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



            /*var chunkDataRange = 24;
            for (int x = curX - chunkDataRange / 2; x < curX + chunkDataRange / 2; x++)
                for (int z = curZ - chunkDataRange / 2; z < curZ + chunkDataRange / 2; z++)
                    AddChunkData(Mathf.Clamp(x, 0, WorldSize - 1), Mathf.Clamp(z, 0, WorldSize - 1));

            var meshDataRange = 12;
            for (int x = curX - meshDataRange / 2; x < curX + meshDataRange / 2; x++)
                for (int z = curZ - meshDataRange / 2; z < curZ + meshDataRange / 2; z++)
                {
                    var c = ChunkGenerator.ChunkData[Mathf.Clamp(x, 0, WorldSize - 1), Mathf.Clamp(z, 0, WorldSize - 1)];
                    
                    if (!c.Generated) 
                        c.GenerateMesh();
                }*/

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

                    var dist = c.GetCenterPos().DistanceSquaredTo(camPos);
                    if (dist < 1000)
                        c.ClearMesh();
                }

            /*for (int i = 0; i < ChunkGenerator.LoadedChunks.Count; i++)
            {
                var pos = ChunkGenerator.LoadedChunks[i];
                var chunk = ChunkGenerator.ChunkData[pos.X, pos.Z];
                var dist = chunk.GetCenterPos().DistanceSquaredTo(camPos);

                if (dist < 500)
                    AddChunkData(pos.X, pos.Z);

                if (dist < 250)
                    chunk.GenerateMesh();

                if (dist > 1000)
                    chunk.ClearMesh();
            }*/
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