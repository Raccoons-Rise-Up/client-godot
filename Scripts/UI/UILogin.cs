using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
	public class UILogin : Control
	{
		private ENetClient ENetClient;

		public override void _Ready()
		{
			ENetClient = GetNode<ENetClient>("/root/ENetClient");
		}

		private void _on_Btn_Connect_pressed()
		{
			ENetClient.Connect();
		}
	}
}

