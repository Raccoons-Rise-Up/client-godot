using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;

namespace KRU.Networking
{
    public class HandlePacketPlayerData : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPlayerData()
        {
            Opcode = ServerPacketOpcode.PlayerData;
        }

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPlayerData();
            data.Read(packetReader);

            UIGame.SetResourceCounts(data.ResourceCounts.ToDictionary(x => x.Key, x => (double)x.Value));
            UIGame.SetStructureCounts(data.StructureCounts);
        }
    }
}