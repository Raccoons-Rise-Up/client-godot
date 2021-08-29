using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
    public class UILogin : Node
    {
        private ENetClient ENetClient;

        public override void _Ready()
        {
            ENetClient = GetNode<ENetClient>("/root/ENetClient");
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                    GetTree().ChangeScene("res://Scenes/SceneMainMenu.tscn");
        }

        private void _on_Btn_Connect_pressed()
        {
            ENetClient.Connect();
        }
    }
}

