using Godot;
using System;

public class UISocial : Node
{
#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private readonly NodePath nodePathWindowFriends;
    [Export] private readonly NodePath nodePathWindowUsers;
    [Export] private readonly NodePath nodePathWindowChat;
#pragma warning restore CS0649 // Values are assigned in the editor

    private static PanelContainer windowFriends;
    private static PanelContainer windowUsers;
    private static PanelContainer windowChat;

    public override void _Ready()
    {
        windowFriends = GetNode<PanelContainer>(nodePathWindowFriends);
        windowUsers = GetNode<PanelContainer>(nodePathWindowUsers);
        windowChat = GetNode<PanelContainer>(nodePathWindowChat);
    }

    private void _on_Btn_Friends_pressed()
    {
        HideAllWindows();
        windowFriends.Visible = true;
    }

    private void _on_Btn_Users_pressed()
    {
        HideAllWindows();
        windowUsers.Visible = true;
    }

    private void _on_Btn_Chat_pressed()
    {
        HideAllWindows();
        windowChat.Visible = true;
    }

    private void HideAllWindows()
    {
        windowFriends.Hide();
        windowUsers.Hide();
        windowChat.Hide();
    }
}
