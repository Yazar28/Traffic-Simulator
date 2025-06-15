using Godot;
using System;

public partial class Pathfinding : Node
{
    public static MyList<GraphNode> CalculateDijkstra(TrafficGraph graph, string startId, string endId)
    {
        var nodes = graph.GetAllNodes();
        var distances = new MyMap<string, float>();
        var previous = new MyMap<string, string>();
        var unvisited = new MyList<string>();

        for (int i = 0; i < nodes.Keys.Count; i++)
        {
            string id = nodes.Keys[i];
            distances.Add(id, float.PositiveInfinity);
            previous.Add(id, null);
            unvisited.Add(id);
        }
        distances[startId] = 0;

        while (unvisited.Count > 0)
        {
            string currentId = GetMinDistanceNode(distances, unvisited);
            if (currentId == endId)
                break;

            for (int i = 0; i < unvisited.Count; i++)
            {
                if (unvisited[i] == currentId)
                {
                    unvisited.RemoveAt(i);
                    break;
                }
            }

            var currentNode = nodes[currentId];

            for (int i = 0; i < currentNode.Edges.Count; i++)
            {
                var edge = currentNode.Edges[i];
                string neighborId = edge.EndNode.NodeId;

                GD.Print($"Evaluando arista desde {currentId} hacia {neighborId} - Peso: {edge.Weight}");

                bool isUnvisited = false;
                for (int j = 0; j < unvisited.Count; j++)
                    if (unvisited[j] == neighborId)
                        isUnvisited = true;

                if (!isUnvisited)
                    continue;

                float alt = distances[currentId] + edge.Weight;
                if (alt < distances[neighborId])
                {
                    distances[neighborId] = alt;
                    previous[neighborId] = currentId;
                }
            }
        }

        return BuildPath(previous, nodes, startId, endId); // ✅ ahora sí está dentro del método
    }

    private static string GetMinDistanceNode(MyMap<string, float> distances, MyList<string> unvisited)
    {
        float minDist = float.PositiveInfinity;
        string minNode = null;

        for (int i = 0; i < unvisited.Count; i++)
        {
            string id = unvisited[i];
            float dist = distances[id];
            if (dist < minDist)
            {
                minDist = dist;
                minNode = id;
            }
        }

        return minNode;
    }

    private static MyList<GraphNode> BuildPath(MyMap<string, string> previous, MyMap<string, GraphNode> nodes, string startId, string endId)
    {
        var path = new MyList<GraphNode>();
        string currentId = endId;

        while (currentId != null)
        {
            path.Add(nodes[currentId]);
            currentId = previous[currentId];
        }

        if (path.Count == 0 || path[path.Count - 1].NodeId != startId)
            return new MyList<GraphNode>();

        var reversed = new MyList<GraphNode>();
        for (int i = path.Count - 1; i >= 0; i--)
            reversed.Add(path[i]);

        return reversed;
    }
}
