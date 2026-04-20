/* Spawn NPCs into the Simulation scene upon loading */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npc_prefab;
    public TextAsset textJSON;

    // Class to store NPC data being read in from JSON file
    [System.Serializable]
    public class NPC
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
        public NPC[] npc;
    }

    public NPCList thisNPCList = new NPCList();

    [SerializeField]
    private int num_npcs;

    private GameObject[] npcs;

    // Create a list to store all possible NPC spawn points
    private List<SpawnPoint> spawn_points = new List<SpawnPoint>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure the data is read in first before spawning NPCs
        Invoke("SpawnNPCs", 0.02f);
    }

    void SpawnNPCs()
    {
        // Add all of the spawn points to the list
        spawn_points.AddRange(FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None));

        // Read in the NPC file into the list
        thisNPCList = JsonUtility.FromJson<NPCList>(SimulationSettings.file.text);
        num_npcs = thisNPCList.npc.Count();

        npcs = new GameObject[num_npcs];
        // If the NPC count has exceeded 100, spawn them in any random point on the nav mesh
        if (num_npcs > 100)
        {
            // For each NPC
            for (int i = 0; i < num_npcs; i++)
            {
                // Spawn the NPC at their preset location
                npcs[i] = Instantiate(npc_prefab, thisNPCList.npc[i].spawnPoint, Quaternion.identity);
                // Set the NPC's internal traits from the file 
                npcs[i].GetComponent<NPC_Behaviour>().id = thisNPCList.npc[i].id;
                npcs[i].GetComponent<NPC_Behaviour>().bravery = thisNPCList.npc[i].bravery;
                npcs[i].GetComponent<NPC_Behaviour>().distress = thisNPCList.npc[i].distress;
                npcs[i].GetComponent<NPC_Behaviour>().fear = thisNPCList.npc[i].fear;
                npcs[i].GetComponent<NPC_Behaviour>().anxiety = thisNPCList.npc[i].anxiety;
            }
        }
        // Otherwise, use the preset seats to spawn NPCs
        else
        {
            // For each NPC, find a random free spot and spawn the NPC 
            for (int i = 0; i < num_npcs; i++)
            {
                // Set the NPCs spawn point to the chosen seat from the file 
                SpawnPoint spot = spawn_points.ElementAt(thisNPCList.npc[i].seat);
                if (spot != null)
                {
                    // The spot is now occupied
                    spot.is_occupied = true;
                    npcs[i] = Instantiate(npc_prefab, spot.transform.position, Quaternion.identity);
                    // Set the NPC's internal traits from the file 
                    npcs[i].GetComponent<NPC_Behaviour>().id = thisNPCList.npc[i].id;
                    npcs[i].GetComponent<NPC_Behaviour>().bravery = thisNPCList.npc[i].bravery;
                    npcs[i].GetComponent<NPC_Behaviour>().distress = thisNPCList.npc[i].distress;
                    npcs[i].GetComponent<NPC_Behaviour>().fear = thisNPCList.npc[i].fear;
                    npcs[i].GetComponent<NPC_Behaviour>().anxiety = thisNPCList.npc[i].anxiety;
                }
            }
        }
    }
}
