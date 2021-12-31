using System.Collections.Generic;

namespace Common.Game
{
    public class User
    {
        public string Username { get; set; }
        public Status Status = Status.Online;
        public List<uint> Channels = new List<uint>();
#if CLIENT
        public KRU.UI.UIUser UIUser { get; set; }
        private static Godot.PackedScene PrefabUIUser = Godot.ResourceLoader.Load<Godot.PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");

        public void CreateUIUser(uint id)
        {
            UIUser = (KRU.UI.UIUser)PrefabUIUser.Instance();
            UIUser.Init();
            UIUser.SetUsername(Username);
            UIUser.SetStatus(Status.Online);
            UIUser.Id = id;
        }
#endif
    }

    public enum Status
    {
        Online,
        Away,
        Offline
    }
}