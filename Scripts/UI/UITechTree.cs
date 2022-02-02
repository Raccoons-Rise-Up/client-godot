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
        private Vector2 ScreenStartPos = Vector2.Zero;
        private Vector2 PrevCameraPos = Vector2.Zero;

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

            GameViewport.ViewportSizeChanged += OnViewportSizeChanged;

            Mask = GetNode<Control>(nodePathMask);
            Content = this;
            SetCameraBounds();

            // Center tech tree panel content
            UITechViewport.Camera2D.Position = Content.RectPosition + Content.RectSize / 2;

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

        public async override void _PhysicsProcess(float delta)
        {
            var viewportMoveSpeed = 2;

            if (Input.IsActionPressed("ui_left"))
                UITechViewport.Camera2D.Position -= new Vector2(viewportMoveSpeed, 0);
            
            if (Input.IsActionPressed("ui_right"))
                UITechViewport.Camera2D.Position += new Vector2(viewportMoveSpeed, 0);

            if (Input.IsActionPressed("ui_up"))
                UITechViewport.Camera2D.Position -= new Vector2(0, viewportMoveSpeed);

            if (Input.IsActionPressed("ui_down"))
                UITechViewport.Camera2D.Position += new Vector2(0, viewportMoveSpeed);

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
                UITechViewport.Camera2D.Position = PrevCameraPos + ScreenStartPos - GetViewport().GetMousePosition();
            }
            else 
            {
                
            }
        }

        private bool ScrollDown, ScrollUp;

        private void _on_Content_gui_input(InputEvent @event)
        {
            if (@event is InputEventMouse eventMouse)
            {
                if (Input.IsActionJustPressed("left_click"))
                {
                    PrevCameraPos = UITechViewport.Camera2D.Position;
                    ScreenStartPos = GetViewport().GetMousePosition();
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

        public void OnViewportSizeChanged(object source, EventArgs e) => SetCameraBounds();

        private void SetCameraBounds()
        {
            var cam = UITechViewport.Camera2D;
            cam.LimitLeft = (int)(Content.RectPosition.x);
            cam.LimitRight = (int)(Content.RectPosition.x + Content.RectSize.x);
            cam.LimitTop = (int)(Content.RectPosition.y);

            // Offset needs to be accounted for when window height is smaller than viewport height
            var windowViewportOffset = Mathf.Min(0, OS.WindowSize.y - GetViewportRect().Size.y - UITechViewport.CanvasLayer.RectGlobalPosition.y);
            cam.LimitBottom = (int)(Content.RectPosition.y + Content.RectSize.y - windowViewportOffset);
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