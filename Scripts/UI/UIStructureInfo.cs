using Godot;
using Common.Game;
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
        [Export] private readonly NodePath nodePathProductionTotalList;
        [Export] private readonly NodePath nodePathTechRequiredList;
#pragma warning restore CS0649 // Values are assigned in the editor

        // UI Elements
        private static Label uiTitle;
        private static Label uiDescription;
        private static Label uiStructuresOwned;
        private static HBoxContainer uiCostList;
        private static HBoxContainer uiProductionList;
        private static HBoxContainer uiProductionTotalList;
        private static GridContainer uiTechRequiredList;

        // Logic
        private static StructureType activeStructureId;

        public override void _Ready()
        {
            uiTitle = GetNode<Label>(nodePathTitle);
            uiDescription = GetNode<Label>(nodePathDescription);
            uiStructuresOwned = GetNode<Label>(nodePathStructuresOwned);
            uiCostList = GetNode<HBoxContainer>(nodePathCostList);
            uiProductionList = GetNode<HBoxContainer>(nodePathProductionList);
            uiProductionTotalList = GetNode<HBoxContainer>(nodePathProductionTotalList);
            uiTechRequiredList = GetNode<GridContainer>(nodePathTechRequiredList);
        }

        private void _on_Btn_Buy_pressed()
        {
            GD.Print("Active Structure ID: " + activeStructureId);
            ENetClient.PurchaseItem(activeStructureId);
        }

        private void _on_Btn_Sell_pressed()
        {
            // TODO: Not implemented yet
        }

        public static void UpdateDetails(StructureType id)
        {
            activeStructureId = id;

            var structure = UIGame.StructureInfoData[id];
            uiTitle.Text = structure.Name;
            uiDescription.Text = structure.Description;

            var structuresOwned = UIGame.StructureCounts[id];

            uiStructuresOwned.Text = "" + structuresOwned;

            foreach (Node child in uiCostList.GetChildren())
                child.QueueFree();

            var cost = structure.Cost;

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

            foreach (Node child in uiProductionTotalList.GetChildren())
                child.QueueFree();

            if (production.Count == 0)
            {
                var label = new Label
                {
                    Text = "Does not produce anything"
                };
                uiProductionTotalList.AddChild(label);
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
                    amount.Text = "" + (resource.Value * structuresOwned);

                    uiProductionTotalList.AddChild(resourceCountIcon);
                }
            }
        }
    }
}