using Godot;
using KRU.Networking;

namespace KRU.UI 
{
    public class UILabelCount
    {
        public static PackedScene PrefabLabelCount = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");
        private Node Node { get; set; }
        private Label LabelName { get; set; }
        private Label LabelValue { get; set; }

        public UILabelCount(VBoxContainer resourceList, string resourceType, int value)
        {
            this.Node = PrefabLabelCount.Instance();

            LabelName = Node.GetNode<Label>("Name");
            LabelName.Text = resourceType;

            LabelValue = Node.GetNode<Label>("Value");
            LabelValue.Text = "" + value;

            resourceList.AddChild(Node);
        }

        public void Reset() => LabelValue.Text = "";
        public void Set(uint amount) => LabelValue.Text = "" + amount;
        public void Add(uint amount) => LabelValue.Text = "" + uint.Parse(LabelValue.Text) + amount;
        public uint Get() => uint.Parse(LabelValue.Text);
    }
}