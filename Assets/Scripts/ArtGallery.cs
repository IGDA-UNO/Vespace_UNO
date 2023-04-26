using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ArtGallery : MonoBehaviour
{
    public GameObject theaterPlans;
    private GameObject artGalleryObject;
    public GameObject SteamVRObjects;
    public GameObject planImage;
    public Camera playerCamera;
    public Camera fallBackCamera;
    public HUD hud;
    public GameObject backstageLeft;
    public GameObject leftHand;

    int galleryIndex = 0;

    public List<Texture> theatreImages = new List<Texture>();

    public ViewingPositionsManager vm;
    public ENSEMBLE_UIHandler ensembleUI;

    void Awake()
    {
        //Initialize all interface references needed to display information
        //Initialisation de toutes les références à l'interface nécessaires pour l'affichage des informations
        artGalleryObject = GameObject.Find("ArtGallery");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!SteamVRObjects.activeSelf)
        {
            playerCamera = fallBackCamera;
        }

        UpdatePlanToDisplay();
    }

    public void PreviousButtonPushed(){
       
        galleryIndex -= 1;
        if(galleryIndex < 0){
            //galleryIndex = theatreImages.Count - 1;
            galleryIndex = 0;
        }
        UpdatePlanToDisplay();
        Debug.Log("Previous button pushed! gallery index is now " + galleryIndex);
    }

    public void NextButtonPushed()
    {
        Debug.Log("Next button pushed!");
        galleryIndex += 1;
        if(galleryIndex >= theatreImages.Count){
            //galleryIndex = 0;
            galleryIndex = theatreImages.Count - 1;
        }
        Debug.Log("Next button pushed! gallery index is now " + galleryIndex);
        UpdatePlanToDisplay();
    }

    public void FinishedButtonPushed(){
        Debug.Log("Finished button pushed!");
        hud.UpdateQuestProgress(HUD.POSSESS_PLANS);
        ensembleUI.SetCharacterAvailability();
        StartCoroutine(ensembleUI.ShowProgress(1, "You have the plans! Now you will return to the theatre. Seek out the last person you spoke to, in order to negotiate an escape."));
    }

    private void UpdatePlanToDisplay(){
        planImage.GetComponent<RawImage>().texture = theatreImages[galleryIndex];
        Debug.Log("Now showcasing: " + theatreImages[galleryIndex].name + " at index " + galleryIndex);
    }
}