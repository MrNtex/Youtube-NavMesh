using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// Enum to define various states of the enemy
public enum EnemyState
{
    PassiveGoing,
    PassiveAction,
    ActivelyChasing,
    Searching,
    SearchingPassive,
    MeleeAttack,
}

public class AgentMovement : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent component

    // Serialized fields allow customization in the Unity Editor
    [SerializeField]
    private float maxRange, meleeRange, searchRadius; // Range values for different behaviors
    [SerializeField]
    private Transform target; // The target the enemy will interact with

    // Variables for internal use
    private Vector3 lastRayDirection, idleDefaultPosition; // To store ray direction and default position
    [SerializeField]
    private float searchTime = 5; // Time to search in one direction
    [SerializeField]
    private int maxSearchCount = 3; // Max number of times to search
    private Vector3 randomDirection; // Direction for random movement

    [SerializeField]
    private Transform[] passiveAreas; // Points of interest for passive behavior
    private Transform currentPassiveArea; // Current point of interest

    public EnemyState currentState; // Current state of the enemy

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        idleDefaultPosition = transform.position; // Set the default position
    }

    void Update()
    {
        if (!CheckForTarget()) // Check if the target is within range
        {
            switch (currentState)
            {
                case EnemyState.MeleeAttack:
                    // Implement melee attack logic here
                    break;
                case EnemyState.ActivelyChasing:
                    // Switch to searching if the target is lost
                    currentState = EnemyState.Searching;
                    break;
                case EnemyState.Searching:
                    // Continue moving towards the last known position of the target
                    if (Vector3.Distance(transform.position, navMeshAgent.destination) < 1.2f)
                    {
                        // Switch to passive search after reaching the last known position
                        currentState = EnemyState.SearchingPassive;
                        idleDefaultPosition = transform.position;
                        StartCoroutine(FindNewAreas());
                    }
                    break;
                case EnemyState.SearchingPassive:
                    // Do nothing and return while in passive search mode (It is being handled by FindNewAreas() coroutine)
                    return;
                case EnemyState.PassiveGoing:
                    // Move towards a passive area
                    if (Vector3.Distance(transform.position, navMeshAgent.destination) < 1.2f)
                    {
                        currentState = EnemyState.PassiveAction;
                        StartCoroutine(FindPassiveActivity(5)); // Start passive activity after reaching the area
                    }
                    break;
            }
        }
    }

    // Method to check if the target is within specified ranges and within line of sight
    bool CheckForTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < maxRange)
        {
            if (distance < meleeRange)
            {
                currentState = EnemyState.MeleeAttack;
                navMeshAgent.destination = transform.position;
                return false;
            }
            currentState = EnemyState.ActivelyChasing;
            Vector3 direction = target.position - transform.position;
            lastRayDirection = direction;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRange))
            {
                if (hit.transform == target)
                {
                    navMeshAgent.destination = target.position;
                    return true;
                }
            }
        }
        return false;
    }

    // Coroutine to find new areas to search
    private IEnumerator FindNewAreas()
    {
        int searchCount = 0;
        while (searchCount < maxSearchCount)
        {
            randomDirection = idleDefaultPosition + (Random.insideUnitSphere.normalized * Random.Range(1, searchRadius));
            randomDirection = new Vector3(randomDirection.x, transform.position.y, randomDirection.z);
            navMeshAgent.destination = randomDirection;
            yield return new WaitForSeconds(searchTime);
            searchCount++;
        }
        StartCoroutine(FindPassiveActivity(0)); // Transition to passive activity after searching
    }

    // Coroutine for determining passive actions
    private IEnumerator FindPassiveActivity(float cooldown = 0)
    {
        yield return new WaitForSeconds(cooldown);
        currentState = EnemyState.PassiveGoing;

        int rnd = Random.Range(0, passiveAreas.Length);
        currentPassiveArea = passiveAreas[rnd];
        navMeshAgent.destination = currentPassiveArea.position;
    }

    // Draw a ray in the Scene view for debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, lastRayDirection);
    }
}
