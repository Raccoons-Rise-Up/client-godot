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
        private Dictionary<string, Control> controlSectionsMenu = new Dictionary<string, Control>();

        public override void _Ready()
        {
            // Menu Sections
            controlSectionsMenu["Nav"] = GetNode<Control>(nodePathSectionNav);
            controlSectionsMenu["Options"] = GetNode<Control>(nodePathSectionOptions);

            // Game Menu Parent
            this.Visible = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                {
                    // User is in options
                    if (controlSectionsMenu["Options"].Visible)
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
            UIUsers.RemoveAllUsers();
            ENetClient.ENetCmds.Enqueue(ENetInstructionOpcode.Disconnect);
            ShowMenuSection("Nav");
            this.Visible = false;
            UILogin.LoadMenuScene();
            UIChat.Clear();
        }

        private void ShowMenuSection(string name)
        {
            HideAllMenuSections();
            controlSectionsMenu[name].Visible = true;
        }

        private void HideAllMenuSections()
        {
            foreach (var section in controlSectionsMenu.Values)
                section.Visible = false;
        }
    }
}