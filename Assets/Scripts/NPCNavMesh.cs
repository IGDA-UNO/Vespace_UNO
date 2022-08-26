using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour
{
    [SerializeField] private Transform followTransform;
    public Transform[] viewingTransforms;
    public Transform[] taken = new Transform[9];
    public Transform myViewingTransform;
    public bool spotTaken;
    public bool isServant;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        SetViewingTransforms();

        navMeshAgent = GetComponent<NavMeshAgent>();

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

    public Transform RandomizeViewingPositions()
    {
        int i = 0;
        int randIndex = Random.Range(0, viewingTransforms.Length);

        while (taken[i] != null)
        {
            if (i < viewingTransforms.Length - 1)
            {
                taken[i] = viewingTransforms[randIndex];
                i++;
            }
        }

        return viewingTransforms[randIndex];
    }

    private void SetViewingTransforms()
    {
        viewingTransforms[0] = GameObject.Find("Position (1)").transform;
        viewingTransforms[1] = GameObject.Find("Position (2)").transform;
        viewingTransforms[2] = GameObject.Find("Position (3)").transform;
        viewingTransforms[3] = GameObject.Find("Position (4)").transform;
        viewingTransforms[4] = GameObject.Find("Position (5)").transform;
        viewingTransforms[5] = GameObject.Find("Position (6)").transform;
        viewingTransforms[6] = GameObject.Find("Position (7)").transform;
        viewingTransforms[7] = GameObject.Find("Position (8)").transform;
        viewingTransforms[8] = GameObject.Find("Position (9)").transform;

    }

    private void FindMySpot()
    {
        if (!spotTaken)
        {
            myViewingTransform = RandomizeViewingPositions();
        }

        spotTaken = true;
    }
}
