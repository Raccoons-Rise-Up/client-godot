using Godot;

namespace KRU.UI
{
    public class UIMap : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathPanel;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static PanelContainer NodePanel { get; set; }

        public override void _Ready()
        {
            NodePanel = GetNode<PanelContainer>(nodePathPanel);
        }

        public static void HideMap()
        {
            NodePanel.Visible = true;
        }

        public static void ShowMap()
        {
            NodePanel.Visible = false;
        }
    }
}