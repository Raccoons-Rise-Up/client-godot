namespace Common.Game
{
    public class User
    {
        public string Username { get; set; }
        public Status Status { get; set; }
#if CLIENT
        public KRU.UI.UIUser UIUser { get; set; }
        public static Godot.PackedScene PrefabUIUser = Godot.ResourceLoader.Load<Godot.PackedScene>("res://Scenes/UI/Elements/UIUser.tscn");
#endif

        public User(string username, Status status = Status.Online) 
        {
            Username = username;
            Status = status;
        }

#if CLIENT
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