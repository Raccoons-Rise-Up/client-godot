using Common.Networking.IO;
using Common.Networking.Message;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketCreateChannel : IReadable
    {
        public string ChannelName { get; set; }
        public uint OtherUserId { get; set; }

        public void Read(PacketReader reader)
        {
            ChannelName = reader.ReadString();
            OtherUserId = reader.ReadUInt32();
        }
    }
}