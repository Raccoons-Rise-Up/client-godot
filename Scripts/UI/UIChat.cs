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
        [Export] private readonly NodePath nodePathUserListScrollContainer;
        [Export] private readonly NodePath nodePathChatArea;
        [Export] private readonly NodePath nodePathUserList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static RichTextLabel ChatText { get; set; }
        public static LineEdit ChatInput { get; set; }
        public static ScrollContainer UserListScrollContainer { get; set; }
        public static Control ChatArea { get; set; }
        public static Control UserList { get; set; }

        public override void _Ready()
        {
            ChatText = GetNode<RichTextLabel>(nodePathChatText);
            ChatText.ScrollFollowing = true;

            ChatInput = GetNode<LineEdit>(nodePathChatInput);
            UserListScrollContainer = GetNode<ScrollContainer>(nodePathUserListScrollContainer);
            ChatArea = GetNode<Control>(nodePathChatArea);
            UserList = GetNode<Control>(nodePathUserList);
        }

        public static void AddMessageGlobal(string message) => AddMessage((uint)SpecialChannel.Global, new UIMessage {
            Message = message
        });
        public static void AddMessageGame(string message) => AddMessage((uint)SpecialChannel.Game, new UIMessage {
            Message = message
        });

        public static void AddMessage(uint channelId, UIMessage message)
        {
            if (UIChannels.ActiveChannel == channelId) 
            {
                if (UIChannels.ActiveChannel == (uint)SpecialChannel.Game || message.Special)
                    ChatText.AddText($"{message.Message}\n");
                else
                    ChatText.AddText($"{UIGame.Players[message.UserId].Username}: {message.Message}\n");

                ChatText.ScrollToLine(ChatText.GetLineCount() - 1);
            }
            
            UIChannels.Channels[channelId].Messages.Add(message);
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
                HandleCmds(text.Split()[0].ToLower());
                return;
            }

            if (UIChannels.ActiveChannel == (uint)SpecialChannel.Game)
                return;

            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
                ChannelId = UIChannels.ActiveChannel,
                Message = text
            }));
        }

        private void HandleCmds(string text)
        {
            var cmd = text.Substring(1);
            if (cmd == "help")
            {
                AddMessage(UIChannels.ActiveChannel, new UIMessage{
                    Message = "Usage: /w <user> /r",
                    Special = true
                });
                return;
            }

            AddMessage(UIChannels.ActiveChannel, new UIMessage { 
                Message = $"Unknown command: {cmd}",
                Special = true
            });
        }
    }
}