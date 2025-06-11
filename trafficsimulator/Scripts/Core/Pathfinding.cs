using Godot;
using System;

public partial class Pathfinding : Node
{
    // Calcula la ruta más corta usando Dijkstra
    public static Godot.Collections.Array<GraphNode> CalculateDijkstra(TrafficGraph graph, string startId, string endId)
    {
        var nodes = graph.GetAllNodes();
        var distances = new Godot.Collections.Dictionary<string, float>();
        var previous = new Godot.Collections.Dictionary<string, string>();
        var unvisited = new Godot.Collections.Array<string>();

        // Inicialización
        foreach (var id in nodes.Keys)
        {
            distances[id] = float.PositiveInfinity;
            previous[id] = null;
            unvisited.Add(id);
        }
        distances[startId] = 0;

        // Bucle principal
        while (unvisited.Count > 0)
        {
            string currentId = GetMinDistanceNode(distances, unvisited);
            if (currentId == endId)
                break;

            unvisited.Remove(currentId);
            var currentNode = nodes[currentId];

            foreach (var edge in currentNode.Edges)
            {
                string neighborId = edge.EndNode.NodeId;
                if (!unvisited.Contains(neighborId)) continue;

                float alt = distances[currentId] + edge.Weight;
                if (alt < distances[neighborId])
                {
                    distances[neighborId] = alt;
                    previous[neighborId] = currentId;
                }
            }
        }

        return BuildPath(previous, nodes, startId, endId);
    }

    // Encuentra el nodo sin visitar con la distancia mínima
    private static string GetMinDistanceNode(Godot.Collections.Dictionary<string, float> distances, Godot.Collections.Array<string> unvisited)
    {
        float minDist = float.PositiveInfinity;
        string minNode = null;
        foreach (var id in unvisited)
        {
            if (distances[id] < minDist)
            {
                minDist = distances[id];
                minNode = id;
            }
        }
        return minNode;
    }

    // Reconstruye la ruta desde el mapa "previous"
    private static Godot.Collections.Array<GraphNode> BuildPath(Godot.Collections.Dictionary<string, string> previous, Godot.Collections.Dictionary<string, GraphNode> nodes, string startId, string endId)
    {
        var path = new Godot.Collections.Array<GraphNode>();
        var currentId = endId;

        while (currentId != null)
        {
            path.Insert(0, nodes[currentId]);
            currentId = previous[currentId];
        }

        if (path.Count == 0 || path[0].NodeId != startId)
            return new Godot.Collections.Array<GraphNode>();

        return path;
    }
}
