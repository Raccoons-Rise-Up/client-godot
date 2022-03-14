using Godot;
using System;

public class WorldDebugGUI : Node
{
#pragma warning disable CS0649 // Values are assigned in the editor
    [Export] private readonly NodePath nodePathSliderPeriod;
#pragma warning restore CS0649 // Values are assigned in the editor

    private HSlider SliderPeriod;

    private float Period;

    public override void _Ready()
    {
        SliderPeriod = GetNode<HSlider>(nodePathSliderPeriod);
    }

    private void _on_Period_Slider_value_changed(float value)
    {
        Period = value;
    }

    private void _on_Btn_Regenerate_pressed() 
    {
        //var chunk = new Chunk();
        //chunk.Generate(Period);
    }
}
