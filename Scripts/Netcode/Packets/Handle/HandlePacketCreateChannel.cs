using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Godot;

namespace KRU.Networking
{
    public class HandlePacketCreateChannel : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketCreateChannel() => Opcode = ServerPacketOpcode.CreateChannel;

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketCreateChannel();
            data.Read(packetReader);

            UIChat.CreateChannel(data.OtherUserId, data.ChannelName);
        }
    }
}