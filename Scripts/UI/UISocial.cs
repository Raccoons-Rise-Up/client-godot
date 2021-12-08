using Godot;
using System;

public class UISocial : Node
{
    private readonly static PackedScene prefabPanelFriends = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/Friends.tscn");

#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private readonly NodePath nodePathBtnFriends;
#pragma warning restore CS0649 // Values are assigned in the editor

    private static Button btnFriends;

    public override void _Ready()
    {
        btnFriends = GetNode<Button>(nodePathBtnFriends);
    }

    private void _on_Btn_Friends_pressed()
    {
        GD.Print("friend");
    }

    // TODO: On click friends button spawn friends panel

    // TODO: On click general button spawn general chat panel

    // TODO: On message friend spawn new chat window
}
