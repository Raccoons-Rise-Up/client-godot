using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketCreateChannel : IReadable
    {
        public ResponseChannelCreateOpcode ResponseChannelCreateOpcode { get; set; }
        public Dictionary<uint, string> Users { get; set; }
        public uint ChannelId { get; set; }
        public uint CreatorId { get; set; }

        public void Read(PacketReader reader)
        {
            ResponseChannelCreateOpcode = (ResponseChannelCreateOpcode)reader.ReadByte();

            ChannelId = reader.ReadUInt32();

            if (ResponseChannelCreateOpcode == ResponseChannelCreateOpcode.Success) 
            {
                CreatorId = reader.ReadUInt32();

                Users = new Dictionary<uint, string>();
                for (int i = 0; i < reader.ReadUInt16(); i++)
                {
                    Users.Add(reader.ReadUInt32(), reader.ReadString());
                }
            }
        }
    }
}