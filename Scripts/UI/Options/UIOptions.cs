using Godot;
using System;
using Client.Options;

namespace Client.UI 
{
    public class UIOptions : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathCheckboxFullscreen;
        [Export] private readonly NodePath nodePathCheckboxFullscreenBorderless;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static CheckBox CheckBoxFullscreen;
        private static CheckBox CheckBoxFullscreenBorderless;

        public override void _Ready()
        {
            CheckBoxFullscreen = GetNode<CheckBox>(nodePathCheckboxFullscreen);
            CheckBoxFullscreenBorderless = GetNode<CheckBox>(nodePathCheckboxFullscreenBorderless);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();
        }

        private void _on_Music_Volume_Control_value_changed(float value)
        {
            MusicManager.SetVolume(value);
        }

        private void _on_Fullscreen_pressed()
        {
            if (!OS.WindowBorderless)
                OS.WindowFullscreen = !OS.WindowFullscreen;
        }

        private void _on_Fullscreen_Borderless_pressed()
        {
            if (OS.WindowBorderless) 
            {
                OS.WindowBorderless = false;
                if (CheckBoxFullscreen.Pressed)
                    OS.WindowFullscreen = true;
            }
            else 
            {
                OS.WindowFullscreen = false;
                OS.WindowBorderless = true;
                OS.WindowPosition = new Vector2(0, 0);
                OS.WindowSize = OS.GetScreenSize() + new Vector2(1, 1); // need to add (1, 1) otherwise will act like fullscreen mode (seems like a Godot bug)
            }
        }
    }
}
