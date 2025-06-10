using Godot;
using System;

public partial class Edge : Line2D
{
    public Node StartNode { get; private set; }
    public Node EndNode { get; private set; }
    private float weight = 1f;
    public bool IsBlocked { get; private set; } = false;

    // Propiedad pública para leer/personalizar el peso
    public float Weight
    {
        get => IsBlocked ? float.PositiveInfinity : weight;
        private set => weight = value;
    }

    public void Initialize(Node start, Node end, float initialWeight)
    {
        StartNode = start;
        EndNode = end;
        weight = initialWeight;
        UpdateVisual();
    }

    public void SetWeight(float newWeight)
    {
        weight = Mathf.Max(newWeight, 0.01f);
        UpdateVisual();
    }

    public void ToggleBlocked(bool blocked)
    {
        IsBlocked = blocked;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        // Color o estilo diferente si está bloqueada
        DefaultColor = IsBlocked ? new Color(1, 0, 0) : new Color(0.7f, 0.7f, 0.7f);
        // Anotamos el ancho en función del peso
        Width = 2 + weight;
        // Actualizamos puntos
        Points = new Vector2[] {
            StartNode.GlobalPosition,
            EndNode.GlobalPosition
        };
    }

    public override void _Process(double delta)
    {
        // Mantener la línea unida a los nodos si estos se mueven
        UpdateVisual();
    }
}
