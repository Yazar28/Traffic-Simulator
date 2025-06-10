using Godot;
using System;

public partial class Node : Node2D
{
    public string NodeId { get; set; }
    public Godot.Collections.Array<Edge> Edges = new Godot.Collections.Array<Edge>();

    private bool isDragging = false;
    private const float radius = 10f; // Radio aproximado de tu ColorRect (20x20 / 2)

    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }

    public override void _Input(InputEvent @event)
    {
        // 1) Mouse button pressed / released
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left)
        {
            Vector2 mouseGlobal = GetGlobalMousePosition();

            if (mbe.Pressed)
            {
                // Si el clic cae dentro del radio del nodo, empezamos a arrastrar
                if (mouseGlobal.DistanceTo(GlobalPosition) <= radius)
                    isDragging = true;
            }
            else
            {
                // Al soltar, dejamos de arrastrar
                isDragging = false;
            }
        }
        // 2) Si estamos arrastrando y hay movimiento de ratón, movemos el nodo
        else if (@event is InputEventMouseMotion mem && isDragging)
        {
            Position += mem.Relative;
        }
    }
}
