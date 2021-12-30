# Contributing
## Table of Contents
1. [Setup](#setup)
  - [VScode](#vscode)
  - [Godot](#godot)
3. [Threads](#threads)
4. [Networking](#networking)
5. [Exporting](#exporting)

## Setup
### VSCode
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

### Godot
1. Make sure the [server](https://github.com/Kittens-Rise-Up/server) and [website](https://github.com/Kittens-Rise-Up/website) are running otherwise the client will not be able to connect properly.
2. Fork this repository
3. Clone your fork with [git scm](https://git-scm.com) 
4. Install [Godot Mono 64 Bit](https://godotengine.org)
5. Install [Build Tools for Visual Studio 2019](https://visualstudio.microsoft.com/downloads/?q=build+tools)
6. Launch Godot through VSCode

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
