using Godot;
using System;

namespace KRU.UI 
{
    public class UIUserDialogOptions : Control
    {
        public string username;
        public uint id;

        private void _on_Whisper_pressed()
        {
            GD.Print($"Whisper {id}: {username}");

            if (UIChat.channels.ContainsKey(username)) 
            {
                GD.Print($"Channel '{username}' exists, going to this channel");
                UIChat.GoToChannel(username);
            }
            else 
            {
                GD.Print($"Channel '{username}' does not exist, sending channel create request to server");
                UIChat.SendCreateChannelRequest(id, username);
            }

            QueueFree();
            UIUser.activeDialog = null;
        }

        private void _on_AddFriend_pressed()
        {

        }

        private void _on_Block_pressed()
        {

        }
    }
}
