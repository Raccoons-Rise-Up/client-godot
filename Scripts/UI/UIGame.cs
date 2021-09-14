using Godot;
using KRU.Game;
using KRU.Networking;
using System.Collections.Generic;

namespace KRU.UI
{
    public class UIGame : Control
    {
        // Prefabs
        public static PackedScene PrefabButton = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIButton.tscn");
        public static PackedScene PrefabUILabelCountIcon = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCountIcon.tscn");

        // Title
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathTitle;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static Label labelTitle;

        public static Dictionary<ushort, UILabelCount> ResourceCountLabels { get; set; }
        public static Dictionary<ushort, UILabelCount> StructureCountLabels { get; set; }
        public static Dictionary<ushort, ResourceInfo> ResourceInfoData { get; set; }
        public static Dictionary<ushort, StructureInfo> StructureInfoData { get; set; }

        public override void _Ready()
        {
            labelTitle = GetNode<Label>(nodePathTitle); // Title
        }

        public static void UpdateResourceLabels(Dictionary<ushort, uint> resources)
        {
            foreach (var resource in resources)
                ResourceCountLabels[resource.Key].SetAmount(resource.Value);
        }

        public static void UpdateStructureLabel(ushort structureId, uint amount)
        {
            StructureCountLabels[structureId].AddAmount(amount);
        }

        public static void InitGame() 
        {
            UIResources.ClearLabelCounts();
            UIStructures.ClearLabelCounts();
            UIStore.ClearButtons();
            UITerminal.ClearMessages();

            ResourceCountLabels = new Dictionary<ushort, UILabelCount>();
            StructureCountLabels = new Dictionary<ushort, UILabelCount>();
            ResourceInfoData = new Dictionary<ushort, ResourceInfo>();
            StructureInfoData = new Dictionary<ushort, StructureInfo>();

            UITerminal.Log("Welcome");

            ShowGameSection("Resources");
        }

        public static void InitResourceLabels(Dictionary<ushort, uint> resourceCounts)
        {
            foreach (var resource in resourceCounts) 
            {
                var resourceName = ResourceInfoData[resource.Key].Name;
                ResourceCountLabels.Add(resource.Key, new UILabelCount(UIResources.ResourceList, resourceName, resource.Value));
            }
        }

        public static void InitStructureLabels(Dictionary<ushort, uint> structureCounts) 
        {
            foreach (var structure in structureCounts) 
            {
                var structureName = StructureInfoData[structure.Key].Name;
                StructureCountLabels.Add(structure.Key, new UILabelCount(UIStructures.StructureList, structureName, structure.Value));
            }
        }

        public static void InitStore()
        {
            UIStructureInfo.UpdateDetails(0);

            foreach (var structure in StructureInfoData)
                UIStore.AddStructure(structure.Value.Name, structure.Key);
        }

        private void _on_Btn_Resources_pressed() 
        {
            ShowGameSection("Resources");
            UIMap.HideMap();
        }

        private void _on_Btn_Structures_pressed() 
        {
            ShowGameSection("Structures");
            UIMap.HideMap();
        }

        private void _on_Btn_Kittens_pressed() 
        {
            ShowGameSection("Kittens");
            UIMap.HideMap();
        }

        private void _on_Btn_Store_pressed() 
        {
            ShowGameSection("Store");
            UIMap.HideMap();
        }

        private void _on_Btn_Research_pressed() 
        {
            ShowGameSection("Research");
            UIMap.HideMap();
        }

        private void _on_Btn_Map_pressed() 
        {
            UIMap.ShowMap();
            ShowGameSection("Map");
        }

        private static void ShowGameSection(string name)
        {
            HideAllGameSections();

            var section = UIGameSections.ControlSections[name];
            section.Visible = true;
            labelTitle.Text = section.Name;
        }

        private static void HideAllGameSections()
        {
            foreach (var section in UIGameSections.ControlSections.Values)
                section.Visible = false;
        }
    }
}