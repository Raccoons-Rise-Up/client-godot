using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class WPacketChatMessage : IWritable
    {
        public uint ChannelId { get; set; }
        public string Message { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(ChannelId);
            writer.Write(Message);
        }
    }
}