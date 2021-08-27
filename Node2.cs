using Godot;
using System;
using System.Threading;
using ENet;

using Thread = System.Threading.Thread;

public class Node2 : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("_Ready()");
        new Thread(WorkerThread).Start();
    }

    private static void WorkerThread()
    {
        using (Host client = new Host()) {
            Address address = new Address();

            address.SetHost("127.0.0.1");
            address.Port = 25565;
            client.Create();

            Peer peer = client.Connect(address);

            Event netEvent;

            while (true) {
                bool polled = false;

                while (!polled) {
                    if (client.CheckEvents(out netEvent) <= 0) {
                        if (client.Service(15, out netEvent) <= 0)
                            break;

                        polled = true;
                    }

                    switch (netEvent.Type) {
                        case EventType.None:
                            break;

                        case EventType.Connect:
                            GD.Print("Client connected to server");
                            break;

                        case EventType.Disconnect:
                            GD.Print("Client disconnected from server");
                            break;

                        case EventType.Timeout:
                            GD.Print("Client connection timeout");
                            break;

                        case EventType.Receive:
                            GD.Print("Packet received from server - Channel ID: " + netEvent.ChannelID + ", Data length: " + netEvent.Packet.Length);
                            netEvent.Packet.Dispose();
                            break;
                    }
                }
            }

            client.Flush();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }
}
