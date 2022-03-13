using Godot;
using System;

public class CameraController : Camera
{
    private Vector3 Direction = Vector3.Zero;
    private float MaxSpeed = 5;

    public override void _PhysicsProcess(float delta)
    {
        var curMovement = Vector3.Zero;
        curMovement = curMovement.LinearInterpolate(Direction, MaxSpeed);

        Translate(curMovement * delta);

        Direction.x = Mathf.MoveToward(Direction.x, 0, 0.01f);
        Direction.y = Mathf.MoveToward(Direction.y, 0, 0.01f);
    }

    public override void _Input(InputEvent @event)
    {
        var left = Input.IsActionPressed("ui_left");
        var right = Input.IsActionPressed("ui_right");
        var down = Input.IsActionPressed("ui_down");
        var up = Input.IsActionPressed("ui_up");

        if (left)
            Direction.x = 1;
        if (right)
            Direction.x = -1;
        if (down)
            Direction.y = 1;
        if (up)
            Direction.y = -1;
    }
}
