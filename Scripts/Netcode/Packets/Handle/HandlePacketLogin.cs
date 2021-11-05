using Common.Networking.IO;
using Common.Networking.Packet;
using ENet;
using System.Linq;
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

                UIGame.ResourceCounts = data.ResourceCounts.ToDictionary(x => x.Key, x => (double)x.Value);
                UIGame.StructureCounts = data.StructureCounts;

                UIGame.InitResourceLabels(data.ResourceCounts);
                UIGame.InitStructureLabels(data.StructureCounts);

                UIGame.InitStore();

                UILogin.UpdateResponse("Login success!");
                UILogin.LoadGameScene();

                UIGame.InGame = true;
            }

            if (opcode == LoginResponseOpcode.LoginSuccessNewPlayer)
            {
                UIGame.InitGame();

                UIGame.InitStore();

                UIGame.InitResourceLabels(UIGame.ResourceCounts.ToDictionary(x => x.Key, x => (uint)x.Value));
                UIGame.InitStructureLabels(UIGame.StructureCounts.ToDictionary(x => x.Key, x => (uint)x.Value));

                UILogin.UpdateResponse("Login success!");
                UILogin.LoadGameScene();

                UIGame.InGame = true;
            }
        }
    }
}