using Godot;
using KRU.Networking;

namespace KRU.UI 
{
    public class UIResource
    {
        private Node Node { get; set; }
        private Label LabelName { get; set; }
        private Label LabelValue { get; set; }

        public UIResource(Node node, ResourceType resourceType, int value)
        {
            this.Node = node;

            LabelName = Node.GetNode<Label>("Name");
            LabelName.Text = resourceType.ToString();

            LabelValue = Node.GetNode<Label>("Value");
            LabelValue.Text = "" + value;

            UIResources.ResourceList.AddChild(node);
            UIGame.Resources.Add(resourceType, this);
        }

        public void Reset() => LabelValue.Text = "";
        public void Set(uint amount) => LabelValue.Text = "" + amount;
        public void Add(uint amount) => LabelValue.Text = "" + uint.Parse(LabelValue.Text) + amount;
        public uint Get() => uint.Parse(LabelValue.Text);
    }
}