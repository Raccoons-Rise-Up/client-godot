using Common.Networking.IO;

namespace Common.Networking.Message 
{
    public interface IWritable 
    {
        void Write(PacketWriter writer);
    }
}