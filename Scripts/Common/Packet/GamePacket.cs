using Common.Networking.Message;
using Common.Networking.IO;
using ENet;

namespace Common.Networking.Packet 
{
    public class GamePacket 
    {
        public byte Opcode { get; set; }
        public PacketFlags PacketFlags = PacketFlags.Reliable; // Lets make packets reliable by default
        public byte[] Data { get; set; }
        public long Size { get; set; }
    }
}