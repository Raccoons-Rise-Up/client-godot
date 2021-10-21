/*
 * Kittens Rise Up is a long term progression MMORPG.
 * Copyright (C) 2021  valkyrienyanko
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Contact valkyrienyanko by joining the Kittens Rise Up discord at
 * https://discord.gg/cDNf8ja or email sebastianbelle074@protonmail.com
 */

using Common.Networking.IO;
using Common.Networking.Packet;
using Common.Game;
using ENet;
using Godot;
using KRU.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Thread = System.Threading.Thread;

namespace KRU.Networking
{
    public class ENetClient : Node
    {
        public static string IP = "127.0.0.1";
        public static ushort Port = 25565;

        public static ClientVersion Version = new ClientVersion
        {
            Major = 0,
            Minor = 1,
            Patch = 0
        };

        public static string JsonWebToken { get; set; }

        public static ConcurrentQueue<GodotInstructions> GodotCmds { get; set; }
        public static ConcurrentQueue<ENetInstructionOpcode> ENetCmds { get; set; }
        private static ConcurrentQueue<ClientPacket> Outgoing { get; set; }
        private static ConcurrentBag<Event> Incoming { get; set; }

        public static Dictionary<ServerPacketOpcode, HandlePacket> HandlePacket { get; private set; }

        private static Peer Peer { get; set; }
        private static bool TryingToConnect { get; set; }
        public static bool ConnectedToServer { get; set; }
        private static bool RunningNetCode { get; set; }
        private static SceneTree SceneTree { get; set; }

        public override void _Ready()
        {
            SceneTree = GetTree();

            // Need a way to communicate with the Unity thread from the ENet thread
            GodotCmds = new ConcurrentQueue<GodotInstructions>();

            // Need a way to communicate with the ENet thread from the Unity thread
            ENetCmds = new ConcurrentQueue<ENetInstructionOpcode>();

            // The packets that are sent to the server
            Outgoing = new ConcurrentQueue<ClientPacket>();

            // The packets received from the server
            Incoming = new ConcurrentBag<Event>();

            HandlePacket = typeof(HandlePacket).Assembly.GetTypes()
                .Where(x => typeof(HandlePacket)
                .IsAssignableFrom(x) && !x.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<HandlePacket>()
                .ToDictionary(x => x.Opcode, x => x);

            // Make sure queues are completely drained before starting
            if (Incoming != null) while (Incoming.TryTake(out _)) ;
            if (Outgoing != null) while (Outgoing.TryDequeue(out _)) ;
            if (GodotCmds != null) while (GodotCmds.TryDequeue(out _)) ;
            if (ENetCmds != null) while (ENetCmds.TryDequeue(out _)) ;
        }

        public override void _Process(float delta)
        {
            while (GodotCmds.TryDequeue(out GodotInstructions result))
            {
                foreach (var cmd in result.Instructions)
                {
                    var opcode = cmd.Key;

                    if (opcode == GodotInstructionOpcode.Quit)
                    {
                        GetTree().Quit();
                    }
                }
            }
        }

        public override void _Notification(int what)
        {
            // Called when user presses top right window X button or does Alt + F4
            if (what == MainLoop.NotificationWmQuitRequest)
            {
                FreeUpNodes();

                if (ConnectedToServer)
                {
                    ENetCmds.Enqueue(ENetInstructionOpcode.UserWantsToQuit);

                    GetTree().SetAutoAcceptQuit(false);

                    return;
                }

                GetTree().Quit();
            }
        }

        // Exit application from main menu by pressing "Quit" button
        public static void ExitApplication()
        {
            FreeUpNodes();

            SceneTree.Quit();
        }

        private static void FreeUpNodes()
        {
            if (UIGame.ResourceInfoData != null)
                foreach (var icon in UIGame.ResourceIconData.Values)
                    icon.Free();
        }

        public static void Connect()
        {
            if (TryingToConnect || ConnectedToServer)
                return;

            TryingToConnect = true;
            new Thread(WorkerThread).Start();
        }

        public bool IsConnected() => ConnectedToServer;

        private static void WorkerThread()
        {
            Library.Initialize();

            using (Host client = new Host())
            {
                var address = new Address();
                address.SetHost(IP);
                address.Port = Port;
                client.Create();

                uint pingInterval = 1000; // Pings are used both to monitor the liveness of the connection and also to dynamically adjust the throttle during periods of low traffic so that the throttle has reasonable responsiveness during traffic spikes.
                uint timeout = 5000; // Will be ignored if maximum timeout is exceeded
                uint timeoutMinimum = 5000; // The timeout for server not sending the packet to the client sent from the server
                uint timeoutMaximum = 5000; // The timeout for server not receiving the packet sent from the client

                Peer = client.Connect(address);
                Peer.PingInterval(pingInterval);
                Peer.Timeout(timeout, timeoutMinimum, timeoutMaximum);
                UILogin.UpdateResponse("Attempting to connect...");

                bool wantsToQuit = false;

                RunningNetCode = true;
                while (RunningNetCode)
                {
                    var polled = false;

                    // ENet Instructions (from Godot Thread)
                    while (ENetCmds.TryDequeue(out ENetInstructionOpcode result))
                    {
                        if (result == ENetInstructionOpcode.Disconnect)
                        {
                            GD.Print("Disconnected");
                            Peer.Disconnect(0);
                            RunningNetCode = false;
                            break;
                        }

                        if (result == ENetInstructionOpcode.CancelConnection)
                        {
                            GD.Print("Cancel connection");
                            RunningNetCode = false;
                            break;
                        }

                        if (result == ENetInstructionOpcode.UserWantsToQuit)
                        {
                            Peer.Disconnect(0);
                            wantsToQuit = true;
                            RunningNetCode = false;
                            break;
                        }
                    }

                    // Incoming
                    while (Incoming.TryTake(out Event netEvent))
                    {
                        var packetSizeMax = 8192;
                        var readBuffer = new byte[packetSizeMax];
                        var packetReader = new PacketReader(readBuffer);
                        packetReader.BaseStream.Position = 0;

                        netEvent.Packet.CopyTo(readBuffer);

                        var opcode = (ServerPacketOpcode)packetReader.ReadByte();
                        GD.Print(opcode);

                        HandlePacket[opcode].Handle(netEvent, packetReader);

                        netEvent.Packet.Dispose();
                    }

                    // Outgoing
                    while (Outgoing.TryDequeue(out ClientPacket clientPacket))
                    {
                        switch ((ClientPacketOpcode)clientPacket.Opcode)
                        {
                            case ClientPacketOpcode.Login:
                                GD.Print("Sending login request to game server..");

                                Send(clientPacket, PacketFlags.Reliable);

                                break;

                            case ClientPacketOpcode.PurchaseItem:
                                GD.Print("Sending purchase item request to game server..");

                                Send(clientPacket, PacketFlags.Reliable);

                                break;
                        }
                    }

                    // Receiving Data
                    while (!polled)
                    {
                        if (client.CheckEvents(out Event netEvent) <= 0)
                        {
                            if (client.Service(15, out netEvent) <= 0)
                                break;

                            polled = true;
                        }

                        var eventType = netEvent.Type;

                        if (eventType == EventType.None)
                        {
                            GD.Print("Nothing");
                        }

                        if (eventType == EventType.Connect)
                        {
                            // Successfully connected to the game server
                            GD.Print("Client connected to game server");

                            // Send login request
                            var clientPacket = new ClientPacket((byte)ClientPacketOpcode.Login, new WPacketLogin
                            {
                                JsonWebToken = JsonWebToken,
                                VersionMajor = Version.Major,
                                VersionMinor = Version.Minor,
                                VersionPatch = Version.Patch
                            });

                            Outgoing.Enqueue(clientPacket);

                            // Keep track of networking logic
                            TryingToConnect = false;
                            ConnectedToServer = true;
                        }

                        if (eventType == EventType.Disconnect)
                        {
                            var opcode = (DisconnectOpcode)netEvent.Data;

                            switch (opcode)
                            {
                                case DisconnectOpcode.Disconnected:
                                    UILogin.UpdateResponse("Client was disconnected");
                                    break;

                                case DisconnectOpcode.Maintenance:
                                    UILogin.UpdateResponse("Client was disconnected because the server is going down for maintenance");
                                    break;

                                case DisconnectOpcode.Restarting:
                                    UILogin.UpdateResponse("Client was disconnected because the server is restarting");
                                    break;

                                case DisconnectOpcode.Kicked:
                                    UILogin.UpdateResponse("Client was kicked");
                                    break;

                                case DisconnectOpcode.Banned:
                                    UILogin.UpdateResponse("Client was banned");
                                    break;
                            }

                            RunningNetCode = false;
                            UILogin.LoadMenuScene();
                        }

                        if (eventType == EventType.Timeout)
                        {
                            UILogin.UpdateResponse("Client connection timeout to game server");
                            RunningNetCode = false;
                            UILogin.LoadMenuScene();
                        }

                        if (eventType == EventType.Receive)
                        {
                            //GD.Print("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + packet.Length);
                            Incoming.Add(netEvent);
                        }
                    }
                }

                client.Flush();
                client.Dispose();

                Library.Deinitialize();

                ConnectedToServer = false;
                TryingToConnect = false;

                if (wantsToQuit)
                    GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.Quit));
            }
        }

