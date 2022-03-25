using Godot;
using System;

namespace Client.UI 
{
    public class UIUser : Button
    {
        private static PackedScene PrefabUserDialogOptions = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/UserDialogOptions.tscn");

        public static Control UserDialogOptionsInstance;
        public static bool MouseIsInsideUserDialog;

        public uint id;

        public override void _Ready()
        {
            
        }

        public override void _Input(InputEvent @event)
        {
            var leftclick = @event.IsActionPressed("ui_left_click");
            var rightclick = @event.IsActionPressed("ui_right_click");

            // despawn popup if clicked outside of popup
            if (leftclick || rightclick) 
            {
                if (UserDialogOptionsInstance != null) 
                {
                    if (!UserDialogOptionsInstance.GetRect().HasPoint(GetGlobalMousePosition())) 
                    {
                        UserDialogOptionsInstance.QueueFree();
                        UserDialogOptionsInstance = null;
                    }
                }
            }
                    
        }

        private void _on_Button_gui_input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_right_click"))
            {
                var userDialog = PrefabUserDialogOptions.Instance<UIUserDialogOptions>();
                userDialog.RectPosition = GetGlobalMousePosition();


                // keep track of dialog
                UserDialogOptionsInstance = userDialog;

                UIGame.Instance.AddChild(userDialog);

                // Godot doing some weird resizing so we resize it back
                userDialog.RectSize = new Vector2(200, 50);
            }
        }
    }
}

