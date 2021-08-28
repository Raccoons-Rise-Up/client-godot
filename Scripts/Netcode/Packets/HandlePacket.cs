using Godot;
using ENet;
using Common.Networking.IO;

namespace KRU.Networking
{
    public static class HandlePacket
    {
        public static void Handle(ref Event netEvent)
        {
            var packetSizeMax = 1024;
            var readBuffer = new byte[packetSizeMax];
            var packetReader = new PacketReader(readBuffer);
            packetReader.BaseStream.Position = 0;

            netEvent.Packet.CopyTo(readBuffer);

            var opcode = (ServerPacketOpcode)packetReader.ReadByte();

            if (opcode == ServerPacketOpcode.LoginResponse)
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

            if (opcode == ServerPacketOpcode.PurchasedItem)
            {
                var data = new RPacketPurchaseItem();
                data.Read(packetReader);

                var itemResponseOpcode = data.PurchaseItemResponseOpcode;

                if (itemResponseOpcode == PurchaseItemResponseOpcode.NotEnoughGold)
                {
                    var message = $"You do not have enough gold for {(ItemType)data.ItemId}.";

                    GD.Print(message);

                    var cmd = new GodotInstructions();
                    cmd.Set(GodotInstructionOpcode.LogMessage, message);

                    ENetClient.GodotCmds.Enqueue(cmd);

                    // Update the player gold
                    //ENetClient.GameScript.Player.Gold = data.Gold;
                }

                if (itemResponseOpcode == PurchaseItemResponseOpcode.Purchased)
                {
                    var message = $"Bought {(ItemType)data.ItemId} for 25 gold.";

                    GD.Print(message);

                    var cmd = new GodotInstructions();
                    cmd.Set(GodotInstructionOpcode.LogMessage, message);

                    ENetClient.GodotCmds.Enqueue(cmd);

                    // Update the player gold
                    //ENetClient.GameScript.Player.Gold = data.Gold;

                    // Update the items
                    switch ((ItemType)data.ItemId)
                    {
                        case ItemType.Hut:
                            //ENetClient.GameScript.Player.StructureHuts++;
                            break;
                        case ItemType.Farm:
                            break;
                    }
                }
            }

            packetReader.Dispose();
        }
    }
}