using Godot;
using System;
using Client.Netcode;
using Client.Utils;

namespace Client.UI 
{
    public class UIMainMenu : Node
    {
        private void _on_Multiplayer_pressed() => GetTree().ChangeScene("res://Scenes/Login.tscn");
    }
}
