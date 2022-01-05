using Godot;
using System;

namespace Client.UI 
{
    public class UIChat : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
            [Export] private readonly NodePath nodePathChat;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static Control Chat;

        public override void _Ready()
        {
            Chat = GetNode<Control>(nodePathChat);
            Chat.Visible = false;
        }

        private void _on_Chat_Toggle_pressed() => Chat.Visible = !Chat.Visible;
    }
}
