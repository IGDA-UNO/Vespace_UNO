using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewingPositionsManager : MonoBehaviour
{

    public List<Transform> viewingTransformsList = new List<Transform>();
    public List<GameObject> charactersList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        PositionAssigner();
    }

    public void PositionAssigner()
    {
        foreach (GameObject character in charactersList) {
            
            int currentSpot = Random.Range(0, viewingTransformsList.Count);
            character.GetComponent<NPCNavMesh>().myViewingTransform = viewingTransformsList[currentSpot];
            viewingTransformsList.RemoveAt(currentSpot);
        }
    }

}
