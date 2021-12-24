using Godot;
using System;

namespace KRU.UI 
{
    public class UIUsers : PanelContainer
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer list;

        // Prefab
        private readonly static PackedScene prefabUIUser = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");

        public override void _Ready()
        {
            list = GetNode<VBoxContainer>(nodePathList);
        }

        public static void AddUser()
        {
            var user = (UIUser)prefabUIUser.Instance();
            user.SetUsername("asd");
        }
    }
}
