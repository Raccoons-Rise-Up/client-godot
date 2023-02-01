using Godot;
using System;
using Client.UI;
using Client.Utilities;
using Client.Game;

namespace Client.UI 
{
    public partial class UIOptions : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathCheckboxFullscreen;
        [Export] private NodePath nodePathCheckboxFullscreenBorderless;
        [Export] private NodePath nodePathHSliderMusicVolume;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static CheckBox CheckBoxFullscreen;
        private static CheckBox CheckBoxFullscreenBorderless;
        private static HSlider HSliderMusicVolume;

        public static Options Options { get; set; }

        public override void _Ready()
        {
            CheckBoxFullscreen = GetNode<CheckBox>(nodePathCheckboxFullscreen);
            CheckBoxFullscreenBorderless = GetNode<CheckBox>(nodePathCheckboxFullscreenBorderless);

            CheckBoxFullscreen.ButtonPressed = Options.Fullscreen;
            CheckBoxFullscreenBorderless.ButtonPressed = Options.FullscreenBorderless;

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
            if (!DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless))
            {
                if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                else
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

                //OS.WindowFullscreen = !OS.WindowFullscreen;
            }

            Options.Fullscreen = CheckBoxFullscreen.ButtonPressed;
        }

        private void _on_Fullscreen_Borderless_pressed()
        {
			if (DisplayServer.WindowGetFlag(DisplayServer.WindowFlags.Borderless))
			{
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);

                if (CheckBoxFullscreen.ButtonPressed)
					DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
            else 
            {
                EnableFullscreenBorderless();
            }

            Options.FullscreenBorderless = CheckBoxFullscreenBorderless.ButtonPressed;
        }

        public static void EnableFullscreenBorderless()
        {
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
            DisplayServer.WindowSetPosition(new Vector2I(0, 0));
            DisplayServer.WindowSetSize(DisplayServer.ScreenGetSize() + new Vector2I(1, 1)); // need to add (1, 1) otherwise will act like fullscreen mode (seems like a Godot bug)
        }

        public static void Init()
        {
            MusicManager.SetVolume(Options.VolumeMusic);
            MusicManager.PlayCurrentTrack();

            if (Options.FullscreenBorderless) 
                UIOptions.EnableFullscreenBorderless();

            // TODO: Convert to Godot 4
            //if (Options.Fullscreen && !Options.FullscreenBorderless)
                //OS.WindowFullscreen = Options.Fullscreen;
        }

        public static void Save()
        {
            GD.Print("Saving options..");
            FileManager.WriteConfig(FileManager.PathOptions, Options);
        }
    }
}
