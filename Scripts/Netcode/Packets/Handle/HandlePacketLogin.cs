using Godot;
using System;
using ENet;
using Common.Networking.IO;

namespace KRU.Networking
{
    public class HandlePacketLogin : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketLogin()
        {
            Opcode = ServerPacketOpcode.LoginResponse;
        }

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketLogin();
            data.Read(packetReader);

            if (data.LoginOpcode == LoginResponseOpcode.VersionMismatch)
            {
                var serverVersion = $"{data.VersionMajor}.{data.VersionMinor}.{data.VersionPatch}";
                var clientVersion = $"{ENetClient.Version.Major}.{ENetClient.Version.Minor}.{ENetClient.Version.Patch}";
                var message = $"Version mismatch. Server ver. {serverVersion} Client ver. {clientVersion}";

                GD.Print(message);

                var cmd = new GodotInstructions();
                cmd.Set(GodotInstructionOpcode.ServerResponseMessage, message);

                ENetClient.GodotCmds.Enqueue(cmd);
            }

            if (data.LoginOpcode == LoginResponseOpcode.LoginSuccess)
            {
                GD.Print("Loggin success!");

                // Load the main game 'scene'
                ENetClient.GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.LoadMainScene));

                // Update player values
                /*ENetClient.MenuScript.gameScript.Player = new Player
                {
                    Gold = data.Gold,
                    StructureHuts = data.StructureHut
                };*/

                ENetClient.GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.LoginSuccess));
            }
        }
    }

}