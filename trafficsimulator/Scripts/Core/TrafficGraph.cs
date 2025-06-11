using Godot;
using System;

public partial class TrafficGraph : Node
{
    // Diccionario de nodos: clave=NodeId, valor=instancia de GraphNode
    private Godot.Collections.Dictionary<string, GraphNode> nodes = new Godot.Collections.Dictionary<string, GraphNode>();

    // Añade un nodo al grafo
    public void AddNode(GraphNode node)
    {
        if (!nodes.ContainsKey(node.NodeId))
            nodes[node.NodeId] = node;
    }

    // Elimina un nodo y todas sus aristas asociadas
    public void RemoveNode(string id)
    {
        if (!nodes.ContainsKey(id))
            return;

        // 1) Limpiar aristas salientes
        var target = nodes[id];
        target.Edges.Clear();

        // 2) Limpiar aristas entrantes
        foreach (var kv in nodes)
        {
            var other = kv.Value;
            for (int i = other.Edges.Count - 1; i >= 0; i--)
            {
                if (other.Edges[i].EndNode.NodeId == id)
                    other.Edges.RemoveAt(i);
            }
        }

        // 3) Eliminar nodo del diccionario
        nodes.Remove(id);
    }

    // Añade una arista lógica entre dos nodos
    public void AddEdge(string fromId, string toId, float weight)
    {
        if (!nodes.ContainsKey(fromId) || !nodes.ContainsKey(toId))
            return;

        var from = nodes[fromId];
        var to = nodes[toId];
        var edge = new GraphEdge();
        edge.Initialize(from, to, weight);
        from.AddEdge(edge);
    }

    // Elimina una arista lógica entre dos nodos
    public void RemoveEdge(GraphNode from, GraphNode to)
    {
        for (int i = from.Edges.Count - 1; i >= 0; i--)
        {
            if (from.Edges[i].EndNode == to)
                from.Edges.RemoveAt(i);
        }
    }

    // Obtiene un nodo por su identificador
    public GraphNode GetNode(string id) => nodes.ContainsKey(id) ? nodes[id] : null;

    // Retorna todos los nodos del grafo
    public Godot.Collections.Dictionary<string, GraphNode> GetAllNodes() => nodes;
}
