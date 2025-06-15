using Godot;
using System;

public partial class Vehicle : Node2D
{
    public Godot.Collections.Array<GraphNode> Path = new Godot.Collections.Array<GraphNode>();
    private int currentIndex = 0;

    [Export] public float Speed = 100f;

    public void SetPath(Godot.Collections.Array<GraphNode> newPath)
    {
        Path = newPath;
        currentIndex = 0;

        if (Path.Count > 0)
            GlobalPosition = Path[0].GlobalPosition;

        CallDeferred("update");
    }


    public override void _Process(double delta)
    {
        if (Path.Count == 0 || currentIndex >= Path.Count)
            return;

        Vector2 target = Path[currentIndex].GlobalPosition;
        Vector2 direction = (target - GlobalPosition).Normalized();
        Position += direction * Speed * (float)delta;

        if (Position.DistanceTo(target) < 5f)
            currentIndex++;
    }

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, 10, new Color(0, 0.6f, 1)); 
    }
}
