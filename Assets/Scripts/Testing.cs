using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TESTING TESTING");
        Debug.Log("TESTING: selected character is: " + EnsemblePlayer.GetSelectedCharacter());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
