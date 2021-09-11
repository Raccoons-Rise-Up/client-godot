using Common.Networking.IO;
using ENet;
using Godot;
using KRU.UI;
using System.Collections.Generic;

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
                var structure = UIGame.StructureInfoData[data.ItemId];
                var message = $"Could not afford 1 x {structure.Name}, {ConvertCostToString(data.Resources)} is needed";

                UITerminal.Log(message);
            }

            if (itemResponseOpcode == PurchaseItemResponseOpcode.Purchased)
            {
                var structure = UIGame.StructureInfoData[data.ItemId];
                var message = $"Bought 1 x {structure.Name} for {ConvertCostToString(structure.Cost)}.";

                UITerminal.Log(message);

                foreach (var resource in data.Resources)
                    UIGame.Resources[resource.Key].Set(resource.Value);
            }
        }

        private string ConvertCostToString(Dictionary<ushort, uint> resourceListCost)
        {
            string readableResourceListCost = "";
            foreach (var resource in resourceListCost)
            {
                var name = UIGame.ResourceInfoData[resource.Key].Name;
                var amount = resource.Value;

                readableResourceListCost += $"{amount} {name}, ";
            }

            return readableResourceListCost;
        }
    }
}