using Godot;
using System;
using System.Collections.Generic;
using KRU.Networking;
using Common.Networking;
using Common.Networking.Packet;
using Common.Game;

namespace KRU.UI 
{
    public class UIChannels : Node
    {
        public static Dictionary<uint, UIChannel> Channels { get; set; } // Note: uint is the ID of the channel
        public static uint ActiveChannel { get; set; }
        public static Node Instance { get; set; }

        public override void _Ready()
        {
            Channels = new Dictionary<uint, UIChannel>();
            ActiveChannel = (uint)SpecialChannel.Global;
            Instance = this;
        }

        public static void SendCreateChannelRequest(uint otherUserId)
        {
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.CreateChannel, new WPacketCreateChannel {
                OtherUserId = otherUserId
            }));
        }

        // This method is called on client login
        public static void SetupChannels(Dictionary<uint, UIChannel> channelsFromServer)
        {
            SetupChannel((uint)SpecialChannel.Global , new UIChannel { ChannelName = "Global"});
            SetupChannel((uint)SpecialChannel.Game, new UIChannel { ChannelName = "Game"});

            foreach (var pair in channelsFromServer)
            {
                var channelId = pair.Key;
                var channel = pair.Value;

                SetupChannel(channelId, channel);
            }
        }

        private static void SetupChannel(uint channelId, UIChannel channel) 
        {
            // Create the channel tab button
            var btn = new Button();

            if (channel.Users.ContainsKey(channel.CreatorId))
                btn.Text = channel.Users[channel.CreatorId]; // e.g. User defined channels
            else
                btn.Text = channel.ChannelName; // e.g. Global / Game
            
            btn.Connect("pressed", Instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{ channelId });

            // Add the button to the channel tab list in the scene
            Instance.AddChild(btn);

            // Keep track of the channel
            Channels.Add(channelId, new UIChannel { 
                ChannelName = channel.ChannelName,
                CreatorId = channel.CreatorId,
                Button = btn 
            });
        }

        public static void RemoveAllChannels()
        {
            foreach (var channel in Channels.Values)
                channel.Button.QueueFree();

            Channels.Clear();
        }

        public static void CreateChannel(RPacketCreateChannel data)
        {
            if (Channels.ContainsKey(data.ChannelId)) 
            {
                GD.PrintErr("WARNING: A new channel with the same key tried to be added but was ignored");
                return;
            }

            if (data.CreatorId == UIGame.ClientPlayerId) 
            {
                // If this client is the creator of the channel

                // Find the other user the creator opened this channel with
                uint otherUserId = 0;
                foreach (var user in data.Users)
                    if (user.Key != data.CreatorId)
                        otherUserId = user.Key;

                SetupChannel(data.ChannelId, new UIChannel {
                    CreatorId = data.CreatorId,
                    ChannelName = data.Users[otherUserId],
                    Users = data.Users
                });
                GoToChannel(data.ChannelId);
            }
            else
            {
                // If this client is the other user the channel is being opened to
                SetupChannel(data.ChannelId, new UIChannel {
                    CreatorId = data.CreatorId,
                    ChannelName = data.Users[data.CreatorId],
                    Users = data.Users
                });
            }
        }

        public static void GoToChannel(uint channelId)
        {
            if (!Channels.ContainsKey(channelId))
            {
                GD.PrintErr($"WARNING: Channel with channel ID '{channelId}' does not exist (ignoring)");
                GD.Print("A list of all the channels this client can see are listed below");
                foreach (var value in Channels.Values)
                    GD.Print(value.ChannelName);
                return;
            }

            var channel = Channels[channelId];

            GD.Print($"Switched to channel '{channel.ChannelName}'");

            UIChat.ChatText.Text = channel.Content;
            ActiveChannel = channelId;
        }

        private static void _on_Channel_Global_pressed() 
        {
            UIChat.ChatText.Text = Channels[(uint)SpecialChannel.Global].Content;
            ActiveChannel = (uint)SpecialChannel.Global;
        }

        private static void _on_Channel_Game_pressed()
        {
            UIChat.ChatText.Text = Channels[(uint)SpecialChannel.Game].Content;
            ActiveChannel = (uint)SpecialChannel.Game;
        }

        private void _on_Channel_Tab_Btn_pressed(uint channelId)
        {
            UIChat.ChatText.Text = Channels[channelId].Content;
            ActiveChannel = channelId;
        }
    }
}