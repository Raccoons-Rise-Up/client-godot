using Godot;
using System;

namespace KRU.UI 
{
    public class UISocial : Node
    {
    #pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathWindowChat;
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer WindowChat { get; set; }

        public override void _Ready()
        {
            WindowChat = GetNode<VBoxContainer>(nodePathWindowChat);
        }

        private void _on_Btn_Chat_pressed()
        {
            if (WindowChat.Visible)
            {
                WindowChat.Visible = false;
                return;
            }

            WindowChat.Hide();
            WindowChat.Visible = true;
        }
    }
}