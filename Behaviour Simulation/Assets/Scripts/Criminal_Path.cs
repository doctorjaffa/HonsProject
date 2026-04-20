/* Control the criminal's movement when the scene first loads and pause them upon reaching destination */

using UnityEngine;

public class Criminal_Path : MonoBehaviour
{
    // List of waypoint nodes to follow
    [SerializeField]
    private Transform[] waypoints;

    // Victim actor to destroy when crime is committed
    [SerializeField] private GameObject victim;

    private float speed = 4f;
    // Index of current node the agent is on
    private int current_index = 0;
    // How close to the node before moving onto the next 
    private float reach_threshold = 0.06f;

    private Rigidbody2D body;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        victim = GameObject.FindWithTag("Victim");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move towards the current waypoint node 
        Transform target = waypoints[current_index];
        Vector2 direction = target.position - transform.position;

        body.MovePosition(body.position + direction.normalized * speed * Time.fixedDeltaTime);

        // If the agent is closer than the threshold
        if (direction.magnitude < reach_threshold)
        {
            // Move to next waypoint 
            current_index++;
            if (current_index >= waypoints.Length)
            {
                // Reached end of path, commit the crime 
                CommitCrime();
            }
        }
    }

    void CommitCrime()
    {
        // Commit the murder
        Object.Destroy(victim);
        // Broadcast the event (will be picked up by NPCs) 
        EventManager.TriggerCrimeResponse();
        // Stop moving 
        enabled = false;
    }
}
