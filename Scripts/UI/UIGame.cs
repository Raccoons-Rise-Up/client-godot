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

        // Title
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathTitle;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static Label labelTitle;

        public static Dictionary<ushort, UILabelCount> Resources { get; set; }
        public static Dictionary<ushort, ResourceInfo> ResourceInfoData { get; set; }
        public static Dictionary<ushort, StructureInfo> StructureInfoData { get; set; }

        public override void _Ready()
        {
            labelTitle = GetNode<Label>(nodePathTitle); // Title
        }

        public static void InitGame() 
        {
            UIResources.ClearLabelCounts();
            UIStructures.ClearLabelCounts();
            UIStore.ClearButtons();
            UITerminal.ClearMessages();

            Resources = new Dictionary<ushort, UILabelCount>();
            ResourceInfoData = new Dictionary<ushort, ResourceInfo>();
            StructureInfoData = new Dictionary<ushort, StructureInfo>();

            UITerminal.Log("Welcome");

            ShowGameSection("Resources");
        }

        public static void InitStore()
        {
            UIStructureInfo.PopulateDetails(0);

            foreach (var structure in StructureInfoData)
                UIStore.AddStructure(structure.Value.Name, structure.Key);
        }

        private void _on_Btn_Resources_pressed() => ShowGameSection("Resources");

        private void _on_Btn_Structures_pressed() => ShowGameSection("Structures");

        private void _on_Btn_Kittens_pressed() => ShowGameSection("Kittens");

        private void _on_Btn_Store_pressed() => ShowGameSection("Store");

        private void _on_Btn_Research_pressed() => ShowGameSection("Research");

        private void _on_Btn_Map_pressed() => ShowGameSection("Map");

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