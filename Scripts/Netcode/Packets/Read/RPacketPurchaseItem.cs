using Common.Networking.IO;
using Common.Networking.Message;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketPurchaseItem : IReadable
    {
        public PurchaseItemResponseOpcode PurchaseItemResponseOpcode { get; set; }
        public ushort ItemId { get; set; }
        public Dictionary<ushort, uint> Resources { get; set; }
        public byte ResourcesLength { get; set; }

        public void Read(PacketReader reader)
        {
            PurchaseItemResponseOpcode = (PurchaseItemResponseOpcode)reader.ReadByte();
            ItemId = reader.ReadUInt16();

            Resources = new Dictionary<ushort, uint>();

            ResourcesLength = reader.ReadByte();

            for (int i = 0; i < ResourcesLength; i++)
            {
                var resourceType = reader.ReadByte();
                var resourceValue = reader.ReadUInt32();

                Resources.Add(resourceType, resourceValue);
            }
        }
    }
}