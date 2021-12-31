using System.Collections.Generic;
using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using Common.Game;
using KRU.UI;

namespace KRU.Networking
{
    public class RPacketPlayerList : IReadable
    {
        public Dictionary<uint, User> Players = new Dictionary<uint, User>();

        public void Read(PacketReader reader)
        {
            var playerCount = reader.ReadByte();
            for (int i = 0; i < playerCount; i++) 
            {
                var userId = reader.ReadUInt32();
                var userUsername = reader.ReadString();
                Players.Add(userId, new User { Username = userUsername });
            }
        }
    }
}
