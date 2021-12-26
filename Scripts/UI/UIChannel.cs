using System.Collections.Generic;

namespace KRU.UI 
{
    public class UIChannel 
    {
        public string Name { get; set; }
        public List<uint> OtherUserIds { get; set; }
        public string Content { get; set; }

        public UIChannel(string name)
        {
            Name = name;
            Content = $"A new channel with {name} was created";
            OtherUserIds = new List<uint>();
        }

        public void AddUser(uint id)
        {
            OtherUserIds.Add(id);
        }
    }
}