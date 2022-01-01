using Godot;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ENet;

using Thread = System.Threading.Thread;

namespace Client.Netcode
{
    public class ENetClient : Node
    {
        private static ConcurrentQueue<GodotCmd> GodotCmds = new ConcurrentQueue<GodotCmd>();
        private static ConcurrentQueue<ENetCmd> ENetCmds = new ConcurrentQueue<ENetCmd>();
        private static bool ENetThreadRunning;
        private static bool RunningNetCode;

        public override void _Process(float delta)
        {
            while (GodotCmds.TryDequeue(out GodotCmd cmd)) 
            {
                switch (cmd.Opcode)
                {
                    case GodotOpcode.LogMessage:
                        GD.Print((string)cmd.Data[0]);
                        break;
                    case GodotOpcode.ExitApp:
                        GetTree().Quit();
                        break;
                }
            }
        }

        public override void _Notification(int what)
        {
            // Called when user presses top right window X button or does Alt + F4
            if (what == MainLoop.NotificationWmQuitRequest) 
            {
                if (ENetThreadRunning)
                {
                    GetTree().SetAutoAcceptQuit(false);
                    ENetCmds.Enqueue(new ENetCmd { Opcode = ENetOpcode.ClientWantsToExitApp });
                    return;
                }

                GetTree().Quit();
            }
        }

        public static void Connect(string ip, ushort port) 
        {
            if (ENetThreadRunning) 
            {
                GD.Print("ENet thread is running already");
                return;
            }
            
            ENetThreadRunning = true;
            new Thread(() => ENetThreadWorker(ip, port)).Start();
        }

        private static void ENetThreadWorker(string ip, ushort port)
        {
            Library.Initialize();
            var wantsToExit = false;

            using (var client = new Host()) 
            {
                var address = new Address();

                address.SetHost(ip);
                address.Port = port;
                client.Create();

                GDLog("Connecting...");
                var peer = client.Connect(address);

                uint pingInterval = 1000; // Pings are used both to monitor the liveness of the connection and also to dynamically adjust the throttle during periods of low traffic so that the throttle has reasonable responsiveness during traffic spikes.
                uint timeout = 5000; // Will be ignored if maximum timeout is exceeded
                uint timeoutMinimum = 5000; // The timeout for server not sending the packet to the client sent from the server
                uint timeoutMaximum = 5000; // The timeout for server not receiving the packet sent from the client

                peer.PingInterval(pingInterval); 
                peer.Timeout(timeout, timeoutMinimum, timeoutMaximum);

                Event netEvent;

                RunningNetCode = true;
                while (RunningNetCode) {
                    var polled = false;

                    // ENet Cmds from Godot Thread
                    while (ENetCmds.TryDequeue(out ENetCmd cmd))
                    {
                        switch (cmd.Opcode)
                        {
                            case ENetOpcode.ClientWantsToExitApp:
                                peer.Disconnect(0);
                                RunningNetCode = false;
                                wantsToExit = true;
                                break;
                        }
                    }

                    while (!polled) {
                        if (client.CheckEvents(out netEvent) <= 0) {
                            if (client.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }

                        switch (netEvent.Type) {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                Console.WriteLine("Client connected to server");
                                break;

                            case EventType.Disconnect:
                                Console.WriteLine("Client disconnected from server");
                                break;

                            case EventType.Timeout:
                                Console.WriteLine("Client connection timeout");
                                break;

                            case EventType.Receive:
                                Console.WriteLine($"Packet received from server - Channel ID: {netEvent.ChannelID}, Data length: {netEvent.Packet.Length}");
                                netEvent.Packet.Dispose();
                                break;
                        }
                    }
                }

                client.Flush();
            }

            Library.Deinitialize();
            ENetThreadRunning = false;

            if (wantsToExit)
                GodotCmds.Enqueue(new GodotCmd { Opcode = GodotOpcode.ExitApp });
        }

        private static void GDLog(string text) => GodotCmds.Enqueue(new GodotCmd { Opcode = GodotOpcode.LogMessage, Data = new List<object> { text }});
    }

    public class GodotCmd 
    {
        public GodotOpcode Opcode { get; set; }
        public List<object> Data { get; set; }
    }

    public class ENetCmd 
    {
        public ENetOpcode Opcode { get; set; }
        public List<object> Data { get; set; }
    }

    public enum GodotOpcode 
    {
        LogMessage,
        ExitApp
    }

    public enum ENetOpcode 
    {
        ClientWantsToExitApp
    }
}
