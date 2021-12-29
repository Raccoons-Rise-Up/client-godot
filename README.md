# Raccoons Rise Up Game Client
## My First Serious Project :)
This is my first serious project in Godot. I have learned so much from this project it is not even funny. There are so many challenges to tackle and they keep presenting themselves as I continue to add new features to the game. For example, who would have known implementing netcode for user text channels would be so much work? If you're new here I will tell you right away it will take some time to understand how everything comes together, but once you understand it all, working on it is a blast!

## About
![image](https://user-images.githubusercontent.com/6277739/147399688-057c43de-7538-42a0-8703-b84c119e649d.png)
Raccoons Rise Up will be a text based long term progressive resource management MMORPG. Players will be able to purchase structures with resources which in turn will generate more resources. Research from the tech tree will help speed up this process. Players will also be able to assign raccoons to jobs to make resource gathering slightly more interesting.

A wrapper called ENet-CSharp for ENet is being used for the netcode. A web server is being used to manage the user account authentication / creation. The game client is being made in the Godot C# engine. The game server acts as both a server and minecraft like console. A game launcher will eventually be created using Electron.

Currently I'm working on expanding on the "private text channel" netcode logic. Lots of bugs in this area, almost overwhelming at times.

## Things That Need to be Done Eventually
Here are a list of things that need to be done eventually in order
1. Friend system / group text channels / chat settings
2. Tech tree / research logic
3. Concept of "Raccoon Jobs" e.g. Woodcutter / Miner / etc
4. Trading resources with other players
5. A 2D or 3D map where you can see other players civilizations
6. Fog System for the map
7. Caravans
8. The concept of battle units that players can train / purchase and PvP with other players
9. 3D environments of raccoons mining / wood cutting in the background instead of just seeing numbers and text?

All contributions are very much welcome, please contact me through Discord if you plan on contributing, my Discord is valk#9904. If you have any questions, I'll try my best to answer them.

[Raccoons Rise Up Roadmap](https://trello.com/b/XkhJxR2x/raccoons-rise-up) (outdated / not managed - please talk to me through Discord to learn more about whats to come and what needs fixing)

## Setup
Note: The [server](https://github.com/Kittens-Rise-Up/server) and [website](https://github.com/Kittens-Rise-Up/website) need to be running so the client can connect to the game server properly.

1. Fork this repository
2. Clone your fork with [git scm](https://git-scm.com) 
3. Install [Godot Mono 64 Bit](https://godotengine.org)
4. Install [Build Tools for Visual Studio 2019](https://visualstudio.microsoft.com/downloads/?q=build+tools)

## Issues
Please see the projects [current issues](https://github.com/Kittens-Rise-Up/client-godot/issues)

## Contributing
Please see [CONTRIBUTING.md](https://github.com/Kittens-Rise-Up/client-godot/blob/main/CONTRIBUTING.md)

Not all the things to do are listed in the [issues](https://github.com/Raccoons-Rise-Up/client-godot/issues), if you want to know more of what needs to be done please talk to `valk#9904` in the [Raccoons Rise Up](https://discord.gg/cDNf8ja) discord for more info.
