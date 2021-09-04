using Godot;
using System;

namespace KRU.UI
{
    public class UITerminal : Control
    {
        // Terminal Label Prefab
        private static PackedScene labelTerminalPrefab = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/TerminalLabel.tscn");

        // Terminal
        [Export] private NodePath nodePathTerminal;
        private static VBoxContainer terminal;

        public override void _Ready()
        {
            // Terminal
            terminal = GetNode<VBoxContainer>(nodePathTerminal);
        }

        public static void Log(string message)
        {
            var label = (Label)labelTerminalPrefab.Instance();
            label.Text = message;
            terminal.AddChild(label);
        }
    }
}
