using Godot;
using Godot.Collections;

namespace KRU.UI
{
    public class UIStore : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathStructureList;
        [Export] private readonly NodePath nodePathStructureInfo;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static GridContainer ResourceList { get; set; }

        // Singleton
        public static UIStore uiStore;

        public override void _Ready()
        {
            uiStore = this;
            ResourceList = GetNode<GridContainer>(nodePathStructureList);
        }

        public static void AddStructure(string name, ushort id)
        {
            var button = (Button)UIGame.PrefabButton.Instance();
            button.Text = name;
            button.Connect("pressed", uiStore, nameof(_on_Btn_pressed), new Array { id });

            ResourceList.AddChild(button);
        }

        public static void ClearButtons()
        {
            foreach (Node label in ResourceList.GetChildren())
                ResourceList.RemoveChild(label);
        }

        private void _on_Btn_pressed(ushort id)
        {
            UIStructureInfo.PopulateDetails(id);
        }
    }
}