using Godot;

namespace KRU.UI
{
    public class UIStructures : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathStructureList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static VBoxContainer StructureList { get; set; }

        public override void _Ready() => StructureList = GetNode<VBoxContainer>(nodePathStructureList);

        public static void ClearLabelCounts()
        {
            foreach (Node label in StructureList.GetChildren())
                label.QueueFree();
        }
    }
}