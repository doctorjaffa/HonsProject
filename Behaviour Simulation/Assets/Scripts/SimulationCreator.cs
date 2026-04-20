/* Class used to generate the SimulationData files with random values for each NPC */

using UnityEngine;
using System.IO;
using UnityEngine.AI;

public class SimulationCreator : MonoBehaviour
{
    [System.Serializable]
    public class NPCValues
    {
        public int id;
        public int bravery;
        public int distress;
        public int fear;
        public int anxiety;
        public int seat;
        public Vector3 spawnPoint;
    }

    [System.Serializable]
    public class NPCList
    {
        public NPCValues[] npc;
    }

    public NPCList npcs = new NPCList();

    // How many NPCs should be generated for this simulation
    public int npcCount = 0;

    public string saveNumber = "";

    public void Start()
    {
        // Set the length of NPCs being made, and find the spawner object in the scene for random spawn positions
        npcs.npc = new NPCValues[npcCount];
        Vector3 spawnerPosition = GameObject.FindFirstObjectByType<NPCSpawner>().transform.position;

        // Determine what the save number is based on the number of NPCs
        switch (npcCount)
        {
            case 10:
                saveNumber = "01";
                break;
            case 25:
                saveNumber = "02";
                break;
            case 100:
                saveNumber = "03";
                break;
            case 250:
                saveNumber = "04";
                break;
            case 500:
                saveNumber = "05";
                break;
            case 1000:
                saveNumber = "06";
                break;
            case 2500:
                saveNumber = "07";
                break;
            default:
                saveNumber = "Error";
                break;
        }

        for (int i = 0; i < npcCount; i++)
        {
            npcs.npc[i] = new NPCValues();

            // Randomise NPC traits
            npcs.npc[i].id = i;
            npcs.npc[i].bravery = Random.Range(1, 10);
            npcs.npc[i].distress = Random.Range(1, 10);
            npcs.npc[i].fear = Random.Range(1, 10);
            npcs.npc[i].anxiety = Random.Range(1, 10);

            // If the NPC count is 100 or below, assign them a seat also
            if (npcCount <= 100)
            {
                do
                {
                    npcs.npc[i].seat = Random.Range(0, 100);
                } while (CheckOccupied(i));
            }
            // If the NPC count is above 100, find a random spawn point
            else
            {
                // Keep searching for a possible spawn point until it is a valid point on the nav mesh
                bool foundSpawn = false;
                while (!foundSpawn)
                {
                    // Get a randomised point around the NPC_Spawner game object in a 10 unit radius
                    Vector3 possibleSpawn = spawnerPosition + new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
                    NavMeshHit hit;

                    // If the random spot is an available point on the nav mesh, set the npc's spawn point to that spot
                    if (NavMesh.SamplePosition(possibleSpawn, out hit, 10f, NavMesh.AllAreas))
                    {
                        foundSpawn = true;
                        npcs.npc[i].spawnPoint = hit.position;
                    }
                }
            }
        }

        // Print the NPC data to the JSON file
        OutputJSON();
    }

    public bool CheckOccupied(int currentIndex)
    {
        for (int i = 0; i < currentIndex; i++)
        {
            if (npcs.npc[currentIndex].seat == npcs.npc[i].seat) return true;
        }
        return false;
    }

    // Function to store the data in the class instance and write it to the file
    public void OutputJSON()
    {
        string strOutput = JsonUtility.ToJson(npcs, true);
        File.WriteAllText(Application.dataPath + "/Resources/SimulationData_" + saveNumber + ".JSON", strOutput);
    }
}
