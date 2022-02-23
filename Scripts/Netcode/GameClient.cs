using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ENet;
using Common.Netcode;
using Client.UI;
using Client.Utilities;

using Thread = System.Threading.Thread; // CS0104: Ambigious reference between 'Godot.Thread' and 'System.Threading.Thread'
using Version = Common.Netcode.Version; // CS0104: Ambiguous reference between 'Common.Netcode.Version' and 'System.Version'

namespace Client.Netcode
{
    public class GameClient : ENetClient 
    {
        public static readonly Version Version = new Version { Major = 0, Minor = 1, Patch = 0 };

        public override void ProcessGodotCommands(GodotCmd cmd)
        {
            switch (cmd.Opcode)
            {
                case GodotOpcode.LogMessage:
                    GD.Print((string)cmd.Data[0]);
                    break;
                case GodotOpcode.LoadMainMenu:
                    UIGameMenu.ClientPressedDisconnect = false;
                    UIMainMenu.LoadMainMenu();
                    break;
                case GodotOpcode.ExitApp:
                    GetTree().Quit();
                    break;
            }
        }

        public override void Connect(Event netEvent, string jwt)
        {
            // Connect
            GDLog("Client connected to server");

            // Send login request
            Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.Login, new WPacketLogin
            {
                JsonWebToken = jwt,
                VersionMajor = Version.Major,
                VersionMinor = Version.Minor,
                VersionPatch = Version.Patch
            }));
        }

        public override void Timeout(Event netEvent)
        {
            // Timeout
            GDLog("Client connection timeout");
        }

        public override void Disconnect(Event netEvent)
        {
            // Disconnect
            GDLog("Client disconnected from server");
        }
    }
}