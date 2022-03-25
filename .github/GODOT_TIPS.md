# Godot Tips
### Removing and adding a child in the same frame
Almost always this leads to many errors (sometimes game crashing errors), and generally the work-a-around is to wait 1 idle_frame between removing and adding the child

### Renaming export node paths
Take note of where the node path leads to in the inspector before renaming it as you will have to manually set the node path again after you rename it.

### Changing the location of a script
Do not change the location of a script inside vscode, instead do it inside Godot. Changing the location of the script in Godot will update the necessary references unlike in Vscode.
