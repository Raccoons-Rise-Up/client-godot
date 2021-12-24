using System.Collections.Generic;
using Common.Networking.IO;
using Common.Networking.Message;
using Common.Networking.Packet;
using Common.Game;

namespace KRU.Networking
{
    public class RPacketPlayerJoinLeave : IReadable
    {
        public JoinLeaveOpcode JoinLeaveOpcode { get; set; }
        public uint PlayerId { get; set; }
        public string PlayerName { get; set; }

        public void Read(PacketReader reader)
        {
            JoinLeaveOpcode = (JoinLeaveOpcode)reader.ReadByte();
            PlayerId = reader.ReadUInt32();

            if (JoinLeaveOpcode == JoinLeaveOpcode.Join)
                PlayerName = reader.ReadString();
        }
    }
}
