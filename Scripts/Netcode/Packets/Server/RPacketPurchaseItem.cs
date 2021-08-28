using System.Collections;
using System.Collections.Generic;
using Common.Networking.Packet;
using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking 
{
    public class RPacketPurchaseItem : IReadable
    {
        public PurchaseItemResponseOpcode PurchaseItemResponseOpcode { get; set; }
        public ushort ItemId { get; set; }
        public uint Gold { get; set; }

        public void Read(PacketReader reader)
        {
            PurchaseItemResponseOpcode = (PurchaseItemResponseOpcode)reader.ReadByte();
            ItemId = reader.ReadUInt16();
            Gold = reader.ReadUInt32();
        }
    }
}
