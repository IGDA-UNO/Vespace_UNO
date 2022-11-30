using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ensemble;

public class EmotiveState : MonoBehaviour
{
    
    public Transform halo;
    public ParticleSystemRenderer haloParticleRenderer;

    private Cast cast = new Cast { 
        "Male Noble Player", 
        "Female Noble Player", 
        "Servant Player", 
		"Marie-Catherine Bienfait, ticket taker",
		"Bruno Dufort, semainier",
		"Monsieur d'Ain d'Ygeste",
		"Monsieur d'Hautainville",
		"Monsieur de Gentilly",
		"Monsieur des Trois-Landes",
		"Monsieur d'Issy",
		"Madame de Blasé-l'Evêque",
		"Chérubin",
		"Madame de Cher-sur-Tendre",
		"Mademoiselle Eloïse de Hauteclaire",
		"Ninon",
		"Toinette",
		"Fruit-seller / la Fruitière",
		"Cellist",
		"L'Animateur",
		"Fanon la Poche",
		"Henriette Lavocat",
		"Jeannot la Panse",
		"Perso 2",
		"Seated Female Noble"
    };

    // Start is called before the first frame update
    void Start()
    {
        halo = gameObject.transform.GetChild(0);
    }

    public IEnumerator<object> RunApproachableUpdate()
    {
        yield return new WaitForSeconds(0);

        bool isApproachable = false;

        //Run Ensemble data to find out if this person is friends with the player.
        ENSEMBLE_UIHandler uiHandler = GameObject.Find("UIHandler").GetComponent<ENSEMBLE_UIHandler>();

        bool result;
        if (uiHandler.characterAvailable.TryGetValue(transform.parent.name, out result)) {
            Debug.Log("isApproachable: " + result);
            isApproachable = result;
        }

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

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(RunApproachableUpdate());
    }
}
