using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float maxRange;

    [SerializeField]
    private Transform target;

    // To store the ray's direction for drawing Gizmos
    private Vector3 lastRayDirection;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < maxRange)
        {
            // Create a ray from the agent to the target
            Vector3 direction = target.position - transform.position;
            lastRayDirection = direction; // Store the direction for Gizmos
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, maxRange))
            {
                Debug.Log(hit.transform);
                // Check if the raycast hit the target
                if (hit.transform == target)
                {
                    // Set the destination to the target's position
                    navMeshAgent.destination = target.position;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw the ray in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastRayDirection);
    }
}
