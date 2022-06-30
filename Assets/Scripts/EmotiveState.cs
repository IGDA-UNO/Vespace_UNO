using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotiveState : MonoBehaviour
{
    public Transform credibility;
    public Transform emulation;
    public Transform curiosity;
    public Transform affinity;
    public Transform esteem;
    public Transform allie;
    public Transform rival;

    void OnAwake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        credibility = gameObject.transform.GetChild(0);
        emulation = gameObject.transform.GetChild(1);
        curiosity = gameObject.transform.GetChild(2);
        affinity = gameObject.transform.GetChild(3);
        esteem = gameObject.transform.GetChild(4);
        allie = gameObject.transform.GetChild(5);
        rival = gameObject.transform.GetChild(6);
    }

    // Update is called once per frame
    void Update()
    {
        credibility.GetComponent<SpriteRenderer>().color = Color.red;
        emulation.GetComponent<SpriteRenderer>().color = Color.green;
        curiosity.GetComponent<SpriteRenderer>().color = Color.blue;
        affinity.GetComponent<SpriteRenderer>().color = Color.yellow;
        esteem.GetComponent<SpriteRenderer>().color = Color.magenta;

    }
}
