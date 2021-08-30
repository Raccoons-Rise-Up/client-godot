using Godot;
using System;

namespace KRU.UI
{
	public class UIMainMenu : Node
	{
		private void _on_Btn_Multiplayer_pressed()
		{
			GetTree().ChangeScene("res://Scenes/SceneLogin.tscn");
		}

		private void _on_Btn_Options_pressed()
		{
			GetTree().ChangeScene("res://Scenes/SceneOptions.tscn");
		}

		private void _on_Btn_Credits_pressed()
		{
			GetTree().ChangeScene("res://Scenes/SceneCredits.tscn");
		}

		private void _on_Btn_Quit_pressed()
		{
			GetTree().Quit();
		}
	}
}

