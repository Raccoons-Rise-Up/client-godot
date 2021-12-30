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
        public static Dictionary<uint, Channel> Channels { get; set; } // Note: uint is the ID of the channel
        public static uint ActiveChannel { get; set; }
        public static Node Instance { get; set; }

        public override void _Ready()
        {
            Channels = new Dictionary<uint, Channel>();
            ActiveChannel = (uint)SpecialChannel.Global;
            Instance = this;
        }

        public static void SendCreateChannelRequest(uint otherUserId)
        {
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.CreateChannel, new WPacketCreateChannel {
                OtherUserId = otherUserId
            }));
        }

        public static void SetupChannels(Dictionary<uint, Channel> channelsFromServer)
        {
            foreach (var pair in channelsFromServer)
            {
                var channelId = pair.Key;
                var channel = pair.Value;

                SetupChannel(channelId, channel);
            }
        }

        private static void SetupChannel(uint channelId, Channel channel) 
        {
            // Create the channel tab button
            var btn = new Button();

            if (string.IsNullOrEmpty(channel.ChannelName)) 
            {
                if (channelId == (uint)SpecialChannel.Global)
                    channel.ChannelName = "Global";
                else if (channelId == (uint)SpecialChannel.Game)
                    channel.ChannelName = "Game";
                else
                    channel.ChannelName = channel.Users[channel.CreatorId].Username;
            }
                
            btn.Text = channel.ChannelName;
            btn.Connect("pressed", Instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{ channelId });

            // Add the button to the channel tab list in the scene
            Instance.AddChild(btn);

            channel.TabButton = btn;

            // Keep track of the channel
            Channels.Add(channelId, channel);
        }

        public static void RemoveAllChannels()
        {
            foreach (var channel in Channels.Values)
                channel.TabButton.QueueFree();

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

                SetupChannel(data.ChannelId, new Channel {
                    CreatorId = data.CreatorId,
                    ChannelName = data.Users[otherUserId].Username,
                    Users = data.Users
                });
                GoToChannel(data.ChannelId);
            }
            else
            {
                // If this client is the other user the channel is being opened to
                SetupChannel(data.ChannelId, new Channel {
                    CreatorId = data.CreatorId,
                    ChannelName = data.Users[data.CreatorId].Username,
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

            ActiveChannel = channelId;

            if (ActiveChannel == (uint)SpecialChannel.Game) 
                UIChat.UserListScrollContainer.Visible = false;
            else
                UIChat.UserListScrollContainer.Visible = true;

            UIChat.ChatText.Text = ConvertMessagesToString(channel);
            UIChat.ChatInput.GrabFocus();

            // Empty the UI User list
            UIChat.ClearUIUsers();
            // Populate the UI User list
            foreach (var user in channel.Users.Values)
                UIChat.UserList.AddChild(user.UIUser);
        }

        private static string ConvertMessagesToString(Channel channel) 
        {
            string content = "";

            foreach (var message in channel.Messages)
                if (ActiveChannel == (uint)SpecialChannel.Game || message.Special)
                    content += $"{message.Message}\n";
                else
                    content += $"{channel.Users[message.UserId]}: {message.Message}\n";
                    
            return content;
        }

        private void _on_Channel_Tab_Btn_pressed(uint channelId)
        {
            GoToChannel(channelId);
        }
    }
}