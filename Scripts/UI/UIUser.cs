using Godot;
using System;

namespace KRU.UI 
{
    public class UIUser : Node
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathUsername;
        [Export] private readonly NodePath nodePathStatus;
#pragma warning restore CS0649 // Values are assigned in the editor

        private Label labelUsername;
        public TextureRect textureRectStatus;

        public void Init()
        {
            labelUsername = GetNode<Label>(nodePathUsername);
            textureRectStatus = GetNode<TextureRect>(nodePathStatus);
            GD.Print(nodePathUsername);
            GD.Print(labelUsername.Name);
        }

        public void SetUsername(string username) => labelUsername.Text = username;

        public void SetStatus(Status status)
        {
            textureRectStatus.Texture = ResourceLoader.Load<Texture>($"res://Sprites/Friend{Enum.GetName(typeof(Status), status)}.png");
        }
    }

    public enum Status 
    {
        Online,
        Away,
        Offline
    }
}

