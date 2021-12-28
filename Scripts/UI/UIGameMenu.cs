using Godot;
using KRU.Networking;
using System.Collections.Generic;

namespace KRU.UI
{
    public class UIGameMenu : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathSectionNav, nodePathSectionOptions;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static Dictionary<string, Control> ControlSectionsMenu { get; set; }

        public override void _Ready()
        {
            // Menu Sections
            ControlSectionsMenu = new Dictionary<string, Control>();
            ControlSectionsMenu["Nav"] = GetNode<Control>(nodePathSectionNav);
            ControlSectionsMenu["Options"] = GetNode<Control>(nodePathSectionOptions);

            // Game Menu Parent
            this.Visible = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                {
                    // User is in options
                    if (ControlSectionsMenu["Options"].Visible)
                    {
                        ShowMenuSection("Nav");
                    }
                    else
                    {
                        // User is in nav
                        if (this.Visible)
                        {
                            this.Visible = false;
                        }
                        else
                        {
                            ShowMenuSection("Nav");
                            this.Visible = true;
                        }
                    }
                }
        }

        private void _on_Btn_Options_pressed()
        {
            ShowMenuSection("Options");
        }

        private void _on_Btn_Disconnect_pressed()
        {
            UIGame.ResetGame();
            ENetClient.ENetCmds.Enqueue(ENetInstructionOpcode.Disconnect);
            ShowMenuSection("Nav");
            this.Visible = false;
        }

        private void ShowMenuSection(string name)
        {
            HideAllMenuSections();
            ControlSectionsMenu[name].Visible = true;
        }

        private void HideAllMenuSections()
        {
            foreach (var section in ControlSectionsMenu.Values)
                section.Visible = false;
        }
    }
}