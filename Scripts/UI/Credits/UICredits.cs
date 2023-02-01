using Godot;
using System;

namespace Client.UI 
{
    public partial class UICredits : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();

            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }
    }
}
