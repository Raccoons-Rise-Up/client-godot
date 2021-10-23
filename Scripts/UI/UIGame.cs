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

        // Labels
        public static Dictionary<ResourceType, UILabelResourceCount> ResourceCountLabels { get; set; }
        public static Dictionary<StructureType, UILabelStructureCount> StructureCountLabels { get; set; }

        // Data
        public static Dictionary<ResourceType, ResourceInfo> ResourceInfoData { get; set; }
        public static Dictionary<ResourceType, TextureRect> ResourceIconData { get; set; }
        public static Dictionary<StructureType, StructureInfo> StructureInfoData { get; set; }

        // Counts
        public static Dictionary<ResourceType, double> ResourceCounts { get; set; }
        public static Dictionary<StructureType, uint> StructureCounts { get; set; }
        public static DateTime StructuresLastChecked { get; set; }

        // Msc
        public static bool InGame { get; set; }

        public override void _Ready()
        {
            StructuresLastChecked = DateTime.Now; // TODO: Sync with game server!
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

        public static Dictionary<ResourceType, uint> GetLackingResources(StructureInfo structure)
        {
            var lackingResources = new Dictionary<ResourceType, uint>();
            foreach (var resource in ResourceCountLabels)
            {
                var playerResourceKey = resource.Key;
                var playerResourceAmount = (uint)ResourceCounts[resource.Key];

                if (structure.Cost.TryGetValue(playerResourceKey, out uint structureResourceValue)) 
                {
                    if (playerResourceAmount < structureResourceValue)
                    {
                        lackingResources.Add(playerResourceKey, structureResourceValue - playerResourceAmount);
                    }
                }
            }

            return lackingResources;
        }

        public static void AddResourcesGeneratedFromStructures() 
        {
            for (int i = 0; i < UIGame.StructureCounts.Count; i++)
            {
                var structureCount = UIGame.StructureCounts.ElementAt(i);

                var timeDiff = DateTime.Now - UIGame.StructuresLastChecked;

                // Look up the info for the structure
                var structureData = UIGame.StructureInfoData[structureCount.Key];

                var resources = new Dictionary<ResourceType, double>();
                
                foreach (var prod in structureData.Production)
                {
                    // amountGenerated = production * structure count * time diff
                    var amountGenerated = prod.Value * structureCount.Value * (double)timeDiff.TotalSeconds;
                    resources.Add(prod.Key, amountGenerated);
                }

                UIGame.AddResourceCounts(resources);
            }

            UIGame.StructuresLastChecked = DateTime.Now;
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

            ResourceCountLabels = new Dictionary<ResourceType, UILabelResourceCount>();
            StructureCountLabels = new Dictionary<StructureType, UILabelStructureCount>();

            UITerminal.Log("Welcome");

            ShowGameSection("Resources");
        }

        public static void InitResourceLabels(Dictionary<ResourceType, uint> resourceCounts)
        {
            foreach (var resource in resourceCounts) 
            {
                ResourceCountLabels.Add(resource.Key, new UILabelResourceCount(UIResources.ResourceList, resource.Key, resource.Value));
            }
        }

        public static void UpdateStructureCount(StructureType key, uint amount)
        {
            // Update structure counts
            StructureCounts[key] = amount;

            // Update display label
            UIGame.UpdateStructureLabel(key, ENetClient.PurchaseAmount);
        }

        public static void AddResourceCounts(Dictionary<ResourceType, double> resources)
        {
            // DEBUG
            foreach (var resource in resources)
                GD.Print($"{System.Enum.GetName(typeof(ResourceType), resource.Key)}: {resource.Value}");

            // Update resource counts
            foreach (var resource in resources)
                ResourceCounts[resource.Key] += resource.Value;

            // Update display labels
            UIGame.AddResourceLabels(resources);
        }

        public static void SetResourceCounts(Dictionary<ResourceType, double> resources)
        {
            // Update resource counts
            foreach (var resource in resources)
                ResourceCounts[resource.Key] = resource.Value;

            // Update display labels
            UIGame.SetResourceLabels(resources);
        }

        public static void SetResourceLabels(Dictionary<ResourceType, double> resources)
        {
            foreach (var resource in resources)
                ResourceCountLabels[resource.Key].SetAmount(resource.Value);
        }

        public static void AddResourceLabels(Dictionary<ResourceType, double> resources)
        {
            foreach (var resource in resources)
                ResourceCountLabels[resource.Key].AddAmount(resource.Value);
        }

        public static void InitStructureLabels(Dictionary<StructureType, uint> structureCounts) 
        {
            foreach (var structure in structureCounts) 
            {
                StructureCountLabels.Add(structure.Key, new UILabelStructureCount(UIStructures.StructureList, structure.Key, structure.Value));
            }
        }

        public static void InitStore()
        {
            UIStructureInfo.UpdateDetails(0);

            foreach (var structure in StructureInfoData)
                UIStore.AddStructure(structure.Value.Name, structure.Key);
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

#region Buttons
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
#endregion
    }
}