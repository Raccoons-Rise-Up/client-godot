using Godot;
using System;
using Common.Game;

namespace KRU.UI 
{
    public class UIUser : Panel
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathUsername;
        [Export] private readonly NodePath nodePathStatus;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static PackedScene PrefabUIUserDialogOptions { get; set; }
        public static UIUserDialogOptions ActiveDialog { get; set; }

        public uint Id { get; set; }
        private Label LabelUsername { get; set; }
        public TextureRect TextureRectStatus { get; set; }

        public override void _Ready()
        {
            PrefabUIUserDialogOptions = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUserDialogOptions.tscn");
        }

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("ui_right_click"))
            {
                if (ActiveDialogExists())
                    return;
                
                // Create the popup dialog on right click UIUser
                ActiveDialog = PrefabUIUserDialogOptions.Instance<UIUserDialogOptions>();
                ActiveDialog.SetPosition(UIGame.DialogPopups.GetLocalMousePosition());
                ActiveDialog.Username = LabelUsername.Text;
                ActiveDialog.UserId = Id;

                UIGame.DialogPopups.AddChild(ActiveDialog);
            } 
        }

        public static bool ActiveDialogExists() => ActiveDialog != null;

        public static void RemoveActiveDialog()
        {
            if (ActiveDialog == null)
                return;

            ActiveDialog.QueueFree();
            ActiveDialog = null;
        }

        public void Init()
        {
            LabelUsername = GetNode<Label>(nodePathUsername);
            TextureRectStatus = GetNode<TextureRect>(nodePathStatus);
        }

        public void SetUsername(string username) => LabelUsername.Text = username;

        public void SetStatus(Status status)
        {
            TextureRectStatus.Texture = ResourceLoader.Load<Texture>($"res://Sprites/Friend{Enum.GetName(typeof(Status), status)}.png");
        }
    }
}

