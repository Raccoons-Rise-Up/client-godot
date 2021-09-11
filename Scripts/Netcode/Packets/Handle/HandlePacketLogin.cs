using Common.Networking.IO;
using ENet;
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

            if (opcode == LoginResponseOpcode.LoginSuccessReturningPlayer)
            {
                UIGame.InitGame();
                UIGame.ResourceInfoData = data.ResourceInfoData;
                UIGame.StructureInfoData = data.StructureInfoData;

                foreach (var resource in data.ResourceCounts) 
                    UIResources.AddLabelCount(resource.Key, resource.Value);

                foreach (var structure in data.StructureCounts)
                    UIStructures.AddLabelCount(structure.Key, structure.Value);

                UIGame.InitStore();

                UILogin.UpdateResponse("Login success!");
                UILogin.LoadGameScene();
            }

            if (opcode == LoginResponseOpcode.LoginSuccessNewPlayer)
            {
                UIGame.InitGame();
                UIGame.ResourceInfoData = data.ResourceInfoData;
                UIGame.StructureInfoData = data.StructureInfoData;

                UIGame.InitStore();

                UILogin.UpdateResponse("Login success!");
                UILogin.LoadGameScene();
            }
        }
    }
}