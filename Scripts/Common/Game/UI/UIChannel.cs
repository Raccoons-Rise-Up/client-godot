using System.Collections.Generic;

namespace Common.Game
{
    public class UIChannel
    {
        public string Name { get; set; }
        public List<uint> Users = new List<uint>();
        public uint Creator { get; set; }
        public string Content = "";

        public void AddUser(uint id)
        {
            Users.Add(id);
        }
    }
}