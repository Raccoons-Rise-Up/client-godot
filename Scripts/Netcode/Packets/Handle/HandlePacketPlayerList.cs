using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;

namespace KRU.Networking
{
    public class HandlePacketPlayerList : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPlayerList() => Opcode = ServerPacketOpcode.PlayerList;

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPlayerList();
            data.Read(packetReader);

            UIGame.Players = data.Players;
        }
    }
}