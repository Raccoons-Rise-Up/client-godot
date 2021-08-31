using Common.Networking.Packet;
using Common.Networking.IO;
using Common.Networking.Message;

namespace KRU.Networking
{
    public class RPacketLogin : IReadable
    {
        public LoginResponseOpcode LoginOpcode { get; set; }
        public byte VersionMajor { get; set; }
        public byte VersionMinor { get; set; }
        public byte VersionPatch { get; set; }
        public uint Gold { get; set; }
        public uint StructureHut { get; set; }

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
                case LoginResponseOpcode.LoginSuccess:
                    Gold = reader.ReadUInt32();
                    StructureHut = reader.ReadUInt32();
                    break;
            }
        }
    }
}
