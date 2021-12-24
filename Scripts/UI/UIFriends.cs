using Godot;
using System;

namespace KRU.UI 
{
    public class UIFriends : PanelContainer
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer list;

        public override void _Ready()
        {
            list = GetNode<VBoxContainer>(nodePathList);
        }
    }
}