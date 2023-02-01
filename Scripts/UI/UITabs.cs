using Godot;
using System;

namespace Client.UI 
{
    public partial class UITabs : Node
    {
        public static Tab CurrentTab = Tab.Colony;

        private void _on_Tabs_tab_changed(int tab) 
        {
            CurrentTab = (Tab)tab;

            // Disable camera when not needed to save performance
            if (CurrentTab == Tab.Research)
                UITechViewport.Camera2D.MakeCurrent();
            //else
                //UITechViewport.Camera2D.Current = false;
        }
    }

    public enum Tab 
    {
        Colony,
        Jobs,
        Build,
        Research
    }

}