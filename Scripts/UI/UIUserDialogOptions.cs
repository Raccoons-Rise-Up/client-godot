using Godot;
using System;
using Common.Game;

namespace KRU.UI 
{
    public class UIUserDialogOptions : Control
    {
        public string Username { get; set; }
        public uint UserId { get; set; } // The user ID attached to this dialog

        private void _on_Whisper_pressed()
        {
            //GD.Print($"Whisper {UserId}: {Username}");

            // You cannot whisper yourself
            if (UserId == UIGame.ClientPlayerId)
            {
                QueueFree();
                UIUser.ActiveDialog = null;
                return;
            }
                
            foreach (var channelPair in UIChannels.Channels)
            {
                // Do not check the special channels
                if (channelPair.Key == (uint)SpecialChannel.Global || channelPair.Key == (uint)SpecialChannel.Game)
                    continue;

                var users = channelPair.Value.Users;
                if (users.ContainsKey(UIGame.ClientPlayerId) && users.ContainsKey(UserId))
                {
                    //GD.Print($"Channel '{Username}' exists, going to this channel");
                    UIChannels.GoToChannel(channelPair.Key);
                    return;
                }
            }

            //GD.Print($"Channel '{Username}' does not exist, sending channel create request to server");
            UIChannels.SendCreateChannelRequest(UserId);

            // Remove the dialog popup
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
