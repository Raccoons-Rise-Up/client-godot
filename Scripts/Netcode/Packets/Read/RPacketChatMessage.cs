using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class RPacketChatMessage : IReadable
    {
        public uint PlayerId { get; set; }
        public string Message { get; set; }

        public void Read(PacketReader reader) 
        {
            PlayerId = reader.ReadUInt32();
            Message = reader.ReadString();
        }
    }
}