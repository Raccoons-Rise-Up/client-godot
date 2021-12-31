using System.Collections.Generic;

namespace Common.Game
{
    public class Channel
    {
        public string ChannelName { get; set; }
        public List<uint> Users = new List<uint>();
        public List<UIMessage> Messages = new List<UIMessage>();
        public uint CreatorId { get; set; }

#if CLIENT
        public Godot.Button TabButton { get; set; }
#endif
    }

    public struct UIMessage
    {
        public uint UserId { get; set; }
        public string Message { get; set; }
        public bool Special { get; set; }
    }

    public enum SpecialChannel
    {
        Global,
        Game
    }
}