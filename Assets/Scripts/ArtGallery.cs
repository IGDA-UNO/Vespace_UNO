using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ArtGallery : MonoBehaviour
{

    private GameObject artGalleryObject;
    public GameObject SteamVRObjects;
    public GameObject planImage;
    public Camera playerCamera;
    public Camera fallBackCamera;
    public Player player;
    public GameObject backstageLeft;

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
        Debug.Log("TEST: SteamVRObjects is enabled: " + SteamVRObjects.activeSelf);
        if (!SteamVRObjects.activeSelf)
        {
            Debug.Log("NO PLAYER CAMERA!?!?!");
            playerCamera = fallBackCamera;
        }
        placeArtGallery();
        UpdatePlanToDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void placeArtGallery()
    {
        artGalleryObject.transform.rotation = playerCamera.transform.rotation;
        artGalleryObject.transform.SetParent(playerCamera.transform);
        artGalleryObject.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
    }

    public void PreviousButtonPushed(){
       
        galleryIndex -= 1;
        if(galleryIndex < 0){
            galleryIndex = theatreImages.Count - 1;
        }
        UpdatePlanToDisplay();
        Debug.Log("Previous button pushed! gallery index is now " + galleryIndex);
    }

    public void NextButtonPushed()
    {
        Debug.Log("Next button pushed!");
        galleryIndex += 1;
        if(galleryIndex >= theatreImages.Count){
            galleryIndex = 0;
        }
        Debug.Log("Next button pushed! gallery index is now " + galleryIndex);
        UpdatePlanToDisplay();
    }

    public void SetPlayer(Player p)
    {
        player = p;
    }

    private IEnumerator<object> TransportReturnToTheater()
    {
        SteamVR_Fade.Start(Color.black, 10);
        yield return new WaitForSeconds(3);
        player.transform.position = new Vector3(3f, 0f, 2f);
		SteamVR_Fade.Start(Color.clear, 10);
    }

    public void FinishedButtonPushed(){
        StartCoroutine(TransportReturnToTheater());
        // vm.PositionAssigner();
        // GameObject lastInterlocutor = GameObject.Find(ensembleUI.finalInterlocutor);
        // lastInterlocutor.transform.position = new Vector3(2.5f, 0f, 2f);
        Debug.Log("Finished button pushed!");
        gameObject.SetActive(false);
    }

    private void UpdatePlanToDisplay(){
        planImage.GetComponent<RawImage>().texture = theatreImages[galleryIndex];
        Debug.Log("Now showcasing: " + theatreImages[galleryIndex].name);
    }
}