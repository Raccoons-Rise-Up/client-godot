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
        public Dictionary<uint, User> Users { get; set; }
        public uint ChannelId { get; set; }
        public uint CreatorId { get; set; }

        public void Read(PacketReader reader)
        {
            ResponseChannelCreateOpcode = (ResponseChannelCreateOpcode)reader.ReadByte();

            ChannelId = reader.ReadUInt32();

            if (ResponseChannelCreateOpcode == ResponseChannelCreateOpcode.Success) 
            {
                CreatorId = reader.ReadUInt32();

                Users = new Dictionary<uint, User>();
                var userCount = reader.ReadUInt16();
                for (int i = 0; i < userCount; i++)
                {
                    var userId = reader.ReadUInt32();
                    var userUsername = reader.ReadString();
                    Users.Add(userId, new User(userUsername));
                }
            }
        }
    }
}