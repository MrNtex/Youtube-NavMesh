using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    ActivelyChasing,
    Searching,
    meleeAttack,
}
public class AgentMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float maxRange, meleeRange, searchRadius;

    [SerializeField]
    private Transform target;

    // To store the ray's direction for drawing Gizmos
    private Vector3 lastRayDirection, idleDefaultPosition;

    public EnemyState currentState;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        idleDefaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < maxRange)
        {
            if(distance < meleeRange)
            {
                currentState = EnemyState.meleeAttack;
                navMeshAgent.destination = transform.position;
                return;
            }
            currentState = EnemyState.ActivelyChasing;
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
        }else if(currentState == EnemyState.ActivelyChasing)
        {
            // We have lost the target, so start searching
            currentState = EnemyState.Searching;
        }
        else if(currentState == EnemyState.Searching)
        {
            Debug.Log($"Searching... {Vector3.Distance(transform.position, navMeshAgent.destination)}");
            // We are searching, so keep moving in the last known direction
            if(Vector3.Distance(transform.position, navMeshAgent.destination) < 1.2f)
            {
                // We have reached the last known position, so stop searching
                currentState = EnemyState.Idle;
                idleDefaultPosition = transform.position;
            }
        }else if(currentState == EnemyState.Idle && Vector3.Distance(transform.position, navMeshAgent.destination) > 1.2f)
        {
            // We are idle, travel to random positions nearly in hope of finding the target
            Vector3 randomDirection = idleDefaultPosition + (Random.insideUnitSphere * searchRadius);
            Debug.Log(randomDirection);
            navMeshAgent.destination = randomDirection;
        }


    }

    void OnDrawGizmos()
    {
        // Draw the ray in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastRayDirection);
    }
}
