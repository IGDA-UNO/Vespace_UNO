using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotiveState : MonoBehaviour
{
    
    public Transform halo;
    public ParticleSystemRenderer haloParticleRenderer;
    public bool isApproachable;

    // Start is called before the first frame update
    void Start()
    {
        halo = gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        haloParticleRenderer = halo.GetComponent<ParticleSystemRenderer>();
        
        if (isApproachable)
        {
            haloParticleRenderer.material = Resources.Load("HoverHighlight_Yes") as Material;
        }
        else
        {
            haloParticleRenderer.material = Resources.Load("HoverHighlight_No") as Material;
        }

    }
}
