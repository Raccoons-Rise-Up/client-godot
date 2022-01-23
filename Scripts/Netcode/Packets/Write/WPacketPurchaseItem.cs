using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class WPacketPurchaseItem : IWritable
    {
        public ushort StructureID { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(StructureID);
        }
    }
}