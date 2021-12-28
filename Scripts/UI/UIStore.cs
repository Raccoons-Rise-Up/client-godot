using Godot;
using Godot.Collections;
using Common.Game;

namespace KRU.UI
{
    public class UIStore : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathStructureList;
#pragma warning restore CS0649 // Values are assigned in the editor

        public static GridContainer ResourceList { get; set; }
        public static UIStore Instance { get; set; }

        public override void _Ready()
        {
            Instance = this;
            ResourceList = GetNode<GridContainer>(nodePathStructureList);
        }

        public static void AddStructure(string name, StructureType id)
        {
            var button = (Button)UIGame.PrefabButton.Instance();
            button.Text = name;
            button.Connect("pressed", Instance, nameof(_on_Btn_pressed), new Array { id });

            ResourceList.AddChild(button);
        }

        public static void ClearButtons()
        {
            foreach (Node label in ResourceList.GetChildren())
                label.QueueFree();
        }

        private void _on_Btn_pressed(StructureType id)
        {
            UIStructureInfo.SwitchActiveStructure(id);
            UIStructureInfo.UpdateDetails();
        }
    }
}