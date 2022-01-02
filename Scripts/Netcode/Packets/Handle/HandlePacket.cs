using Common.Netcode;
using ENet;

namespace Client.Netcode
{
    public abstract class HandlePacket
    {
        public abstract ServerPacketOpcode Opcode { get; set; }

        public abstract void Handle(PacketReader packetReader);
    }
}