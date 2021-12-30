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
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {data.PlayerName} joined",
                    Special = true
                });
                UIGame.Players.Add(data.PlayerId, data.PlayerName);

                var user = new User(data.PlayerName);
                user.CreateUIUser(data.PlayerId);
                UIChannels.Channels[(uint)SpecialChannel.Global].Users.Add(data.PlayerId, user);
                UIChat.UserList.AddChild(user.UIUser);
            }
            
            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Leave)
            {
                UIChat.AddMessage((uint)SpecialChannel.Global, new UIMessage {
                    Message = $"Server: {UIGame.Players[data.PlayerId]} left",
                    Special = true
                });
                UIGame.Players.Remove(data.PlayerId);

                var users = UIChannels.Channels[(uint)SpecialChannel.Global].Users;
                var user = users[data.PlayerId];

                if (user.UIUser.IsInsideTree()) 
                {
                    GD.Print("UIUser is inside tree, removing child");
                    UIChat.UserList.RemoveChild(user.UIUser);
                } else
                    GD.Print("UIUser is not inside tree");
                    

                user.UIUser.QueueFree();
                users.Remove(data.PlayerId);
            }
        }
    }
}