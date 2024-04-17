using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Valve.VR.Extras;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Ensemble;
using System.Linq;
using System.Text;

public class ENSEMBLE_UIHandler_Introduction : MonoBehaviour
{
    public GameObject ensembleUI;
    public bool mainActiveUI;

    public GameObject actionsUI;
    public bool actionsActiveUI;

    public GameObject historyUI;
    public bool historyActiveUI;

    public GameObject omekaUI;
    public bool omekaActiveUI;

    public GameObject leftHand;

    public SteamVR_LaserPointer laserPointer;
    public Color hit_color;
    public Color miss_color;

    public Player playerObject;

    private int currentOmekaIDOfClickedCharacter;
    private Vector3 currentPositionOfClickedCharacter;

    private bool exitClick;

    public PROUVE_OmekaPad prouve;
    public Camera playerCamera;
    public Camera fallBackCamera;
    public GameObject SteamVRObjects;

    //For keeping track of moments when using a headset or not.
    public bool isUsingVRHeadset;

    void Start()
    {
        Debug.Log("UIHANDLER_INTRODUCTION HELLO...?");
        mainActiveUI = false;
        actionsActiveUI = false;
        historyActiveUI = false;
        omekaActiveUI = false;

        laserPointer.color = miss_color;
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    void Update(){}

    public void DisplayMain()
    {
        if (!mainActiveUI)
        {
            ensembleUI.SetActive(true);
            mainActiveUI = true;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            historyUI.SetActive(false);
            historyActiveUI = false;

            omekaUI.SetActive(false);
            omekaActiveUI = false;
        }
        prouve.closePad();
    }

    public void DisplayActions()
    {
        if (!actionsActiveUI)
        {
            ensembleUI.SetActive(false);
            mainActiveUI = false;

            actionsUI.SetActive(true);
            actionsActiveUI = true;

            historyUI.SetActive(false);
            historyActiveUI = false;

            omekaUI.SetActive(false);
            omekaActiveUI = false;
        }
        
    }

    public void DisplayHistory()
    {
        if (!historyActiveUI)
        {
            ensembleUI.SetActive(false);
            mainActiveUI = false;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            historyUI.SetActive(true);
            historyActiveUI = true;

            omekaUI.SetActive(false);
            omekaActiveUI = false;
        }
    }

    public void DisplayOmeka()
    {
        Debug.Log("ummm Display Omeka???");
        if (!omekaActiveUI)
        {
            ensembleUI.SetActive(false);
            mainActiveUI = false;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            historyUI.SetActive(false);
            historyActiveUI = false;

            omekaUI.SetActive(true);
            omekaActiveUI = true;
        }
    }

    // public void getCharacterOmeka(string objectName){
    //     GameObject characterInQuestion = GameObject.Find(objectName);
    //     currentOmekaIDOfClickedCharacter = characterInQuestion.GetComponent<EnsembleObjectIntro>().omekaDatabaseID;
    //     currentPositionOfClickedCharacter = characterInQuestion.transform.position;
    // }

/*
    public void DisplayOmeka()
    {
        ensembleUI.SetActive(false);
        mainActiveUI = false;

        actionsUI.SetActive(false);
        actionsActiveUI = false;

        historyUI.SetActive(false);
        historyActiveUI = false;

        //make use of existing prouve system!
        //But I think we need to do things slightly differently if we are using
        //a mouse or if we are using the headset!
        
        if(isUsingVRHeadset){
            GameObject.Find("PROUVE/SceneHandler").GetComponent<PROUVE_SceneHandler>().SetInterfaceMode(2);
        }
        else{
            GameObject.Find("PROUVE/SceneHandler").GetComponent<PROUVE_SceneHandler>().SetInterfaceMode(4);
        }

        Debug.Log("currentOmekaIDOfClickedCharacter: " + currentOmekaIDOfClickedCharacter);
        Debug.Log("currentPositionOfClickedCharacter: " + currentPositionOfClickedCharacter);

        GameObject.Find("PROUVE/SceneHandler").GetComponent<PROUVE_SceneHandler>().clickOnObject(currentOmekaIDOfClickedCharacter, currentPositionOfClickedCharacter);
    }
*/

    public void CloseMenu()
    {

        ensembleUI.SetActive(false);
        mainActiveUI = false;

        actionsUI.SetActive(false);
        actionsActiveUI = false;

        historyUI.SetActive(false);
        historyActiveUI = false;

        omekaUI.SetActive(false);
        omekaActiveUI = false;
    }

    public void Orientation()
    {
        ensembleUI.transform.SetParent(leftHand.transform);
        ensembleUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        ensembleUI.transform.localPosition = new Vector3(0.1f, 0.0f, 0.2f);
        ensembleUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        actionsUI.transform.SetParent(leftHand.transform);
        actionsUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        actionsUI.transform.localPosition = new Vector3(0.1f, 0.1f, 0.2f);
        actionsUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        historyUI.transform.SetParent(leftHand.transform);
        historyUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        historyUI.transform.localPosition = new Vector3(0.1f, 0.2f, 0.2f);
        historyUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        omekaUI.transform.SetParent(leftHand.transform);
        omekaUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        omekaUI.transform.localPosition = new Vector3(0.1f, 0.0f, 0.2f);
        omekaUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
    }

    //LaserPointer functions overload:
    public void PointerClick(object sender, PointerEventArgs e)
    {
        EnsembleObjectIntro ensemble = e.target.GetComponent<EnsembleObjectIntro>();
        Debug.Log("Intro scene: clicked with laser pointer");
        if (ensemble != null)
            {
            //The object is an Ensemble object (in the intro that basically means it is the ticket taker.)
            Orientation();
            DisplayMain();
        }
        /* //This is handled in Prouve for the introduction. Don't need to worry about this here.
        if (e.target.gameObject.layer == 5)
        {
            //layer of the UI: 5 
            Button button = e.target.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
        */
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        laserPointer.color = hit_color;
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        laserPointer.color = miss_color;
    }

    public void clickOnObject(string objectName, Vector3 position)  
    {
        DisplayMain();

        ensembleUI.transform.SetParent(fallBackCamera.transform);
        ensembleUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
        ensembleUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
        ensembleUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

        actionsUI.transform.SetParent(fallBackCamera.transform);
        actionsUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
        actionsUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
        actionsUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

        historyUI.transform.SetParent(fallBackCamera.transform);
        historyUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
        historyUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
        historyUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

        omekaUI.transform.SetParent(fallBackCamera.transform);
        omekaUI.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        omekaUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
        omekaUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        //getCharacterOmeka(objectName);
    }
}
