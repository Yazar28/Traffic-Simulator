using Godot;
using System;

public partial class SimulationManager : Node
{
    [Export]
    public Graph Graph;

    [Export]
    public VehicleManager VehicleManager;

    public void RunSimulation(int vehicleCount)
    {
        // Creamos un Godot.Collections.Array<string> e insertamos todas las llaves
        var keyArray = new Godot.Collections.Array<string>();
        foreach (var k in Graph.GetAllNodes().Keys)
            keyArray.Add(k);

        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();

        for (int i = 0; i < vehicleCount; i++)
        {
            int startIndex = rng.RandiRange(0, keyArray.Count - 1);
            int endIndex = rng.RandiRange(0, keyArray.Count - 1);
            if (startIndex == endIndex) continue;

            string startId = keyArray[startIndex];
            string endId = keyArray[endIndex];

            VehicleManager.SpawnVehicle(startId, endId);
        }
    }

}
