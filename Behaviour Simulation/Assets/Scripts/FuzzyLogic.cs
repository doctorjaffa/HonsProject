/* Fuzzy Logic version of the behaviour logic */

using UnityEngine;

public class FuzzyLogic : MonoBehaviour
{
    // Boolean triggered when criminal reaches specific point in scene
    public bool crime_committed = false;

    public NPC_ReactionState current_state = NPC_ReactionState.NONE;

    // Personality traits
    public float bravery = 0f;
    public float distress = 0f;
    public float anxiety = 0f;
    public float fear = 0f;

    // How close the NPC is to the crime 
    public float awareness;
    public float distance;

    public float close;
    public float medium;
    public float far;

    // Fuzzy scores
    public float fight_value;
    public float flight_value;
    public float freeze_value;

    // Decision variables
    private float decision_timer = 0f;
    private float decision_length = 1.5f; 

    public float momentary_fear;

    public void UpdateFuzzyLogic(Transform criminal)
    {
        // The more NPCs nearby fleeing, the higher the pressure to flee
        float social_pressure = Mathf.Clamp01(CheckForFleeingCrowd() / 8f);
        
        // Dynamic variables affected by internal traits and situational factors (proximity, group pressure)
        float situational_anxiety = Mathf.Clamp01(anxiety + (social_pressure * 0.3f));
        momentary_fear = Mathf.Clamp01(fear + (distance <= 3f ? 0.5f : 0f));

        // Sugeno Fuzzy Inference Rules
        // FIGHT - Closer to the crime, the braver the NPC, the less fear in the moment (FFFS), the less distressed
        fight_value = (awareness + bravery + (1 - momentary_fear) + distress) / 4f;

        // FLIGHT - Further from the crime, the more distressed, the more fear in the moment (FFFS), the more group pressure
        flight_value = (awareness + distress + momentary_fear + social_pressure) / 4f;

        // FREEZE - Medium distance to crime, the more anxious (BIS) and a moderate momentary fear
        freeze_value = (awareness * anxiety + (1 - Mathf.Abs(momentary_fear - 0.5f))) / 2f;

        // If enough time has passed, reconsider the NPC's decision
        decision_timer -= Time.deltaTime;
        if (decision_timer <= 0f)
        {
            decision_timer = decision_length;

            // Determine the NPC's reaction based on the given fuzzy scores
            if (fight_value >= flight_value && fight_value >= freeze_value) current_state = NPC_ReactionState.FIGHT;
            else if (flight_value >= fight_value && flight_value >= freeze_value) current_state = NPC_ReactionState.FLIGHT;
            else current_state = NPC_ReactionState.FREEZE;
        }
    }

    private int CheckForFleeingCrowd()
    {
        // Only check a total of 50 nearby NPCs (enough to have a major influence)
        Collider[] hits = new Collider[50];

        // Check how many NPCs are fleeing nearby
        int num_hits = Physics.OverlapSphereNonAlloc(transform.position, 3f, hits, LayerMask.GetMask("NPC"), QueryTriggerInteraction.Ignore);

        int fleeing_count = 0;

        // For each hit, make sure it is an NPC
        for (int i = 0; i < num_hits; i++)
        {
            NPC_Behaviour npc = hits[i].GetComponent<NPC_Behaviour>();
            // If the NPC is fleeing currently, increment the number of nearby fleeing neighbours
            if (npc != null && npc != this && npc.current_state == NPC_ReactionState.FLIGHT)
                fleeing_count++;
        }

        return fleeing_count;
    }
}