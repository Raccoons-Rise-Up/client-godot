using Godot;

namespace KRU.UI
{
    public class UIResources : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathResourceList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static VBoxContainer ResourceList { get; set; }

        public override void _Ready() => ResourceList = GetNode<VBoxContainer>(nodePathResourceList);

        public static void ClearLabelCounts()
        {
            foreach (Node label in ResourceList.GetChildren())
                label.QueueFree();
        }
    }
}