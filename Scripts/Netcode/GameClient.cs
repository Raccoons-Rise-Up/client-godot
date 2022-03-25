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
        public static string Username = "Unnamed";

        public override void ProcessGodotCommands(GodotCmd cmd)
        {
            switch (cmd.Opcode)
            {
                case GodotOpcode.LoadMainMenu:
                    UIGameMenu.ClientPressedDisconnect = false;
                    UIMainMenu.LoadMainMenu();
                    break;
            }
        }

        public override void Connect(Event netEvent)
        {
            GDLog("Client connected to server");
        }

        public override void Timeout(Event netEvent)
        {
            GDLog("Client connection timeout");
        }

        public override void Disconnect(Event netEvent)
        {
            GDLog("Client disconnected from server");
        }
    }
}