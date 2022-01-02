namespace Common.Netcode
{
    public class ServerPacket : GamePacket
    {
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