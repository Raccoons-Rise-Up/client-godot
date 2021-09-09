using System.Collections.Generic;
using Common.Networking.Packet;
using Common.Networking.IO;
using Common.Networking.Message;
using KRU.Game;

namespace KRU.Networking
{
    public class RPacketLogin : IReadable
    {
        public LoginResponseOpcode LoginOpcode { get; set; }
        public byte VersionMajor { get; set; }
        public byte VersionMinor { get; set; }
        public byte VersionPatch { get; set; }
        public uint Wood { get; set; }
        public uint Stone { get; set; }
        public uint Wheat { get; set; }
        public uint Gold { get; set; }
        public uint StructureHuts { get; set; }
        public uint StructureWheatFarms { get; set; }

        public Dictionary<uint, Structure> Structures { get; set; }

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
                    for (int i = 0; i < 6; i++)
                        reader.ReadUInt32();
                    break;
            }

            ReadStructureData(ref reader);
        }

        public void ReadStructureData(ref PacketReader reader)
        {
            Structures = new Dictionary<uint, Structure>();
            var structureCount = reader.ReadUInt32();
            for (int i = 0; i < structureCount; i++)
            {
                var structure = new Structure();

                structure.Id = reader.ReadUInt32();
                structure.Name = reader.ReadString();
                structure.Description = reader.ReadString();

                var structureCostCount = reader.ReadByte();
                for (int j = 0; j < structureCostCount; j++)
                {
                    var resourceName = reader.ReadUInt16();
                    var resourceValue = reader.ReadUInt32();

                    structure.Cost.Add((ResourceType)resourceName, resourceValue);
                }

                var structureProductionCount = reader.ReadByte();
                for (int j = 0; j < structureProductionCount; j++)
                {
                    var resourceName = reader.ReadUInt16();
                    var resourceValue = reader.ReadUInt32();
                    
                    structure.Production.Add((ResourceType)resourceName, resourceValue);
                }

                var structureTechRequiredCount = reader.ReadByte();
                for (int j = 0; j < structureTechRequiredCount; j++)
                {
                    var tech = reader.ReadUInt16();

                    structure.TechRequired.Add((TechType)tech);
                }

                Structures.Add(structure.Id, structure);
            }
        }
    }
}
