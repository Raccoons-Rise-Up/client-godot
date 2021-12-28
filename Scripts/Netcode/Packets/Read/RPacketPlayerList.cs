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
        public Dictionary<uint, string> Players = new Dictionary<uint, string>();

        public void Read(PacketReader reader)
        {
            var playerCount = reader.ReadByte();
            for (int i = 0; i < playerCount; i++) 
            {
                var userId = reader.ReadUInt32();
                var userUsername = reader.ReadString();
                Players.Add(userId, userUsername);
            }
        }
    }
}
