using Common.Networking.Message;
using Common.Networking.IO;

namespace Common.Networking.Packet 
{
    public class GamePacket 
    {
        public byte[] Data { get; set; }
        public long Size { get; set; }
    }
}