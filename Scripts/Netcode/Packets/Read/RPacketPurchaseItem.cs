using Common.Networking.IO;
using Common.Networking.Message;
using Common.Game;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketPurchaseItem : IReadable
    {
        public PurchaseItemResponseOpcode PurchaseItemResponseOpcode { get; set; }
        public Dictionary<ResourceType, uint> Resources { get; set; }
        public byte ResourcesLength { get; set; }

        public void Read(PacketReader reader)
        {
            PurchaseItemResponseOpcode = (PurchaseItemResponseOpcode)reader.ReadByte();
            Resources = new Dictionary<ResourceType, uint>();

            ResourcesLength = reader.ReadByte();

            for (int i = 0; i < ResourcesLength; i++)
            {
                var resourceType = (ResourceType)reader.ReadByte();
                var resourceValue = reader.ReadUInt32();

                Resources.Add(resourceType, resourceValue);
            }
        }
    }
}