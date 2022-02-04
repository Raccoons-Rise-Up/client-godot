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

        private bool Drag { get; set; }
        private Vector2 ScreenStartPos = Vector2.Zero;
        private Vector2 PrevCameraPos = Vector2.Zero;

        public async override void _Ready()
        {
            await ToSignal(GetTree(), "idle_frame"); // Wait for UITechViewport.cs to initialize

            //GameViewport.ViewportSizeChanged += OnViewportSizeChanged;

            // Center camera
            UITechViewport.Camera2D.ResetSmoothing(); // otherwise you will see camera move to center
            UITechViewport.Camera2D.Position = RectPosition + RectSize / 2;

            UITechTreeResearch.Init(this);
        }

        public override void _Draw()
        {
            // UITechTree lines between nodes will be drawn here
        }

        public override void _PhysicsProcess(float delta)
        {
            if (ScrollDown)
                ScrollDown = false;

            if (ScrollUp)
                ScrollUp = false;

            var cam = UITechViewport.Camera2D;

            UITechTreeMoveControls.HandleCameraMovementSpeed(cam);
            UITechTreeMoveControls.HandleScrollZoom(cam, this);
            UITechTreeMoveControls.HandleArrowKeys();
            UITechTreeMoveControls.HandleMouseDrag(cam, this, PrevCameraPos, ScreenStartPos, Drag);
            UITechTreeMoveControls.HandleCameraBounds(cam, this);
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
                    Drag = false;

                var scrollSpeed = 0.05f;

                if (Input.IsActionPressed("ui_scroll_down"))
                {
                    // Zooming out
                    UITechTreeMoveControls.ScrollSpeed += new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = false;
                    ScrollDown = true;
                }

                if (Input.IsActionPressed("ui_scroll_up"))
                {
                    // Zooming in
                    UITechTreeMoveControls.ScrollSpeed -= new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = true;
                    ScrollUp = true;
                }
                    
            }
        }

        //public void OnViewportSizeChanged(object source, EventArgs e) => SetCameraBounds();
    }
}