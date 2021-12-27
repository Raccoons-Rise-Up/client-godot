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
        private static LineEdit chatInput;

        public override void _Ready()
        {
            ChatText = GetNode<RichTextLabel>(nodePathChatText);
            ChatText.ScrollFollowing = true;

            chatInput = GetNode<LineEdit>(nodePathChatInput);
        }

        public static void AddMessageGame(string message) 
        {
            AddText("Game", message);
            UIChannels.channelGame.Content += $"{message}\n";
        }

        public static void AddMessageGlobal(string message) 
        {
            AddText("Global", message);
            UIChannels.channelGlobal.Content += $"{message}\n";
        }

        public static void AddMessage(string channel, string message)
        {
            AddText(channel, message);
            UIChannels.ChannelsPrivate.Find(x => x.Name == channel).Content += $"{message}\n";
        }

        private static void AddText(string channel, string text)
        {
            if (channel.Equals(UIChannels.ActiveChannel)) 
            {
                ChatText.AddText($"{text}\n");
                ChatText.ScrollToLine(ChatText.GetLineCount() - 1);
            }
        }

        public static void ClearChat() => ChatText.Clear(); // Clear chat

        private void _on_LineEdit_text_entered(string t) 
        {
            // Remember the server has the final say in everything (never trust the client)
            // Do not allow empty messages to be sent
            var text = t.Trim();
            if (text == "")
                return;

            chatInput.Clear();

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

            AddMessageGlobal($"Unknown command: {cmd}");
        }
    }
}