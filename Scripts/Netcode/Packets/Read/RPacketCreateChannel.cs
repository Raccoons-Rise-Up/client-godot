using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using System.Collections.Generic;
using Common.Game;

namespace KRU.Networking
{
    public class RPacketCreateChannel : IReadable
    {
        public ResponseChannelCreateOpcode ResponseChannelCreateOpcode { get; set; }
        public List<uint> Users { get; set; }
        public uint ChannelId { get; set; }
        public uint CreatorId { get; set; }

        public void Read(PacketReader reader)
        {
            ResponseChannelCreateOpcode = (ResponseChannelCreateOpcode)reader.ReadByte();

            ChannelId = reader.ReadUInt32();

            if (ResponseChannelCreateOpcode == ResponseChannelCreateOpcode.Success) 
            {
                CreatorId = reader.ReadUInt32();

                Users = new List<uint>();
                var userCount = reader.ReadUInt16();
                for (int i = 0; i < userCount; i++)
                {
                    var userId = reader.ReadUInt32();
                    var userUsername = reader.ReadString();
                    var status = (Status)reader.ReadByte();

                    Users.Add(userId);
                }
            }
        }
    }
}