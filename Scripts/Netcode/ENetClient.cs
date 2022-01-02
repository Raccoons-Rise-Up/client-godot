using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ENet;
using Common.Netcode;
using Client.UI;

using Thread = System.Threading.Thread; // CS0104: Ambigious reference between 'Godot.Thread' and 'System.Threading.Thread'
using Version = Common.Netcode.Version; // CS0104: Ambiguous reference between 'Common.Netcode.Version' and 'System.Version'

namespace Client.Netcode
{
    public class ENetClient : Node
    {
        public static Version Version = new Version { Major = 0, Minor = 1, Patch = 0 };
        private static ConcurrentQueue<GodotCmd> GodotCmds = new ConcurrentQueue<GodotCmd>();
        private static ConcurrentQueue<ENetCmd> ENetCmds = new ConcurrentQueue<ENetCmd>();
        public static ConcurrentQueue<ClientPacket> Outgoing = new ConcurrentQueue<ClientPacket>();
        private static Dictionary<ServerPacketOpcode, HandlePacket> HandlePacket { get; set; }
        private static bool ENetThreadRunning { get; set; }
        private static bool RunningNetCode { get; set; }
        public static uint ClientId { get; set; }

        public override void _Ready()
        {
            HandlePacket = typeof(HandlePacket).Assembly.GetTypes().Where(x => typeof(HandlePacket).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<HandlePacket>().ToDictionary(x => x.Opcode, x => x);
        }

        public override void _Process(float delta)
        {
            while (GodotCmds.TryDequeue(out GodotCmd cmd)) 
            {
                switch (cmd.Opcode)
                {
                    case GodotOpcode.ENetPacket:
                        var packetReader = (PacketReader)cmd.Data[0];
                        var opcode = (ServerPacketOpcode)packetReader.ReadByte();

                        HandlePacket[opcode].Handle(packetReader);
                        break;
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

        public static void Connect(string ip, ushort port, string jwt) 
        {
            if (ENetThreadRunning) 
            {
                GD.Print("ENet thread is running already");
                return;
            }
            
            ENetThreadRunning = true;
            new Thread(() => ENetThreadWorker(ip, port, jwt)).Start();
        }

        private static void ENetThreadWorker(string ip, ushort port, string jwt)
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

                    // Outgoing
                    while (Outgoing.TryDequeue(out ClientPacket clientPacket))
                    {
                        byte channelID = 0; // The channel all networking traffic will be going through
                        var packet = default(Packet);
                        packet.Create(clientPacket.Data, clientPacket.PacketFlags);
                        peer.Send(channelID, ref packet);
                    }

                    while (!polled) {
                        if (client.CheckEvents(out Event netEvent) <= 0) {
                            if (client.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }

                        switch (netEvent.Type) {
                            case EventType.None:
                                break;

                            case EventType.Connect:
                                GDLog("Client connected to server");

                                // Send login request
                                Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.Login, new WPacketLogin
                                {
                                    JsonWebToken = jwt,
                                    VersionMajor = Version.Major,
                                    VersionMinor = Version.Minor,
                                    VersionPatch = Version.Patch
                                }));
                                break;

                            case EventType.Disconnect:
                                GDLog("Client disconnected from server");
                                break;

                            case EventType.Timeout:
                                GDLog("Client connection timeout");
                                break;

                            case EventType.Receive:
                                var packet = netEvent.Packet;
                                
                                if (packet.Length > GamePacket.MaxSize) 
                                {
                                    GDLog($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
                                    packet.Dispose();
                                    continue;
                                }

                                GodotCmds.Enqueue(new GodotCmd { Opcode = GodotOpcode.ENetPacket, Data = new List<object> { new PacketReader(packet) }});
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
        ENetPacket,
        LogMessage,
        ExitApp
    }

    public enum ENetOpcode 
    {
        ClientWantsToExitApp
    }
}
