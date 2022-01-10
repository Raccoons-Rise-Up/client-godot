using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class WPacketCreateChannel : IWritable
    {
        public uint OtherUserId { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(OtherUserId);
        }
    }
}