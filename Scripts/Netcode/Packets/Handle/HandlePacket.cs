using Common.Networking.IO;
using ENet;

namespace KRU.Networking
{
    public abstract class HandlePacket
    {
        public abstract ServerPacketOpcode Opcode { get; set; }

        public abstract void Handle(Event netEvent, PacketReader packetReader);
    }
}