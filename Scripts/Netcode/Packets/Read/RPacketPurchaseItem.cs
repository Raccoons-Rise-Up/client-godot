using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using Common.Networking.Packet;
using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class RPacketPurchaseItem : IReadable
    {
        public PurchaseItemResponseOpcode PurchaseItemResponseOpcode { get; set; }
        public ushort ItemId { get; set; }
        public Dictionary<ResourceType, uint> Resources { get; set; }
        public byte ResourcesLength { get; set; }

        public void Read(PacketReader reader)
        {
            PurchaseItemResponseOpcode = (PurchaseItemResponseOpcode)reader.ReadByte();
            ItemId = reader.ReadUInt16();

            Resources = new Dictionary<ResourceType, uint>();

            ResourcesLength = reader.ReadByte();

            for (int i = 0; i < ResourcesLength; i++)
            {
                var resourceType = reader.ReadByte();
                var resourceValue = reader.ReadUInt32();

                Resources.Add((ResourceType)resourceType, resourceValue);
            }
        }
    }
}
