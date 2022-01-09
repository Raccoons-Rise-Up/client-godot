using Godot;
using System;
using Client.Options;

namespace Client.UI 
{
    public class UIOptions : Node
    {
        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();
        }

        private void _on_Music_Volume_Control_value_changed(float value)
        {
            MusicManager.SetVolume(value);
        }
    }
}
