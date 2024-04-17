using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmotiveHandler : MonoBehaviour
{
    public GameObject player;
    public GameObject[] characters;
    public GameObject closestCharacter;
    ENSEMBLE_UIHandler uiHandler; 

    public void Start(){
        uiHandler = GameObject.Find("UIHandler").GetComponent<ENSEMBLE_UIHandler>();
    }

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
        //if the object collides with the gazee collider and it has the "Emotive" tag,
        //the tag changes to indicate the object is in the player's "SightLine"
        if (other.gameObject.tag == "Emotive" )
        {
            other.gameObject.tag = "SightLine";
        }
        //if the object is in the "Sightline" and it is the closest object to the player
        if(other.gameObject.tag == "SightLine" && other.gameObject == FindClosest())
        {
           /* MeshRenderer emotive;
            emotive = other.gameObject.GetComponent<MeshRenderer>();
            emotive.enabled = true;
            Debug.Log("enabled");*/

            //HALO VERSION -- TURN HALO ON
            //other.gameObject.transform.GetChild(0).gameObject.SetActive(true);

            //MATERIAL COLOR VERSION -- CHANGE COLOR BASED ON AVAILABILITY! GREEN = AVAILABLE, RED = NOT AVAILABLE
            MeshRenderer characterToColorMeshRenderer = other.gameObject.GetComponentInParent<MeshRenderer>();



            bool result;
            bool isApproachable = false;

            //Going to do a bit of hackey game design magic!
            //This is what we normally do -- in the theatre
            //in the 'intro scene' we want to always turn the ticket taker green, even though there aren't actually any actions.
            Scene currentScene = SceneManager.GetActiveScene();
            if(currentScene.name != "Intro"){
                if (uiHandler.characterAvailable.TryGetValue(characterToColorMeshRenderer.gameObject.name, out result))
                {
                    //Debug.Log("The character " + characterToColorMeshRenderer.name + " is available: " + result);
                    isApproachable = result;
                }
            }
            else{
                //we are in the intro! Just say isApproachable is true!
                result = true;
                isApproachable = true;
            }

            if (isApproachable)
            {
                characterToColorMeshRenderer.material.color = Color.green;
            }
            else
            {
                characterToColorMeshRenderer.material.color = Color.red;
            }

            

            //Debug.Log("I AM NOW LOOKING AT " + characterToColorMeshRenderer.gameObject.name + "COLOR THEM GREEN OR WHATEVER!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if the object is no longer in the "Sightline" go back to the default state
        if (other.gameObject.tag == "SightLine")
        {
            other.gameObject.tag = "Emotive";
        }

        if (other.gameObject.tag == "Emotive")
        {
            /*MeshRenderer emotive;
            emotive = other.gameObject.GetComponent<MeshRenderer>();
            emotive.enabled = false;
            Debug.Log("disabled");*/

            //HALO VERSION -- TURN HALO OFF
            //other.gameObject.transform.GetChild(0).gameObject.SetActive(false);

            //MATERIAL COLOR VERSION -- CHANGE COLOR BACK TO WHITE
            MeshRenderer characterToColorMeshRenderer = other.gameObject.GetComponentInParent<MeshRenderer>();
            characterToColorMeshRenderer.material.color = Color.white;
        }
    }
}
