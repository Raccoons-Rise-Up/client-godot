using System.Collections.Generic;
using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using Common.Game;

namespace KRU.Networking
{
    public class RPacketPlayerJoined : IReadable
    {
        public uint PlayerId { get; set; }
        public string PlayerName { get; set; }

        public void Read(PacketReader reader)
        {
            PlayerId = reader.ReadUInt32();
            PlayerName = reader.ReadString();
        }
    }
}
