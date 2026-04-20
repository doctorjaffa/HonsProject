/* Class used to take multiple files and calculate the average metrics of that simulation data */

using UnityEngine;
using System.IO;

public class AverageResultsCollator : MonoBehaviour
{
    [System.Serializable]
    public class NPCList
    {
        public int fightCount;
        public int flightCount;
        public int freezeCount;

        public double performance_ms;
        public double minMs;
        public double maxMs;
    }

    public NPCList[] simulationLists;
    public NPCList averageList;
    public TextAsset[] resultsJSON;

    // JSON file name variables
    public string saveNumber;

    private void Start()
    {
        Invoke("CreateFile", 1f);
    }
    // Function to read in the given files, calculate averages and write them into the JSON file
    private void CreateFile()
    {
        simulationLists = new NPCList[3];

        simulationLists[0] = JsonUtility.FromJson<NPCList>(resultsJSON[0].text);
        simulationLists[1] = JsonUtility.FromJson<NPCList>(resultsJSON[1].text);
        simulationLists[2] = JsonUtility.FromJson<NPCList>(resultsJSON[2].text);

        averageList.fightCount = (simulationLists[0].fightCount + simulationLists[1].fightCount + simulationLists[2].fightCount) / 3;
        averageList.flightCount = (simulationLists[0].flightCount + simulationLists[1].flightCount + simulationLists[2].flightCount) / 3;
        averageList.freezeCount = (simulationLists[0].freezeCount + simulationLists[1].freezeCount + simulationLists[2].freezeCount) / 3;

        averageList.performance_ms = (simulationLists[0].performance_ms + simulationLists[1].performance_ms + simulationLists[2].performance_ms) / 3;
        averageList.maxMs = (simulationLists[0].maxMs + simulationLists[1].maxMs + simulationLists[2].maxMs) / 3;

        OutputJSON();
    }

    // Function to store the data in the class instance and write it to the file
    public void OutputJSON()
    {
        string strOutput = JsonUtility.ToJson(averageList, true);
        string saveName = SimulationSettings.isFuzzy ? "FuzzyResults" : "BinaryResults";
        File.WriteAllText(saveName + saveNumber + "_AVG.JSON", strOutput);
    }
}
