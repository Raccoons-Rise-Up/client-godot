using Common.Netcode;
using ENet;
using Godot;

namespace Client.Netcode
{
    public abstract class HandlePacket
    {
        public abstract ServerPacketOpcode Opcode { get; set; }

        public abstract void Handle(PacketReader packetReader);
    }
}