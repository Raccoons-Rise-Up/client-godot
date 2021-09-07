using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
    public class UIResources : Control
    {
        private static PackedScene prefabUIResource = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");

#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathResourceList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static VBoxContainer ResourceList { get; set; }

        public override void _Ready() => ResourceList = GetNode<VBoxContainer>(nodePathResourceList);
        public static void AddResource(ResourceType resourceType, int value = 0) => new UIResource(prefabUIResource.Instance(), resourceType, value);
    }

}
