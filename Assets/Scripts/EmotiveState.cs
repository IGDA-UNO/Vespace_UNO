using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotiveState : MonoBehaviour
{
    public Transform yes;
    public Transform available;

    public Transform affinity;
   
    public Transform no;

    public bool isAvailable;

    public int howRed = 255;

    void OnAwake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        available = gameObject.transform.GetChild(0);
        yes = available.transform.GetChild(0);
        no = available.transform.GetChild(1);

        affinity = gameObject.transform.GetChild(1);
    
    }

    // Update is called once per frame
    void Update()
    {
        
        affinity.GetComponent<SpriteRenderer>().color = new Color(howRed,0,0);

        if (isAvailable)
        {
            yes.gameObject.SetActive(true);
            no.gameObject.SetActive(false);
        }
        else
        {
            yes.gameObject.SetActive(false);
            no.gameObject.SetActive(true);
        }

    }
}
