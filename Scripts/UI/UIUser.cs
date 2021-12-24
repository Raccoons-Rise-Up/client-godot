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

        private static Label labelUsername;
        private static TextureRect textureRectStatus;

        public override void _Ready()
        {
            labelUsername = GetNode<Label>(nodePathUsername);
            textureRectStatus = GetNode<TextureRect>(nodePathStatus);
        }

        public void SetUsername(string username) => labelUsername.Text = username;

        public void SetStatus(Status status)
        {
            textureRectStatus = new TextureRect {
                Texture = ResourceLoader.Load<StreamTexture>($"res://Sprites/Friend{Enum.GetName(typeof(Status), status)}.png")
            };
        }
    }

    public enum Status 
    {
        Online,
        Away,
        Offline
    }
}

