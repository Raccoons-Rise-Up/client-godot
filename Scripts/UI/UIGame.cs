using Godot;
using System;

namespace KRU.UI
{
    public class UIGame : Node
    {
        [Export] public NodePath NodePathGameMenu { get; set; }

        private Control controlGameMenu;

        public override void _Ready()
        {
            controlGameMenu = GetNode<Control>(NodePathGameMenu);
            controlGameMenu.Visible = Global.GameMenuOptionsActive;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                {
                    if (Global.GameMenuOptionsActive)
                    {
                        Global.GameMenuOptionsActive = false;
                        controlGameMenu.Visible = false;
                    }
                    else
                    {
                        Global.GameMenuOptionsActive = true;
                        controlGameMenu.Visible = true;
                    }
                }
        }
    }
}
