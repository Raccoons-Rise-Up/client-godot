using Godot;
using KRU.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Common.Game;
using Timer = System.Timers.Timer;

namespace KRU.UI
{
    public class UIGame : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] public readonly string fieldWebServerIp;
        [Export] public readonly string fieldWebServerPort;
        [Export] public readonly string fieldGameServerIp;
        [Export] public readonly ushort fieldGameServerPort;
        [Export] private readonly NodePath nodePathTitle;
        [Export] private readonly NodePath nodePathDialogPopups;
        [Export] private readonly NodePath nodePathClientUsername;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static string WebServerIp { get; set; }
        public static string WebServerPort { get; set; }
        public static string GameServerIp { get; set; }
        public static ushort GameServerPort { get; set; }

        public static Control DialogPopups { get; set; }

        // Prefabs
        public static PackedScene PrefabButton = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIButton.tscn");
        public static PackedScene PrefabUILabelCountIcon = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCountIcon.tscn");

        private static Label LabelTitle { get; set; }
        private static Label LabelClientUsername { get; set; }

        // Labels
        public static Dictionary<ResourceType, UILabelResourceCount> ResourceCountLabels { get; set; }
        public static Dictionary<StructureType, UILabelStructureCount> StructureCountLabels { get; set; }

        // Data
        public static Dictionary<uint, string> Players { get; set; }
        public static Dictionary<ResourceType, ResourceInfo> ResourceInfoData { get; set; }
        public static Dictionary<ResourceType, TextureRect> ResourceIconData { get; set; }
        public static Dictionary<StructureType, StructureInfo> StructureInfoData { get; set; }
        public static string ClientPlayerName { get; set; }
        public static uint ClientPlayerId { get; set; }

        // Counts
        public static Dictionary<ResourceType, double> ResourceCounts { get; set; }
        public static Dictionary<StructureType, uint> StructureCounts { get; set; }
        public static DateTime StructuresLastChecked { get; set; }

        // Game Loop
        private static Timer GameLoopTimer { get; set; }

        // Msc
        public static bool InGame { get; set; }

        public override void _Ready()
        {
            LabelClientUsername = GetNode<Label>(nodePathClientUsername);
            DialogPopups = GetNode<Control>(nodePathDialogPopups);

            WebServerIp = fieldWebServerIp;
            WebServerPort = fieldWebServerPort;
            GameServerIp = fieldGameServerIp;
            GameServerPort = fieldGameServerPort;

            Players = new Dictionary<uint, string>();

            ResourceCounts = typeof(ResourceInfo).Assembly.GetTypes().Where(x => typeof(ResourceInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<ResourceInfo>()
                .ToDictionary(x => (ResourceType)Enum.Parse(typeof(ResourceType), x.GetType().Name.Replace(typeof(ResourceInfo).Name, "")), x => 0d);
            StructureCounts = typeof(StructureInfo).Assembly.GetTypes().Where(x => typeof(StructureInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<StructureInfo>()
                .ToDictionary(x => (StructureType)Enum.Parse(typeof(StructureType), x.GetType().Name.Replace(typeof(StructureInfo).Name, "")), x => (uint)0);

            StructuresLastChecked = DateTime.Now;
            LabelTitle = GetNode<Label>(nodePathTitle); // Title

            ResourceInfoData = typeof(ResourceInfo).Assembly.GetTypes().Where(x => typeof(ResourceInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<ResourceInfo>()
                .ToDictionary(x => (ResourceType)Enum.Parse(typeof(ResourceType), x.GetType().Name.Replace(typeof(ResourceInfo).Name, "")), x => x);

            ResourceIconData = new Dictionary<ResourceType, TextureRect>();
            foreach (var resourceType in ResourceInfoData.Keys)
                ResourceIconData.Add(resourceType, GetResourceImage(resourceType));

            StructureInfoData = typeof(StructureInfo).Assembly.GetTypes().Where(x => typeof(StructureInfo).IsAssignableFrom(x) && !x.IsAbstract).Select(Activator.CreateInstance).Cast<StructureInfo>()
                .ToDictionary(x => (StructureType)Enum.Parse(typeof(StructureType), x.GetType().Name.Replace(typeof(StructureInfo).Name, "")), x => x);

            GameLoopTimer = new Timer();
            GameLoopTimer.Elapsed += new ElapsedEventHandler(GameUpdateLoop);
            GameLoopTimer.Interval = 1000; // 1000ms
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton && @event.IsPressed() && UIUser.ActiveDialogExists())
            {
                // Check if user is clicking inside of the dialog
                if (DialogPopups.GetChildCount() == 0)
                    return;
                
                var dialog = DialogPopups.GetChild<Control>(0);

                var dialogLocalPos = dialog.GetLocalMousePosition();
                var dialogRectSize = dialog.GetRect().Size;

                if (dialogLocalPos.x >= 0 && dialogLocalPos.y >= 0 && dialogLocalPos.x <= dialogRectSize.x && dialogLocalPos.y <= dialogRectSize.y)
                    return;

                // Remove the dialog
                UIUser.RemoveActiveDialog();
            }
        }

        public static void ResetGame()
        {
            UIUsers.RemoveAllUsers();
            UIChannels.RemoveAllChannels();
            UIChat.ClearChat();
            
            UILogin.LoadMenuScene();
        }

        private void GameUpdateLoop(object source, ElapsedEventArgs e)
        {
            UIGame.AddResourcesGeneratedFromStructures();
            //UIStructureInfo.UpdateCurrentAmounts();
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

        public static void DisableGameLoop() => GameLoopTimer.Enabled = false;
        public static void EnableGameLoop() => GameLoopTimer.Enabled = true;

        public static void InitGame() 
        {
            LabelClientUsername.Text = ClientPlayerName;

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

        public static void SetStructureCounts(Dictionary<StructureType, uint> structures)
        {
            // Update resource counts
            foreach (var structure in structures)
                StructureCounts[structure.Key] = structure.Value;

            // Update display labels
            UIGame.SetStructureLabels(structures);
        }

        public static void SetResourceLabels(Dictionary<ResourceType, double> resources)
        {
            foreach (var resource in resources)
                ResourceCountLabels[resource.Key].SetAmount(resource.Value);
        }

        public static void SetStructureLabels(Dictionary<StructureType, uint> structures)
        {
            foreach (var structure in structures)
                StructureCountLabels[structure.Key].SetAmount(structure.Value);
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
            UIStructureInfo.SwitchActiveStructure(0);
            UIStructureInfo.UpdateDetails();

            foreach (var structure in StructureInfoData)
                UIStore.AddStructure(structure.Value.Name, structure.Key);
        }

        private static void ShowGameSection(string name)
        {
            HideAllGameSections();

            var section = UIGameSections.ControlSections[name];
            section.Visible = true;
            LabelTitle.Text = section.Name;
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