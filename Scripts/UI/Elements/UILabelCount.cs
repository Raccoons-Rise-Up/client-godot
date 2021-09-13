using Godot;

namespace KRU.UI
{
    public class UILabelCount
    {
        public static PackedScene PrefabLabelCount = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");
        private Node Node { get; set; }
        private Label LabelName { get; set; }
        private Label LabelValue { get; set; }
        private uint Amount { get; set; }

        public UILabelCount(Node list, string name, uint amount)
        {
            Node = PrefabLabelCount.Instance();

            LabelName = Node.GetNode<Label>("Name");
            LabelName.Text = name;

            Amount = amount;

            LabelValue = Node.GetNode<Label>("Value");
            LabelValue.Text = "" + amount;

            list.AddChild(Node);
        }

        public void ResetAmount() 
        {
            Amount = 0;
            LabelValue.Text = "" + 0;
        }

        public void SetAmount(uint amount)
        {
            Amount = amount;
            LabelValue.Text = "" + amount;
        }

        public void AddAmount(uint amount)
        {
            Amount += amount;
            LabelValue.Text = "" + Amount;
        }

        public uint GetAmount() => Amount;
    }
}