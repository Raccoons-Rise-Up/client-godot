using Godot;
using System;
using System.Collections.Generic;
using Common.Game;

namespace Client.UI 
{
    public class UIChat : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath NodePathChat;
        [Export] private readonly NodePath NodePathSettings;
        [Export] private readonly NodePath NodePathSettingsButton;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static PackedScene PrefabChannel = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Channel.tscn");

        // Node References
        private static TabContainer Chat;
        private static Control Settings;
        private static Control SettingsButton;


        private static Dictionary<int, Control> Channels = new Dictionary<int, Control>();
        private static int ChannelId = 0;

        public override void _Ready()
        {
            Chat = GetNode<TabContainer>(NodePathChat);
            Chat.Visible = false;

            Settings = GetNode<Control>(NodePathSettings);
            Settings.Visible = false;

            SettingsButton = GetNode<Control>(NodePathSettingsButton);
            SettingsButton.Visible = false;

            CreateChannel("test");
        }

        public static void CreateChannel(string name)
        {
            var channel = (Control)PrefabChannel.Instance();
            channel.Name = name;
            Chat.AddChild(channel);

            // keep track of channel
            Channels.Add(ChannelId++, channel);
        }

        public static void SwitchChannels(int tab) => Chat.CurrentTab = tab;

        private void _on_Chat_Toggle_pressed() 
        {
            Chat.Visible = !Chat.Visible;
            SettingsButton.Visible = !SettingsButton.Visible;
            Settings.Visible = false;
        }

        private void _on_Button_Settings_pressed() => Settings.Visible = true;
    }
}
