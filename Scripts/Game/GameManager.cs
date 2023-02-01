using Godot;
using System;
using System.Threading;
using Client.UI;
using Client.Utilities;
using System.Threading.Tasks;
using Client.Netcode;

namespace Client.Game 
{
    public partial class GameManager : Node
    {
        public static GameClient Client { get; set; }
        public static SceneTree Tree { get; set; }

        public override void _Ready()
        {
            Client = new GameClient();
            AddChild(Client);
            
            UIOptions.Options = FileManager.GetConfig<Options>(FileManager.PathOptions);

            if (UIOptions.Options == null) 
                UIOptions.Options = FileManager.WriteConfig<Options>(FileManager.PathOptions);

            UIOptions.Init();
            Tree = GetTree();
        }

        public override void _Notification(int what)
        {
            // Called when user presses top right window X button or does Alt + F4
            if (what == NotificationWMCloseRequest) 
            {
                ExitCleanup();
            }
        }

        public static void ExitCleanup()
        {
            UIOptions.Save();

            if (ENetClient.ENetThreadRunning)
            {
                Tree.AutoAcceptQuit = false;
                ENetClient.ENetCmds.Enqueue(new ENetCmd { Opcode = ENetOpcode.ClientWantsToExitApp });
                return;
            }

            Tree.Quit();
        }

        public static void ChangeSceneToFile(string scene)
        {
            if (scene == "Main/MainMenu")
                MusicManager.ChangeTrack(MusicTrack.Menu);
            
            if (scene == "Main/Game")
                MusicManager.ChangeTrack(MusicTrack.Game);

            Tree.ChangeSceneToFile($"res://Scenes/{scene}.tscn");
        }
    }
}
