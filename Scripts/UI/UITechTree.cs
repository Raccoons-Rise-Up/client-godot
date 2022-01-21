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
        private Vector2 DragClickPos = Vector2.Zero;
        private static Vector2 ResearchStartPos { get; set; }

        private static PackedScene Research = ResourceLoader.Load<PackedScene>("res://Scenes/Prefabs/Research.tscn");

        private static Dictionary<ResearchType, Research> ResearchData = new Dictionary<ResearchType, Research>(){
            { ResearchType.WoodCutting, new Research {
                Children = new ResearchType[] {
                    ResearchType.SharperAxes,
                    ResearchType.LightWeightAxes
                }
            }},
            { ResearchType.SharperAxes, new Research {}},
            { ResearchType.LightWeightAxes, new Research {}}
        };

        private static TechTree[] TechTreeData = new TechTree[] {
            new TechTree {
                Type = TechTreeType.Wood,
                StartingResearchNodes = new ResearchType[] {
                    ResearchType.WoodCutting
                }
            }
        };

        private void _on_Viewport_Resize()
        {
            GD.Print("Resized");
        }

        public override void _Ready()
        {
            Mask = GetNode<Control>(nodePathMask);
            Content = this;

            // This is where the first research is placed on the tech tree
            ResearchStartPos = new Vector2(100, RectSize.y / 2);

            var firstTechType = TechTreeData[0].StartingResearchNodes[0];
            ResearchData[firstTechType].Position = ResearchStartPos;
            Test(firstTechType);

            for (int i = 0; i < ResearchData[firstTechType].Children.Length; i++)
            {
                var horizontalColumnSpacing = 200;
                var verticalChildrenSpacing = 200;
                ResearchData[ResearchData[firstTechType].Children[i]].Position = ResearchData[firstTechType].Position + new Vector2(horizontalColumnSpacing, verticalChildrenSpacing * i);
                Test(ResearchData[firstTechType].Children[i]);
            }
        }

        public static void Test(ResearchType researchType)
        {
            var researchInstance = Research.Instance<UIResearch>();
            researchInstance.SetPosition(ResearchData[researchType].Position);
            researchInstance.Init(Enum.GetName(typeof(ResearchType), researchType));

            Content.AddChild(researchInstance);
        }

        public override void _Draw()
        {
            DrawLine(new Vector2(370, 510), new Vector2(370 + 150, 510), new Color(1, 1, 1), 10);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("debug"))
            {
                GD.Print($"VRS: {-GetViewportRect().Size}");
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            var offset = -GetViewportRect().Size / 2;

            if (Drag)
            {
                var newPos = offset + GetViewport().GetMousePosition() - DragClickPos;
                Content.RectGlobalPosition = Utils.Lerp(newPos, newPos, 25 * delta);
            }
            else 
            {
                var speed = 10;

                var yOffset = GetViewportRect().Size.y / 2;
                var xOffset = GetViewportRect().Size.x / 2;

                // Content too far away from top edge of mask
                if (Content.RectGlobalPosition.y > Mask.RectGlobalPosition.y - yOffset)
                {
                    var diff = Content.RectGlobalPosition.y - Mask.RectGlobalPosition.y + yOffset;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(0, diff), delta * speed);
                }

                // Content too far away from left edge of mask
                if (Content.RectGlobalPosition.x > Mask.RectGlobalPosition.x - xOffset)
                {
                    var diff = Content.RectGlobalPosition.x - Mask.RectGlobalPosition.x + xOffset;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(diff, 0), delta * speed);
                }
                
                // Content too far away from bottom edge of mask
                if (Content.RectGlobalPosition.y + Content.RectSize.y < Mask.RectGlobalPosition.y + Mask.RectSize.y - yOffset)
                {
                    var diff = Content.RectGlobalPosition.y - Mask.RectGlobalPosition.y + Content.RectSize.y - Mask.RectSize.y + yOffset;
                    Content.RectGlobalPosition = Utils.Lerp(Content.RectGlobalPosition, Content.RectGlobalPosition - new Vector2(0, diff), delta * speed);
                }

                // Content too far away from right edge of mask
                if (Content.RectGlobalPosition.x + Content.RectSize.x < Mask.RectGlobalPosition.x + Mask.RectSize.x - xOffset)
                {
                    var diff = Content.RectGlobalPosition.x - Mask.RectGlobalPosition.x + Content.RectSize.x - Mask.RectSize.x + xOffset;
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
                    DragClickPos = Content.GetLocalMousePosition();
                    Drag = true;
                }

                if (Input.IsActionJustReleased("left_click"))
                {
                    Drag = false;
                }
            }
        }
    }

    public struct TechTree
    {
        public TechTreeType Type { get; set; }
        public ResearchType[] StartingResearchNodes { get; set; }
    }

    public class Research
    {
        public Vector2 Position { get; set; }
        public ResearchType[] Children { get; set; }
    }

    public enum TechTreeType 
    {
        Wood
    }

    public enum ResearchType 
    {
        WoodCutting,
        SharperAxes,
        LightWeightAxes,
        EvenSharperAxes
    }
}