using Godot;
using System;
using System.Collections.Generic;

public enum EditorMode
{
    None,
    PlacingNode,
    ConnectingNodes,
    DeletingNode,
    DeletingEdge
}

public partial class GraphEditorUI : Control
{
    [Export] public PackedScene NodePrefab;
    [Export] public PackedScene EdgePrefab;
    [Export] public Node2D GraphCanvas;
    [Export] public TrafficGraph Graph;
    [Export] public SimulationManager Simulation;
    [Export] public VehicleManager VehicleManager;

    private GraphNode firstSelectedNode = null;
    private int nodeCounter = 0;
    private GraphEdge selectedEdge = null;
    private GraphNode tempSelectedNode = null;
    private EditorMode currentMode = EditorMode.None;
    private SpinBox vehicleCountSpinBox;

    public override void _Ready()
    {
        Simulation.Graph = Graph;
        VehicleManager.Init(Graph);

        // Conexión de botones
        GetNode<Button>("UI/Agregar Nodo")
            .Connect("pressed", new Callable(this, nameof(OnAddNodePressed)));
        GetNode<Button>("UI/Conectar Nodos")
            .Connect("pressed", new Callable(this, nameof(OnConnectNodesPressed)));
        GetNode<Button>("UI/Simular Vehículos")
            .Connect("pressed", new Callable(this, nameof(OnSimulateVehiclesPressed)));
        GetNode<Button>("UI/Eliminar Nodo")
            .Connect("pressed", new Callable(this, nameof(OnDeleteNodePressed)));
        GetNode<Button>("UI/Eliminar Arista")
            .Connect("pressed", new Callable(this, nameof(OnDeleteEdgePressed)));

        // Conexión de controles de aristas
        GetNode<SpinBox>("UI/WeightSpinBox")
            .Connect("value_changed", new Callable(this, nameof(OnWeightChanged)));
        GetNode<CheckBox>("UI/BlockCheckBox")
            .Connect("toggled", new Callable(this, nameof(OnBlockToggled)));

        // Configuración inicial de controles de aristas
        var weightSpin = GetNode<SpinBox>("UI/WeightSpinBox");
        var blockCheck = GetNode<CheckBox>("UI/BlockCheckBox");

        weightSpin.Visible = true;
        blockCheck.Visible = true;

        weightSpin.Editable = false;
        blockCheck.Disabled = true;

        weightSpin.Value = 0;
        blockCheck.SetPressedNoSignal(false);

        // Configuración del SpinBox para cantidad de vehículos
        vehicleCountSpinBox = GetNode<SpinBox>("UI/VehicleCountSpinBox");
        vehicleCountSpinBox.MinValue = 1;
        vehicleCountSpinBox.MaxValue = 100;
        vehicleCountSpinBox.Step = 1;
        vehicleCountSpinBox.Value = 1;
    }




    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.Pressed)
        {
            switch (currentMode)
            {
                case EditorMode.PlacingNode:
                    GD.Print("Click detectado en modo agregar nodo.");

                    Vector2 mousePos = GetViewport().GetMousePosition();
                    Vector2 localPos = GraphCanvas.ToLocal(mousePos);
                    GD.Print($"Posición del mouse: global={mousePos}, local={localPos}");

                    var node = NodePrefab.Instantiate<GraphNode>();
                    GD.Print("Instanciado nodo prefab");

                    node.Position = localPos;
                    node.NodeId = "N" + nodeCounter++;

                    if (!node.HasNode("Label"))
                    {
                        GD.PrintErr("ERROR: El prefab no tiene un hijo llamado 'Label'");
                    }
                    else
                    {
                        GD.Print("Label encontrado");
                        node.GetNode<Label>("Label").Text = node.NodeId;
                    }

                    Graph.AddNode(node);
                    GraphCanvas.AddChild(node);

                    GD.Print("Nodo agregado al grafo y al canvas");

                    currentMode = EditorMode.None;
                    break;

                case EditorMode.ConnectingNodes:
                    var clickedNode = GetClosestNodeToMouse();
                    if (clickedNode == null) return;

                    if (tempSelectedNode == null)
                    {
                        tempSelectedNode = clickedNode;
                    }
                    else
                    {
                        var edge = EdgePrefab.Instantiate<GraphEdge>();
                        edge.Initialize(tempSelectedNode, clickedNode, 1f);

                        Graph.AddEdge(tempSelectedNode.NodeId, clickedNode.NodeId, edge);
                        GraphCanvas.AddChild(edge);
                        GraphCanvas.MoveChild(edge, 0);


                        tempSelectedNode = null;
                        currentMode = EditorMode.None;
                    }
                    break;

                case EditorMode.None:
                    {
                        var edge = FindEdgeUnderMouse(GetLocalMousePosition());
                        SelectEdge(edge);
                        break;
                    }

                case EditorMode.DeletingNode:
                    {
                        var targetNode = GetClosestNodeToMouse();
                        if (targetNode == null)
                        {
                            return;
                        }

                        var edgesToRemove = new List<GraphEdge>();

                        foreach (var child in GraphCanvas.GetChildren())
                        {
                            if (child is GraphEdge edge && (edge.StartNode == targetNode || edge.EndNode == targetNode))
                                edgesToRemove.Add(edge);
                        }

                        foreach (var edge in edgesToRemove)
                        {
                            Graph.RemoveEdge(edge.StartNode, edge.EndNode);
                            edge.QueueFree();
                        }

                        Graph.RemoveNode(targetNode.NodeId);
                        targetNode.QueueFree();

                        currentMode = EditorMode.None;
                        break;
                    }

                case EditorMode.DeletingEdge:
                    {
                        var edge = FindEdgeUnderMouse(GetLocalMousePosition());
                        if (edge == null)
                        {
                            return;
                        }

                        Graph.RemoveEdge(edge.StartNode, edge.EndNode);
                        edge.QueueFree();
                        selectedEdge = null;

                        currentMode = EditorMode.None;
                        break;
                    }
            }
        }
    }


    private void SelectEdge(GraphEdge edge)
    {
        selectedEdge = edge;

        var weightSpin = GetNode<SpinBox>("UI/WeightSpinBox");
        var blockCheck = GetNode<CheckBox>("UI/BlockCheckBox");

        weightSpin.Visible = true;
        blockCheck.Visible = true;

        if (edge != null)
        {
            weightSpin.Editable = true;
            blockCheck.Disabled = false;

            weightSpin.SetValueNoSignal(edge.Weight);
            blockCheck.SetPressedNoSignal(edge.IsBlocked);
        }
        else
        {
            weightSpin.Editable = false;
            blockCheck.Disabled = true;

            weightSpin.SetValueNoSignal(0);
            blockCheck.SetPressedNoSignal(false);
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
        currentMode = EditorMode.PlacingNode;
        tempSelectedNode = null;
    }

    private void OnConnectNodesPressed()
    {
        currentMode = EditorMode.ConnectingNodes;
        tempSelectedNode = null;
    }

    private void OnDeleteNodePressed()
    {
        currentMode = EditorMode.DeletingNode;
        tempSelectedNode = null;
    }

    private void OnDeleteEdgePressed()
    {
        currentMode = EditorMode.DeletingEdge;
        tempSelectedNode = null;
    }

    private GraphNode GetClosestNodeToMouse()
    {
        foreach (var child in GraphCanvas.GetChildren())
        {
            if (child is GraphNode node && node.IsMouseOver())
            {
                return node;
            }
        }

        return null;
    }

    public void SetSelectedNode(GraphNode node) => firstSelectedNode = node;

    public bool IsInConnectingMode() => currentMode == EditorMode.ConnectingNodes;

    private void OnSimulateVehiclesPressed()
    {
        int count = (int)vehicleCountSpinBox.Value;
        Simulation.RunSimulation(count);
    }
}
