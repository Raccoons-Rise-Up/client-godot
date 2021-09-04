using Godot;
using Godot.Collections;

namespace KRU.UI
{
    public class UIGame : Node
    {
        // Title
        [Export] private NodePath nodePathTitle;
        private Label labelTitle;

        // Game Sections
        [Export] private Dictionary<string, NodePath> nodePaths;
        private Dictionary<string, Control> controlSectionsGame = new Dictionary<string, Control>();

        public override void _Ready()
        {
            UITerminal.Log("Testing");

            // Title
            labelTitle = GetNode<Label>(nodePathTitle);

            // Game Sections
            foreach (var nodePath in nodePaths)
                controlSectionsGame[nodePath.Key] = GetNode<Control>(nodePath.Value);

            // Setup
            ShowGameSection("Resources");
        }

        private void _on_Btn_Resources_pressed() => ShowGameSection("Resources");
        private void _on_Btn_Structures_pressed() => ShowGameSection("Structures");
        private void _on_Btn_Kittens_pressed() => ShowGameSection("Kittens");
        private void _on_Btn_Store_pressed() => ShowGameSection("Store");
        private void _on_Btn_Research_pressed() => ShowGameSection("Research");
        private void _on_Btn_Map_pressed() => ShowGameSection("Map");

        private void ShowGameSection(string name)
        {
            HideAllGameSections();

            var section = controlSectionsGame[name];
            section.Visible = true;
            labelTitle.Text = section.Name;
        }

        private void HideAllGameSections()
        {
            foreach (var section in controlSectionsGame.Values)
                section.Visible = false;
        }
    }
}
