using Godot;
using System;
using ENet;
using Common.Networking.IO;

namespace KRU.Networking
{
    public class HandlePacketPurchaseItem : HandlePacket
    {
        public override ServerPacketOpcode Opcode { get; set; }

        public HandlePacketPurchaseItem()
        {
            Opcode = ServerPacketOpcode.PurchasedItem;
        }

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPurchaseItem();
            data.Read(packetReader);

            var itemResponseOpcode = data.PurchaseItemResponseOpcode;

            if (itemResponseOpcode == PurchaseItemResponseOpcode.NotEnoughGold)
            {
                var message = $"You do not have enough gold for {(ItemType)data.ItemId}.";

                GD.Print(message);

                //var cmd = new GodotInstructions();
                //cmd.Set(GodotInstructionOpcode.LogMessage, message);

                //ENetClient.GodotCmds.Enqueue(cmd);

                // Update the player gold
                //ENetClient.GameScript.Player.Gold = data.Gold;
            }

            if (itemResponseOpcode == PurchaseItemResponseOpcode.Purchased)
            {
                var message = $"Bought {(ItemType)data.ItemId} for 25 gold.";

                GD.Print(message);

                //var cmd = new GodotInstructions();
                //cmd.Set(GodotInstructionOpcode.LogMessage, message);

                //ENetClient.GodotCmds.Enqueue(cmd);

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
    }

}