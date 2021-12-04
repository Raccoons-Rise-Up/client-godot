using Godot;
using Common.Game;
using System.Collections.Generic;
using KRU.Networking;

namespace KRU.UI
{
    public class UIStructureInfo : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathTitle;
        [Export] private readonly NodePath nodePathDescription;
        [Export] private readonly NodePath nodePathStructuresOwned;
        [Export] private readonly NodePath nodePathCostList;
        [Export] private readonly NodePath nodePathProductionList;
        [Export] private readonly NodePath nodePathCurrentList;
        [Export] private readonly NodePath nodePathTechRequiredList;
#pragma warning restore CS0649 // Values are assigned in the editor

        // UI Elements
        private static Label uiTitle;
        private static Label uiDescription;
        private static Label uiStructuresOwned;
        private static HBoxContainer uiCostList;
        private static HBoxContainer uiProductionList;
        private static HBoxContainer uiCurrentList;
        private static GridContainer uiTechRequiredList;

        // Logic
        private static StructureType activeStructureId;
        private static Dictionary<ResourceType, Label> labelCurrentAmount;

        public override void _Ready()
        {
            uiTitle = GetNode<Label>(nodePathTitle);
            uiDescription = GetNode<Label>(nodePathDescription);
            uiStructuresOwned = GetNode<Label>(nodePathStructuresOwned);
            uiCostList = GetNode<HBoxContainer>(nodePathCostList);
            uiProductionList = GetNode<HBoxContainer>(nodePathProductionList);
            uiCurrentList = GetNode<HBoxContainer>(nodePathCurrentList);
            uiTechRequiredList = GetNode<GridContainer>(nodePathTechRequiredList);

            labelCurrentAmount = new Dictionary<ResourceType, Label>();
        }

        private void _on_Btn_Buy_pressed()
        {
            ENetClient.PurchaseItem(activeStructureId);
        }

        private void _on_Btn_Sell_pressed()
        {
            // TODO: Not implemented yet
        }

        public static void SwitchActiveStructure(StructureType id) => activeStructureId = id;

        public static void UpdateCurrentAmounts()
        {
            foreach (var keyValuePair in labelCurrentAmount)
            {
                keyValuePair.Value.Text = "" + (uint)UIGame.ResourceCounts[keyValuePair.Key];
            }
        }

        public static void UpdateDetails()
        {
            var structure = UIGame.StructureInfoData[activeStructureId];
            uiTitle.Text = structure.Name;
            uiDescription.Text = structure.Description;

            if (!UIGame.StructureCounts.ContainsKey(activeStructureId))
            {
                // Although this should not happen anymore as the server updates the old player config data when new structures are defined
                GD.PrintErr($"Could not find ID '{activeStructureId}' for structure count data, perhaps you forgot to update the structure counts?");
                return;
            }

            var structuresOwned = UIGame.StructureCounts[activeStructureId];

            uiStructuresOwned.Text = "" + structuresOwned;

            foreach (Node child in uiCostList.GetChildren())
                child.QueueFree();

            var cost = structure.Cost;

            // COST
            foreach (var resource in cost)
            {
                var resourceCountIcon = UIGame.PrefabUILabelCountIcon.Instance();
                var image = resourceCountIcon.GetNode<TextureRect>("Image");
                var amount = resourceCountIcon.GetNode<Label>("Amount");

                var resourceInfo = UIGame.ResourceInfoData[resource.Key];

                image.Texture = UIGame.ResourceIconData[resource.Key].Texture;
                amount.Text = "" + resource.Value;

                uiCostList.AddChild(resourceCountIcon);
            }

            foreach (Node child in uiProductionList.GetChildren())
                child.QueueFree();

            var production = structure.Production;

            // PRODUCTION
            if (production.Count == 0)
            {
                var label = new Label
                {
                    Text = "Does not produce anything"
                };
                uiProductionList.AddChild(label);
            } 
            else 
            {
                foreach (var resource in production)
                {
                    var resourceCountIcon = UIGame.PrefabUILabelCountIcon.Instance();
                    var image = resourceCountIcon.GetNode<TextureRect>("Image");
                    var amount = resourceCountIcon.GetNode<Label>("Amount");

                    var resourceInfo = UIGame.ResourceInfoData[resource.Key];

                    image.Texture = UIGame.ResourceIconData[resource.Key].Texture;
                    amount.Text = "" + resource.Value;

                    uiProductionList.AddChild(resourceCountIcon);
                }
            }

            foreach (Node child in uiCurrentList.GetChildren())
                child.QueueFree();

            // CURRENT
            if (production.Count == 0)
            {
                var label = new Label
                {
                    Text = "Does not produce anything"
                };
                uiCurrentList.AddChild(label);
            } 
            else 
            {
                // Remove old labels that were being kept track of
                labelCurrentAmount.Clear();

                foreach (var resource in production)
                {
                    var resourceCountIcon = UIGame.PrefabUILabelCountIcon.Instance();
                    var image = resourceCountIcon.GetNode<TextureRect>("Image");
                    var amount = resourceCountIcon.GetNode<Label>("Amount");

                    // Keep track of current values so we do not have to reupdate everything just to update these values
                    labelCurrentAmount.Add(resource.Key, amount);

                    var resourceInfo = UIGame.ResourceInfoData[resource.Key];

                    image.Texture = UIGame.ResourceIconData[resource.Key].Texture;
                    amount.Text = "" + (uint)UIGame.ResourceCounts[resource.Key];

                    uiCurrentList.AddChild(resourceCountIcon);
                }
            }
        }
    }
}