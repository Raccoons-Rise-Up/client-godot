using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Common.Game;
using Godot;

namespace KRU.Networking
{
    public class HandlePacketPlayerJoinLeave : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPlayerJoinLeave() => Opcode = ServerPacketOpcode.PlayerJoinLeave;

        public override void Handle(PacketReader packetReader)
        {
            var data = new RPacketPlayerJoinLeave();
            data.Read(packetReader);

            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Join)
            {
                // A player has joined
                // Tell everyone in the global channel this player has joined
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {data.PlayerName} joined",
                    Special = true
                });

                // Track player in UIGame
                var user = new User { Username = data.PlayerName };
                user.CreateUIUser(data.PlayerId);
                UIGame.Players.Add(data.PlayerId, user);

                // Add player to user list in global channel
                if (UIChannels.ActiveChannel == (uint)SpecialChannel.Global)
                {
                    UIChat.UserList.AddChild(user.UIUser);
                }

                // Client is currently viewing a channel other than the global channel so add joining player to global
                if (UIChannels.ActiveChannel != (uint)SpecialChannel.Global) 
                {
                    UIChannels.Channels[(uint)SpecialChannel.Global].Users.Add(data.PlayerId);
                }

                // Add UIUser from active channel if joining player exists in active channels users
                var activeChannelUsers = UIChannels.Channels[UIChannels.ActiveChannel].Users;
                if (activeChannelUsers.Contains(data.PlayerId)) 
                {
                    UIChat.UserList.AddChild(UIGame.Players[data.PlayerId].UIUser);
                    activeChannelUsers.Add(data.PlayerId);
                }
            }
            
            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Leave)
            {
                // A player has left
                // Tell everyone in the global channel this player has left
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {UIGame.Players[data.PlayerId]} left",
                    Special = true
                });

                // Track player in UIGame
                UIGame.Players.Remove(data.PlayerId);

                // Remove UIUser from active channel if joining player exists in active channels users
                var activeChannelUsers = UIChannels.Channels[UIChannels.ActiveChannel].Users;
                if (activeChannelUsers.Contains(data.PlayerId)) 
                {
                    UIChat.UserList.RemoveChild(UIGame.Players[data.PlayerId].UIUser);

                    // The player has left the server so we have no more use for UIUser.Instance(), lets QueueFree() it
                    UIGame.Players[data.PlayerId].UIUser.QueueFree();

                    // Client is currently viewing a channel other than the global channel so remove leaving player from global
                    if (UIChannels.ActiveChannel != (uint)SpecialChannel.Global)
                        UIChannels.Channels[(uint)SpecialChannel.Global].Users.Remove(data.PlayerId);

                    activeChannelUsers.Remove(data.PlayerId);
                }
            }
        }
    }
}