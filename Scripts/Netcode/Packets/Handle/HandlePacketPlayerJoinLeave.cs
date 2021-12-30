using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Common.Game;

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
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {data.PlayerName} joined",
                    Special = true
                });
                UIGame.Players.Add(data.PlayerId, data.PlayerName);

                UIChannels.Channels[(uint)SpecialChannel.Global].Users.Add(data.PlayerId, data.PlayerName);

                var uiUser = UIChannels.CreateUIUser(data.PlayerId, data.PlayerName);

                if (UIChannels.Channels[(uint)SpecialChannel.Global].UIUsers.ContainsKey(data.PlayerId)) 
                    Godot.GD.Print($"UIUsers for global channel already contains key {data.PlayerId}");
                else
                    UIChannels.Channels[(uint)SpecialChannel.Global].UIUsers.Add(data.PlayerId, uiUser);
                UIChat.UserList.AddChild(uiUser);

                //UIUsers.AddUser(data.PlayerName, Status.Online, data.PlayerId);
            }
            
            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Leave)
            {
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {UIGame.Players[data.PlayerId]} left",
                    Special = true
                });
                UIGame.Players.Remove(data.PlayerId);

                if (UIChannels.Channels[(uint)SpecialChannel.Global].UIUsers.ContainsKey(data.PlayerId)) 
                    UIChannels.Channels[(uint)SpecialChannel.Global].Users.Remove(data.PlayerId);
                else
                    Godot.GD.Print($"UIUsers for global channel does not contain key {data.PlayerId}");
                var uiUser = UIChannels.Channels[(uint)SpecialChannel.Global].UIUsers[data.PlayerId];
                UIChat.UserList.RemoveChild(uiUser);

                //UIUsers.RemoveUser(data.PlayerId);
            }
        }
    }
}