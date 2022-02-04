using Godot;
using System;
using System.Collections.Generic;
using Client.Utilities;

namespace Client.UI
{
    public class UITechTree : Control
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        //[Export] private readonly NodePath nodePathMask;
#pragma warning restore CS0649 // Values are assigned in the editor

        private bool Drag { get; set; }
        private Vector2 ScreenStartPos = Vector2.Zero;
        private Vector2 PrevCameraPos = Vector2.Zero;
        private bool Draw;
        private Camera2D Camera;

        public async override void _Ready()
        {
            await ToSignal(GetTree(), "idle_frame"); // Wait for UITechViewport.cs to initialize

            //GameViewport.ViewportSizeChanged += OnViewportSizeChanged;

            Camera = UITechViewport.Camera2D;

            // Center camera
            Camera.ResetSmoothing(); // otherwise you will see camera move to center
            Camera.Position = RectPosition + RectSize / 2;

            UITechTreeResearch.Content = this;
            UITechTreeResearch.Init();

            Draw = true;
            Update();
        }

        public override void _Draw()
        {
            if (!Draw)
                return;

            // UITechTree lines between nodes will be drawn here
            var firstNodeInTechCategory = UITechTreeResearch.TechTreeData[0].StartingResearchNodes[0];
            var firstNode = UITechTreeResearch.ResearchData[firstNodeInTechCategory];

            DrawLinesForChildren(firstNodeInTechCategory);
        }

        private float LineThickness = 5.0f;

        private void DrawLinesForChildren(ResearchType type)
        {
            var researchData = UITechTreeResearch.ResearchData;

            var node = researchData[type];

            var children = node.Children;

            if (children == null)
                return;

            var nodeSize = UITechTreeResearch.ResearchNodeSize;

            // horizontal line from parent
            DrawLine(node.CenterPosition, node.CenterPosition + new Vector2(nodeSize.x, 0));

            for (int i = 0; i < children.Length; i++)
            {
                // horizontal lines
                DrawLine(researchData[children[i]].CenterPosition - new Vector2(nodeSize.x, 0), researchData[children[i]].CenterPosition);

                // vertical lines
                if (i != children.Length - 1)
                    DrawLine(researchData[children[i]].CenterPosition - new Vector2(nodeSize.x, LineThickness / 2), researchData[children[i + 1]].CenterPosition - new Vector2(nodeSize.x, -LineThickness / 2));

                DrawLinesForChildren(children[i]);
            }
        }

        private void DrawLine(Vector2 from, Vector2 to) => DrawLine(from, to, Colors.White, LineThickness, false);

        public async override void _PhysicsProcess(float delta)
        {
            await ToSignal(GetTree(), "idle_frame"); // Wait for Camera to be initialized in _Ready()

            UITechTreeMoveControls.HandleCameraMovementSpeed(Camera);
            UITechTreeMoveControls.HandleScrollZoom(Camera, this);
            UITechTreeMoveControls.HandleArrowKeys();
            UITechTreeMoveControls.HandleMouseDrag(Camera, this, PrevCameraPos, ScreenStartPos, Drag);
            UITechTreeMoveControls.HandleCameraBounds(Camera, this);
        }

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
                    Drag = false;

                var scrollSpeed = 0.05f;

                if (Input.IsActionPressed("ui_scroll_down"))
                {
                    // Zooming out
                    UITechTreeMoveControls.ScrollSpeed += new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = false;
                }

                if (Input.IsActionPressed("ui_scroll_up"))
                {
                    // Zooming in
                    UITechTreeMoveControls.ScrollSpeed -= new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = true;
                }
            }

            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }

        //public void OnViewportSizeChanged(object source, EventArgs e) => SetCameraBounds();
    }
}