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

using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using ENet;
using Common.Networking.Packet;

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

        public static ConcurrentQueue<GodotInstructions> GodotCmds { get; set; }
        private static ConcurrentQueue<ENetInstructionOpcode> ENetCmds { get; set; }
        private static ConcurrentQueue<ClientPacket> Outgoing { get; set; }
        private static ConcurrentBag<Event> Incoming { get; set; }

        private static Peer Peer { get; set; }
        private static bool TryingToConnect { get; set; }
        private static bool ConnectedToServer { get; set; }
        private static bool RunningNetCode { get; set; }
        private static bool ReadyToQuitUnity { get; set; }

        private static DateTime LastHutPurchase { get; set; }

        public override void _Ready()
        {
            GD.Print("NEW INSTANCE");

            //Global.ENetClient = this;

            // Need a way to communicate with the Unity thread from the ENet thread
            GodotCmds = new ConcurrentQueue<GodotInstructions>();

            // Need a way to communicate with the ENet thread from the Unity thread
            ENetCmds = new ConcurrentQueue<ENetInstructionOpcode>();

            // The packets that are sent to the server
            Outgoing = new ConcurrentQueue<ClientPacket>();

            // The packets received from the server
            Incoming = new ConcurrentBag<Event>();

            // Make sure queues are completely drained before starting
            if (Incoming != null) while (Incoming.TryTake(out _)) ;
            if (Outgoing != null) while (Outgoing.TryDequeue(out _)) ;
            if (GodotCmds != null) while (GodotCmds.TryDequeue(out _)) ;
            if (ENetCmds != null) while (ENetCmds.TryDequeue(out _)) ;


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
                GD.Print("Attempting to connect...");

                bool wantsToQuit = false;

                RunningNetCode = true;
                while (RunningNetCode)
                {
                    var polled = false;

                    // ENet Instructions (from Unity Thread)
                    while (ENetCmds.TryDequeue(out ENetInstructionOpcode result))
                    {
                        if (result == ENetInstructionOpcode.CancelConnection)
                        {
                            GD.Print("Cancel connection");
                            ConnectedToServer = false;
                            TryingToConnect = false;
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
                        //HandlePacket.Handle(ref netEvent);
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
                                LastHutPurchase = DateTime.Now;

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
                                Username = "appleman",
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
                            var cmd = new GodotInstructions();

                            switch (opcode)
                            {
                                case DisconnectOpcode.Disconnected:
                                    cmd.Set(GodotInstructionOpcode.Disconnect, DisconnectOpcode.Disconnected);
                                    GD.Print("Client was disconnected");
                                    break;
                                case DisconnectOpcode.Maintenance:
                                    cmd.Set(GodotInstructionOpcode.Disconnect, DisconnectOpcode.Maintenance);
                                    GD.Print("Client was disconnected because the server is going down for maintenance");
                                    break;
                                case DisconnectOpcode.Restarting:
                                    cmd.Set(GodotInstructionOpcode.Disconnect, DisconnectOpcode.Restarting);
                                    GD.Print("Client was disconnected because the server is restarting");
                                    break;
                                case DisconnectOpcode.Kicked:
                                    cmd.Set(GodotInstructionOpcode.Disconnect, DisconnectOpcode.Kicked);
                                    GD.Print("Client was kicked");
                                    break;
                                case DisconnectOpcode.Banned:
                                    cmd.Set(GodotInstructionOpcode.Disconnect, DisconnectOpcode.Banned);
                                    GD.Print("Client was banned");
                                    break;
                            }

                            TryingToConnect = false;
                            ConnectedToServer = false;
                            GodotCmds.Enqueue(cmd);
                        }

                        if (eventType == EventType.Timeout)
                        {
                            GD.Print("Client connection timeout to game server");
                            TryingToConnect = false;
                            ConnectedToServer = false;
                            GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.Timeout));
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

                if (wantsToQuit)
                {
                    ReadyToQuitUnity = true;
                    GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.Quit));
                }
            }
        }

        private static void Send(GamePacket gamePacket, PacketFlags packetFlags)
        {
            byte channelID = 0; // The channel all networking traffic will be going through
            var packet = default(Packet);
            packet.Create(gamePacket.Data, packetFlags);
            Peer.Send(channelID, ref packet);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {

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
        LoadMainScene,
        LogMessage,
        ServerResponseMessage,
        Timeout,
        Disconnect,
        LoginSuccess,
        Quit
    }

    public enum ENetInstructionOpcode
    {
        CancelConnection,
        UserWantsToQuit
    }
}