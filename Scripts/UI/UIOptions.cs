using Godot;
using System;

namespace KRU.UI
{
    public class UIOptions : Node
    {
        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                    GetTree().ChangeScene("res://Scenes/SceneMainMenu.tscn");
        }
    }
}

