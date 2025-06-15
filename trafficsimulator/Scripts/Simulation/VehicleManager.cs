using Godot;
using System;

public partial class VehicleManager : Node
{
    [Export] public PackedScene VehiclePrefab;

    private TrafficGraph graph;

    public void Init(TrafficGraph graphReference)
    {
        graph = graphReference;
    }

    private Godot.Collections.Array<GraphNode> ConvertToGodotArray(MyList<GraphNode> path)
    {
        var result = new Godot.Collections.Array<GraphNode>();
        for (int i = 0; i < path.Count; i++)
        {
            result.Add(path[i]);
        }
        return result;
    }

    public bool SpawnVehicle(string startId, string endId)
    {
        var path = Pathfinding.CalculateDijkstra(graph, startId, endId);
        if (path.Count == 0)
            return false;

        var godotPath = ConvertToGodotArray(path);

        Vector2 offset = new Vector2(GD.Randf() * 10 - 5, GD.Randf() * 10 - 5);
        Vehicle vehicle = VehiclePrefab.Instantiate<Vehicle>();
        vehicle.GlobalPosition = godotPath[0].GlobalPosition + offset;
        vehicle.SetPath(godotPath);
        AddChild(vehicle);
        return true;
    }


    public void ClearVehicles()
    {
        foreach (var child in GetChildren())
        {
            if (child is Vehicle vehicle)
                vehicle.QueueFree();
        }
    }
}
