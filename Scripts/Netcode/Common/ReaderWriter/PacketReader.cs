using System.IO;

namespace Common.Networking.IO 
{
    public class PacketReader : BinaryReader 
    {   
        public PacketReader(byte[] data) : base(new MemoryStream(data)) { }

        public bool ReadBool() => base.ReadBoolean();
        public sbyte ReadInt8() => base.ReadSByte();
        public byte ReadUInt8() => base.ReadByte();
        public float ReadFloat() => base.ReadSingle();
    }
}