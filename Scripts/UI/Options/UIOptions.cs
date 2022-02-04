using Godot;
using System;
using Client.UI;
using Client.Utilities;
using Client.Game;

namespace Client.UI 
{
    public class UIOptions : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathCheckboxFullscreen;
        [Export] private readonly NodePath nodePathCheckboxFullscreenBorderless;
        [Export] private readonly NodePath nodePathHSliderMusicVolume;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static CheckBox CheckBoxFullscreen;
        private static CheckBox CheckBoxFullscreenBorderless;
        private static HSlider HSliderMusicVolume;

        public static Options Options { get; set; }

        public override void _Ready()
        {
            CheckBoxFullscreen = GetNode<CheckBox>(nodePathCheckboxFullscreen);
            CheckBoxFullscreenBorderless = GetNode<CheckBox>(nodePathCheckboxFullscreenBorderless);

            CheckBoxFullscreen.Pressed = Options.Fullscreen;
            CheckBoxFullscreenBorderless.Pressed = Options.FullscreenBorderless;

            HSliderMusicVolume = GetNode<HSlider>(nodePathHSliderMusicVolume);
            HSliderMusicVolume.Value = Options.VolumeMusic;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                UIMainMenu.LoadMainMenu();

            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }

        private void _on_Music_Volume_Control_value_changed(float value)
        {
            MusicManager.SetVolume(value);
            Options.VolumeMusic = HSliderMusicVolume.Value;
        }

        private void _on_Fullscreen_pressed()
        {
            if (!OS.WindowBorderless) 
            {
                OS.WindowFullscreen = !OS.WindowFullscreen;
            }

            Options.Fullscreen = CheckBoxFullscreen.Pressed;
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
                EnableFullscreenBorderless();
            }

            Options.FullscreenBorderless = CheckBoxFullscreenBorderless.Pressed;
        }

        public static void EnableFullscreenBorderless()
        {
            OS.WindowFullscreen = false;
            OS.WindowBorderless = true;
            OS.WindowPosition = new Vector2(0, 0);
            OS.WindowSize = OS.GetScreenSize() + new Vector2(1, 1); // need to add (1, 1) otherwise will act like fullscreen mode (seems like a Godot bug)
        }

        public static void Init()
        {
            MusicManager.SetVolume(Options.VolumeMusic);
            MusicManager.PlayCurrentTrack();

            if (Options.FullscreenBorderless) 
                UIOptions.EnableFullscreenBorderless();

            if (Options.Fullscreen && !Options.FullscreenBorderless)
                OS.WindowFullscreen = Options.Fullscreen;
        }

        public static void Save()
        {
            GD.Print("Saving options..");
            FileManager.WriteConfig(FileManager.PathOptions, Options);
        }
    }
}
