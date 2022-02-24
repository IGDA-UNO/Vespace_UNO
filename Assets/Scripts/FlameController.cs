using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameController : MonoBehaviour
{
    public int maxParticles ; 
    //private ParticleSystem[] particleSystems; 

    private List <ParticleSystem> particleSystems = new List<ParticleSystem>() ; 
    void Start()
    {
        foreach (Transform child in transform)
        {
            particleSystems.Add(child.GetComponent<ParticleSystem>()) ; 
        }
        DefineMaxParticles(maxParticles) ; 
    }

    void DefineMaxParticles(int maxP) {
        int length = particleSystems.Count ; 
        for(int i = 0; i<length; i++) {
            particleSystems[i].maxParticles = maxP ; 
        }
    }
}
