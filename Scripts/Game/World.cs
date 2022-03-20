using Godot;
using System.Timers;
using System.Diagnostics;

using Timer = System.Timers.Timer;

namespace Client.Game
{
    public class World : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath NodePathCamera;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static int WorldSize = 100;
        public static int SpawnSize = 10;

        public static Camera Camera;
        public static World Instance;

        public static Timer ChunkTimer;

        public override void _Ready()
        {
            ChunkTimer = new Timer(1000);
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

            var range = 24;
            for (int i = 0; i < ChunkGenerator.LoadedChunks.Count; i++)
            {
                var pos = ChunkGenerator.LoadedChunks[i];
                var chunk = ChunkGenerator.ChunkData[pos.X, pos.Z];

                if (chunk == null)
                    continue;

                var diff = (chunk.GetCenterPos() - camPos);
                diff.y = 0;

                if (diff.LengthSquared() > 500000) 
                {
                    chunk.QueueFree();
                    ChunkGenerator.ChunkData[pos.X, pos.Z] = null;
                    ChunkGenerator.LoadedChunks.RemoveAt(i);
                }
            }

            /*var chunksToRemove = ChunkGenerator.LoadedChunks.Count - 1000;
            if (chunksToRemove > 0) 
            {
                for (int i = 0; i < chunksToRemove; i++)
                {
                    var pos = ChunkGenerator.LoadedChunks[i];
                    var chunk = ChunkGenerator.ChunkData[pos.X, pos.Z];

                    chunk.QueueFree();
                    ChunkGenerator.ChunkData[pos.X, pos.Z] = null;
                    ChunkGenerator.LoadedChunks.RemoveAt(i);
                }
            }*/
            

            await World.Instance.ToSignal(World.Instance.GetTree(), "idle_frame");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

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
                }

            stopwatch.Stop();
            GD.Print($"{stopwatch.ElapsedMilliseconds}ms");
        }

        private static Chunk CreateChunkData(int x, int z)
        {
            var c = new Chunk(ChunkGenerator.ChunkSettings, x, z);
            ChunkGenerator.ChunkData[x, z] = c;
            Instance.AddChild(c);
            return c;
        }
    }
}