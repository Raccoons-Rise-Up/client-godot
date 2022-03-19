using Godot;
using System.Timers;

using Timer = System.Timers.Timer;

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

        private static Timer ChunkTimer;

        public override void _Ready()
        {
            ChunkTimer = new Timer(100);
            ChunkTimer.AutoReset = true;
            ChunkTimer.Elapsed += OnChunkTimedEvent;
            ChunkTimer.Enabled = true;

            Camera = GetNode<Camera>(NodePathCamera);
            Instance = this;
            var gen = new ChunkGenerator();

            // center camera on spawn
            Camera.Translation += new Vector3((WorldSize * ChunkGenerator.ChunkLength) / 2, 0, (WorldSize * ChunkGenerator.ChunkLength) / 2);
        }

        private static async void OnChunkTimedEvent(object source, ElapsedEventArgs e)
        {
            var camPos = Camera.Translation;
            var settings = ChunkGenerator.ChunkSettings;

            var curX = (int)Mathf.Floor(camPos.x / ChunkGenerator.ChunkLength);
            var curZ = (int)Mathf.Floor(camPos.z / ChunkGenerator.ChunkLength);

            var range = 12;
            for (int x = curX - range / 2; x < curX + range / 2; x++)
                for (int z = curZ - range / 2; z < curZ + range / 2; z++)
                {
                    //GD.Print(ChunkGenerator.LoadedChunks.Count);
                    /*if (ChunkGenerator.LoadedChunks.Count > 150)
                    {
                        var chunksToBeRemoved = ChunkGenerator.LoadedChunks.Count - 150;

                        for (int i = 0; i < chunksToBeRemoved; i++)
                        {
                            var pos = ChunkGenerator.LoadedChunks[i];

                            if (ChunkGenerator.ChunkData[pos.X, pos.Z] != null)
                            {
                                ChunkGenerator.ChunkData[pos.X, pos.Z].QueueFree();
                                ChunkGenerator.ChunkData[pos.X, pos.Z] = null;
                            }
                        }

                    }*/

                    for (int i = 0; i < ChunkGenerator.LoadedChunks.Count; i++)
                    {
                        var pos = ChunkGenerator.LoadedChunks[i];
                        var chunk = ChunkGenerator.ChunkData[pos.X, pos.Z];

                        if (chunk == null)
                            continue;

                        

                        var diff = (chunk.GetCenterPos() - camPos);
                        diff.y = 0;

                        if (diff.LengthSquared() > 10000) 
                        {
                            chunk.QueueFree();
                            ChunkGenerator.ChunkData[pos.X, pos.Z] = null;
                        }
                    }

                    //await World.Instance.ToSignal(World.Instance.GetTree(), "idle_frame");

                    var posX = Mathf.Clamp(x, 0, WorldSize - 1);
                    var posZ = Mathf.Clamp(z, 0, WorldSize - 1);
                    var c = ChunkGenerator.ChunkData[posX, posZ];

                    if (c == null)
                        c = CreateChunkData(posX, posZ);

                    if (!c.Generated)
                        c.GenerateMesh();

                    //var chunkPos = c.GetCenterPos();
                    //var diff = (chunkPos - camPos);
                    //diff.y = 0;




                    //if (diff.LengthSquared() > 1000)
                    //c.ClearMesh();
                }
        }

        private static Chunk CreateChunkData(int x, int z)
        {
            var c = new Chunk(ChunkGenerator.ChunkSettings, x, z);
            ChunkGenerator.ChunkData[x, z] = c;
            Instance.CallDeferred("add_child", c);
            //Instance.AddChild(c);
            return c;
        }
    }
}