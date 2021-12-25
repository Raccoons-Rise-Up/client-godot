using Godot;
using System;
using System.Collections.Generic;
using KRU.Networking;
using Common.Networking;
using Common.Networking.Packet;

namespace KRU.UI 
{
    public class UIChat : PanelContainer
    {
    #pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathChatText; // where all the text gets displayed
        [Export] private readonly NodePath nodePathChatInput; // the chat field input
        [Export] private readonly NodePath nodePathGlobalChannelButton;
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static RichTextLabel chatText;
        private static Button globalChannelButton;
        private static LineEdit chatInput;
        private static Dictionary<uint, string> channelText;
        private static uint activeChannel = (uint)Channel.Global;

        public override void _Ready()
        {
            channelText = new Dictionary<uint, string>();
            channelText.Add((uint)Channel.Global, "");
            channelText.Add((uint)Channel.Game, "");

            chatText = GetNode<RichTextLabel>(nodePathChatText);
            chatText.ScrollFollowing = true;

            chatInput = GetNode<LineEdit>(nodePathChatInput);

            globalChannelButton = GetNode<Button>(nodePathGlobalChannelButton);
        }

        public static void Clear() => chatText.Clear(); // Clear chat

        public static void AddMessageGame(string message) => AddMessage((uint)Channel.Game, message);
        public static void AddMessageGlobal(string message) => AddMessage((uint)Channel.Global, message);

        public static void AddMessage(uint channel, string message)
        {
            if (channel == activeChannel) 
            {
                chatText.AddText($"{message}\n");
                chatText.ScrollToLine(chatText.GetLineCount() - 1);
            }

            channelText[channel] += $"{message}\n";
        }

        private void _on_LineEdit_text_entered(string t) 
        {
            // Remember the server has the final say in everything (never trust the client)
            // Do not allow empty messages to be sent
            var text = t.Trim();
            if (text == "")
                return;

            chatInput.Clear();

            if (activeChannel == (uint)Channel.Game) 
            {
                HandleGameChannel(text);
                return;
            }

            if (text == "/") // Avoid empty commands
                return;

            if (text[0] == '/')
            {
                HandleGlobalChannelCmds(text.Split()[0].ToLower().Substring(1));
                return;
            }

            // Not sure if this is allowed with threading and all but going to try anyways!
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage{
                Message = text
            }));
        }

        private void HandleGlobalChannelCmds(string cmd)
        {
            if (cmd == "help")
            {
                AddMessageGlobal("/w <user> /r");
                return;
            }

            AddMessageGlobal($"Unknown command: {cmd}");
        }

        private void HandleGameChannel(string text)
        {
            if (text[0] != '/')
                return;

            var cmd = text.Split()[0].Trim().ToLower().Substring(1);

            if (cmd == "help")
            {
                AddMessageGame($"No cmds were defined for this channel yet");
                return;
            }

            AddMessageGame($"Unknown command: {cmd}");
        }

        // Global Channel
        private void _on_Global_pressed()
        {
            chatText.Text = channelText[(uint)Channel.Global];
            activeChannel = (uint)Channel.Global;
        }

        // Game Channel
        private void _on_Game_pressed()
        {
            chatText.Text = channelText[(uint)Channel.Game];
            activeChannel = (uint)Channel.Game;
        }
    }

    public enum Channel 
    {
        Global,
        Game
    }
}