using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewingPositionsManager : MonoBehaviour
{
    public Transform[] viewingTransforms;
    public Transform[] taken = new Transform[9];
    public NPCNavMesh[] person;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        PopulateTaken();
    }

    public Transform RandomizeViewingPositions()
    {
        
        int randIndex = Random.Range(0, viewingTransforms.Length - 1);
       
        return viewingTransforms[randIndex];
    }

    public void PopulateTaken()
    {
        int i = 0;
        while (i <= viewingTransforms.Length - 1)
        {
           /* if (person[i].SendMySpot() != taken[0] &&
                person[i].SendMySpot() != taken[1] &&
                person[i].SendMySpot() != taken[2] &&
                person[i].SendMySpot() != taken[3] &&
                person[i].SendMySpot() != taken[4] &&
                person[i].SendMySpot() != taken[5] &&
                person[i].SendMySpot() != taken[6] &&
                person[i].SendMySpot() != taken[7] &&
                person[i].SendMySpot() != taken[8])
            {*/
                taken[i] = person[i].SendMySpot();
                
                i++;
            //}
        }
        
    }
}
