using Godot;
using System;

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

        public delegate void ViewportSizeChangedEventHandler(object source, EventArgs args);

        public static event ViewportSizeChangedEventHandler ViewportSizeChanged; // NO LONGER BEING USED (see GameViewport.cs)

        public override void _Ready()
        {
            Camera2D = GetNode<Camera2D>(nodePathCamera2D);
            CanvasLayer = GetNode<Control>(nodePathCanvasLayer);
        }

        private void _on_Viewport_size_changed() => OnViewportSizeChanged();

        protected virtual void OnViewportSizeChanged() 
        {
            if (ViewportSizeChanged != null)
                ViewportSizeChanged(this, EventArgs.Empty);
        }
    }
}
