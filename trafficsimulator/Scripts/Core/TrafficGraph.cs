using Godot;
using System;

public partial class TrafficGraph : Node
{
    private MyMap<string, GraphNode> nodes = new MyMap<string, GraphNode>();

    public void AddNode(GraphNode node)
    {
        if (!nodes.ContainsKey(node.NodeId))
            nodes.Add(node.NodeId, node);
    }

    public void RemoveNode(string id)
    {
        if (!nodes.ContainsKey(id))
            return;

        var target = nodes[id];
        target.Edges.Clear();

        for (int i = 0; i < nodes.Keys.Count; i++)
        {
            string key = nodes.Keys[i];
            var other = nodes[key];

            for (int j = other.Edges.Count - 1; j >= 0; j--)
            {
                if (other.Edges[j].EndNode.NodeId == id)
                    other.Edges.RemoveAt(j);
            }
        }

        nodes.Remove(id);
    }

    public void AddEdge(string fromId, string toId, GraphEdge edge)
    {
        if (!nodes.ContainsKey(fromId) || !nodes.ContainsKey(toId))
            return;

        var from = nodes[fromId];
        var to = nodes[toId];

        edge.Initialize(from, to, edge.Weight);
        from.AddEdge(edge);
    }


    public void RemoveEdge(GraphNode from, GraphNode to)
    {
        for (int i = from.Edges.Count - 1; i >= 0; i--)
        {
            if (from.Edges[i].EndNode == to)
                from.Edges.RemoveAt(i);
        }
    }

    public GraphNode GetNode(string id)
    {
        return nodes.ContainsKey(id) ? nodes[id] : null;
    }

    public MyMap<string, GraphNode> GetAllNodes()
    {
        return nodes;
    }

    public Godot.Collections.Array<string> GetAllNodeIds()
    {
        var result = new Godot.Collections.Array<string>();
        for (int i = 0; i < nodes.Keys.Count; i++)
            result.Add(nodes.Keys[i]);
        return result;
    }

    public MyList<GraphNode> FindPath(string startId, string endId)
    {
        return new MyList<GraphNode>();
    }
}
