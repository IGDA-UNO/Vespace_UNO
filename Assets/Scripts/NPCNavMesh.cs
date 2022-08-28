using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    
    public Transform myViewingTransform;
    public bool isServant;

    private NavMeshAgent navMeshAgent;

    //public ViewingPositionsManager vpm;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isServant && followTransform != null)
        {
            navMeshAgent.destination = followTransform.position;
        }
        else if (!isServant && myViewingTransform != null)
        {
            navMeshAgent.destination = myViewingTransform.position;
        }
    }
}
