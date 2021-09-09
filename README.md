# Kittens Rise Up Game Client

![image](https://user-images.githubusercontent.com/6277739/132770788-9de196ea-548b-4ae7-8e95-4e2a7bbdfa50.png)

Here's a quick rundown of what's been done for the client.
- User can retrieve JWT from web-server and use that to login to game server (all done in one click of a button "Login")
- On successful login, client receives all structure data, and this gets populated in "Store" section
- Player can click on "Hut" or "Farm" and some details will be displayed on the right
- Player can click "Buy" and send a PurchasePacket to server

## Table of Contents
1. [Setup](#setup)
    - [Godot](#godot)
    - [VSCode](#vscode)
2. [Issues](#issues)
3. [Contributing](#contributing)

## Setup
### Godot
1. Fork this repository
2. Clone your fork with [git scm](https://git-scm.com) 
3. Install [Godot Mono 64 Bit](https://godotengine.org)
4. Install [Build Tools for Visual Studio 2019](https://visualstudio.microsoft.com/downloads/?q=build+tools)

### VSCode
Note that Godot also supports VS, however I have not tested this out to see how it works.

1. Install [VSCode](https://code.visualstudio.com)
2. Install the following VSCode extensions
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
3. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor`

## Issues
Please see the projects [current issues](https://github.com/Kittens-Rise-Up/client-godot/issues)

## Contributing
Please see [CONTRIBUTING.md](https://github.com/Kittens-Rise-Up/client-godot/blob/main/CONTRIBUTING.md)

Talk to `valk#9904` in the [Kittens Rise Up](https://discord.gg/cDNf8ja) discord for more info.
