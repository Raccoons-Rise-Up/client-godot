using Godot;
using System;

public class UIGameMenu : Node
{
    private void _on_Btn_Menu_pressed()
    {
        GetTree().ChangeScene("res://Scenes/SceneMenu.tscn");
    }
}
