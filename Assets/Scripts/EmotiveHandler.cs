using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotiveHandler : MonoBehaviour
{
    public GameObject player;
    public GameObject[] characters;
    public GameObject closestCharacter; 

    public GameObject FindClosest()
    {

        GameObject closest = null;
        
        characters = GameObject.FindGameObjectsWithTag("SightLine");

        float distance = Mathf.Infinity;
        Vector3 position = player.transform.position;
        foreach (GameObject c in characters)
        {
            Vector3 diff = c.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = c;
                distance = curDistance;
            }
        }

        closestCharacter = closest;
        return closest;

    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Emotive" )
        {
            other.gameObject.tag = "SightLine";
        }

        if(other.gameObject.tag == "SightLine" && other.gameObject == FindClosest())
        {
            MeshRenderer emotive;
            emotive = other.gameObject.GetComponent<MeshRenderer>();
            emotive.enabled = true;
            Debug.Log("enabled");

            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SightLine")
        {
            other.gameObject.tag = "Emotive";
        }

        if (other.gameObject.tag == "Emotive")
        {
            MeshRenderer emotive;
            emotive = other.gameObject.GetComponent<MeshRenderer>();
            emotive.enabled = false;
            Debug.Log("disabled");

            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
