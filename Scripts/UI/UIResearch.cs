using Godot;
using System;

namespace Client.UI 
{
    public partial class UIResearch : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathLabel;
        [Export] private NodePath nodePathLabelPos;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static Vector2 BoxSize = new Vector2(100, 100);

        public void Init(string name)
        {
            GetNode<Label>(nodePathLabel).Text = name;
            GetNode<Label>(nodePathLabelPos).Text = Position.ToString();
        }
    }
}
