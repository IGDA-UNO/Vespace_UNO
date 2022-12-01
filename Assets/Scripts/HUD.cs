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

    public static int NO_TICKET = 0;
    public static int POSSESS_TICKET = 1;
    public static int HANDED_TICKET_TO_TICKET_TAKER = 2;
    public static int RECEIVED_MARK = 3;
    public static int BACKSTAGE_ACCESS = 4;
    public static int POSSESS_PLANS = 5;
    public static int FINAL_INTERACTION = 6;
    public static int GAME_COMPLETED = 7;
    public static int THROWN_OUT = 999;

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

    public void removeHud()
    {
        HUDObject.SetActive(false);
    }

    public void replaceHud()
    {
        HUDObject.SetActive(true);
    }

    private string GetObjectiveText(){
        if(HUD.questProgress == HUD.NO_TICKET){
            return "Give your ticket to the ticket taker!";
        }
        else if(HUD.questProgress == HUD.POSSESS_TICKET){
            return "Hand your ticket to the ticket taker!";
        }
        else if (HUD.questProgress == HUD.HANDED_TICKET_TO_TICKET_TAKER)
        {
            return "Speak to the ticket taker again to receive your mark!";
        }
        else if (HUD.questProgress == HUD.RECEIVED_MARK)
        {
            return "Enter the theatre, and find someone to help you backstage (Remember to speak to at least two characters in the theatre)";
        }
        else if (HUD.questProgress == HUD.BACKSTAGE_ACCESS)
        {
            return "You've evaded the audience's watch, now find the plans backstage!";
        }
        else if (HUD.questProgress == HUD.POSSESS_PLANS)
        {
            return "You have the plans! Ask the last person you spoke with to help you exit the theatre!";
        }
        else if (HUD.questProgress == HUD.FINAL_INTERACTION)
        {
            return "Now go and speak to the ticket taker!";
        }
        else if (HUD.questProgress == HUD.GAME_COMPLETED)
        {
            return "Congratulations - you've completed the game!";
        }
        else if (HUD.questProgress == HUD.THROWN_OUT)
        {
            return "Oh no - you've been thrown out of the theater!";
        }

        return "Unknown objective!";
    }

    public void UpdateQuestProgress(int newQuestStep){
        questProgress = newQuestStep;
        CurrentObjectiveText.text = GetObjectiveText();
    }

    public int GetQuestProgress(){
        return HUD.questProgress;
    }
}
