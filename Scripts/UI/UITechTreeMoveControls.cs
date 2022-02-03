using Godot;
using System;

namespace Client.UI 
{
    public class UITechTreeMoveControls
    {
        public static void HandleMouseDrag(Control control, Vector2 prevCameraPos, Vector2 screenStartPos, bool drag)
        {
            var cam = UITechViewport.Camera2D;
            var viewportSize = control.GetViewportRect().Size;
            var rectPos = control.RectPosition;
            var rectSize = control.RectSize;
                
            var windowOffsetY = Mathf.Min(0, OS.WindowSize.y - viewportSize.y - UITechViewport.CanvasLayer.RectGlobalPosition.y);
            
            var boundsLeft = rectPos.x + viewportSize.x / 2;
            var boundsRight = rectPos.x + rectSize.x - viewportSize.x / 2;
            var boundsTop = rectPos.y + viewportSize.y / 2;
            var boundsBottom = rectPos.y + rectSize.y - viewportSize.y / 2 - windowOffsetY;

            if (drag)
                cam.Position = prevCameraPos + screenStartPos - control.GetViewport().GetMousePosition();

            if (cam.Position.x < boundsLeft) 
                cam.Position = new Vector2(boundsLeft, cam.Position.y);
            if (cam.Position.x > boundsRight)
                cam.Position = new Vector2(boundsRight, cam.Position.y);
            if (cam.Position.y < boundsTop)
                cam.Position = new Vector2(cam.Position.x, boundsTop);
            if (cam.Position.y > boundsBottom)
                cam.Position = new Vector2(cam.Position.x, boundsBottom);
        }

        public static void HandleArrowKeys(int viewportMoveSpeed)
        {
            if (Input.IsActionPressed("ui_left"))
                UITechViewport.Camera2D.Position -= new Vector2(viewportMoveSpeed, 0);
            
            if (Input.IsActionPressed("ui_right"))
                UITechViewport.Camera2D.Position += new Vector2(viewportMoveSpeed, 0);

            if (Input.IsActionPressed("ui_up"))
                UITechViewport.Camera2D.Position -= new Vector2(0, viewportMoveSpeed);

            if (Input.IsActionPressed("ui_down"))
                UITechViewport.Camera2D.Position += new Vector2(0, viewportMoveSpeed);
        }
    }
}
