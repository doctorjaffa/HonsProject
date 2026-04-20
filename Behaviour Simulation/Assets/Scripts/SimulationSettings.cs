/* Static class passes the simulation settings from the Menu scene to the Simulation scene */

using UnityEngine;

public static class SimulationSettings
{
    public static TextAsset file = Resources.Load<TextAsset>("SimulationData_01");
    public static bool isFuzzy = false;
    public static bool saveSimulation = false;
}
