using Godot;
using System;
using Common.Game;

namespace KRU.UI
{
    public class UILabelStructureCount
    {
        public static PackedScene PrefabLabelCount = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");
        private Node Node { get; set; }
        private Label LabelName { get; set; }
        private Label LabelValue { get; set; }
        private StructureType StructureType { get; set; }

        public UILabelStructureCount(Node list, StructureType structureType, uint amount)
        {
            Node = PrefabLabelCount.Instance();

            StructureType = structureType;

            LabelName = Node.GetNode<Label>("Name");
            LabelName.Text = Enum.GetName(typeof(StructureType), structureType);

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
            LabelValue.Text = "" + (UIGame.StructureCounts[StructureType] + amount);
        }
    }
}