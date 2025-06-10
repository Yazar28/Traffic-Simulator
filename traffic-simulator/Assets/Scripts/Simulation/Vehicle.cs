using Godot;
using System;

public partial class Vehicle : Node2D
{
    public Godot.Collections.Array<Node> Path = new Godot.Collections.Array<Node>();
    private int currentIndex = 0;
    public float Speed = 100f;

    public void SetPath(Godot.Collections.Array<Node> newPath)
    {
        Path = newPath;
        currentIndex = 0;
    }

    public void _Process(float delta)
    {
        if (Path.Count == 0 || currentIndex >= Path.Count)
            return;

        Vector2 target = Path[currentIndex].GlobalPosition;
        Vector2 direction = (target - GlobalPosition).Normalized();
        Position += direction * Speed * delta;

        if (Position.DistanceTo(target) < 5f)
            currentIndex++;
    }
}
