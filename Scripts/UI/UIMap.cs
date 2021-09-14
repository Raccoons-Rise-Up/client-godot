using Godot;

namespace KRU.UI
{
    public class UIMap : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathPanel;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static PanelContainer nodePanel;

        public override void _Ready()
        {
            nodePanel = GetNode<PanelContainer>(nodePathPanel);
        }

        public static void HideMap()
        {
            nodePanel.Visible = true;
        }

        public static void ShowMap()
        {
            nodePanel.Visible = false;
        }
    }
}