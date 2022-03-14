using Godot;
using System;
using Client.Netcode;

namespace Client.UI 
{
    public class UIGameMenu : Control
    {
        public static bool ClientPressedDisconnect { get; set; }

        public override void _Ready() => Visible = false;

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_cancel"))
                Visible = !Visible;
            
            @event.Dispose(); // Godot Bug: Input Events are not reference counted
        }

        private void _on_Disconnect_pressed()
        {
            if (!ClientPressedDisconnect)
            {
                ClientPressedDisconnect = true;
                ENetClient.ENetCmds.Enqueue(new ENetCmd { Opcode = ENetOpcode.ClientWantsToDisconnect });
            }
        }
    }
}