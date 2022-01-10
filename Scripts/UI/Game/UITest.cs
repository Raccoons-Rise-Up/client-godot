using Godot;
using System;

public class UITest : Node
{
#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private readonly NodePath nodePathPanel;
#pragma warning restore CS0649 // Values are assigned in the editor

    private static Control Panel;

    private bool Drag { get; set; }

    public override void _Ready()
    {
        Panel = GetNode<Control>(nodePathPanel);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Drag)
        {
            Panel.RectGlobalPosition = GetViewport().GetMousePosition();
        }
    }
    
    Vector2 Lerp(Vector2 firstVector, Vector2 secondVector, float by)
    {
        float retX = Mathf.Lerp(firstVector.x, secondVector.x, by);
        float retY = Mathf.Lerp(firstVector.y, secondVector.y, by);
        return new Vector2(retX, retY);
    }

    private void _on_PanelContainer2_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            if (Input.IsActionJustPressed("left_click"))
            {
                Drag = true;
            }

            if (Input.IsActionJustReleased("left_click"))
            {
                Drag = false;
            }
        }
    }
}
