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
            if (UIChat.channels.ContainsKey(username))
                UIChat.GoToChannel(username);
            else
                UIChat.SendCreateChannelRequest(id, username);

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
