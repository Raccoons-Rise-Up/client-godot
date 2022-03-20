# Where do you want to go?
## [Project Setup](#setup)
## [Sprites](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/SPRITES.md)
## [User Interface](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/USER_INTERFACE.md)
## [Netcode](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/NETWORKING.md)
## [Useful Godot Tips](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/GODOT_TIPS.md)

## Setup
### Godot Mono (C#)
1. Install [Godot Mono 64 Bit](https://godotengine.org)
2. Install [.NET SDK from this link](https://dotnet.microsoft.com/en-us/download)
3. Install [.NET Framework 4.7.2](https://duckduckgo.com/?q=.net+framework+4.7.2)
4. Launch Godot through [VSCode](#vscode)
5. In Godot Editor > Editor Settings > Mono > Builds > Make sure `Build Tool` is set to `dotnet CLI`

If the startup scene is the main menu, the [game server](https://github.com/Raccoons-Rise-Up/server/blob/main/.github/CONTRIBUTING.md#setup) and [web server](https://github.com/Raccoons-Rise-Up/website/blob/main/.github/CONTRIBUTING.md) will need to be running to get past the login screen to the main game scene, otherwise you can change the startup scene to the main game scene by going to `Godot > Project Settings > Application > Run > Main Scene`.

### VSCode
VSCode is a UI friendly text editor for developers
1. Install [VSCode](https://code.visualstudio.com)
2. Install the following extensions for VSCode
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
3. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor` (to debug the game launch it through vscode by pressing `F5`)

## Exporting
Exporting is useful when you want to run multiple instances of the client at the same time. Export the game by going to `Project > Export...`

Make sure to check `Runnable` and `Export with Debug`

Once the game is exported make sure [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) is beside the games executable, this is required in order for the netcode to run.

Note that running the exported instance with the `--verbose` flag provides useful debug information.
