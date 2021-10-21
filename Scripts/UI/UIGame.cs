using Godot;
using KRU.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using Common.Game;

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

        public static Dictionary<ResourceType, UILabelCount> ResourceCountLabels { get; set; }
        public static Dictionary<StructureType, UILabelCount> StructureCountLabels { get; set; }
        public static Dictionary<ResourceType, ResourceInfo> ResourceInfoData { get; set; }
        public static Dictionary<ResourceType, TextureRect> ResourceIconData { get; set; }
        public static Dictionary<StructureType, StructureInfo> StructureInfoData { get; set; }

        public override void _Ready()
        {
            labelTitle = GetNode<Label>(nodePathTitle); // Title

            ResourceInfoData = typeof(ResourceInfo).Assembly.GetTypes().Where(x => typeof(ResourceInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<ResourceInfo>()
                .ToDictionary(x => (ResourceType)Enum.Parse(typeof(ResourceType), x.GetType().Name.Replace(typeof(ResourceInfo).Name, "")), x => x);

            ResourceIconData = new Dictionary<ResourceType, TextureRect>();
            foreach (var resourceType in ResourceInfoData.Keys)
                ResourceIconData.Add(resourceType, GetResourceImage(resourceType));

            StructureInfoData = typeof(StructureInfo).Assembly.GetTypes().Where(x => typeof(StructureInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<StructureInfo>()
                .ToDictionary(x => (StructureType)Enum.Parse(typeof(StructureType), x.GetType().Name.Replace(typeof(StructureInfo).Name, "")), x => x);
        }

        private static TextureRect GetResourceImage(ResourceType type)
        {
            return new TextureRect
            {
                Texture = ResourceLoader.Load<StreamTexture>($"res://Sprites/Icons/{Enum.GetName(typeof(ResourceType), type).ToLower()}.png")
            };
        }

        public static void UpdateResourceLabels(Dictionary<ResourceType, uint> resources)
        {
            foreach (var resource in resources)
                ResourceCountLabels[resource.Key].SetAmount(resource.Value);
        }

        public static void UpdateStructureLabel(StructureType structureId, uint amount)
        {
            StructureCountLabels[structureId].AddAmount(amount);
        }

        public static void InitGame() 
        {
            UIResources.ClearLabelCounts();
            UIStructures.ClearLabelCounts();
            UIStore.ClearButtons();
            UITerminal.ClearMessages();

            ResourceCountLabels = new Dictionary<ResourceType, UILabelCount>();
            StructureCountLabels = new Dictionary<StructureType, UILabelCount>();

            UITerminal.Log("Welcome");

            ShowGameSection("Resources");
        }

        public static void InitResourceLabels(Dictionary<ResourceType, uint> resourceCounts)
        {
            foreach (var resource in resourceCounts) 
            {
                var resourceName = ResourceInfoData[resource.Key].Name;
                ResourceCountLabels.Add(resource.Key, new UILabelCount(UIResources.ResourceList, resourceName, resource.Value));
            }
        }

        public static void InitStructureLabels(Dictionary<StructureType, uint> structureCounts) 
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