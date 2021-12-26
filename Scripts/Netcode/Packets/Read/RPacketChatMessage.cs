using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class RPacketChatMessage : IReadable
    {
        public string ChannelId { get; set; }
        public uint PlayerId { get; set; }
        public string Message { get; set; }

        public void Read(PacketReader reader) 
        {
            ChannelId = reader.ReadString();
            PlayerId = reader.ReadUInt32();
            Message = reader.ReadString();
        }
    }
}