        public static void PurchaseItem(StructureType itemId)
        {
            var data = new WPacketPurchaseItem { StructureID = (ushort)itemId };
            var clientPacket = new ClientPacket((byte)ClientPacketOpcode.PurchaseItem, data);

            Outgoing.Enqueue(clientPacket);
        }

        private static void Send(GamePacket gamePacket, PacketFlags packetFlags)
        {
            byte channelID = 0; // The channel all networking traffic will be going through
            var packet = default(Packet);
            packet.Create(gamePacket.Data, packetFlags);
            Peer.Send(channelID, ref packet);
        }
    }

    public struct ClientVersion
    {
        public byte Major { get; set; }
        public byte Minor { get; set; }
        public byte Patch { get; set; }
    }

    public class GodotInstructions
    {
        public Dictionary<GodotInstructionOpcode, List<object>> Instructions { get; set; }

        public GodotInstructions()
        {
            Instructions = new Dictionary<GodotInstructionOpcode, List<object>>();
        }

        public GodotInstructions(GodotInstructionOpcode opcode)
        {
            Instructions = new Dictionary<GodotInstructionOpcode, List<object>>
            {
                [opcode] = null
            };
        }

        public void Set(GodotInstructionOpcode opcode, params object[] data)
        {
            Instructions[opcode] = new List<object>(data);
        }
    }

    public enum GodotInstructionOpcode
    {
        Quit
    }

    public enum ENetInstructionOpcode
    {
        CancelConnection,
        UserWantsToQuit,
        Disconnect
    }
}