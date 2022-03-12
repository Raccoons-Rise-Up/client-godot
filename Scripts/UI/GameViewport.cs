using Godot;
using System;

namespace Client.UI 
{
    public class GameViewport : Node
    {
        public delegate void ViewportSizeChangedEventHandler(object source, EventArgs args);

        public static event ViewportSizeChangedEventHandler ViewportSizeChanged;

        private void _on_Game_resized() => OnViewportSizeChanged();

        protected virtual void OnViewportSizeChanged() 
        {
            if (ViewportSizeChanged != null)
                ViewportSizeChanged(this, EventArgs.Empty);
        }
    }
}
