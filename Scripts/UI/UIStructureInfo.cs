using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
    public class UIStructureInfo : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathTitle;
        [Export] private NodePath nodePathDescription;
        [Export] private NodePath nodePathStructuresOwned;
        [Export] private NodePath nodePathCostList;
        [Export] private NodePath nodePathProductionList;
        [Export] private NodePath nodePathProductionTotalList;
        [Export] private NodePath nodePathTechRequiredList;
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
        private static uint activeStructureId;

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
            ENetClient.PurchaseItem(activeStructureId);
        }

        private void _on_Btn_Sell_pressed()
        {
            // TODO: Not implemented yet
        }

        public static void PopulateDetails(uint id)
        {
            activeStructureId = id;

            var structure = UIGame.StructureData[id];
            uiTitle.Text = structure.Name;
            uiDescription.Text = structure.Description;
            uiStructuresOwned.Text = "" + 0;
        }
    }

}