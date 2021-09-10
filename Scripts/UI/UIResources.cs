using Godot;
using KRU.Networking;

namespace KRU.UI
{
    public class UIResources : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathResourceList;
#pragma warning restore CS0649 // Values are assigned in the editor

        private static VBoxContainer ResourceList { get; set; }

        public override void _Ready() => ResourceList = GetNode<VBoxContainer>(nodePathResourceList);

        public static void AddLabelCount(ResourceType resourceType, int value = 0)
        {
            var labelCount = new UILabelCount(ResourceList, resourceType.ToString(), value);
            UIGame.Resources.Add(resourceType, labelCount); // Resources should be received from the server? Not added here?
        }
    }
}