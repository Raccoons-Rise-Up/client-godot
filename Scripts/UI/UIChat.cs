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
        [Export] private readonly NodePath nodePathChannelTabs;
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static RichTextLabel chatText;
        private static LineEdit chatInput;
        public static Dictionary<string, UIChannel> channels;
        public static Dictionary<string, Button> channelBtns = new Dictionary<string, Button>();
        private static string activeChannel = "Global";
        private static HBoxContainer channelTabs;

        public static PanelContainer instance;

        public override void _Ready()
        {
            instance = this;
            channels = new Dictionary<string, UIChannel>();
            channels.Add("Global", new UIChannel { Name = "Global"});
            channels.Add("Game", new UIChannel { Name = "Game"});

            chatText = GetNode<RichTextLabel>(nodePathChatText);
            chatText.ScrollFollowing = true;

            chatInput = GetNode<LineEdit>(nodePathChatInput);

            channelTabs = GetNode<HBoxContainer>(nodePathChannelTabs);
        }
        
        public static void SendCreateChannelRequest(uint id, string name)
        {
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.CreateChannel, new WPacketCreateChannel {
                ChannelName = name,
                OtherUserId = id
            }));
        }

        public static void SetupChannels()
        {
            foreach (var channel in channels) 
            {
                var btn = new Button();
                btn.Text = channel.Value.Name;
                btn.Connect("pressed", instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{channel.Value.Name});
                channelTabs.AddChild(btn);
                channelBtns.Add(channel.Value.Name, btn);
            }
        }

        public static void CreateChannel(uint creatorId, string channelName)
        {
            if (channels.ContainsKey(channelName)) 
            {
                GD.Print("WARNING: A new channel with the same key tried to be added but was ignored");
                return;
            }

            var btn = new Button();
            btn.Text = channelName;
            btn.Connect("pressed", instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{channelName});
            channelTabs.AddChild(btn);
            channelBtns.Add(channelName, btn);

            var channel = new UIChannel { 
                Name = channelName,
                Creator = creatorId
            };
            channels.Add(channelName, channel);

            GoToChannel(channelName);
        }

        private void _on_Channel_Tab_Btn_pressed(string name)
        {
            chatText.Text = channels[name].Content;
            activeChannel = name;
        }

        public static void ClearChat() => chatText.Clear(); // Clear chat

        public static void AddMessageGame(string message) => AddMessage("Game", message);
        public static void AddMessageGlobal(string message) => AddMessage("Global", message);

        public static void AddMessage(string channel, string message)
        {
            if (channel.Equals(activeChannel)) 
            {
                chatText.AddText($"{message}\n");
                chatText.ScrollToLine(chatText.GetLineCount() - 1);
            }

            channels[channel].Content += $"{message}\n";
        }

        private void _on_LineEdit_text_entered(string t) 
        {
            // Remember the server has the final say in everything (never trust the client)
            // Do not allow empty messages to be sent
            var text = t.Trim();
            if (text == "")
                return;

            chatInput.Clear();

            if (activeChannel.Equals("Game")) 
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
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
                ChannelId = activeChannel,
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

        public static void GoToChannel(string channelName)
        {
            if (!channels.ContainsKey(channelName))
            {
                GD.Print($"WARNING: Channel with channel name '{channelName}' does not exist (ignoring)");
                foreach (var channel in channels.Keys)
                    GD.Print(channel);
                return;
            }

            GD.Print($"Switched to channel '{channelName}'");

            chatText.Text = channels[channelName].Content;
            activeChannel = channelName;
        }
    }
}