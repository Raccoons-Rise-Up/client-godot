# Contributing
## Table of Contents
1. [Setup]()
    - [Godot]()
    - [VSCode]()
2. [Formatting Guidelines]()
3. [Creating a Pull Request]()
4. [Threads]()

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

## Formatting Guidelines
- Methods should follow PascalFormat
- Most of the time `{}` should be fully expanded
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

## Exporting
Do not forget to add [enet.dll](https://github.com/nxrighthere/ENet-CSharp/releases) beside the games executable.
