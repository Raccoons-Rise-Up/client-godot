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
        private static Label UITitle { get; set; }
        private static Label UIDescription { get; set; }
        private static Label UIStructuresOwned { get; set; }
        private static HBoxContainer UICostList { get; set; }
        private static HBoxContainer UIProductionList { get; set; }
        private static HBoxContainer UICurrentList { get; set; }
        private static GridContainer UITechRequiredList { get; set; }

        // Logic
        private static StructureType ActiveStructureId { get; set; }
        private static Dictionary<ResourceType, Label> LabelCurrentAmount { get; set; }

        public override void _Ready()
        {
            UITitle = GetNode<Label>(nodePathTitle);
            UIDescription = GetNode<Label>(nodePathDescription);
            UIStructuresOwned = GetNode<Label>(nodePathStructuresOwned);
            UICostList = GetNode<HBoxContainer>(nodePathCostList);
            UIProductionList = GetNode<HBoxContainer>(nodePathProductionList);
            UICurrentList = GetNode<HBoxContainer>(nodePathCurrentList);
            UITechRequiredList = GetNode<GridContainer>(nodePathTechRequiredList);

            LabelCurrentAmount = new Dictionary<ResourceType, Label>();
        }

        private void _on_Btn_Buy_pressed()
        {
            ENetClient.PurchaseItem(ActiveStructureId);
        }

        private void _on_Btn_Sell_pressed()
        {
            // TODO: Not implemented yet
        }

        public static void SwitchActiveStructure(StructureType id) => ActiveStructureId = id;

        public static void UpdateCurrentAmounts()
        {
            foreach (var keyValuePair in LabelCurrentAmount)
            {
                keyValuePair.Value.Text = "" + (uint)UIGame.ResourceCounts[keyValuePair.Key];
            }
        }

        public static void UpdateDetails()
        {
            var structure = UIGame.StructureInfoData[ActiveStructureId];
            UITitle.Text = structure.Name;
            UIDescription.Text = structure.Description;

            if (!UIGame.StructureCounts.ContainsKey(ActiveStructureId))
            {
                // Although this should not happen anymore as the server updates the old player config data when new structures are defined
                GD.PrintErr($"Could not find ID '{ActiveStructureId}' for structure count data, perhaps you forgot to update the structure counts?");
                return;
            }

            var structuresOwned = UIGame.StructureCounts[ActiveStructureId];

            UIStructuresOwned.Text = "" + structuresOwned;

            foreach (Node child in UICostList.GetChildren())
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

                UICostList.AddChild(resourceCountIcon);
            }

            foreach (Node child in UIProductionList.GetChildren())
                child.QueueFree();

            var production = structure.Production;

            // PRODUCTION
            if (production.Count == 0)
            {
                var label = new Label
                {
                    Text = "Does not produce anything"
                };
                UIProductionList.AddChild(label);
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

                    UIProductionList.AddChild(resourceCountIcon);
                }
            }

            foreach (Node child in UICurrentList.GetChildren())
                child.QueueFree();

            // CURRENT
            if (production.Count == 0)
            {
                var label = new Label
                {
                    Text = "Does not produce anything"
                };
                UICurrentList.AddChild(label);
            } 
            else 
            {
                // Remove old labels that were being kept track of
                LabelCurrentAmount.Clear();

                foreach (var resource in production)
                {
                    var resourceCountIcon = UIGame.PrefabUILabelCountIcon.Instance();
                    var image = resourceCountIcon.GetNode<TextureRect>("Image");
                    var amount = resourceCountIcon.GetNode<Label>("Amount");

                    // Keep track of current values so we do not have to reupdate everything just to update these values
                    LabelCurrentAmount.Add(resource.Key, amount);

                    var resourceInfo = UIGame.ResourceInfoData[resource.Key];

                    image.Texture = UIGame.ResourceIconData[resource.Key].Texture;
                    amount.Text = "" + (uint)UIGame.ResourceCounts[resource.Key];

                    UICurrentList.AddChild(resourceCountIcon);
                }
            }
        }
    }
}