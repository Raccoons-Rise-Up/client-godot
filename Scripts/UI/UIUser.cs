using Godot;
using System;

namespace KRU.UI 
{
    public class UIUser : Panel
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathUsername;
        [Export] private readonly NodePath nodePathStatus;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static PackedScene prefabUIUserDialogOptions = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UIUserDialogOptions.tscn");
        public static UIUserDialogOptions activeDialog;

        private Label userUsername;
        private uint userId;
        public TextureRect textureRectStatus;

        public override void _GuiInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton && @event.IsPressed() && @event.IsActionPressed("ui_right_click"))
            {
                if (ActiveDialogExists())
                    return;
                
                activeDialog = prefabUIUserDialogOptions.Instance<UIUserDialogOptions>();
                activeDialog.SetPosition(UIGame.dialogPopups.GetLocalMousePosition());
                activeDialog.username = userUsername.Text;
                activeDialog.id = userId;

                UIGame.dialogPopups.AddChild(activeDialog);
            } 
        }

        public static bool ActiveDialogExists() => activeDialog != null;

        public static void RemoveActiveDialog()
        {
            if (activeDialog == null)
                return;

            activeDialog.QueueFree();
            activeDialog = null;
        }

        public void Init()
        {
            userUsername = GetNode<Label>(nodePathUsername);
            textureRectStatus = GetNode<TextureRect>(nodePathStatus);
        }

        public void SetUsername(string username) => userUsername.Text = username;

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

