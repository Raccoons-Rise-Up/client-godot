using Common.Networking.IO;
using Common.Game;
using ENet;
using System.Linq;
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
                var structure = UIGame.StructureInfoData[(StructureType)ENetClient.PurchaseId];
                var lackingResources = UIGame.GetLackingResources(structure);

                var message = $"Could not afford 1 x {structure.Name} ({ConvertCostToString(lackingResources)} is needed)";

                UITerminal.Log(message);
            }

            if (itemResponseOpcode == PurchaseItemResponseOpcode.Purchased)
            {
                var structureId = (StructureType)ENetClient.PurchaseId;
                var structure = UIGame.StructureInfoData[structureId];
                var message = $"Bought 1 x {structure.Name} for {ConvertCostToString(structure.Cost)}.";

                UITerminal.Log(message);

                foreach (var resource in data.Resources)
                    GD.Print(System.Enum.GetName(typeof(ResourceType), resource.Key) + " " + resource.Value);

                UIGame.UpdateResourceLabels(data.Resources);
                UIGame.UpdateStructureLabel(structureId, ENetClient.PurchaseAmount);
                UIStructureInfo.UpdateDetails(structureId);
            }

            ENetClient.ProcessingPurchaseRequest = false;
        }

        private string ConvertCostToString(Dictionary<ResourceType, uint> resourceListCost) => string.Join(" ", resourceListCost.Select(x => $"{x.Value} :{x.Key}:"));
    }
}