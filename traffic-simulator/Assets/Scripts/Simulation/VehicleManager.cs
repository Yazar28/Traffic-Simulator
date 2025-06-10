using Godot;
using System;

public partial class VehicleManager : Node
{
    [Export]
    public PackedScene VehiclePrefab;

    private Graph graph;

    public void Init(Graph graphReference)
    {
        graph = graphReference;
    }

    public void SpawnVehicle(string startId, string endId)
    {
        var path = Pathfinding.Dijkstra(graph, startId, endId);
        if (path.Count == 0) return;

        Vehicle vehicle = VehiclePrefab.Instantiate<Vehicle>();
        vehicle.GlobalPosition = path[0].GlobalPosition;
        vehicle.SetPath(path);
        AddChild(vehicle);
    }
}
