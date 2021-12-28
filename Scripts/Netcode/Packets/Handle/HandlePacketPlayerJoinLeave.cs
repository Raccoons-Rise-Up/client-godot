using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;

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
                UIChat.AddMessageGlobal($"Server: {data.PlayerName} joined");
                UIGame.Players.Add(data.PlayerId, data.PlayerName);
                UIUsers.AddUser(data.PlayerName, Status.Online, data.PlayerId);
            }
            
            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Leave)
            {
                UIChat.AddMessageGlobal($"Server: {UIGame.Players[data.PlayerId]} left");
                UIGame.Players.Remove(data.PlayerId);
                UIUsers.RemoveUser(data.PlayerId);
            }
        }
    }
}