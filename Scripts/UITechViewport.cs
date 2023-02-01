using Godot;
using System;

namespace Client.UI 
{
    public partial class UITechViewport : SubViewport
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private NodePath nodePathCamera2D;
        [Export] private NodePath nodePathCanvasLayer;
#pragma warning restore CS0649 // Values are assigned in the editor

        // Node Paths
        public static Camera2D Camera2D;
        public static Control CanvasLayer;
        public static SubViewport Instance;

        private static Control ViewportContent;
        private static bool Drag { get; set; }
        private static Vector2 ScreenStartPos = Vector2.Zero;
        private static Vector2 PrevCameraPos = Vector2.Zero;
        public static Vector2 ScrollSpeed = Vector2.Zero;
        public static bool ScrollingUp;

        public async override void _Ready()
        {
            // Set clear color for this viewport
            //SetDefaultClearColor(new Color("#323232"));
            
            ViewportContent = UITechTree.Instance;
            Instance = this;
            Camera2D = GetNode<Camera2D>(nodePathCamera2D);
            CanvasLayer = GetNode<Control>(nodePathCanvasLayer);

            // Center camera
            Camera2D.Position = ViewportContent.Position + new Vector2(500, ViewportContent.Size.Y / 2);

            // This is a bug in Godot where 'HandleInputLocally' is set to false even if it is set to true (solved by waiting 1 frame then setting to true)
            await ToSignal(GetTree(), "idle_frame");
            HandleInputLocally = true;

            // Wait 1 frame otherwise you will see camera move to start position
            Camera2D.PositionSmoothingEnabled = true;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (UITabs.CurrentTab != Tab.Research)
                return;
            
            HandleCameraMovementSpeed();
            HandleScrollZoom();

            if (!UIChannel.ChatInputFocused)
                HandleArrowKeys();
            
            HandleMouseDrag();
            HandleCameraBounds();
        }

        public override void _Input(InputEvent @event)
        {
            if (UITabs.CurrentTab != Tab.Research)
                return;

            if (@event is InputEventMouse eventMouse)
            {
                if (Godot.Input.IsActionJustPressed("ui_left_click"))
                {
                    PrevCameraPos = UITechViewport.Camera2D.Position;
                    ScreenStartPos = GetViewport().GetMousePosition();
                    Drag = true;
                }

                if (Godot.Input.IsActionJustReleased("ui_left_click"))
                    Drag = false;

                var scrollSpeed = 0.05f;

                if (Godot.Input.IsActionPressed("ui_scroll_down"))
                {
                    // Zooming out
                    ScrollSpeed += new Vector2(scrollSpeed, scrollSpeed);
                    ScrollingUp = false;
                }

                if (Godot.Input.IsActionPressed("ui_scroll_up"))
                {
                    // Zooming in
                    ScrollSpeed -= new Vector2(scrollSpeed, scrollSpeed);
                    ScrollingUp = true;
                }
            }

            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }

        // Disable input if mouse not inside viewport
        private void _on_ViewportContainer_mouse_entered() => GuiDisableInput = false;
        private void _on_ViewportContainer_mouse_exited() => GuiDisableInput = true;

        public static void HandleCameraMovementSpeed()
        {
            var baseSpeed = 5.0f;
            Camera2D.PositionSmoothingSpeed = baseSpeed * Camera2D.Zoom.Length();
        }

        public static void HandleMouseDrag()
        {
            if (Drag)
                Camera2D.Position = PrevCameraPos + ScreenStartPos - ViewportContent.GetViewport().GetMousePosition();
        }

        public static void HandleScrollZoom()
        {
            var minZoom = new Vector2(1f, 1f);

            Camera2D.Zoom = Utilities.Utils.Lerp(Camera2D.Zoom, Camera2D.Zoom + ScrollSpeed, 0.3f);

            if (Camera2D.Zoom < minZoom) 
            {
                ScrollSpeed = Vector2.Zero;
                Camera2D.Zoom = minZoom;
            }

            var viewportSize = ViewportContent.GetViewportRect().Size * Camera2D.Zoom;
            var contentSize = ViewportContent.Size;

            // This calculates the absolute maximum zoom, the x and y values will most likely not be the same value
            var maxZoom = (contentSize) / (viewportSize / Camera2D.Zoom);

            if (Camera2D.Zoom > maxZoom) 
            {
                Camera2D.Zoom = new Vector2(maxZoom.X, maxZoom.X); // if the x and y are different then the content will get distorted
                ScrollSpeed = Vector2.Zero;
            }

            var deaccelerationSpeed = new Vector2(0.01f, 0.01f);

            if (ScrollSpeed > Vector2.Zero && !ScrollingUp) 
            {
                ScrollSpeed -= deaccelerationSpeed;
            }
            else if (ScrollSpeed < Vector2.Zero && ScrollingUp) 
            {
                ScrollSpeed += deaccelerationSpeed;
                
                // Move camera to mouse when zooming in
                Camera2D.Position += Camera2D.GetLocalMousePosition() * 0.1f;
            }
            else
                ScrollSpeed = Vector2.Zero;
        }

        public static void HandleCameraBounds()
        {
            var viewportSize = ViewportContent.GetViewportRect().Size;
            var rectPos = ViewportContent.Position;
            var rectSize = ViewportContent.Size;

            var windowOffsetY = Mathf.Min(0, (DisplayServer.WindowGetSize().Y - viewportSize.Y - UITechViewport.CanvasLayer.GlobalPosition.Y) * Camera2D.Zoom.Y);
            
            var boundsLeft = rectPos.X + (viewportSize.X * Camera2D.Zoom.X) / 2;
            var boundsRight = rectPos.X + rectSize.X - (viewportSize.X * Camera2D.Zoom.X) / 2;
            var boundsTop = rectPos.Y + (viewportSize.Y * Camera2D.Zoom.Y) / 2;
            var boundsBottom = rectPos.Y + rectSize.Y - (viewportSize.Y * Camera2D.Zoom.Y) / 2 - windowOffsetY;

            if (Camera2D.Position.X < boundsLeft) 
                Camera2D.Position = new Vector2(boundsLeft, Camera2D.Position.Y);
            if (Camera2D.Position.X > boundsRight)
                Camera2D.Position = new Vector2(boundsRight, Camera2D.Position.Y);
            if (Camera2D.Position.Y < boundsTop)
                Camera2D.Position = new Vector2(Camera2D.Position.X, boundsTop);
            if (Camera2D.Position.Y > boundsBottom)
                Camera2D.Position = new Vector2(Camera2D.Position.X, boundsBottom);
        }

        public static void HandleArrowKeys()
        {
            var arrowSpeed = 10.0f;

            if (Godot.Input.IsActionPressed("ui_left"))
                UITechViewport.Camera2D.Position -= new Vector2(arrowSpeed, 0);
            
            if (Godot.Input.IsActionPressed("ui_right"))
                UITechViewport.Camera2D.Position += new Vector2(arrowSpeed, 0);

            if (Godot.Input.IsActionPressed("ui_up"))
                UITechViewport.Camera2D.Position -= new Vector2(0, arrowSpeed);

            if (Godot.Input.IsActionPressed("ui_down"))
                UITechViewport.Camera2D.Position += new Vector2(0, arrowSpeed);
        }
    }
}
