using Godot;
using System;
using System.Collections.Generic;

namespace KRU.UI 
{
    public class UIUsers : VBoxContainer
    {
        private static PackedScene PrefabUIUser { get; set; }

        public static Dictionary<uint, UIUser> Users { get; set; }

        private static VBoxContainer Container { get; set; }

        public override void _Ready()
        {
            PrefabUIUser = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");
            Users = new Dictionary<uint, UIUser>();
            Container = this;
        }

        public static void RemoveAllUsers()
        {
            foreach (var user in Users) 
                user.Value.QueueFree();

            Users.Clear();
        }

        public static void AddUser(string name, Status status, uint id)
        {
            var user = (UIUser)PrefabUIUser.Instance();
            user.Init();
            user.SetUsername(name);
            user.SetStatus(status);
            user.SetId(id);

            Users.Add(id, user);

            Container.CallDeferred("add_child", user); // Deputy Valk here with Thread Safety ;-; (normally you would do Container.AddChild but since this is being called on a non-Godot thread we have to use this new function)
        }

        public static void RemoveUser(uint id)
        {
            Users[id].QueueFree();
            Users.Remove(id);
        }
    }
}
