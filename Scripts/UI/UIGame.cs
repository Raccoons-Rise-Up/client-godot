using System.Collections.Generic;
using Godot;
using KRU.Networking;
using KRU.Game;

namespace KRU.UI
{
    public class UIGame : Control
    {
        // Title
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathTitle;
#pragma warning restore CS0649 // Values are assigned in the editor
        private Label labelTitle;

        public static Dictionary<ResourceType, UILabelCount> Resources = new Dictionary<ResourceType, UILabelCount>();
        public static List<Structure> StructureData = new List<Structure>();

        public override void _Ready()
        {
            UITerminal.Log("Welcome");

            UIResources.AddLabelCount(ResourceType.Wood);
            UIResources.AddLabelCount(ResourceType.Stone);
            UIResources.AddLabelCount(ResourceType.Gold);
            UIResources.AddLabelCount(ResourceType.Wheat);
            
            // Title
            labelTitle = GetNode<Label>(nodePathTitle);

            // Setup
            ShowGameSection("Resources");
        }

        private void _on_Btn_Resources_pressed() => ShowGameSection("Resources");
        private void _on_Btn_Structures_pressed() => ShowGameSection("Structures");
        private void _on_Btn_Kittens_pressed() => ShowGameSection("Kittens");
        private void _on_Btn_Store_pressed() => ShowGameSection("Store");
        private void _on_Btn_Research_pressed() => ShowGameSection("Research");
        private void _on_Btn_Map_pressed() => ShowGameSection("Map");

        private void ShowGameSection(string name)
        {
            HideAllGameSections();

            var section = UIGameSections.ControlSections[name];
            section.Visible = true;
            labelTitle.Text = section.Name;
        }

        private void HideAllGameSections()
        {
            foreach (var section in UIGameSections.ControlSections.Values)
                section.Visible = false;
        }
    }
}
