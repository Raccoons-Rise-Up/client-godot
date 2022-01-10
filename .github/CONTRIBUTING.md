# Where do you want to go?
## [I need help setting up the project](#setup)
## [I want to help by adding sprites to the game](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/SPRITES.md)
## [I want to help work on the UI](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/USER_INTERFACE.md)
## [I want to help work on the networking code](https://github.com/Raccoons-Rise-Up/client-godot/blob/main/.github/NETWORKING.md)

## Setup
### VSCode
**You can skip the setup for VSCode if you are not going to be adding any code to the game.**
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
Tip #1: You can skip step 1 if you are not going to work on the netcode, just note that in order to get to the game scene you will need to either connect through the game / web servers or add code to bypass this and create a dummy user login.

Tip #2: You do not need to start in the main menu scene which requires user auth to get to the game scene. You can change the startup scene in `Godot > Project Settings > Application > Run > Main Scene`
1. Make sure the [game server](https://github.com/Raccoons-Rise-Up/server/blob/main/.github/CONTRIBUTING.md#setup) and [web server](https://github.com/Raccoons-Rise-Up/website/blob/main/.github/CONTRIBUTING.md) are running otherwise the client will fail to connect
2. Fork this repository
3. Clone your fork with [git scm](https://git-scm.com) 
4. Install [Godot Mono 64 Bit](https://godotengine.org)
5. Install [.NET SDK from this link](https://dotnet.microsoft.com/en-us/download)
6. Launch Godot through VSCode
7. In Godot Editor > Editor Settings > Mono > Builds > Make sure `Build Tool` is set to `dotnet CLI`
8. Setup IPs  

![image](https://user-images.githubusercontent.com/6277739/147781322-7aacb872-cf16-4055-b1c8-2555e7014bea.png)  
Click on `Scene Game` node in scene tree window top left.  

![image](https://user-images.githubusercontent.com/6277739/147781351-98489013-212d-4550-aa20-96131fd693d3.png)  
Make sure the IPs are set to `localhost` or your external IP.  

8. Press `F5` to run the client (if you want to run multiple instances of the client you will need to [export the game](#exporting))

## Exporting
Export the game by going to `Project > Export...`

![image](https://user-images.githubusercontent.com/6277739/147781789-02cc06e8-630c-44fa-8e82-07eb7fe977bd.png)  
![image](https://user-images.githubusercontent.com/6277739/147781833-7762fd21-e683-46e6-9faf-32f20df7ad31.png)  

Once the game is exported make sure [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) is beside the games executable, this is required in order for ENet to run.
