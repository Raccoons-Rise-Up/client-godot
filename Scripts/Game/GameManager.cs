using Godot;
using System;
using Client.Options;

namespace Client.Game 
{
    public class GameManager : Node
    {
        public static SceneTree Tree { get; set; }

        public override void _Ready()
        {
            Tree = GetTree();
        }

        public static void ChangeScene(string scene)
        {
            if (scene == "Main/MainMenu")
                MusicManager.ChangeTrack(MusicTrack.Menu);
            
            if (scene == "Main/Game")
                MusicManager.ChangeTrack(MusicTrack.Game);

            Tree.ChangeScene($"res://Scenes/{scene}.tscn");
        }
    }
}
