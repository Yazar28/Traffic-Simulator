using Godot;
using System;

public partial class Graph : Node
{
    private Godot.Collections.Dictionary<string, Node> nodes = new Godot.Collections.Dictionary<string, Node>();

    public void AddNode(Node node)
    {
        if (!nodes.ContainsKey(node.NodeId))
            nodes[node.NodeId] = node;
    }

    public void RemoveNode(string id)
    {
        if (!nodes.ContainsKey(id))
            return;

        // 1) Eliminar todas las aristas que parten de este nodo
        Node target = nodes[id];
        target.Edges.Clear();

        // 2) Eliminar todas las aristas entrantes: 
        foreach (var kv in nodes)
        {
            var other = kv.Value;
            for (int i = other.Edges.Count - 1; i >= 0; i--)
            {
                if (other.Edges[i].EndNode.NodeId == id)
                    other.Edges.RemoveAt(i);
            }
        }

        // 3) Finalmente eliminar el nodo
        nodes.Remove(id);
    }

    public void AddEdge(string fromId, string toId, float weight)
    {
        if (!nodes.ContainsKey(fromId) || !nodes.ContainsKey(toId))
            return;

        Node from = nodes[fromId];
        Node to = nodes[toId];
        Edge edge = new Edge();
        edge.Initialize(from, to, weight);
        from.AddEdge(edge);
    }

    public void RemoveEdge(Node from, Node to)
    {
        for (int i = from.Edges.Count - 1; i >= 0; i--)
        {
            if (from.Edges[i].EndNode == to)
                from.Edges.RemoveAt(i);
        }
    }

    public Node GetNode(string id) => nodes.ContainsKey(id) ? nodes[id] : null;
    public Godot.Collections.Dictionary<string, Node> GetAllNodes() => nodes;
}
