using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    PassiveGoing,
    PassiveAction,
    ActivelyChasing,
    Searching,
    SearchingPassive,
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

    [SerializeField]
    private float serachTime = 5;
    [SerializeField]
    private int maxSearchCount = 3;
    Vector3 randomDirection;

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
                currentState = EnemyState.SearchingPassive;
                idleDefaultPosition = transform.position;
                StartCoroutine(findNewAreas());
            }
        }else if(currentState == EnemyState.SearchingPassive)
        {
            return;
        }


    }
    private IEnumerator findNewAreas()
    {
        int searchCount = 0;
        while(searchCount < maxSearchCount)
        {
            randomDirection = idleDefaultPosition + (Random.insideUnitSphere.normalized * searchRadius);
            randomDirection = new Vector3(randomDirection.x, transform.position.y, randomDirection.z);
            navMeshAgent.destination = randomDirection;
            Debug.Log(randomDirection);
            yield return new WaitForSeconds(serachTime);
            searchCount++;
        }
        FindPassiveActivity();
    }
    private void FindPassiveActivity()
    {
        currentState = EnemyState.PassiveGoing;
    }
    void OnDrawGizmos()
    {
        // Draw the ray in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastRayDirection);
    }
}
