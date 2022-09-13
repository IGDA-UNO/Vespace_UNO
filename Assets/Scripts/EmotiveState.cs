using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensemble;

public class EmotiveState : MonoBehaviour
{
    
    public Transform halo;
    public ParticleSystemRenderer haloParticleRenderer;
    public bool isApproachable;
    public bool isAcquaintance;

    // Start is called before the first frame update
    void Start()
    {
        halo = gameObject.transform.GetChild(0);

        //Run Ensemble data to find out if this person is friends with the player.
        ENSEMBLE_UIHandler uiHandler = GameObject.Find("UIHandler").GetComponent<ENSEMBLE_UIHandler>();
        Predicate searchPredicate = new Predicate();
        searchPredicate.First = EnsemblePlayer.GetSelectedCharacter();

        List<Predicate> socialRecordData = uiHandler.data.ensemble.getSocialRecordCopyAtTimestep(0);
        List<Predicate> characterData = socialRecordData.FindAll(predicate => predicate.First == EnsemblePlayer.GetSelectedCharacter()); //.ToList();

        foreach (Predicate datum in characterData)
        {
            
            string[] predicateDebug = new string[] { datum.First, datum.Second, datum.Category, datum.Type};
            //bool value = datum.Value;
            Debug.Log("**emotive state. First: " + predicateDebug[0] + " second: " + predicateDebug[1] + " category: " + predicateDebug[2] + " type: " + predicateDebug[3]);
        }
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
