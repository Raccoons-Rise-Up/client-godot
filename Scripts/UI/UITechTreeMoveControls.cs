using Godot;
using System;
using Client.Utilities;

namespace Client.UI 
{
    public class UITechTreeMoveControls
    {
        public static Vector2 ScrollSpeed = Vector2.Zero;
        public static bool ScrollingUp;

        public static void HandleCameraMovementSpeed(Camera2D cam)
        {
            var baseSpeed = 5.0f;
            cam.SmoothingSpeed = baseSpeed * cam.Zoom.Length();
        }

        public static void HandleMouseDrag(Camera2D cam, Control control, Vector2 prevCameraPos, Vector2 screenStartPos, bool drag)
        {
            if (drag)
                cam.Position = prevCameraPos + screenStartPos - control.GetViewport().GetMousePosition();
        }

        private static Vector2 PrevCamZoom = Vector2.Zero;

        public static void HandleScrollZoom(Camera2D cam, Control control)
        {
            var minZoom = new Vector2(1f, 1f);

            cam.Zoom += ScrollSpeed; // Constantly add to camera zoom based on ScrollSpeed

            if (cam.Zoom < minZoom) 
            {
                ScrollSpeed = Vector2.Zero;
                cam.Zoom = minZoom;
            }

            var viewportSize = control.GetViewportRect().Size * cam.Zoom;
            var contentSize = control.RectSize;

            //GD.Print(viewportSize > contentSize);

            var test = 2;
            if (cam.Zoom.x > test)
                cam.Zoom = new Vector2(test, test);

            //GD.Print(contentSize.x - 1024);
            //GD.Print((contentSize.x - viewportSize.x) / (contentSize.x - 1024));

            if (viewportSize > contentSize)
            {
                
                ScrollSpeed = Vector2.Zero;
                //cam.Zoom = PrevCamZoom;
                cam.Zoom -= new Vector2(0.02f, 0.02f);
            }

            PrevCamZoom = cam.Zoom;

            var deaccelerationSpeed = new Vector2(0.01f, 0.01f);

            if (ScrollSpeed > Vector2.Zero && !ScrollingUp) 
                ScrollSpeed -= deaccelerationSpeed;
            else if (ScrollSpeed < Vector2.Zero && ScrollingUp)
                ScrollSpeed += deaccelerationSpeed;
            else
                ScrollSpeed = Vector2.Zero;
        }

        public static void HandleCameraBounds(Camera2D cam, Control control)
        {
            var viewportSize = control.GetViewportRect().Size;
            var rectPos = control.RectPosition;
            var rectSize = control.RectSize;

            var windowOffsetY = Mathf.Min(0, (OS.WindowSize.y - viewportSize.y - UITechViewport.CanvasLayer.RectGlobalPosition.y) * cam.Zoom.y);
            
            var boundsLeft = rectPos.x + (viewportSize.x * cam.Zoom.x) / 2;
            var boundsRight = rectPos.x + rectSize.x - (viewportSize.x * cam.Zoom.x) / 2;
            var boundsTop = rectPos.y + (viewportSize.y * cam.Zoom.y) / 2;
            var boundsBottom = rectPos.y + rectSize.y - (viewportSize.y * cam.Zoom.y) / 2 - windowOffsetY;

            if (cam.Position.x < boundsLeft) 
                cam.Position = new Vector2(boundsLeft, cam.Position.y);
            if (cam.Position.x > boundsRight)
                cam.Position = new Vector2(boundsRight, cam.Position.y);
            if (cam.Position.y < boundsTop)
                cam.Position = new Vector2(cam.Position.x, boundsTop);
            if (cam.Position.y > boundsBottom)
                cam.Position = new Vector2(cam.Position.x, boundsBottom);
        }

        public static void HandleArrowKeys()
        {
            var arrowSpeed = 10.0f;

            if (Input.IsActionPressed("ui_left"))
                UITechViewport.Camera2D.Position -= new Vector2(arrowSpeed, 0);
            
            if (Input.IsActionPressed("ui_right"))
                UITechViewport.Camera2D.Position += new Vector2(arrowSpeed, 0);

            if (Input.IsActionPressed("ui_up"))
                UITechViewport.Camera2D.Position -= new Vector2(0, arrowSpeed);

            if (Input.IsActionPressed("ui_down"))
                UITechViewport.Camera2D.Position += new Vector2(0, arrowSpeed);
        }
    }
}
