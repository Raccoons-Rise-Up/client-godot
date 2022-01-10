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
            //if (Panel.RectGlobalPosition > Mask.RectGlobalPosition)
                //Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Mask.RectGlobalPosition, 25 * delta);

            //if (Panel.RectGlobalPosition + Panel.RectSize - Mask.RectSize < Mask.RectGlobalPosition)
                //Panel.RectGlobalPosition = Utils.Lerp(Panel.RectGlobalPosition, Mask.RectGlobalPosition + Mask.RectSize, 1 * delta);

            // Content too far away from top right corner of mask
            if (Panel.RectGlobalPosition.x + Panel.RectSize.x - Mask.RectSize.x < Mask.RectGlobalPosition.x)
                if (Panel.RectGlobalPosition.y + Panel.RectSize.y - Mask.RectSize.y > Mask.RectGlobalPosition.y) 
                    Panel.RectGlobalPosition = Mask.RectGlobalPosition - new Vector2(Panel.RectSize.x, 0) + new Vector2(Mask.RectSize.x, 0);

            if (Panel.RectGlobalPosition.y + Panel.RectSize.y < Mask.RectGlobalPosition.y + Mask.RectSize.y)
            {
                var diff = Panel.RectGlobalPosition.y - Mask.RectGlobalPosition.y;
                GD.Print(diff);
                Panel.RectGlobalPosition = Panel.RectGlobalPosition - new Vector2(0, diff);
                GD.Print("true");
            } else 
            {
                //GD.Print("false");
            }
                //Panel.RectGlobalPosition = Mask.RectGlobalPosition - Panel.RectSize;
                    
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
