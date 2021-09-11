using Common.Networking.IO;
using Common.Networking.Message;
using KRU.Game;
using System.Collections.Generic;

namespace KRU.Networking
{
    public class RPacketLogin : IReadable
    {
        public LoginResponseOpcode LoginOpcode { get; set; }
        public byte VersionMajor { get; set; }
        public byte VersionMinor { get; set; }
        public byte VersionPatch { get; set; }
        public Dictionary<ushort, uint> ResourceCounts { get; set; }
        public Dictionary<ushort, uint> StructureCounts { get; set; }
        public Dictionary<ushort, ResourceInfo> ResourceInfoData { get; set; }
        public Dictionary<ushort, StructureInfo> StructureInfoData { get; set; }

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

                case LoginResponseOpcode.LoginSuccessReturningPlayer:
                    // Resource counts
                    ResourceCounts = new Dictionary<ushort, uint>();
                    StructureCounts = new Dictionary<ushort, uint>();

                    var resourceCount = reader.ReadUInt16();
                    for (int i = 0; i < resourceCount; i++) 
                    {
                        var resourceKey = reader.ReadUInt16();
                        var resourceValue = reader.ReadUInt32();

                        ResourceCounts.Add(resourceKey, resourceValue);
                    }

                    // Structure counts
                    var structureCount = reader.ReadUInt16();
                    for (int i = 0; i < structureCount; i++) 
                    {
                        var structureKey = reader.ReadUInt16();
                        var structureValue = reader.ReadUInt32();

                        StructureCounts.Add(structureKey, structureValue);
                    }
                    break;
            }

            ReadResourceData(ref reader);
            ReadStructureData(ref reader);
        }

        private void ReadResourceData(ref PacketReader reader) 
        {
            ResourceInfoData = new Dictionary<ushort, ResourceInfo>();
            var resourceCount = reader.ReadUInt16();
            for (int i = 0; i < resourceCount; i++) 
            {
                var resourceId = reader.ReadUInt16();
                var resource = new ResourceInfo
                {
                    Name = reader.ReadString(),
                    Description = reader.ReadString()
                };

                ResourceInfoData.Add(resourceId, resource);
            }
        }

        private void ReadStructureData(ref PacketReader reader)
        {
            StructureInfoData = new Dictionary<ushort, StructureInfo>();
            var structureCount = reader.ReadUInt16();
            for (int i = 0; i < structureCount; i++)
            {
                var structureId = reader.ReadUInt16();
                var structure = new StructureInfo
                {
                    Name = reader.ReadString(),
                    Description = reader.ReadString()
                };

                var structureCostCount = reader.ReadByte();
                for (int j = 0; j < structureCostCount; j++)
                {
                    var resourceName = reader.ReadUInt16();
                    var resourceValue = reader.ReadUInt32();

                    structure.Cost.Add(resourceName, resourceValue);
                }

                var structureProductionCount = reader.ReadByte();
                for (int j = 0; j < structureProductionCount; j++)
                {
                    var resourceName = reader.ReadUInt16();
                    var resourceValue = reader.ReadUInt32();

                    structure.Production.Add(resourceName, resourceValue);
                }

                var structureTechRequiredCount = reader.ReadByte();
                for (int j = 0; j < structureTechRequiredCount; j++)
                {
                    var tech = reader.ReadUInt16();

                    structure.TechRequired.Add(tech);
                }

                StructureInfoData.Add(structureId, structure);
            }
        }
    }
}