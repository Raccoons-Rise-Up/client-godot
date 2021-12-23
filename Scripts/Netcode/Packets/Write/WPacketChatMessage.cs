using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class WPacketChatMessage : IWritable
    {
        public string Message { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(Message);
        }
    }
}