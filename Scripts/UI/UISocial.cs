using Godot;
using System;

namespace KRU.UI 
{
    public class UISocial : Node
    {
    #pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathWindowChat;
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer windowChat;

        public override void _Ready()
        {
            windowChat = GetNode<VBoxContainer>(nodePathWindowChat);
        }

        private void _on_Btn_Chat_pressed()
        {
            if (windowChat.Visible)
            {
                windowChat.Visible = false;
                return;
            }

            windowChat.Hide();
            windowChat.Visible = true;
        }
    }
}