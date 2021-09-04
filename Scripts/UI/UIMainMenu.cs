using Godot;
using Godot.Collections;
using KRU.Networking;

namespace KRU.UI
{
    public class UIMainMenu : Node
    {
        [Export] private Dictionary<string, NodePath> nodePaths;
        private Dictionary<string, Control> controlSections = new Dictionary<string, Control>();

        public override void _Ready()
        {
            foreach (var nodePath in nodePaths)
                controlSections[nodePath.Key] = GetNode<Control>(nodePath.Value);

            ShowSection("Nav");
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
            controlSections[name].Visible = true;
        }

        private void HideAllSections()
        {
            foreach (var section in controlSections.Values)
                section.Visible = false;
        }
    }
}

