using Godot;
using System;
using System.Collections.Generic;

public partial class GraphEditorUI : Control
{
    [Export] public PackedScene NodePrefab;
    [Export] public PackedScene EdgePrefab;
    [Export] public Node2D GraphCanvas;
    [Export] public Graph Graph;

    private Node firstSelectedNode = null;
    private int nodeCounter = 0;
    private Edge selectedEdge = null;


    public override void _Ready()
    {
        // Conexión correcta de botones
        GetNode<Button>("UI/Agregar Nodo").Pressed += OnAddNodePressed;
        GetNode<Button>("UI/Conectar Nodos").Pressed += OnConnectNodesPressed;
        GetNode<Button>("UI/Eliminar Nodo").Pressed += OnDeleteNodePressed;
        GetNode<Button>("UI/Eliminar Arista").Pressed += OnDeleteEdgePressed;

        // Conexión correcta de otros controles
        GetNode<SpinBox>("UI/WeightSpinBox").ValueChanged += OnWeightChanged;
        GetNode<CheckBox>("UI/BlockCheckBox").Toggled += OnBlockToggled;

        // Ocultar controles inicialmente
        GetNode<SpinBox>("UI/WeightSpinBox").Visible = false;
        GetNode<CheckBox>("UI/BlockCheckBox").Visible = false;
    }
    private void OnDeleteNodePressed()
    {
        // Detectar nodo bajo el mouse
        Node node = GetClosestNodeToMouse();
        if (node == null)
        {
            GD.Print("No hay nodo cerca del cursor");
            return;
        }

        // 1) Lógica: eliminar del Graph
        Graph.RemoveNode(node.NodeId);

        // 2) Visual: quitar de escena y también quitar cualquier arista que referencie
        //   a) quitar todas las aristas hijas de GraphCanvas
        foreach (var child in GraphCanvas.GetChildren())
        {
            if (child is Edge edge &&
                (edge.StartNode == node || edge.EndNode == node))
            {
                edge.QueueFree();
            }
        }

        //   b) finalmente eliminar el nodo
        node.QueueFree();
    }

    private void OnDeleteEdgePressed()
    {
        Vector2 mousePos = GetLocalMousePosition();
        Edge edge = FindEdgeUnderMouse(mousePos);
        if (edge == null)
        {
            GD.Print("No hay arista cerca del cursor");
            return;
        }

        // 1) Lógica: eliminar en el Graph
        Graph.RemoveEdge(edge.StartNode, edge.EndNode);

        // 2) Visual: quitar la línea
        edge.QueueFree();
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb
            && mb.ButtonIndex == MouseButton.Left
            && mb.Pressed)
        {
            Edge clicked = FindEdgeUnderMouse(GetLocalMousePosition());
            SelectEdge(clicked);
        }
    }

    private Edge FindEdgeUnderMouse(Vector2 mousePos)
    {
        foreach (var child in GraphCanvas.GetChildren())
        {
            if (child is Edge edge)
            {
                // Calculamos la distancia del punto al segmento [A,B]
                Vector2 a = edge.Points[0];
                Vector2 b = edge.Points[1];
                float dist = DistancePointToSegment(mousePos, a, b);
                if (dist < 8f)  // tolerancia en píxeles
                    return edge;
            }
        }
        return null;
    }

    // Función auxiliar
    private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float ab2 = ab.LengthSquared();
        if (ab2 == 0)
            return p.DistanceTo(a);  // A y B coinciden

        // Proyección de (p–a) sobre ab, normalizada a [0,1]
        float t = (p - a).Dot(ab) / ab2;
        t = Mathf.Clamp(t, 0, 1);

        Vector2 projection = a + ab * t;
        return p.DistanceTo(projection);
    }



    private void SelectEdge(Edge edge)
    {
        selectedEdge = edge;
        var weightSpin = GetNode<SpinBox>("UI/WeightSpinBox");
        var blockCheck = GetNode<CheckBox>("UI/BlockCheckBox");

        if (edge != null)
        {
            weightSpin.Visible = true;
            blockCheck.Visible = true;

            // Usar métodos seguros para asignación inicial
            weightSpin.SetValueNoSignal(edge.Weight);
            blockCheck.SetPressedNoSignal(edge.IsBlocked);
        }
        else
        {
            weightSpin.Visible = false;
            blockCheck.Visible = false;
        }
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
        Node node = NodePrefab.Instantiate<Node>();
        node.Position = GetLocalMousePosition(); // ubicación en el lienzo
        node.NodeId = "N" + nodeCounter++;
        node.GetNode<Label>("Label").Text = node.NodeId;
        Graph.AddNode(node);
        GraphCanvas.AddChild(node);
    }

    private void OnConnectNodesPressed()
    {
        if (firstSelectedNode == null)
        {
            GD.Print("Selecciona el primer nodo");
            return;
        }

        // Buscar nodo más cercano al mouse (como ejemplo)
        Node secondNode = GetClosestNodeToMouse();
        if (secondNode == null || secondNode == firstSelectedNode)
        {
            GD.Print("Segundo nodo no válido");
            return;
        }

        // Crear arista visual
        Edge edge = EdgePrefab.Instantiate<Edge>();
        edge.Initialize(firstSelectedNode, secondNode, 1f); // peso fijo por ahora
        edge.Points = new Vector2[] {
         firstSelectedNode.GlobalPosition,
         secondNode.GlobalPosition
       };
        Graph.AddEdge(firstSelectedNode.NodeId, secondNode.NodeId, 1f);
        GraphCanvas.AddChild(edge);
        firstSelectedNode = null;
    }

    private Node GetClosestNodeToMouse()
    {
        Vector2 mousePos = GetLocalMousePosition();
        float minDist = 30f; // tolerancia de clic
        Node closest = null;

        foreach (Node child in GraphCanvas.GetChildren())
        {
            if (child is Node node)
            {
                if (mousePos.DistanceTo(node.Position) < minDist)
                {
                    minDist = mousePos.DistanceTo(node.Position);
                    closest = node;
                }
            }
        }
        return closest;
    }

    public void SetSelectedNode(Node node)
    {
        firstSelectedNode = node;
    }
}
