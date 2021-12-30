using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
using KRU.UI;
using Common.Game;

namespace KRU.Networking
{
    public class HandlePacketChatMessage : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketChatMessage() => Opcode = ServerPacketOpcode.ChatMessage;

        public override void Handle(PacketReader packetReader)
        {
            var data = new RPacketChatMessage();
            data.Read(packetReader);

            UIChat.AddMessage(data.ChannelId, new UIMessage {
                Message = data.Message,
                UserId = data.UserId
            });
        }
    }
}