/* Binary Logic version of the behaviour logic */

using UnityEngine;

public class BinaryLogic : MonoBehaviour
{
    // Decision variables
    // Boolean triggered when criminal reaches specific point in scene
    public bool crime_committed = false;
    public NPC_ReactionState current_state = NPC_ReactionState.NONE;

    // Static variables 
    public float bravery = 0f;
    public float distress = 0f;
    public float anxiety = 0f;
    public float fear = 0f;

    public float distance = 0f;

    public void UpdateBinaryLogic(Transform criminal)
    {
        int fleeing_count = CheckForFleeingCrowd();
    
        // If the witness is close to the crime
        if (distance < 3f)
        {
            // If the witness is brave or they are distressed, choose fight
            if (bravery > 5 || distress > 5) current_state = NPC_ReactionState.FIGHT;
            // Else, if enough witness nearby are also fleeing, and the NPC is not overcome with fear, then choose flight
            else if (fleeing_count > 3 || fear < 6) current_state = NPC_ReactionState.FLIGHT;
            // Otherwise, freeze on the spot
            else current_state = NPC_ReactionState.FREEZE;
        }
        // If the witness is further away from the crime
        else
        {
            // If the witness is less distressed and braver, choose fight
            if (distress < 3 || bravery > 6) current_state = NPC_ReactionState.FIGHT;
            // Else, if the witness is distressed, or other witnesses are fleeing, then choose flight
            else if (distress > 6 || fleeing_count > 3) current_state = NPC_ReactionState.FLIGHT;
            // Otherwise, freeze on the spot
            else current_state = NPC_ReactionState.FREEZE;
        }
    }

    // Get the number of nearby fleeing NPCs 
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