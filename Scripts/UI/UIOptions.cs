using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
    public class UIOptions : Node
    {
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape) 
                {
                    if (ENetClient.ConnectedToServer)
                        GetTree().ChangeScene("res://Scenes/SceneMainGame.tscn");
                    else
                        GetTree().ChangeScene("res://Scenes/SceneMainMenu.tscn");
                }
        }
    }
}

