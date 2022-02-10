using Godot;

namespace Client.UI 
{
    public class UITechViewport : Viewport
    {
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathCamera2D;
        [Export] private readonly NodePath nodePathCanvasLayer;
#pragma warning restore CS0649 // Values are assigned in the editor

        // Node Paths
        public static Camera2D Camera2D;
        public static Control CanvasLayer;
        public static Viewport Instance;

        private static Control ViewportContent;
        private bool Drag { get; set; }
        private Vector2 ScreenStartPos = Vector2.Zero;
        private Vector2 PrevCameraPos = Vector2.Zero;

        public async override void _Ready()
        {
            ViewportContent = UITechTree.Instance;
            Instance = this;
            Camera2D = GetNode<Camera2D>(nodePathCamera2D);
            CanvasLayer = GetNode<Control>(nodePathCanvasLayer);

            // Center camera
            Camera2D.ResetSmoothing(); // otherwise you will see camera move to center
            Camera2D.Position = ViewportContent.RectPosition + ViewportContent.RectSize / 2;

            // This is a bug in Godot where 'HandleInputLocally' is set to false even if it is set to true (solved by waiting 1 frame then setting to true)
            await ToSignal(GetTree(), "idle_frame");
            HandleInputLocally = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            UITechTreeMoveControls.HandleCameraMovementSpeed(Camera2D);
            UITechTreeMoveControls.HandleScrollZoom(Camera2D, ViewportContent);
            UITechTreeMoveControls.HandleArrowKeys();
            UITechTreeMoveControls.HandleMouseDrag(Camera2D, ViewportContent, PrevCameraPos, ScreenStartPos, Drag);
            UITechTreeMoveControls.HandleCameraBounds(Camera2D, ViewportContent);
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouse eventMouse)
            {
                if (Godot.Input.IsActionJustPressed("left_click"))
                {
                    PrevCameraPos = UITechViewport.Camera2D.Position;
                    ScreenStartPos = GetViewport().GetMousePosition();
                    Drag = true;
                }

                if (Godot.Input.IsActionJustReleased("left_click"))
                    Drag = false;

                var scrollSpeed = 0.05f;

                if (Godot.Input.IsActionPressed("ui_scroll_down"))
                {
                    // Zooming out
                    UITechTreeMoveControls.ScrollSpeed += new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = false;
                }

                if (Godot.Input.IsActionPressed("ui_scroll_up"))
                {
                    // Zooming in
                    UITechTreeMoveControls.ScrollSpeed -= new Vector2(scrollSpeed, scrollSpeed);
                    UITechTreeMoveControls.ScrollingUp = true;
                }
            }

            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }
    }
}
