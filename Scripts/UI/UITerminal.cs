using Godot;

namespace KRU.UI
{
    public class UITerminal : Control
    {
        // Terminal
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathTerminal;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static VBoxContainer terminal;
        private static VScrollBar vScrollBar;

        public override void _Ready()
        {
            // Terminal
            terminal = GetNode<VBoxContainer>(nodePathTerminal);
            vScrollBar = ((ScrollContainer)terminal.GetParent()).GetVScrollbar();
        }

        public static async void Log(string message)
        {
            var hbox = new HBoxContainer();
            var messageIndexStart = 0;

            foreach (var resource in UIGame.ResourceInfoData) 
            {
                var resourceName = $":{resource.Key}:";
                var resourceIndex = message.IndexOf(resourceName);

                if (resourceIndex != -1) 
                {
                    var resourceIcon = UIGame.ResourceIconData[resource.Key];
                    var messageLength = resourceIndex - messageIndexStart;

                    var textLabel = new Label
                    {
                        Text = message.Substring(messageIndexStart, messageLength)
                    };

                    messageIndexStart = resourceIndex + resourceName.Length;

                    hbox.AddChild(textLabel);
                    hbox.AddChild(resourceIcon.Duplicate((int)DuplicateFlags.UseInstancing));
                }
            }

            var lastText = new Label {
                Text = message.Substring(messageIndexStart)
            };
            hbox.AddChild(lastText);
            
            terminal.AddChild(hbox);

            // Wait 1 idle frame before proceeding so scroll container can scroll to the true max value
            await terminal.ToSignal(terminal.GetTree(), "idle_frame");

            // Auto scroll to bottom
            vScrollBar.Value = vScrollBar.MaxValue;
        }

        public static void ClearMessages()
        {
            foreach (Node label in terminal.GetChildren())
            {
                label.QueueFree();
            }
        }
    }
}