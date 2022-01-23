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
            { ResearchType.A, new Research {
                Children = new ResearchType[] {
                    ResearchType.B,
                    ResearchType.C,
                    ResearchType.D
                }
            }},
            { ResearchType.B, new Research {}},
            { ResearchType.C, new Research {}},
            { ResearchType.D, new Research {}}
        };

        private static TechTree[] TechTreeData = new TechTree[] {
            new TechTree {
                Type = TechTreeType.Wood,
                StartingResearchNodes = new ResearchType[] {
                    ResearchType.A
                }
            }
        };

        public async override void _Ready()
        {
            await ToSignal(GetTree(), "idle_frame"); // Needed or Mask.RectSize will show up as (0, 0)

            UITechViewport.ViewportSizeChanged += OnViewportSizeChanged;

            Mask = GetNode<Control>(nodePathMask);
            Content = this;

            // Center tech tree panel content
            Content.RectPosition = new Vector2(-Content.RectSize.x / 4, -(Content.RectSize.y - Mask.RectSize.y) / 2);

            // This is where the first research is placed on the tech tree
            ResearchStartPos = new Vector2(Content.RectSize.x / 2 - 50, Content.RectSize.y / 2 - 50);

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
                GD.Print($"{GetViewportRect()}");
            }
        }

        public async override void _PhysicsProcess(float delta)
        {
            await ToSignal(GetTree(), "idle_frame"); // Since _Ready() waits 1 frame, lets wait 1 frame here otherwise weird physics will occur

            if (ScrollDown)
            {
                ScrollDown = false;
            }

            if (ScrollUp)
            {
                ScrollUp = false;
            }


            if (Drag)
            {
                var newPos = GetViewport().GetMousePosition() - DragClickPos;
                Content.RectGlobalPosition = newPos;
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

        private bool ScrollDown, ScrollUp;

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

                if (Input.IsActionPressed("ui_scroll_down"))
                {
                    // Zooming out
                    UITechViewport.Camera2D.Zoom += new Vector2(0.3f, 0.3f);
                    ScrollDown = true;
                }

                if (Input.IsActionPressed("ui_scroll_up"))
                {
                    // Zooming in
                    UITechViewport.Camera2D.Zoom -= new Vector2(0.3f, 0.3f);
                    ScrollUp = true;
                }
            }
        }

        public void OnViewportSizeChanged(object source, EventArgs e) => UITechViewport.Camera2D.Offset = new Vector2(GetViewportRect().Size / 2);
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
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I
    }
}