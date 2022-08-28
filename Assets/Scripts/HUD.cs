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
    public Text CurrentObjectiveText;

    public static int questProgress; // = POSSESS_TICKET; //start off assuming you possess a ticket? 

    public static int POSSESS_TICKET = 0;
    public static int HANDED_TICKET_TO_TICKET_TAKER = 1;
    public static int RECEIVED_MARK = 2;

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

        //Update the hud's text based on quest objective
        CurrentObjectiveText.text = GetObjectiveText();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("space key was pressed");
            CurrentObjectiveText.text = GetObjectiveText();
            HUD.questProgress++;
        }
    }

    public void placeHud()
    {
        HUDObject.transform.rotation = playerCamera.transform.rotation;
        HUDObject.transform.SetParent(playerCamera.transform);
        HUDObject.transform.localPosition = new Vector3(-0.06f, 0.1f, 0.5f);
    }

    private string GetObjectiveText(){
        if(HUD.questProgress == HUD.POSSESS_TICKET ){
            return "Hand your ticket to the ticket taker!";
        }
        else if (HUD.questProgress == HUD.HANDED_TICKET_TO_TICKET_TAKER)
        {
            return "Receive your mark so you may enter the theatre";
        }
        else if (HUD.questProgress == HUD.RECEIVED_MARK)
        {
            return "Enter the theatre, and find someone to help you backstage!";
        }

        return "Unknown objective!";
    }

    public void UpdateQuestProgress(int newQuestStep){
        questProgress = newQuestStep;
        CurrentObjectiveText.text = GetObjectiveText();
    }
}
