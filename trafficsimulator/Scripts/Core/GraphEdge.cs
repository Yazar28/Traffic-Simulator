using Godot;
using System;


public partial class GraphEdge : Line2D
{
    // Referencias a los nodos de tipo GraphNode (antes “Node”)
    public GraphNode StartNode { get; private set; }
    public GraphNode EndNode { get; private set; }

    private float weight = 1f;
    public bool IsBlocked { get; private set; } = false;

    // Peso efectivo ( si está bloqueada)
    public float Weight
    {
        get => IsBlocked ? float.PositiveInfinity : weight;
        private set => weight = value;
    }

    // Inicializa la arista con sus nodos y peso
    public void Initialize(GraphNode start, GraphNode end, float initialWeight)
    {
        StartNode = start;
        EndNode = end;
        weight = initialWeight;
        UpdateVisual();
    }

    // Cambia el peso (mínimo 0.01)
    public void SetWeight(float newWeight)
    {
        weight = Mathf.Max(newWeight, 0.01f);
        UpdateVisual();
    }

    // Activa/desactiva bloqueo (peso)
    public void ToggleBlocked(bool blocked)
    {
        IsBlocked = blocked;
        UpdateVisual();
    }

    // Dibuja color, grosor y puntos según estado
    private void UpdateVisual()
    {
        DefaultColor = IsBlocked
            ? new Color(1, 0, 0)
            : new Color(0.7f, 0.7f, 0.7f);

        Width = 2 + weight;
        if (StartNode != null && EndNode != null)
        {
            Points = new Vector2[] {
                StartNode.GlobalPosition,
                EndNode.GlobalPosition
            };
        }
    }

    public override void _Process(double delta)
    {
        // Mantiene actualizada la posición de la línea si los nodos se mueven
        UpdateVisual();
    }
}
