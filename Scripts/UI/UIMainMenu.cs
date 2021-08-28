using Godot;
using System;
using KRU.Networking;

public class UIMainMenu : Node
{
    private ENetClient ENetClient;

    public override void _Ready()
    {
        ENetClient = GetNode<ENetClient>("/root/ENetClient");
    }

    private void _on_BtnConnect_pressed()
    {
        ENetClient.Connect();
        GetTree().ChangeScene("res://Scenes/SceneMain.tscn");
    }
}
