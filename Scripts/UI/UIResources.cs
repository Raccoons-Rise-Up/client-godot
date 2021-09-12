using Godot;
using KRU.Networking;

namespace KRU.UI
{
    public class UIResources : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathResourceList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer ResourceList { get; set; }

        public override void _Ready() => ResourceList = GetNode<VBoxContainer>(nodePathResourceList);

        public static void AddLabelCount(ushort resourceId, uint value = 0)
        {
            var labelCount = new UILabelCount(ResourceList, UIGame.ResourceInfoData[resourceId].Name, value);
            UIGame.Resources.Add(resourceId, labelCount); // Resources should be received from the server? Not added here?
        }

        public static void ClearLabelCounts()
        {
            foreach (Node label in ResourceList.GetChildren())
                label.QueueFree();
        }
    }
}