using Godot;
using System;
using Common.Game;
using Common.Utils;

namespace KRU.UI
{
    public class UILabelResourceCount
    {
        public static PackedScene PrefabLabelCount = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");
        private Node Node { get; set; }
        private Label LabelName { get; set; }
        private Label LabelValue { get; set; }
        private ResourceType ResourceType { get; set; }

        public UILabelResourceCount(Node list, ResourceType resourceType, uint amount)
        {
            Node = PrefabLabelCount.Instance();

            ResourceType = resourceType;

            LabelName = Node.GetNode<Label>("Name");
            LabelName.Text = SharedUtils.AddSpaceBeforeEachCapital(Enum.GetName(typeof(ResourceType), resourceType));

            LabelValue = Node.GetNode<Label>("Value");
            LabelValue.Text = "" + amount;

            list.AddChild(Node);
        }

        public void ResetAmount() 
        {
            LabelValue.Text = "" + 0;
        }

        public void SetAmount(double amount)
        {
            LabelValue.Text = "" + amount;
        }

        public void AddAmount(double amount)
        {
            LabelValue.Text = "" + (uint)(UIGame.ResourceCounts[ResourceType] + amount);
        }
    }
}