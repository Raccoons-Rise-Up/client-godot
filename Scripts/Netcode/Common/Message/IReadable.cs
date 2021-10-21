using Common.Networking.IO;

namespace Common.Networking.Message
{
    public interface IReadable 
    {
        void Read(PacketReader reader);
    }
}