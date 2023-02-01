using Common.Netcode;

namespace Client.Netcode
{
    public partial class WPacketChatMessage : IWritable
    {
        public uint ChannelId { get; set; }
        public string Message { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(ChannelId);
            writer.Write(Message);
        }
    }
}