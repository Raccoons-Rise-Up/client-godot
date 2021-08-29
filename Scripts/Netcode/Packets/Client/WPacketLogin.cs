using Common.Networking.Message;
using Common.Networking.IO;
using Common.Networking.Packet;

namespace KRU.Networking
{
	public class WPacketLogin : IWritable
	{
		public byte VersionMajor { private get; set; }
		public byte VersionMinor { private get; set; }
		public byte VersionPatch { private get; set; }
		public string Username { private get; set; }
		public string PasswordHash { private get; set; }

		public void Write(PacketWriter writer)
		{
			writer.Write(VersionMajor);
			writer.Write(VersionMinor);
			writer.Write(VersionPatch);
			writer.Write(Username);
			writer.Write(PasswordHash);
		}
	}
}
