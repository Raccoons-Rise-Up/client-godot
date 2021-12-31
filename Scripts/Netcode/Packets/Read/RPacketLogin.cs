using Common.Networking.IO;
using Common.Networking.Packet;
using Common.Networking.Message;
using Godot;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Common.Game;

namespace KRU.Networking
{
    public class RPacketLogin : IReadable
    {
        public LoginResponseOpcode LoginOpcode { get; set; }
        public byte VersionMajor { get; set; }
        public byte VersionMinor { get; set; }
        public byte VersionPatch { get; set; }
        public Dictionary<ResourceType, uint> ResourceCounts { get; set; }
        public Dictionary<StructureType, uint> StructureCounts { get; set; }
        public uint PlayerId { get; set; }
        public string PlayerName { get; set; }
        public Dictionary<uint, User> Players { get; set; }

        public void Read(PacketReader reader)
        {
            LoginOpcode = (LoginResponseOpcode)reader.ReadByte();

            if (LoginOpcode == LoginResponseOpcode.VersionMismatch) 
            {
                VersionMajor = reader.ReadByte();
                VersionMinor = reader.ReadByte();
                VersionPatch = reader.ReadByte();
                return;
            }

            var playerCount = reader.ReadUInt32();
            Players = new Dictionary<uint, User>();
            for (int i = 0; i < playerCount; i++)
            {
                var playerId = reader.ReadUInt32();
                var playerName = reader.ReadString();

                var user = new User{ Username = playerName };
                user.CreateUIUser(playerId);
                Players.Add(playerId, user);
            }

            if (LoginOpcode == LoginResponseOpcode.LoginSuccessNewPlayer) 
            {
                PlayerId = reader.ReadUInt32();
                PlayerName = reader.ReadString();
                return;
            }

            if (LoginOpcode == LoginResponseOpcode.LoginSuccessReturningPlayer) 
            {
                PlayerId = reader.ReadUInt32();
                PlayerName = reader.ReadString();

                // Resource counts
                ResourceCounts = new Dictionary<ResourceType, uint>();
                StructureCounts = new Dictionary<StructureType, uint>();

                var resourceCount = reader.ReadUInt16();
                for (int i = 0; i < resourceCount; i++)
                {
                    var resourceKey = (ResourceType)reader.ReadUInt16();
                    var resourceValue = reader.ReadUInt32();

                    ResourceCounts.Add(resourceKey, resourceValue);
                }

                // Structure counts
                var structureCount = reader.ReadUInt16();
                for (int i = 0; i < structureCount; i++)
                {
                    var structureKey = (StructureType)reader.ReadUInt16();
                    var structureValue = reader.ReadUInt32();

                    StructureCounts.Add(structureKey, structureValue);
                }
                return;
            }
        }
    }
}