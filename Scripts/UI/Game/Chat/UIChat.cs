using Godot;
using System;
using System.Collections.Generic;
using Common.Game;

namespace Client.UI 
{
    public class UIChat : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
            [Export] private readonly NodePath nodePathChat;
            [Export] private readonly NodePath nodePathSettings;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static PackedScene PrefabChannel = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Channel.tscn");

        private static TabContainer Chat;
        private static Control Settings;
        private static Control ActiveChannel;
        private static Dictionary<int, Control> Channels = new Dictionary<int, Control>();
        private static int ChannelId = 0;

        public override void _Ready()
        {
            Chat = GetNode<TabContainer>(nodePathChat);
            Chat.Visible = false;

            Settings = GetNode<Control>(nodePathSettings);
            Settings.Visible = false;

            // Server needs to tell client about Global and Game

            // ServerLoginPacket

            // Users
            // ExampleUser
            // id: 0
            // name: Example User
            // status: Status
            // channels: int[]

            // Channels

            // GLOBAL CHANNEL
            // id: 0
            // name: Global
            // users: uint[]
            // no need to send 'content'

            // GAME CHANNEL
            // id: 1
            // name: Game
            // users: uint[]
            // no need to send 'content'

            //Chat.CurrentTab = (int)SpecialChannel.Global;
        }

        private void _on_Chat_Toggle_pressed() => Chat.Visible = !Chat.Visible;

        private void _on_Button_Settings_pressed() => Settings.Visible = true;

        public static void SetupChannels()
        {

        }

        public static void CreateChannel(string name)
        {
            var channel = (Control)PrefabChannel.Instance();
            channel.Name = name;
            Chat.AddChild(channel);
            Channels.Add(ChannelId++, channel);
        }

        public static void SwitchChannels(int id) => Chat.CurrentTab = id;
    }
}
