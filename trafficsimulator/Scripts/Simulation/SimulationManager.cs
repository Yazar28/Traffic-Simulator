using Godot;
using System;

public partial class SimulationManager : Node
{
    public TrafficGraph Graph;
    [Export] public VehicleManager VehicleManager;

    public async void RunSimulation(int count)
    {
        VehicleManager.ClearVehicles();

        var nodeIds = Graph.GetAllNodeIds();
        var rng = new Random();

        int created = 0;
        int attempts = 0;

        while (created < count && attempts < 100)
        {
            attempts++;
            if (nodeIds.Count < 2) return;

            string start = nodeIds[rng.Next(nodeIds.Count)];
            string end = nodeIds[rng.Next(nodeIds.Count)];
            while (end == start)
                end = nodeIds[rng.Next(nodeIds.Count)];

            if (VehicleManager.SpawnVehicle(start, end))
            {
                GD.Print($"✅ Vehículo creado de {start} a {end}");
                created++;
                await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
            }
            else
            {
                GD.Print($"❌ Falló la creación del vehículo de {start} a {end}");
            }
        }

        // ✅ Verificar al final si no se logró crear ninguno
        if (created == 0)
        {
            GD.Print("⚠️ No se pudo crear ningún vehículo. Verifique conexiones o bloqueos.");
            // Aquí podrías también activar un Popup o Label si querés mostrarlo en UI
        }
    }
}
