using Godot;
using System;
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
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static RichTextLabel chatText;
        private static LineEdit chatInput;

        public override void _Ready()
        {
            chatText = GetNode<RichTextLabel>(nodePathChatText);
            chatText.ScrollFollowing = true;

            chatInput = GetNode<LineEdit>(nodePathChatInput);
        }

        public static void Clear() => chatText.Clear(); // Clear chat

        public static void AddMessage(string playerName, string message)
        {
            // Trust the server formatted the message
            chatInput.Clear();
            chatText.AddText($"{playerName}: {message}\n");
            chatText.ScrollToLine(chatText.GetLineCount() - 1);
        }

        private void _on_LineEdit_text_entered(string text) 
        {
            // Remember the server has the final say in everything (never trust the client)
            // Do not allow empty messages to be sent
            if (text == "")
                return;

            // Not sure if this is allowed with threading and all but going to try anyways!
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage{
                Message = text.Trim()
            }));
        }
    }
}