/* Write the performance results into an appropriate JSON file */

using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class JSONWriter : MonoBehaviour
{
    // The data to be written into the JSON file 
    [System.Serializable]
    public class NPCData
    {
        public int id;
        public string state;
        public int bravery;
        public int distress;
        public int fear;
        public int anxiety;
        public float awareness;
        public float fightValue;
        public float flightValue;
        public float freezeValue;
    }

    [System.Serializable]
    public class NPCList
    {
        public int fightCount;
        public int flightCount;
        public int freezeCount;

        public double performance_ms;
        public double maxMs;

        public NPCData[] data;
    }

    // Variables to store NPCs and the stored data
    public NPCList thisNPCList;
    private GameObject[] npcs;

    // JSON file name variables
    private string saveName;
    public string saveNumber;
    public string simNumber;

    public double avg_ms;
    public double maxMs;

    public void CalculateData()
    { 
        // Store all the NPCs in the scene
        npcs = GameObject.FindGameObjectsWithTag("NPC");

        // Set the length of the data (npc traits) to how many NPCs exist
        thisNPCList.data = new NPCData[npcs.Length];

        // For each NPC in the scene
        for (int i = 0; i < npcs.Length; i++) 
        {
            // Get the behaviour script from the NPC, and create a new instance of data
            NPC_Behaviour behaviour = npcs[i].GetComponent<NPC_Behaviour>();
            thisNPCList.data[i] = new NPCData();

            // Depending on their current state, increment the corresponding counter by one 
            switch (behaviour.current_state)
            {
                case NPC_ReactionState.NONE:
                    thisNPCList.data[i].state = "NONE";
                    break;
                case NPC_ReactionState.FIGHT:
                    thisNPCList.data[i].state = "FIGHT";
                    thisNPCList.fightCount++;
                    break;
                case NPC_ReactionState.FLIGHT:
                    thisNPCList.data[i].state = "FLIGHT";
                    thisNPCList.flightCount++;
                    break;
                case NPC_ReactionState.FREEZE:
                    thisNPCList.data[i].state = "FREEZE";
                    thisNPCList.freezeCount++;
                    break;
            }

            // Store the NPCs data into the corresponding spot
            thisNPCList.data[i].id = behaviour.id;
            thisNPCList.data[i].bravery = behaviour.bravery;
            thisNPCList.data[i].distress = behaviour.distress;
            thisNPCList.data[i].fear = behaviour.fear;
            thisNPCList.data[i].anxiety = behaviour.anxiety;

            thisNPCList.data[i].awareness = behaviour.GetComponent<FuzzyLogic>().awareness;
            thisNPCList.data[i].fightValue = behaviour.GetComponent<FuzzyLogic>().fight_value;
            thisNPCList.data[i].flightValue = behaviour.GetComponent<FuzzyLogic>().flight_value;
            thisNPCList.data[i].freezeValue = behaviour.GetComponent<FuzzyLogic>().freeze_value;

            thisNPCList.performance_ms = avg_ms;
            thisNPCList.maxMs = maxMs;
        }

        // When all the information has been stored, save to JSON file 
        OutputJSON();
    }

    // Function to store the data in the class instance and write it to the file
    public void OutputJSON()
    {
        string strOutput = JsonUtility.ToJson(thisNPCList, true);
        saveName = SimulationSettings.isFuzzy ? "FuzzyResults" : "BinaryResults";
        File.WriteAllText(saveName + saveNumber + ".JSON", strOutput);

        SceneManager.LoadScene("Menu");
    }
}
