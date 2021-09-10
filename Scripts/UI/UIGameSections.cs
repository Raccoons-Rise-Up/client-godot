using Godot;
using Godot.Collections;

namespace KRU.UI
{
    public class UIGameSections : Control
    {
        public static Dictionary<string, Control> ControlSections { get; private set; }

        public override void _Ready()
        {
            ControlSections = new Dictionary<string, Control>();
            ControlSections["Resources"] = GetNode<Control>("Resources");
            ControlSections["Structures"] = GetNode<Control>("Structures");
            ControlSections["Kittens"] = GetNode<Control>("Kittens");
            ControlSections["Store"] = GetNode<Control>("Store");
            ControlSections["Research"] = GetNode<Control>("Research");
            ControlSections["Map"] = GetNode<Control>("Map");
        }
    }
}