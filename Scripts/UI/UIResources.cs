using Godot;
using System;

public class UIResources : Control
{
    private static PackedScene prefabUIResource = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Elements/UILabelCount.tscn");

#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private NodePath nodePathResourceList;
#pragma warning restore CS0649 // Values are assigned in the editor

    private static VBoxContainer resourceList;

    public override void _Ready()
    {
        resourceList = GetNode<VBoxContainer>(nodePathResourceList);
        AddResource("Wood", 23);
        AddResource("Stone", 5);
    }

    public static void AddResource(string name, int value)
    {
        var resource = (HBoxContainer)prefabUIResource.Instance();
        var labelName = resource.GetNode<Label>("Name");
        var labelValue = resource.GetNode<Label>("Value");

        labelName.Text = name;
        labelValue.Text = "" + value;

        resourceList.AddChild(resource);
    }
}
