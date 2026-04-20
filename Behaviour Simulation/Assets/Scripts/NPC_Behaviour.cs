/* General behaviour script that determines which action an NPC should take based on its current reaction state 
   Binary and Fuzzy behaviour scripts logic is controlled through this script */

using UnityEngine;
using UnityEngine.AI;

public class NPC_Behaviour : MonoBehaviour
{
    // Store rigid body component for movement
    private Rigidbody2D body;

    // Editable speed value 
    [SerializeField] private float speed = 3.0f;

    // Store the door's transform for fleeing behaviour
    [SerializeField] private Transform door;
    // Store the criminal's transform for fighting behaviour 
    [SerializeField] private Transform criminal;

    public NPC_ReactionState current_state;

    // Target position for NPC to move towards 
    private Vector2 target_pos;

    // Pathfinding agent to move towards target
    [SerializeField] private NavMeshAgent agent;

    // NPC traits (internal + dynamic)
    public int id;
    public int bravery;
    public int distress;
    public int fear;
    public int anxiety;
    public float awareness;
    public float distance;

    FuzzyLogic fuzzyLogic;
    BinaryLogic binaryLogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        // If the door and criminal have not been set in the editor, find them in the scene by their tags (if they exist)
        if (door == null) door = GameObject.FindWithTag("Door")?.transform;
        if (criminal == null) criminal = GameObject.FindWithTag("Criminal")?.transform;

        // Set default state to NONE
        current_state = NPC_ReactionState.NONE;

        // Get the navmesh agent component and ensure it stays within 2D space and does not rotate
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // For binary simulations, pass the trait values as-is (inside the script will determine true/false)
        if (!SimulationSettings.isFuzzy)
        {
            binaryLogic = GetComponent<BinaryLogic>();
            binaryLogic.bravery = bravery;
            binaryLogic.distress = distress;
            binaryLogic.fear = fear;
            binaryLogic.anxiety = anxiety;

        }
        // For fuzzy simulations, pass the trait values (normalized to between 0-1)
        else
        {
            fuzzyLogic = GetComponent<FuzzyLogic>();
            fuzzyLogic.distress = Mathf.Clamp01(distress / 10f);
            fuzzyLogic.bravery = Mathf.Clamp01(bravery / 10f);
            fuzzyLogic.fear = Mathf.Clamp01(fear / 10f);
            fuzzyLogic.anxiety = Mathf.Clamp01(anxiety / 10f);
        }
    }

    // FixedUpdate is called once per physics tick
    public void SimulationTick()
    {
        // Binary logic update
        if (!SimulationSettings.isFuzzy)
        {
            // Update the current frame and set reaction state
            binaryLogic.UpdateBinaryLogic(criminal);
            current_state = binaryLogic.current_state;
        }
        // Fuzzy logic update
        else
        {
            // Update the current frame and set reaction state
            fuzzyLogic.UpdateFuzzyLogic(criminal);
            current_state = fuzzyLogic.current_state;
        }

        // Switch to control which behaviour function to run 
        switch (current_state)
        {
            case NPC_ReactionState.NONE:
                break;
            case NPC_ReactionState.FREEZE:
                agent.isStopped = true;
                CowerResponse();
                break;
            case NPC_ReactionState.FLIGHT:
                agent.isStopped = false;
                MoveTowardTarget(door.transform.position);
                break;
            case NPC_ReactionState.FIGHT:
                agent.isStopped = false;
                MoveTowardTarget(criminal.position);
                break;
        }
    }

    // Shake on the spot to imitate shaking in fear 
    void CowerResponse()
    {
        Vector2 cower_shake = new Vector2(Mathf.Sin(Time.time * speed) * 0.1f, 0);
        body.MovePosition(body.position + cower_shake * Time.deltaTime);
    }

    // Move towards target in the scene (door or criminal)
    private void MoveTowardTarget(Vector2 target)
    {
        //if (!agent.hasPath)
        agent.SetDestination(target);
    }

    // Handle NPC collisions 
    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    // Set the behaviour specific variables
    public void SetAwarness(float distance, float awareness)
    {
        if (!SimulationSettings.isFuzzy) binaryLogic.distance = distance;
        else
        {
            // Determine how aware the NPC is (the closer they are to the criminal, the more aware they are)
            fuzzyLogic.distance = distance;
            fuzzyLogic.awareness = Mathf.Max(Mathf.Clamp01(1f - (distance / 10)), 0.01f); // Ensure the value cannot be 0 
        }
    }
}
