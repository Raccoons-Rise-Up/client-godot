using Godot;
using System;
using System.Collections.Generic;
using Client.Utilities;

namespace Client.UI
{
    public class UITechTree : Control
    {
    #pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathMask;
    #pragma warning restore CS0649 // Values are assigned in the editor

        private static Control Content;
        private static Control Mask;

        private bool Drag { get; set; }
        private Vector2 ClickPos = Vector2.Zero;

        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        private static Dictionary<ResearchType, Research> Researches = new Dictionary<ResearchType, Research>(){
            { ResearchType.WoodCutting, new Research {
                Inherited = new ResearchType[] {
                    ResearchType.SharperAxes,
                    ResearchType.LightWeightAxes
                }
            }},
            { ResearchType.SharperAxes, new Research {}},
            { ResearchType.LightWeightAxes, new Research {}}
        };

        private static Vector2 Cursor = Vector2.Zero;
        private static ResearchType StartingResearch = ResearchType.WoodCutting;

        public override void _Ready()
        {
            Mask = GetNode<Control>(nodePathMask);
            Content = this;

            Cursor = new Vector2(200, 500);

            
        }

        public override void _Draw()
        {
            DrawLine(new Vector2(370, 510), new Vector2(370 + 150, 510), new Color(1, 1, 1), 10);
        }

        public static void AddResearch(Vector2 startPos, ResearchType researchType)
        {
            var researchInstance = Research.Instance<UIResearch>();
            researchInstance.SetPosition(startPos);
            researchInstance.Init("ASD");

            Content.AddChild(researchInstance);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Drag)
            {
                Content.RectGlobalPosition = Utils.Lerp(GetViewport().GetMousePosition() - ClickPos, GetViewport().GetMousePosition() - ClickPos, 25 * delta);
            }
            else 
            {
                var speed = 10;

                // Content too far away from top edge of mask
                if (Content.RectGlobalPosition.y > Mask.RectGlobalPosition.y)
                {
                    var diff = Content.RectGlobalPosition.y - Mask.RectGlobalPosition.y;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(0, diff), delta * speed);
                }

                // Content too far away from left edge of mask
                if (Content.RectGlobalPosition.x > Mask.RectGlobalPosition.x)
                {
                    var diff = Content.RectGlobalPosition.x - Mask.RectGlobalPosition.x;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(diff, 0), delta * speed);
                }
                
                // Content too far away from bottom edge of mask
                if (Content.RectGlobalPosition.y + Content.RectSize.y < Mask.RectGlobalPosition.y + Mask.RectSize.y)
                {
                    var diff = Content.RectGlobalPosition.y - Mask.RectGlobalPosition.y + Content.RectSize.y - Mask.RectSize.y;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(0, diff), delta * speed);
                }

                // Content too far away from right edge of mask
                if (Content.RectGlobalPosition.x + Content.RectSize.x < Mask.RectGlobalPosition.x + Mask.RectSize.x)
                {
                    var diff = Content.RectGlobalPosition.x - Mask.RectGlobalPosition.x + Content.RectSize.x - Mask.RectSize.x;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(diff, 0), delta * speed);
                }
            }
        }

        private void _on_Content_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouse eventMouse)
            {
                if (Input.IsActionJustPressed("left_click"))
                {
                    ClickPos = Content.GetLocalMousePosition();
                    Drag = true;
                }

                if (Input.IsActionJustReleased("left_click"))
                {
                    Drag = false;
                }
            }
        }
    }

    public struct Research 
    {
        public ResearchType[] Inherited { get; set; }
    }

    public enum ResearchType 
    {
        WoodCutting,
        SharperAxes,
        LightWeightAxes
    }
}