using Godot;
using System;
using System.Collections.Generic;
using Common.Netcode;
using Client.Netcode;

namespace Client.UI 
{
    public partial class UIChannel : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath NodePathChatContent;
        [Export] private NodePath NodePathChatInput;
        [Export] private NodePath NodePathUserList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private RichTextLabel ChatContent;
        private LineEdit ChatInput;
        private VBoxContainer UserList;

        public static bool ChatInputFocused;

        public override void _Ready()
        {
            ChatContent = GetNode<RichTextLabel>(NodePathChatContent);
            ChatInput = GetNode<LineEdit>(NodePathChatInput);
            UserList = GetNode<VBoxContainer>(NodePathUserList);
        }

        public override void _Input(InputEvent @event)
        {
            var leftclick = @event.IsActionPressed("ui_left_click");
            var rightclick = @event.IsActionPressed("ui_right_click");

            // apparently Godot doesn't do this automatically
            if (leftclick || rightclick)
                if (!ChatInput.GetRect().HasPoint(GetGlobalMousePosition()))
                    ChatInput.ReleaseFocus();
        }

        private void _on_Chat_Input_text_entered(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var textFormatted = text.Trim();

            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
                ChannelId = 2,
                Message = textFormatted
            }));

            //ChatContent.Text += $"{GameClient.Username}: {textFormatted}\n";
            ChatInput.Clear();
        }

        private void _on_Chat_Input_focus_entered() => ChatInputFocused = true;
        private void _on_Chat_Input_focus_exited() => ChatInputFocused = false;
    }
}
