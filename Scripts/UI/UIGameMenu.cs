using Godot;
using System;
using KRU.Networking;

namespace KRU.UI 
{
	public class UIGameMenu : Control
	{
		private void _on_Btn_Options_pressed()
		{
			GetTree().ChangeScene("res://Scenes/SceneOptions.tscn");
		}

		private void _on_Btn_Disconnect_pressed()
		{
			ENetClient.ENetCmds.Enqueue(ENetInstructionOpcode.Disconnect);
			GetTree().ChangeScene("res://Scenes/SceneMainMenu.tscn");
		}
	}
}
