using Godot;
using System;
using Client.Netcode;
using Client.Utils;

namespace Client.UI 
{
    public class UIMainMenu : Node
    {
        private static SceneTree Tree { get; set; }

        public override void _Ready() => Tree = GetTree();

        private void _on_Multiplayer_pressed() => Tree.ChangeScene("res://Scenes/Main/Login.tscn");
        private void _on_Options_pressed() => Tree.ChangeScene("res://Scenes/Main/Options.tscn");
        private void _on_Credits_pressed() => Tree.ChangeScene("res://Scenes/Main/Credits.tscn");
        private void _on_Quit_pressed() => Tree.Quit();

        public static void LoadMainMenu() => Tree.ChangeScene("res://Scenes/Main/MainMenu.tscn");
    }
}
