using ENet;

namespace Common.Netcode
{
    public partial class GamePacket 
    {
        public const int MaxSize = 8192;
        public byte Opcode { get; set; }
        public PacketFlags PacketFlags = PacketFlags.Reliable; // Lets make packets reliable by default
        public byte[] Data { get; set; }
        public long Size { get; set; }
    }
}