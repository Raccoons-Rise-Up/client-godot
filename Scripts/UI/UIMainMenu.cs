using Godot;
using System;
using Client.Netcode;

namespace Client.UI 
{
    public class UIMainMenu : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly string GameServerIp;
        [Export] private readonly ushort GameServerPort;
#pragma warning restore CS0649 // Values are assigned in the editor

        private void _on_Login_pressed()
        {
            ENetClient.Connect(GameServerIp, GameServerPort);
        }
    }
}
