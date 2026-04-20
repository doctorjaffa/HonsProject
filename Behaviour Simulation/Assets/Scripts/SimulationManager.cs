/* Control the overall flow of the simulation scene, updating each NPC and recording performance metrics */
using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    // List of NPCs in the scene
    private List<NPC_Behaviour> npcs = new List<NPC_Behaviour>();

    // Variables to measure per-tick performance
    private Stopwatch stopwatch = new Stopwatch();
    public double accumulated_ms = 0;
    public double avg_ms = 0;
    private double maxMs = 0;
    private string saveName = "";
    private double thisMs = 0;
    public int frame_count = 0;
    public int NUM_FRAMES_MEASURED = 400;
    private bool has_recorded = false;

    private JSONWriter writer;

    // When the crime commmitted event is fired, begin measuring performance
    private bool crime_committed = false;

    [SerializeField]
    private Transform criminal;

    private void OnEnable()
    {
        EventManager.OnCommitCrime += RespondToCrime;
    }

    private void OnDisable()
    {
        EventManager.OnCommitCrime -= RespondToCrime;
    }

    private void Start()
    {
        writer = GameObject.FindFirstObjectByType<JSONWriter>();
        Invoke("SetSaveFile", 0.5f);

        UnityEngine.Debug.Log("Simulation Data File: " + SimulationSettings.file.name + 
            "\nSimulation Type: " + SimulationSettings.isFuzzy.ToString() + 
            "\nSave Simulation: " + SimulationSettings.saveSimulation.ToString());
    }

    private void SetSaveFile()
    {
        // Store all NPCs in the scene and start the simulation
        npcs.AddRange(FindObjectsByType<NPC_Behaviour>(FindObjectsSortMode.None));

        // Set the save number according to what simulation was loaded in
        switch (npcs.Count)
        {
            case 10:
                saveName = "01";
                break;
            case 25:
                saveName = "02";
                break;
            case 100:
                saveName = "03";
                break;
            case 250:
                saveName = "04";
                break;
            case 500:
                saveName = "05";
                break;
            case 1000:
                saveName = "06";
                break;
            case 2500:
                saveName = "07";
                break;
            default:
                saveName = "Error";
                break;
        }

        writer.saveNumber = saveName;
    }

    private void RespondToCrime()
    {
        // Store all NPCs in the scene and start the simulation
        npcs.AddRange(FindObjectsByType<NPC_Behaviour>(FindObjectsSortMode.None));

        // Set the values of the distance and awareness for each NPC when the crime has been committed
        for (int i = 0; i < npcs.Count; i++)
        {
            npcs[i].SetAwarness(Vector2.Distance(npcs[i].transform.position, criminal.position), Mathf.Clamp01(Mathf.Exp(-npcs[i].distance * 0.25f)));
        }

        // Begin the simulation ticks
        crime_committed = true;
    }
    void FixedUpdate()
    {
        // Early return if the crime has not been committed yet
        if (!crime_committed) return;

        // Begin the stopwatch
        stopwatch.Restart();

        // Update each NPC's tick 
        for (int i = 0; i < npcs.Count; i++)
        {
            npcs[i].SimulationTick();
        }

        // Stop the stopwatch after every NPC has updated
        stopwatch.Stop();

        // Store the accumulated time for the tick to occur
        thisMs = stopwatch.Elapsed.TotalMilliseconds;
        frame_count++;

        // If the current iteration is the highest spike so far, record it
        if (thisMs > maxMs) maxMs = thisMs;

        // Add to the accumulated time 
        accumulated_ms += thisMs;

        // When enough frames have passed, record the final average time per tick
        if (frame_count >= NUM_FRAMES_MEASURED && !has_recorded)
        {
            has_recorded = true;
            avg_ms = accumulated_ms / NUM_FRAMES_MEASURED;

            writer.avg_ms = avg_ms;
            writer.maxMs = maxMs;
            writer.saveNumber = saveName;

            if (SimulationSettings.saveSimulation)
                writer.CalculateData();
            else 
                SceneManager.LoadScene("Menu");
        }
    }
}
