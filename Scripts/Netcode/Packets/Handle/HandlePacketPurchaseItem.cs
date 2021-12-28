using Common.Networking.IO;
using Common.Networking.Packet;
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

        public override void Handle(PacketReader packetReader)
        {
            var data = new RPacketPurchaseItem();
            data.Read(packetReader);

            var itemResponseOpcode = data.PurchaseItemResponseOpcode;

            if (itemResponseOpcode == PurchaseItemResponseOpcode.NotEnoughResources)
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

                // Reminder: This does not set all the resources
                UIGame.SetResourceCounts(data.Resources.ToDictionary(x => x.Key, x => (double)x.Value));
                UIGame.UpdateStructureCount(structureId, UIGame.StructureCounts[structureId] + ENetClient.PurchaseAmount);
                
                UIStructureInfo.SwitchActiveStructure(structureId);
                UIStructureInfo.UpdateDetails();
            }

            ENetClient.ProcessingPurchaseRequest = false;
        }

        private string ConvertCostToString(Dictionary<ResourceType, uint> resourceListCost) => string.Join(" ", resourceListCost.Select(x => $"{x.Value} :{x.Key}:"));
    }
}