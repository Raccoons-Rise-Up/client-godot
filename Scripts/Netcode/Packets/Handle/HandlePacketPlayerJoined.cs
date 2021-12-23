using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;

namespace KRU.Networking
{
    public class HandlePacketPlayerJoined : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPlayerJoined() => Opcode = ServerPacketOpcode.PlayerJoined;

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPlayerJoined();
            data.Read(packetReader);

            UIGame.Players.Add(data.PlayerId, data.PlayerName);
        }
    }
}