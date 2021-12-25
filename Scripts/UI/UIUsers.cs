using Godot;
using System;
using System.Collections.Generic;

namespace KRU.UI 
{
    public class UIUsers : VBoxContainer
    {
        private static PackedScene prefabUIUser = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");

        public static Dictionary<uint, UIUser> uiUsers = new Dictionary<uint, UIUser>();

        private static VBoxContainer container;

        public override void _Ready()
        {
            container = this;
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

            container.AddChild(user);
        }

        public static void RemoveUser(uint id)
        {
            uiUsers[id].QueueFree();
            uiUsers.Remove(id);
        }
    }
}
