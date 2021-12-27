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
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathChannelTabs;
        [Export] private readonly NodePath nodePathChannelBtnGlobal;
        [Export] private readonly NodePath nodePathChannelBtnGame;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static Button buttonChannelGlobal;
        private static Button buttonChannelGame;
        public static UIChannel channelGlobal;
        public static UIChannel channelGame;
        public static List<UIChannel> ChannelsPrivate { get; set; }
        public static List<UIChannel> ChannelsGroup { get; set; }
        public static string ActiveChannel { get; set; }
        public static Node instance;

        public override void _Ready()
        {
            instance = this;

            ChannelsPrivate = new List<UIChannel>();
            ChannelsGroup = new List<UIChannel>();

            buttonChannelGlobal = GetNode<Button>(nodePathChannelBtnGlobal);
            buttonChannelGlobal.Connect("pressed", this, nameof(_on_Channel_Global_pressed));
            channelGlobal = new UIChannel { Name = "Global", Button = buttonChannelGlobal };

            buttonChannelGame = GetNode<Button>(nodePathChannelBtnGame);
            buttonChannelGame.Connect("pressed", this, nameof(_on_Channel_Game_pressed));
            channelGame = new UIChannel { Name = "Game", Button = buttonChannelGame};

            ActiveChannel = "Global";
        }

        public static void SendCreateChannelRequest(uint id, string name)
        {
            ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.CreateChannel, new WPacketCreateChannel {
                ChannelName = name,
                OtherUserId = id
            }));
        }

        public static void SetupAllChannels()
        {
            foreach (var channel in ChannelsPrivate) 
            {
                var btn = new Button();
                btn.Text = channel.Name;
                btn.Connect("pressed", instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{channel.Name});
                instance.AddChild(btn);
            }
        }

        public static void RemoveAllChannels()
        {
            foreach (var channel in ChannelsPrivate)
                channel.Button.QueueFree();

            foreach (var channel in ChannelsGroup)
                channel.Button.QueueFree();
            
            ChannelsPrivate.Clear();
            ChannelsGroup.Clear();
        }

        public static void CreateChannel(uint creatorId, string channelName)
        {
            foreach (var c in ChannelsPrivate)
            {
                if (c.Name.Equals(channelName)) 
                {
                    GD.Print("WARNING: A new channel with the same key tried to be added but was ignored");
                        return;
                }
            }

            var btn = new Button();
            btn.Text = channelName;
            btn.Connect("pressed", instance, nameof(_on_Channel_Tab_Btn_pressed), new Godot.Collections.Array{channelName});
            instance.AddChild(btn);

            var channel = new UIChannel { 
                Name = channelName,
                Creator = creatorId
            };

            ChannelsPrivate.Add(channel);

            GoToChannel(channelName);
        }

        public static void GoToChannel(string channelName)
        {
            if (ChannelsPrivate.Find(x => x.Name == channelName) == null)
            {
                GD.Print($"WARNING: Channel with channel name '{channelName}' does not exist (ignoring)");
                foreach (var channel in ChannelsPrivate)
                    GD.Print(channel.Name);
                return;
            }

            GD.Print($"Switched to channel '{channelName}'");

            UIChat.ChatText.Text = ChannelsPrivate.Find(x => x.Name == channelName).Content;
            ActiveChannel = channelName;
        }

        private static void _on_Channel_Global_pressed() 
        {
            UIChat.ChatText.Text = channelGlobal.Content;
            ActiveChannel = "Global";
        }

        private static void _on_Channel_Game_pressed()
        {
            UIChat.ChatText.Text = channelGame.Content;
            ActiveChannel = "Game";
        }

        private void _on_Channel_Tab_Btn_pressed(string name)
        {
            UIChat.ChatText.Text = ChannelsPrivate.Find(x => x.Name == name).Content;
            ActiveChannel = name;
        }
    }
}