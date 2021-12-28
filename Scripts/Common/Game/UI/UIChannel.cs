using System.Collections.Generic;

namespace Common.Game
{
    public class UIChannel
    {
        public string ChannelName { get; set; }
        public Dictionary<uint, string> Users = new Dictionary<uint, string>();
        public string Content = "";
        public uint CreatorId { get; set; }

#if CLIENT
        public Godot.Button Button { get; set; }
#endif

        public void AddUser(uint id, string username)
        {
            Users.Add(id, username);
        }
    }

    public enum SpecialChannel 
    {
        Global,
        Game
    }
}