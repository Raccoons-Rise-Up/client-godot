using Godot;

namespace KRU.UI
{
    public class UITerminal : Control
    {
        // Terminal
#pragma warning disable CS0649 // Values are assigned in the editor
        [Export] private readonly NodePath nodePathTerminal;
#pragma warning restore CS0649 // Values are assigned in the editor
        private static VBoxContainer Terminal { get; set; }
        private static VScrollBar VScrollBar { get; set; }

        public override void _Ready()
        {
            // Terminal
            Terminal = GetNode<VBoxContainer>(nodePathTerminal);
            VScrollBar = ((ScrollContainer)Terminal.GetParent()).GetVScrollbar();
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
            
            Terminal.AddChild(hbox);

            // Wait 1 idle frame before proceeding so scroll container can scroll to the true max value
            await Terminal.ToSignal(Terminal.GetTree(), "idle_frame");

            // Auto scroll to bottom
            VScrollBar.Value = VScrollBar.MaxValue;
        }

        public static void ClearMessages()
        {
            foreach (Node label in Terminal.GetChildren())
            {
                label.QueueFree();
            }
        }
    }
}