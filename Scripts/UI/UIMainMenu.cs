using Godot;
using Godot.Collections;
using KRU.Networking;

namespace KRU.UI
{
    public class UIMainMenu : Node
    {
        private Dictionary<string, Control> ControlSections { get; set; }

        public override void _Ready()
        {
            ControlSections = new Dictionary<string, Control>();
            ControlSections["Nav"] = GetNode<Control>("Nav");
            ControlSections["Login"] = GetNode<Control>("Login");
            ControlSections["Options"] = GetNode<Control>("Options");
            ControlSections["Credits"] = GetNode<Control>("Credits");

            ShowSection("Nav");
            UILogin.LoadMenuScene();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                {
                    if (ControlSections["Options"].Visible || ControlSections["Credits"].Visible)
                        ShowSection("Nav");

                    if (ControlSections["Login"].Visible)
                    {
                        // TODO: Cancel connection if connecting
                        ShowSection("Nav");
                    }
                }
        }

        private void _on_Btn_Multiplayer_pressed()
        {
            UILogin.InitLoginSection();
            ShowSection("Login");
        }

        private void _on_Btn_Options_pressed() => ShowSection("Options");

        private void _on_Btn_Credits_pressed() => ShowSection("Credits");

        private void _on_Btn_Quit_pressed() => ENetClient.ExitApplication();

        private void ShowSection(string name)
        {
            HideAllSections();
            GetNode<Control>(name).Visible = true;
        }

        private void HideAllSections()
        {
            foreach (var section in ControlSections.Values)
                section.Visible = false;
        }
    }
}