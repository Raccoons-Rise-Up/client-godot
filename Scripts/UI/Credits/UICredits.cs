using Godot;
using System;

namespace Client.UI 
{
    public class UICredits : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();
        }
    }
}
