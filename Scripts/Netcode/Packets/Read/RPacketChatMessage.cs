using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class RPacketChatMessage : IReadable
    {
        public uint ChannelId { get; set; }
        public uint UserId { get; set; }
        public string Message { get; set; }

        public void Read(PacketReader reader) 
        {
            ChannelId = reader.ReadUInt32();
            UserId = reader.ReadUInt32();
            Message = reader.ReadString();
        }
    }
}