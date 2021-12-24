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

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPlayerJoinLeave();
            data.Read(packetReader);

            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Join)
            {
                UIChat.AddMessage("Server", $"{data.PlayerName} joined");
                UIGame.Players.Add(data.PlayerId, data.PlayerName);
            }
            
            if (data.JoinLeaveOpcode == JoinLeaveOpcode.Leave)
            {
                UIChat.AddMessage("Server", $"{UIGame.Players[data.PlayerId]} left");
                UIGame.Players.Remove(data.PlayerId);
            }
        }
    }
}