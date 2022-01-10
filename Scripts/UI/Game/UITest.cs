using Godot;
using System;
using Client.Utilities;

public class UITest : Node
{
#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private readonly NodePath nodePathPanel;
    [Export] private readonly NodePath nodePathMask;
#pragma warning restore CS0649 // Values are assigned in the editor

    private static Control Panel;
    private static Control Mask;

    private bool Drag { get; set; }
    private Vector2 ClickPos = Vector2.Zero;

    public override void _Ready()
    {
        Panel = GetNode<Control>(nodePathPanel);
        Mask = GetNode<Control>(nodePathMask);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Drag)
        {
            Panel.RectGlobalPosition = Utils.Lerp(GetViewport().GetMousePosition() - ClickPos, GetViewport().GetMousePosition() - ClickPos, 25 * delta);
        }
        else 
        {
            var speed = 10;

            // Content too far away from top edge of mask
            if (Panel.RectGlobalPosition.y > Mask.RectGlobalPosition.y)
            {
                var diff = Panel.RectGlobalPosition.y - Mask.RectGlobalPosition.y;
                Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Panel.RectGlobalPosition - new Vector2(0, diff), delta * speed);
            }

            // Content too far away from left edge of mask
            if (Panel.RectGlobalPosition.x > Mask.RectGlobalPosition.x)
            {
                var diff = Panel.RectGlobalPosition.x - Mask.RectGlobalPosition.x;
                Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Panel.RectGlobalPosition - new Vector2(diff, 0), delta * speed);
            }
            
            // Content too far away from bottom edge of mask
            if (Panel.RectGlobalPosition.y + Panel.RectSize.y < Mask.RectGlobalPosition.y + Mask.RectSize.y)
            {
                var diff = Panel.RectGlobalPosition.y - Mask.RectGlobalPosition.y + Panel.RectSize.y - Mask.RectSize.y;
                Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Panel.RectGlobalPosition - new Vector2(0, diff), delta * speed);
            }

            // Content too far away from right edge of mask
            if (Panel.RectGlobalPosition.x + Panel.RectSize.x < Mask.RectGlobalPosition.x + Mask.RectSize.x)
            {
                var diff = Panel.RectGlobalPosition.x - Mask.RectGlobalPosition.x + Panel.RectSize.x - Mask.RectSize.x;
                Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Panel.RectGlobalPosition - new Vector2(diff, 0), delta * speed);
            }
        }
    }

    private void _on_PanelContainer2_gui_input(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            if (Input.IsActionJustPressed("left_click"))
            {
                ClickPos = Panel.GetLocalMousePosition();
                Drag = true;
            }

            if (Input.IsActionJustReleased("left_click"))
            {
                Drag = false;
            }
        }
    }
}
