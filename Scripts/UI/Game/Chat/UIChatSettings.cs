using Godot;
using System;
using System.Collections.Generic;
using Common.Game;

namespace Client.UI 
{
    public class UIChatSettings : Control
    {
        private void _on_Close_Settings_pressed() 
        {
            Visible = false;
        }

        private void _on_HSlider_value_changed(float value)
        {
            GD.Print(value);
        }
    }
}