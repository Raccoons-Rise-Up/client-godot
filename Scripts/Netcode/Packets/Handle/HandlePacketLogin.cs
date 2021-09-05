using Godot;
using System;
using ENet;
using Common.Networking.IO;
using KRU.UI;

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

            var opcode = data.LoginOpcode;

            if (opcode == LoginResponseOpcode.InvalidToken)
            {
                UILogin.UpdateResponse("Invalid JWT");
            }

            if (opcode == LoginResponseOpcode.VersionMismatch)
            {
                var serverVersion = $"{data.VersionMajor}.{data.VersionMinor}.{data.VersionPatch}";
                var clientVersion = $"{ENetClient.Version.Major}.{ENetClient.Version.Minor}.{ENetClient.Version.Patch}";
                var message = $"Version mismatch. Server ver. {serverVersion} Client ver. {clientVersion}";

                UILogin.UpdateResponse(message);
            }

            if (opcode == LoginResponseOpcode.LoginSuccess)
            {
                UILogin.UpdateResponse("Login success!");

                // Load the main game 'scene'
                //ENetClient.GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.LoadMainScene));
                UILogin.LoadGameScene();

                // Update player values
                /*ENetClient.MenuScript.gameScript.Player = new Player
                {
                    Gold = data.Gold,
                    StructureHuts = data.StructureHut
                };*/
            }
        }
    }

}