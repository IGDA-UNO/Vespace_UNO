using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    
    public Transform myViewingTransform;
    public Transform candidateViewingTransform;
    public bool spotTaken;
    public bool isServant;

    private NavMeshAgent navMeshAgent;

    public ViewingPositionsManager vpm;

    void Start()
    {
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        myViewingTransform = null;
        candidateViewingTransform = null;
        FindMySpot();
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

    private void FindMySpot()
    {
       // candidateViewingTransform = vpm.RandomizeViewingPositions();

        while (myViewingTransform == null)
        {
            candidateViewingTransform = vpm.RandomizeViewingPositions();
            if (candidateViewingTransform != vpm.taken[0] &&
                candidateViewingTransform != vpm.taken[1] &&
                candidateViewingTransform != vpm.taken[2] &&
                candidateViewingTransform != vpm.taken[3] &&
                candidateViewingTransform != vpm.taken[4] &&
                candidateViewingTransform != vpm.taken[5] &&
                candidateViewingTransform != vpm.taken[6] &&
                candidateViewingTransform != vpm.taken[7] &&
                candidateViewingTransform != vpm.taken[8])
            {
                myViewingTransform = candidateViewingTransform;
            }
            else
            {
                myViewingTransform = null;
            }
        }
    }

    public Transform SendMySpot()
    {
        if (myViewingTransform != null)
        {
            return myViewingTransform;
        }
        else
        {
            return null;
        }
        

    }
}
