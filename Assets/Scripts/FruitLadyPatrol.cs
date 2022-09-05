using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FruitLadyPatrol : MonoBehaviour
{
   

    public Transform[] points;
    public int current;
    public Transform cp;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        current = 0;
        InvokeRepeating(nameof(Current), 30f, 30f);
    }

    // Update is called once per frame
    private void Update()
    {
        navMeshAgent.destination = points[current].position;

        cp = points[current];
    }

    private void Current()
    {
        current = Random.Range(0, points.Length);
    }
}
