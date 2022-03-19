using Godot;
using System;

public class CameraController : Camera
{
    private Vector3 Direction = Vector3.Zero;
    private float MaxSpeed = 30;
    private float Deacceleration = 0.05f;
    private Vector3 curMovement = Vector3.Zero;

    public override void _PhysicsProcess(float delta)
    {
        curMovement = curMovement.LinearInterpolate(Direction * MaxSpeed, Deacceleration);

        Translate(curMovement * delta);
    }

    public override void _Input(InputEvent @event)
    {
        var left = Input.IsActionPressed("ui_left");
        var right = Input.IsActionPressed("ui_right");
        var down = Input.IsActionPressed("ui_down");
        var up = Input.IsActionPressed("ui_up");

        Direction = Vector3.Zero;

        if (left)
            Direction.x = -1;
        if (right)
            Direction.x = 1;
        if (down)
            Direction.y = -1;
        if (up)
            Direction.y = 1;
    }
}
