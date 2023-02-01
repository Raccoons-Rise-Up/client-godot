using Godot;
using System;

namespace Client.UI 
{
    public partial class UIGame : Control
    {
        public static Control Instance;

        public override void _Ready() => Instance = this;
    }
}
