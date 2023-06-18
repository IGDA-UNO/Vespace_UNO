using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensemble;

public class EmotiveStateIntro : MonoBehaviour
{
    
    public Transform halo;
    public ParticleSystemRenderer haloParticleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //halo = gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        //haloParticleRenderer = halo.GetComponent<ParticleSystemRenderer>();
        //haloParticleRenderer.material = Resources.Load("HoverHighlight_Yes") as Material;
    }
}