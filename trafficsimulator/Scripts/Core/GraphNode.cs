using Godot;
using System;


public partial class GraphNode : Node2D
{
    [Export] public string NodeId { get; set; }
    public MyList<GraphEdge> Edges = new MyList<GraphEdge>();

    private bool isDragging = false;
    private const float radius = 10f;

    public void AddEdge(GraphEdge edge)
    {
        Edges.Add(edge);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mbe && mbe.ButtonIndex == MouseButton.Left && !IsConnecting())
        {
            if (mbe.Pressed)
            {
                var colorRect = GetNode<ColorRect>("ColorRect");

                Vector2 localMouse = this.ToLocal(GetGlobalMousePosition());

                Rect2 rect = new Rect2(
                    colorRect.Position,
                    colorRect.Size
                );

                if (rect.HasPoint(localMouse))
                    isDragging = true;
            }
            else
            {
                isDragging = false;
            }
        }
        else if (@event is InputEventMouseMotion mem && isDragging)
        {
            Position += mem.Relative;
        }
    }
    private bool IsConnecting()
    {
        var editor = GetTree().Root.GetNodeOrNull<GraphEditorUI>("GraphEditor");
        return editor != null && editor.IsInConnectingMode();
    }
    
    public bool IsMouseOver()
    {
        var colorRect = GetNode<ColorRect>("ColorRect");
        Vector2 mouseGlobal = GetViewport().GetMousePosition();
        Vector2 mouseLocal = ToLocal(mouseGlobal);

        Rect2 rect = new Rect2(colorRect.Position, colorRect.Size);
        return rect.HasPoint(mouseLocal);
    }
}
