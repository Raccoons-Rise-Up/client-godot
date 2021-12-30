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
4. In VSCode, make sure your `launch.json` looks like this under `.vscode/`
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
1. Make sure the [game server](https://github.com/Raccoons-Rise-Up/server/blob/main/.github/CONTRIBUTING.md#setup) and [web server](https://github.com/Raccoons-Rise-Up/website/blob/main/.github/CONTRIBUTING.md) are running otherwise the client will fail to connect
2. Fork this repository
3. Clone your fork with [git scm](https://git-scm.com) 
4. Install [Godot Mono 64 Bit](https://godotengine.org)
5. Install [Build Tools for Visual Studio 2019](https://visualstudio.microsoft.com/downloads/?q=build+tools)
6. Launch Godot through VSCode
7. Setup IPs  

![image](https://user-images.githubusercontent.com/6277739/147781322-7aacb872-cf16-4055-b1c8-2555e7014bea.png)  
Click on `Scene Game` node in scene tree window top left.  

![image](https://user-images.githubusercontent.com/6277739/147781351-98489013-212d-4550-aa20-96131fd693d3.png)  
Make sure the IPs are set to `localhost` or your external IP.  

8. Press `F5` to run the client (if you want to run multiple instances of the client you will need to [export the game](#exporting))

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

// Since packets are being enqueued to a ConcurrentQueue they can be called from any thread
ENetClient.Outgoing.Enqueue(new ClientPacket((byte)ClientPacketOpcode.ChatMessage, new WPacketChatMessage {
    ChannelId = UIChannels.ActiveChannel,
    Message = text
}));
```

## Exporting
Export the game by going to `Project > Export...`

![image](https://user-images.githubusercontent.com/6277739/147781789-02cc06e8-630c-44fa-8e82-07eb7fe977bd.png)  
![image](https://user-images.githubusercontent.com/6277739/147781833-7762fd21-e683-46e6-9faf-32f20df7ad31.png)  

Once the game is exported make sure [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) is beside the games executable, this is required in order for ENet to run.
