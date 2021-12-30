using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Godot;
using Common.Game;

namespace KRU.Networking
{
    public class HandlePacketChannelList : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketChannelList() => Opcode = ServerPacketOpcode.ChannelList;

        public override void Handle(PacketReader packetReader)
        {
            var data = new RPacketChannelList();
            data.Read(packetReader);

            UIChannels.SetupChannels(data.Channels);
            UIChannels.GoToChannel((uint)SpecialChannel.Global);
        }
    }
}