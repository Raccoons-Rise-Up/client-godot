using Common.Networking.Message;
using Common.Networking.IO;

namespace Common.Networking.Packet 
{
    public class ServerPacket : GamePacket
    {
        public byte Opcode { get; set; }

        public ServerPacket(byte opcode, IWritable writable = null)
        {
            using (var writer = new PacketWriter()) 
            {
                writer.Write(opcode);
                if (writable != null) writable.Write(writer);

                var stream = writer.GetStream();
                Data = stream.ToArray();
                Size = stream.Length;
            }

            Opcode = opcode;
        }
    }
}