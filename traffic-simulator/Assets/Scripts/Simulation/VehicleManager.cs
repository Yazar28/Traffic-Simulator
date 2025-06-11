using Godot;
using System;

public partial class VehicleManager : Node
{
    [Export] public PackedScene VehiclePrefab;

    private TrafficGraph graph;

    // Inicializa referencia al grafo lógico
    public void Init(TrafficGraph graphReference)
    {
        graph = graphReference;
    }

    // Crea un vehículo y le asigna una ruta calculada
    public void SpawnVehicle(string startId, string endId)
    {
        var path = Pathfinding.CalculateDijkstra(graph, startId, endId);
        if (path.Count == 0)
            return;

        Vehicle vehicle = VehiclePrefab.Instantiate<Vehicle>();
        vehicle.GlobalPosition = path[0].GlobalPosition;
        vehicle.SetPath(path);
        AddChild(vehicle);
    }
}
