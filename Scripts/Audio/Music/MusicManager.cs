using Godot;
using System;
using Client.Utilities;

namespace Client.Options 
{
    public class MusicManager : AudioStreamPlayer
    {
        private static AudioStream MusicMenu = ResourceLoader.Load<AudioStream>("res://Audio/Music/Menu/Unsolicited trailer music loop edit.wav");
        private static AudioStream MusicGame = ResourceLoader.Load<AudioStream>("res://Audio/Music/Game/prepare_your_swords.ogg");
        private static AudioStreamPlayer Instance { get; set; }

        public override void _Ready()
        {
            Instance = this;
            Stream = MusicMenu;
            Playing = true;
        }

        public static void ChangeTrack(MusicTrack track) 
        {
            switch (track)
            {
                case MusicTrack.Menu:
                    PlayTrack(MusicMenu);
                    break;
                case MusicTrack.Game:
                    PlayTrack(MusicGame);
                    break;
            }
        }

        private static void PlayTrack(AudioStream stream)
        {
            if (Instance.Stream != stream) 
            {
                Instance.Stream = stream;
                Instance.Playing = true;
            }
        }

        public static void SetVolume(float volume)
        {
            Instance.VolumeDb = volume;

            if (volume <= -40)
                Instance.VolumeDb = -80; // can't go lower than this (this essentially mutes the track)
        }
    }

    public enum MusicTrack 
    {
        Menu,
        Game
    }
}

