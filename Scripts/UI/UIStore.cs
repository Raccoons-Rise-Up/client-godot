using Godot;
using Godot.Collections;

namespace KRU.UI
{
    public class UIStore : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathStructureList;
        [Export] private NodePath nodePathStructureInfo;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static GridContainer ResourceList { get; set; }

        // Singleton
        public static UIStore uiStore;

        public override void _Ready()
        {
            uiStore = this;
            ResourceList = GetNode<GridContainer>(nodePathStructureList);
        }

        public static void AddStructure(string name, uint id)
        {
            var button = (Button)UIGame.PrefabButton.Instance();
            button.Text = name;
            button.Connect("pressed", uiStore, nameof(_on_Btn_pressed), new Array { id });

            ResourceList.AddChild(button);
        }

        private void _on_Btn_pressed(uint id)
        {
            var structure = UIGame.StructureData[id];
            UIStructureInfo.PopulateDetails(structure.Id);
        }
    }
}