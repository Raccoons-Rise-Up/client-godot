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

        public void Read(PacketReader reader)
        {
            LoginOpcode = (LoginResponseOpcode)reader.ReadByte();

            switch (LoginOpcode)
            {
                case LoginResponseOpcode.VersionMismatch:
                    VersionMajor = reader.ReadByte();
                    VersionMinor = reader.ReadByte();
                    VersionPatch = reader.ReadByte();
                    break;

                case LoginResponseOpcode.LoginSuccessNewPlayer:
                    PlayerId = reader.ReadUInt32();
                    PlayerName = reader.ReadString();
                    break;

                case LoginResponseOpcode.LoginSuccessReturningPlayer:
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
                    break;
            }
        }
    }
}