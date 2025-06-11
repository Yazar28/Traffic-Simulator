using Godot;
using System;
using System.Collections.Generic;

public partial class GraphEditorUI : Control
{
    [Export] public PackedScene NodePrefab;
    [Export] public PackedScene EdgePrefab;
    [Export] public Node2D GraphCanvas;
    [Export] public TrafficGraph Graph;

    private GraphNode firstSelectedNode = null;
    private int nodeCounter = 0;
    private GraphEdge selectedEdge = null;

    public override void _Ready()
    {
        // Conectar botones
        GetNode<Button>("UI/Agregar Nodo")
            .Connect("pressed", new Callable(this, nameof(OnAddNodePressed)));
        GetNode<Button>("UI/Conectar Nodos")
            .Connect("pressed", new Callable(this, nameof(OnConnectNodesPressed)));
        GetNode<Button>("UI/Eliminar Nodo")
            .Connect("pressed", new Callable(this, nameof(OnDeleteNodePressed)));
        GetNode<Button>("UI/Eliminar Arista")
            .Connect("pressed", new Callable(this, nameof(OnDeleteEdgePressed)));

        // Conectar controles de edición de arista
        GetNode<SpinBox>("UI/WeightSpinBox")
            .Connect("value_changed", new Callable(this, nameof(OnWeightChanged)));
        GetNode<CheckBox>("UI/BlockCheckBox")
            .Connect("toggled", new Callable(this, nameof(OnBlockToggled)));

        // Ocultar controles hasta selección
        GetNode<SpinBox>("UI/WeightSpinBox").Visible = false;
        GetNode<CheckBox>("UI/BlockCheckBox").Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
        {
            var clicked = FindEdgeUnderMouse(GetLocalMousePosition());
            SelectEdge(clicked);
        }
    }

    private void SelectEdge(GraphEdge edge)
    {
        selectedEdge = edge;
        var weightSpin = GetNode<SpinBox>("UI/WeightSpinBox");
        var blockCheck = GetNode<CheckBox>("UI/BlockCheckBox");

        if (edge != null)
        {
            weightSpin.Visible = true;
            blockCheck.Visible = true;
            // Asignación sin señales
            weightSpin.SetValueNoSignal(edge.Weight);
            blockCheck.SetPressedNoSignal(edge.IsBlocked);
        }
        else
        {
            weightSpin.Visible = false;
            blockCheck.Visible = false;
        }
    }

    private GraphEdge FindEdgeUnderMouse(Vector2 mousePos)
    {
        foreach (var child in GraphCanvas.GetChildren())
        {
            if (child is GraphEdge edge)
            {
                var a = edge.Points[0];
                var b = edge.Points[1];
                if (DistancePointToSegment(mousePos, a, b) < 8f)
                    return edge;
            }
        }
        return null;
    }

    private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        var ab2 = ab.LengthSquared();
        if (ab2 == 0) return p.DistanceTo(a);
        var t = (p - a).Dot(ab) / ab2;
        t = Mathf.Clamp(t, 0, 1);
        var proj = a + ab * t;
        return p.DistanceTo(proj);
    }

    private void OnWeightChanged(double value)
    {
        selectedEdge?.SetWeight((float)value);
    }

    private void OnBlockToggled(bool pressed)
    {
        selectedEdge?.ToggleBlocked(pressed);
    }

    private void OnAddNodePressed()
    {
        var node = NodePrefab.Instantiate<GraphNode>();
        node.Position = GetLocalMousePosition();
        node.NodeId = "N" + nodeCounter++;
        node.GetNode<Label>("Label").Text = node.NodeId;
        Graph.AddNode(node);
        GraphCanvas.AddChild(node);
    }

    private void OnConnectNodesPressed()
    {
        if (firstSelectedNode == null)
        {
            GD.Print("Selecciona el primer nodo antes de conectar");
            return;
        }
        var second = GetClosestNodeToMouse();
        if (second == null || second == firstSelectedNode) return;

        var edge = EdgePrefab.Instantiate<GraphEdge>();
        edge.Initialize(firstSelectedNode, second, 1f);
        Graph.AddEdge(firstSelectedNode.NodeId, second.NodeId, 1f);
        GraphCanvas.AddChild(edge);
        firstSelectedNode = null;
    }

    private void OnDeleteNodePressed()
    {
        var node = GetClosestNodeToMouse();
        if (node == null) return;
        Graph.RemoveNode(node.NodeId);
        // Liberar aristas relacionadas
        foreach (var child in GraphCanvas.GetChildren())
            if (child is GraphEdge e && (e.StartNode == node || e.EndNode == node))
                e.QueueFree();
        node.QueueFree();
    }

    private void OnDeleteEdgePressed()
    {
        var edge = FindEdgeUnderMouse(GetLocalMousePosition());
        if (edge == null) return;
        Graph.RemoveEdge(edge.StartNode, edge.EndNode);
        edge.QueueFree();
    }

    private GraphNode GetClosestNodeToMouse()
    {
        var mouse = GetLocalMousePosition();
        float minDist = 30f;
        GraphNode closest = null;
        foreach (var child in GraphCanvas.GetChildren())
            if (child is GraphNode node)
            {
                var d = mouse.DistanceTo(node.Position);
                if (d < minDist) { minDist = d; closest = node; }
            }
        return closest;
    }

    public void SetSelectedNode(GraphNode node) => firstSelectedNode = node;
}
