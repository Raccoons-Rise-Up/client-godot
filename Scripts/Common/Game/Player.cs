using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Game
{
    public partial class Player
    {
        public List<uint> Channels { get; set; }
        public string Username { get; set; }
        public string Ip { get; set; }

        public override string ToString() => Username;
    }

    public enum Status
    {
        Online,
        Away,
        Offline
    }
}
