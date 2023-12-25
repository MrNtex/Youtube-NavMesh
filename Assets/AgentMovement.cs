using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private Transform target;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();   
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.destination = target.position;
    }
}