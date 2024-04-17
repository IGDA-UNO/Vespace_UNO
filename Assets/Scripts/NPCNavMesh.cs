using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    
    public Transform myViewingTransform;
    public bool isServant;
    ENSEMBLE_UIHandler ensembleUI;

    private NavMeshAgent navMeshAgent;

    //public ViewingPositionsManager vpm;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        ensembleUI = GameObject.Find("UIHandler").GetComponent<ENSEMBLE_UIHandler>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isServant && followTransform != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.destination = followTransform.position;
        }
        else if (!isServant && myViewingTransform != null && ensembleUI.hasActIPlayStarted)
        {
            //if the play has started, have them go to their seats
            navMeshAgent.destination = myViewingTransform.position;
        }
        else if (!isServant && myViewingTransform != null && !ensembleUI.hasActIPlayStarted)
        {
            //if the play hasn't started, have them generally head to their seats, but with a little bit of variance.
            Vector3 positionOffset = new Vector3();
            positionOffset.x = myViewingTransform.position.x + Random.Range(0f,1f);
            positionOffset.y = myViewingTransform.position.y;
            positionOffset.z = myViewingTransform.position.z + Random.Range(0f,1f);
            navMeshAgent.destination = positionOffset;
        }
       
        
    }
}
