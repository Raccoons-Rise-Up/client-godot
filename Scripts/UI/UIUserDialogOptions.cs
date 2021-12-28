using Godot;
using System;

namespace KRU.UI 
{
    public class UIUserDialogOptions : Control
    {
        public string Username { get; set; }
        public uint Id { get; set; }

        private void _on_Whisper_pressed()
        {
            GD.Print($"Whisper {Id}: {Username}");

            if (UIChannels.Channels.ContainsKey(Id)) 
            {
                GD.Print($"Channel '{Username}' exists, going to this channel");
                UIChannels.GoToChannel(Id);
            }
            else 
            {
                GD.Print($"Channel '{Username}' does not exist, sending channel create request to server");
                UIChannels.SendCreateChannelRequest(Id);
            }

            QueueFree();
            UIUser.ActiveDialog = null;
        }

        private void _on_AddFriend_pressed()
        {

        }

        private void _on_Block_pressed()
        {

        }
    }
}
