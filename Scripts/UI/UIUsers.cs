using Godot;
using System;
using System.Collections.Generic;

namespace KRU.UI 
{
    public class UIUsers : VBoxContainer
    {
        private static PackedScene PrefabUIUser { get; set; }
        private static VBoxContainer Container { get; set; }

        public override void _Ready()
        {
            Container = this;
        }
    }
}
