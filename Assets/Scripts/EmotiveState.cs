using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensemble;

public class EmotiveState : MonoBehaviour
{
    
    public Transform halo;
    public ParticleSystemRenderer haloParticleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        halo = gameObject.transform.GetChild(0);
    }

    bool getIsApproachable(string characterName)
    {
        bool isApproachable = false;

        //Run Ensemble data to find out if this person is friends with the player.
        ENSEMBLE_UIHandler uiHandler = GameObject.Find("UIHandler").GetComponent<ENSEMBLE_UIHandler>();
        Predicate searchPredicate = new Predicate();
        searchPredicate.First = EnsemblePlayer.GetSelectedCharacter();

        List<Predicate> socialRecordData = uiHandler.data.ensemble.getSocialRecordCopyAtTimestep(0);
        List<Predicate> characterData = socialRecordData.FindAll(predicate => predicate.Category == "role" && predicate.First == characterName); //.ToList();

        foreach (Predicate datum in characterData)
        {

            if (datum.Type == "stranger" && datum.Value is bool && datum.Value is true) {
                isApproachable = false;
            } else if (datum.Type == "acquaintance" && datum.Value is bool && datum.Value is true) {
                isApproachable = true;
            }
        }

        return isApproachable;
    }

    // Update is called once per frame
    void Update()
    {
        bool isApproachable = getIsApproachable(transform.parent.name);
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
