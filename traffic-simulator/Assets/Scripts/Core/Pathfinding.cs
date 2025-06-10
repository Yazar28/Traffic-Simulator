using Godot;
using System;

public partial class Pathfinding : Node
{
    public static Godot.Collections.Array<Node> Dijkstra(Graph graph, string startId, string endId)
    {
        var nodes = graph.GetAllNodes();
        var distances = new Godot.Collections.Dictionary<string, float>();
        var previous = new Godot.Collections.Dictionary<string, string>();
        var unvisited = new Godot.Collections.Array<string>();

        foreach (string id in nodes.Keys)
        {
            distances[id] = float.PositiveInfinity;
            previous[id] = null;
            unvisited.Add(id);
        }

        distances[startId] = 0;

        while (unvisited.Count > 0)
        {
            string currentId = GetMinDistanceNode(distances, unvisited);
            if (currentId == endId)
                break;

            unvisited.Remove(currentId);
            Node currentNode = nodes[currentId];

            foreach (Edge edge in currentNode.Edges)
            {
                string neighborId = edge.EndNode.NodeId;
                if (!unvisited.Contains(neighborId)) continue;

                float alt = distances[currentId] + edge.Weight;
                if (alt < (float)distances[neighborId])
                {
                    distances[neighborId] = alt;
                    previous[neighborId] = currentId;
                }
            }
        }

        return BuildPath(previous, nodes, startId, endId);
    }

    private static string GetMinDistanceNode(Godot.Collections.Dictionary<string, float> distances, Godot.Collections.Array<string> unvisited)
    {
        float minDistance = float.PositiveInfinity;
        string minNode = null;
        foreach (string id in unvisited)
        {
            if ((float)distances[id] < minDistance)
            {
                minDistance = (float)distances[id];
                minNode = id;
            }
        }
        return minNode;
    }

    private static Godot.Collections.Array<Node> BuildPath(Godot.Collections.Dictionary<string, string> previous, Godot.Collections.Dictionary<string, Node> nodes, string startId, string endId)
    {
        var path = new Godot.Collections.Array<Node>();
        string currentId = endId;

        while (currentId != null)
        {
            path.Insert(0, nodes[currentId]);
            currentId = previous[currentId];
        }

        if (path.Count == 0 || path[0].NodeId != startId)
            return new Godot.Collections.Array<Node>(); // No path

        return path;
    }
}
