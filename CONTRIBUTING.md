# Contributing
## Table of Contents
1. [Debug Setup](#debug-setup)
4. [Threads](#threads)
5. [Networking](#networking)
6. [Exporting](#exporting)
7. [Formatting Guidelines](#formatting-guidelines)
3. [Creating a Pull Request](#creating-a-pull-request)

## Debug Setup
1. Install [VSCode](https://code.visualstudio.com)
2. Install the following extensions for VSCode
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
3. In Godot `Project Settings > Mono > Debugger Agent` make sure `Wait for Debugger` is enabled and `Port` is set to `23685`. 
4. In VSCode, make sure your `launch.json` looks something like this under `.vscode`
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
5. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor`

## Threads
The client runs on 2 threads; the Godot thread and the ENet thread. Never run Godot code in the ENet thread and likewise never run ENet code in the Godot thread. If you ever need to communicate between the threads, use the proper `ConcurrentQueue`'s in `ENetClient.cs`.

## Networking
The netcode utilizes [ENet-CSharp](https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md), a reliable UDP networking library.

Never give the client any authority, the server always has the final say in everything. This should always be thought of when sending new packets.

Packets are sent like this.
```cs
// WPacketChatMessage.cs
namespace KRU.Networking
{
    public class WPacketChatMessage : IWritable
    {
        public uint ChannelId { get; set; }
        public string Message { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(ChannelId);
            writer.Write(Message);
        }
    }
}

// Since the packet is being enqueued to a ConcurrentQueue the following code can be called from any thread
ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
    ChannelId = UIChannels.ActiveChannel,
    Message = text
}));
```

## Exporting
Do not forget to add [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) beside the games executable.

## Formatting Guidelines
- Methods should follow PascalFormat
- Try to use `var` where ever possible

## Creating a Pull Request
1. Always test the application to see if it works as intended with no additional bugs you may be adding!
2. State all the changes you made in the PR, not everyone will understand what you've done!
