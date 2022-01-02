using Common.Netcode;
using Godot;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Client.Netcode
{
    public class RPacketLogin : IReadable
    {
        public LoginResponseOpcode LoginOpcode { get; set; }
        public Version Version { get; set; }

        public void Read(PacketReader reader)
        {
            LoginOpcode = (LoginResponseOpcode)reader.ReadByte();

            if (LoginOpcode == LoginResponseOpcode.VersionMismatch) 
            {
                Version = new Version {
                    Major = reader.ReadByte(),
                    Minor = reader.ReadByte(),
                    Patch = reader.ReadByte()
                };
                return;
            }
        }
    }
}