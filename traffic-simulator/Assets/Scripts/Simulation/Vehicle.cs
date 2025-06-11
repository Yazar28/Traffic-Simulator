using Godot;
using System;

public partial class Vehicle : Node2D
{
    // Ruta calculada de GraphNode
    public Godot.Collections.Array<GraphNode> Path = new Godot.Collections.Array<GraphNode>();
    private int currentIndex = 0;
    [Export] public float Speed = 100f;

    // Asigna nueva ruta
    public void SetPath(Godot.Collections.Array<GraphNode> newPath)
    {
        Path = newPath;
        currentIndex = 0;
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
}
