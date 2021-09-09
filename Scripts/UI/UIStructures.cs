using Godot;
using System;
using KRU.Networking;

namespace KRU.UI
{
    public class UIStructures : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathStructureList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static VBoxContainer ResourceList { get; set; }

        public override void _Ready() => ResourceList = GetNode<VBoxContainer>(nodePathStructureList);
        public static void AddLabelCount(string name, int value = 0) => new UILabelCount(ResourceList, name, value);
    }
}

