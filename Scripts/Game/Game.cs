using Godot;
using System;
using System.Collections.Generic;
using Common.Game;

namespace Client.Game
{
    public class Game : Node
    {
        public static Dictionary<uint, Player> Players = new Dictionary<uint, Player>();

        public override void _Ready()
        {
            
        }
    }
}
