# Contributing
## Table of Contents
1. [Debugging](#debugging)
    - [VSCode Setup](#vscode-setup)
    - [Attaching the Debugger](#attaching-the-debugger)
3. [Formatting Guidelines](#formatting-guidelines)
4. [Creating a Pull Request](#creating-a-pull-request)
5. [Threads](#threads)
    - [Communicating from Godot to ENet](#communicating-from-godot-to-enet)
    - [Communicating from ENet to Godot](#communicating-from-enet-to-godot)
6. [Networking](#networking)
    - [Security](#security)
    - [Sending a Packet from the Client to the Server](#sending-a-packet-from-the-client-to-the-server)
7. [Exporting](#exporting)

## Debugging
### VSCode Setup
Note that Godot also supports Visual Studio (VS) but the debugger for VS is currently NOT supported!

I use VS to edit the code and only use VSCode when debugging. Note if you do not see the value of for e.g. a property in the VSCode debugger, you will need to add it to the VSCode debug "watch list".

1. Install [VSCode](https://code.visualstudio.com)
2. Install the following VSCode extensions
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
3. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor`

### Attaching the Debugger
In Godot `Project Settings > Mono > Debugger Agent` make sure `Wait for Debugger` is enabled and `Port` is set to `23685`. 

In VSCode, make sure your `launch.json` looks something like this under `.vscode`
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch",
            "type": "mono",
            "request": "launch",
            "program": "${workspaceRoot}/program.exe",
            "cwd": "${workspaceRoot}"
        },
        {
            "name": "Attach",
            "type": "mono",
            "request": "attach",
            "address": "localhost",
            "port": 23685
        }
    ]
}
```

### Errors
Issue: Attempted to convert an unmarshallable managed type to variant  
Cause: Add a class that does not extend from `Node` in a `Godot.Collections.Dictionary`  
Fix: Use `System.Collections.Generic.Dictionary` or continue using Godot Dict and make sure all classes extend from `Node`  

## Formatting Guidelines
- Methods should follow PascalFormat
- Try to use `var` where ever possible

## Creating a Pull Request
1. Always test the application to see if it works as intended with no additional bugs you may be adding!
2. State all the changes you made in the PR, not everyone will understand what you've done!

## Threads
The networking library ENet-CSharp needs its own thread to execute on to ensure Godot thread is not clogging up the network. If you are unfamiliar with threads please read [Using threads and threading](https://docs.microsoft.com/en-us/dotnet/standard/threading/using-threads-and-threading).

### Communicating from Godot to ENet
In `Scripts/Netcode/Packets/Opcodes.cs`, add the 'opcode' to the following enum. For example maybe you want to instruct ENet to disconnect from the server entirely, so you would add something like `CancelConnection`.
```cs
public enum ENetInstructionOpcode 
{
    CancelConnection
}
```

Enqueue the instruction from the Godot thread
```cs
ENetCmds.Enqueue(ENetInstructionOpcode.CancelConnection);
```

Then dequeue the instruction in the ENet thread
```cs
// ENet Instructions (from Godot Thread)
while (ENetInstructions.TryDequeue(out ENetInstructionOpcode result))
{
    if (result == ENetInstructionOpcode.CancelConnection)
    {
        // Cancel connection
        break;
    }
}
```

### Communicating from ENet to Godot
Lets say you want to display a message in the Godot UI when something happens in the ENet thread.

First add the opcode `LogMessage` to the `GodotInstructionOpcode` enum
```cs
public enum GodotInstructionOpcode
{
    LogMessage
}
```

Enqueue the data in the ENet thread
```cs
var cmd = new GodotInstructions();
cmd.Set(GodotInstructionOpcode.LogMessage, "Hello world");

GodotCmds.Enqueue(cmd);
```

Or enqueue a command without any data attached.
```cs
GodotCmds.Enqueue(new GodotInstructions(GodotInstructionOpcode.DoSomething));
```

Dequeue the data in the Godot thread
```cs
while (GodotCmds.TryDequeue(out GodotInstructions result))
{
    foreach (var cmd in result.Instructions)
    {
        var opcode = cmd.Key;
        
        if (opcode == GodotInstructionOpcode.LogMessage)
        {
            GD.Print((string)cmd.Value[0]);
        }
        
        if (opcode == GodotInstructionOpcode.DoSomething)
        {
            // Do something
        }
    }
}
```

If you want to send data along with the opcode then the `ENetInstructionOpcode` would have to become a class with a opcode enum inside however the need for such a purpose has not risen yet.

## Networking
Documentation for ENet-CSharp can be found [here](https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md).

### Security
This section is a little dry right now, more will be added in time.

TODO: Talk about password hash.

Never give the client any authority, the server always has the final say in everything. This should always be thought of when sending new packets.

### Sending a Packet from the Client to the Server

1. [Creating the Writer Packet](#creating-the-writer-packet)
2. [Adding the Opcode](#adding-the-opcode)
3. [Sending the Packet](#sending-the-packet)
4. [Creating the Reader Packet Server Side](#creating-the-reader-packet-server-side)

#### Creating the Writer Packet
Create a new packet class under `Scripts/Netcode/Packets/Write`, the packet class should be called something like `WPacketPositionData` or `WPacketDisconnect`.

Use the following code as a template
```cs
using Common.Networking.Message;
using Common.Networking.IO;
using Common.Networking.Packet;

namespace KRU.Networking 
{
    public class WPacketSendSomething : IWritable
    {
        public ushort ItemId { private get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(ItemID);
	    // more data could be writen here
        }
    }
}
```

#### Adding the Opcode
Opcodes are what make packets unique from each other. For example, `ClientPacketOpcode.Login` is a opcode used to identify that this packet holds login information.

On the client, navigate to `Scripts/Netcode/Packets/Opcodes.cs` and add your opcode to the enum `ClientPacketOpcode`. (also do the same on the server in `src/Server/Packets/Opcodes.cs`, the `Opcodes.cs` client-side should look exactly like that of the `Opcodes.cs` server-side.)

#### Sending the Packet
Navigate to `Scripts/Netcode/ENetClient.cs`, this is the main networking script for the client.

```cs
// Create the packet
var data = new WPacketSendSomething({ ItemID = (ushort)itemId });
var clientPacket = new ClientPacket((byte)ClientPacketOpcode.PurchaseItem, data);

// Enqueue the packet to outgoing concurrent queue
Outgoing.Enqueue(clientPacket);
```

Finally, dequeue the packet from the outgoing concurrent queue and send it to the server. Note that `PacketFlags.Unreliable` should be used for sending data that is not important such as positional data while `PacketFlags.Reliable` should be used when sending important data such as item the client wants to purchase.
```cs
while (outgoing.TryDequeue(out ClientPacket clientPacket)) 
{
    switch ((ClientPacketType)clientPacket.Opcode) 
    {
        case ClientPacketType.PurchaseItem:
            Send(clientPacket, PacketFlags.Reliable);
            break;
    }
}
```

#### Creating the Reader Packet Server Side
The server needs to know how to read this packet.

In the server, navigate to `src/Server/Packets/Client/` and create a class named `RPacketPurchaseItem.cs`

Use the following code as a template.

```cs
using Common.Networking.Message;
using Common.Networking.IO;
using Common.Networking.Packet;

namespace GameServer.Server.Packets
{
    // ================================== Sizes ==================================
    // sbyte   -128 to 127                                                   SByte
    // byte       0 to 255                                                   Byte
    // short   -32,768 to 32,767                                             Int16
    // ushort  0 to 65,535                                                   UInt16
    // int     -2,147,483,648 to 2,147,483,647                               Int32
    // uint    0 to 4,294,967,295                                            UInt32
    // long    -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807       Int64
    // ulong   0 to 18,446,744,073,709,551,615                               UInt64

    public class RPacketPurchaseItem : IReadable
    {
        public ushort ItemId { get; set; }

        public void Read(PacketReader reader)
        {
            itemId = reader.ReadUInt16(); // we sent it as a ushort so we must read it as a ushort (see to the table above)
        }
    }
}
```

Create a new class called `HandlePacketPurchaseItem` which extends from `HandlePacket` in `src/Server/Packets/Handle`
```cs
using Common.Networking.Packet;
using Common.Networking.IO;
using ENet;
using GameServer.Logging;

namespace GameServer.Server.Packets
{
    public class HandlePacketPurchaseItem : HandlePacket
    {
        public override ClientOpcode Opcode { get; set; }

        public HandlePacketPurchaseItem()
        {
            Opcode = ClientOpcode.PurchaseItem;
        }

        public override void Handle(Event netEvent, PacketReader packetReader)
        {
            var data = new RPacketPurchaseItem();
            data.Read(packetReader);

            var itemType = (ItemType)data.ItemId;

            if (itemType == ItemType.Hut)
            {
                Logger.Log("Hut");
            }

            packetReader.Dispose(); // very important!
        }
    }
}
```

## Exporting
Do not forget to add [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) beside the games executable.
