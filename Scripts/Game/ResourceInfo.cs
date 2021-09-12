using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace KRU.Game
{
    public class ResourceInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TextureRect TextureRectIcon { get; set; }

        public ResourceInfo(string name, string description) 
        {
            Name = name;
            Description = description;
            TextureRectIcon = new TextureRect
            {
                Texture = ResourceLoader.Load<StreamTexture>($"res://Sprites/Icons/{Name.ToLower()}.png")
            };
        }
    }
}
