using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketCreateChannel : IReadable
    {
        public ResponseChannelCreateOpcode ResponseChannelCreateOpcode { get; set; }
        public string ChannelName { get; set; }
        public uint OtherUserId { get; set; }

        public void Read(PacketReader reader)
        {
            ResponseChannelCreateOpcode = (ResponseChannelCreateOpcode)reader.ReadByte();

            if (ResponseChannelCreateOpcode == ResponseChannelCreateOpcode.Success) 
            {
                ChannelName = reader.ReadString();
                OtherUserId = reader.ReadUInt32();
            }

            if (ResponseChannelCreateOpcode == ResponseChannelCreateOpcode.ChannelExistsAlready)
            {
                ChannelName = reader.ReadString();
            }
        }
    }
}