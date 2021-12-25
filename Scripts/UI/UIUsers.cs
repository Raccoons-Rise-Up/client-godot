using Godot;
using System;
using System.Collections.Generic;

namespace KRU.UI 
{
    public class UIUsers : PanelContainer
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer list;

        // Prefab
        private static PackedScene prefabUIUser = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");

        public static Dictionary<uint, UIUser> uiUsers = new Dictionary<uint, UIUser>();

        public override void _Ready()
        {
            list = GetNode<VBoxContainer>(nodePathList);
        }

        public static void RemoveAllUsers()
        {
            foreach (var user in uiUsers) 
                user.Value.QueueFree();

            uiUsers.Clear();
        }

        public static void AddUser(string name, Status status, uint id)
        {
            var user = (UIUser)prefabUIUser.Instance();
            user.Init();
            user.SetUsername(name);
            user.SetStatus(status);

            uiUsers.Add(id, user);

            list.AddChild(user);
        }

        public static void RemoveUser(uint id)
        {
            uiUsers[id].QueueFree();
            uiUsers.Remove(id);
        }
    }
}
