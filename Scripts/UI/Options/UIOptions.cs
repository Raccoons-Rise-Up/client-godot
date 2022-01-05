using Godot;
using System;

namespace Client.UI 
{
    public class UIOptions : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();
        }
    }
}
