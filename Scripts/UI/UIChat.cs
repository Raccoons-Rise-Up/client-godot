using Godot;
using System;
using System.Collections.Generic;
using KRU.Networking;
using Common.Networking;
using Common.Networking.Packet;
using Common.Game;

namespace KRU.UI 
{
    public class UIChat : PanelContainer
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathChatText; // where all the text gets displayed
        [Export] private readonly NodePath nodePathChatInput; // the chat field input
#pragma warning restore CS0649 // Values are assigned in the editor

        public static RichTextLabel ChatText { get; set; }
        private static LineEdit ChatInput { get; set; }

        public override void _Ready()
        {
            ChatText = GetNode<RichTextLabel>(nodePathChatText);
            ChatText.ScrollFollowing = true;

            ChatInput = GetNode<LineEdit>(nodePathChatInput);
        }

        public static void AddMessageGlobal(string message) => AddMessage((uint)SpecialChannel.Global, message);
        public static void AddMessageGame(string message) => AddMessage((uint)SpecialChannel.Game, message);

        public static void AddMessage(uint channelId, string message)
        {
            if (UIChannels.ActiveChannel == channelId) 
            {
                ChatText.AddText($"{message}\n");
                ChatText.ScrollToLine(ChatText.GetLineCount() - 1);
            }

            UIChannels.Channels[channelId].Content += $"{message}\n";
        }

        public static void ClearChat() => ChatText.Clear(); // Clear chat

        private void _on_LineEdit_text_entered(string t) 
        {
            // Remember the server has the final say in everything (never trust the client)
            // Do not allow empty messages to be sent
            var text = t.Trim();
            if (text == "")
                return;

            ChatInput.Clear();

            if (text == "/") // Avoid empty commands
                return;

            if (text[0] == '/')
            {
                HandleCmds(text.Split()[0].ToLower().Substring(1));
                return;
            }

            // Not sure if this is allowed with threading and all but going to try anyways!
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
                ChannelId = UIChannels.ActiveChannel,
                Message = text
            }));
        }

        private void HandleCmds(string cmd)
        {
            if (cmd == "help")
            {
                AddMessageGlobal("/w <user> /r");
                return;
            }

            AddMessageGame($"Unknown command: {cmd}");
        }
    }
}