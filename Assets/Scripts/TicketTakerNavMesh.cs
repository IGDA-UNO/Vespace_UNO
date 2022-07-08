using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;

public class TicketTakerNavMesh : MonoBehaviour
{
    [SerializeField] private Transform movePositionTransform;
    public Player player;
    private NavMeshAgent navMeshAgent;
    public GameObject playerHead;
    public SphereCollider playerColl; 
    public float speed = 0.1f;

    public bool kickedOut = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerHead = GameObject.Find("HeadCollider");
        playerColl = playerHead.GetComponent<SphereCollider>();
        navMeshAgent.destination = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!kickedOut)
        {
            //ticket taker remains in place
            navMeshAgent.destination = transform.position;
        }
        else
        {
            //tickettaker moves toward the player to escort them out
            navMeshAgent.destination = movePositionTransform.position;

            //player is escorted out of the theatre
            Vector3 personalSpace = new Vector3(-0.5f, 0, 0);
            player.transform.position = transform.position + personalSpace;
        }


    }
    //if player collides with ticket taker, they get kicked out
    private void OnTriggerEnter(Collider other)
    {
        if (playerColl == other)
        {
            kickedOut = true;
        }
    }
}
