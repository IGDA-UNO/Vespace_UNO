using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Camera playerCamera;
    public Camera fallBackCamera;
    private GameObject HUDObject;
    public GameObject SteamVRObjects;
    public GameObject playerIcon;
    public Texture NoblemanTexture;
    public Texture NoblewomanTexture;
    public Texture ServantTexture;

    void Awake(){
        //Initialize all interface references needed to display information
        //Initialisation de toutes les références à l'interface nécessaires pour l'affichage des informations
        HUDObject = GameObject.Find("HUD");

        //figure out what image to display
        //playerIcon.GetComponent<RawImage>().texture = 

        if(EnsemblePlayer.GetSelectedCharacter() == EnsemblePlayer.SERVANT){
            playerIcon.GetComponent<RawImage>().texture = ServantTexture;
        }
        else if (EnsemblePlayer.GetSelectedCharacter() == EnsemblePlayer.MALE_NOBLE)
        {
            playerIcon.GetComponent<RawImage>().texture = NoblemanTexture;
        }
        else if (EnsemblePlayer.GetSelectedCharacter() == EnsemblePlayer.FEMALE_NOBLE)
        {
            playerIcon.GetComponent<RawImage>().texture = NoblewomanTexture;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("TEST: SteamVRObjects is enabled: " + SteamVRObjects.activeSelf);
        if (!SteamVRObjects.activeSelf)
        {
            Debug.Log("NO PLAYER CAMERA!?!?!");
            playerCamera = fallBackCamera;
        }
        placeHud();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void placeHud()
    {
        HUDObject.transform.rotation = playerCamera.transform.rotation;
        HUDObject.transform.SetParent(playerCamera.transform);
        HUDObject.transform.localPosition = new Vector3(-0.25f, 0.22f, 0.5f);
    }
}
