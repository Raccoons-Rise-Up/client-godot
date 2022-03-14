using Godot;
using System;
using Client.Netcode;
using Client.Utilities;
using Client.Game;

namespace Client.UI 
{
    public class UIMainMenu : Node
    {
        private void _on_Multiplayer_pressed() => GameManager.ChangeScene("Main/Login");
        private void _on_Options_pressed() => GameManager.ChangeScene("Main/Options");
        private void _on_Credits_pressed() => GameManager.ChangeScene("Main/Credits");
        private void _on_Quit_pressed() => GameManager.ExitCleanup();

        public static void LoadMainMenu() => GameManager.ChangeScene("Main/MainMenu");
    }
}
