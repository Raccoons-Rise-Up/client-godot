using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;

namespace KRU.Networking
{
    public class HandlePacketChatMessage : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketChatMessage() => Opcode = ServerPacketOpcode.ChatMessage;

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketChatMessage();
            data.Read(packetReader);

            UIChat.AddMessage((uint)Channel.Global, $"{UIGame.Players[data.PlayerId]}: {data.Message}");
        }
    }
}