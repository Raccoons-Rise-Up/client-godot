using Godot;
using System;

namespace Client.UI 
{
    public class UIResearch : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        //[Export] private readonly NodePath nodePathTextureRect;
        [Export] private readonly NodePath nodePathLabel;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static Label Label { get; set; }

        public void Init(string name)
        {
            Label = GetNode<Label>(nodePathLabel);
            Label.Text = name;
        }
    }
}
