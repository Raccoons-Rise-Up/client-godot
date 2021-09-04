using Godot;
using Godot.Collections;
using KRU.Networking;

namespace KRU.UI
{
    public class UIMainMenu : Node
    {
        private Dictionary<string, Control> controlSections = new Dictionary<string, Control>();

        public override void _Ready()
        {
            controlSections["Nav"] = GetNode<Control>("Nav");
            controlSections["Login"] = GetNode<Control>("Login");
            controlSections["Options"] = GetNode<Control>("Options");
            controlSections["Credits"] = GetNode<Control>("Credits");

            ShowSection("Nav");
            UILogin.LoadMenuScene();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey eventKey)
                if (eventKey.Pressed && eventKey.Scancode == (int)KeyList.Escape)
                {
                    if (controlSections["Options"].Visible || controlSections["Credits"].Visible)
                        ShowSection("Nav");

                    if (controlSections["Login"].Visible)
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
        private void _on_Btn_Quit_pressed() => GetTree().Quit();

        private void ShowSection(string name)
        {
            HideAllSections();
            GetNode<Control>(name).Visible = true;
        }

        private void HideAllSections()
        {
            foreach (var section in controlSections.Values)
                section.Visible = false;
        }
    }
}

