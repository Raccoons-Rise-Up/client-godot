using Godot;
using System;

namespace KRU.UI 
{
    public class UIGameMenu : Node
    {
        private void _on_Btn_Menu_pressed()
        {
            GetTree().ChangeScene("res://Scenes/SceneMainMenu.tscn");
        }
    }
}
