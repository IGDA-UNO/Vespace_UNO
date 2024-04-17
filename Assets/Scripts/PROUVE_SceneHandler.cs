//PROUVE (Extensions Omeka et Unity pour le Patrimoine en Réalité Virtuelle)
//Version 2.0
//Scene Handler, interface between user and PROUVE components.
//Paul François - VESPACE

using System.Collections;
using System.IO ; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using Valve.VR.Extras ; 
using Valve.VR ; 

public class PROUVE_SceneHandler : MonoBehaviour
{
    public Camera playerCamera; 
    public GameObject fallBackCamera ; 
    public GameObject leftHand ; 
    public float recordingInterval = 0.2f ; 
    public string mainSceneName ;
    public SteamVR_LaserPointer laserPointer;
    public bool demoMode ; //En mode démo : démarrage directement sur la scène de théâtre. Incrément automatique des utilisateurs, sauvegarde automatique à chaque changement d'utilisateur.
    public string CurrentLanguage = "fra"; //fra pour français et eng pour anglais

    public SteamVR_Action_Vibration hapticVibration ; 

    public Color hit_color; 
    public Color miss_color;

    public PROUVE_OmekaPad omekaPad;

    private int demoPlayerID = 0 ; 
    private bool allowRestart ; 
    private bool proModeActive = false ;
    private bool permitProMode;
    private int interfaceMode = 2 ; // 0: interface fixée à la caméra ; 1: interface fixée à l'objet ; 2: interface fixée au controlleur gauche 
    private float interfaceHeight = 1.5f; 

    private GameObject OmekaInterface ;
    private ScrollRect descriptionScroll ; 
    private ScrollRect relationsScroll ; 

    private GameObject mainMenu ;
   
    private Text mainMenuText ; 
    private Text proButtonText ; 

    private PROUVE_DataCaller dataCaller; 
    private PROUVE_ExperienceManager experienceManager ; 
   
    private PROUVE_interfaceController interfaceController ; 

    private SteamVR_Action_Boolean actionBoolean;

    void Awake() 
    {
        //Initialize all interface references needed to display information
        //Initialisation de toutes les références à l'interface nécessaires pour l'affichage des informations
        mainMenu = GameObject.Find("PROUVE_MainMenu") ; 
        mainMenuText = GameObject.Find("PROUVE_MainMenu/Text").GetComponent<Text>() ; 
        OmekaInterface = GameObject.Find("PROUVE_Pad") ;
        descriptionScroll = GameObject.Find("PROUVE_Pad/MainView/ScrollViewDescription").GetComponent<ScrollRect>() ;
        relationsScroll = GameObject.Find("PROUVE_Pad/MainView/ScrollViewRelations").GetComponent<ScrollRect>() ;
        proButtonText = GameObject.Find("PROUVE_MainMenu/ProButton/Text").GetComponent<Text>() ; 
        dataCaller = GetComponent<PROUVE_DataCaller>() ; 
        experienceManager = GetComponent<PROUVE_ExperienceManager>() ; 
        interfaceController = GetComponent<PROUVE_interfaceController>() ; 
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initializing actions and events
        actionBoolean = SteamVR_Actions._default.GrabGrip;
        laserPointer.color = miss_color; 
        laserPointer.PointerIn += PointerInside; 
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick; 
        interfaceController.SwipeHorizontal += SwipingHorizontal; 
        interfaceController.SwipeVertical += SwipingVertical ; 
        permitProMode = false;
        if(CurrentLanguage != "fra" && CurrentLanguage != "eng") {
            CurrentLanguage = "fra";
        }
        omekaPad.initWithDataCallerAndExperienceManager(dataCaller,experienceManager,CurrentLanguage) ;
     

        //Initializing interface 
        omekaPad.closePad();
        mainMenu.SetActive(false) ; 
        placeMenu() ;

        //Initializing objects : 
        dataCaller.initDataCaller(this, omekaPad,"http://vespace.univ-nantes.fr/","5bd7dff39253e822352f22d6b2370f7b06887b9b") ; 
        experienceManager.setSceneHandler(this, playerCamera) ; 
        if(PlayerPrefs.GetInt("LocalMode",0) == 1) {
            dataCaller.switchLocalMode() ; 
        }
        if(SceneManager.GetActiveScene().name == mainSceneName) {
            if(demoMode) {
                demoPlayerID = PlayerPrefs.GetInt("PlayerID",0) +1 ; 
                PlayerPrefs.SetInt("PlayerID",demoPlayerID) ; 
                PlayerPrefs.SetString("UserName",demoPlayerID.ToString()) ;
                PlayerPrefs.SetString("StartDate",System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) ;
                PlayerPrefs.Save() ; 
                experienceManager.startRecord(recordingInterval,false) ; 
            } else {
                experienceManager.startRecord(recordingInterval,true) ; 
            }
            Debug.Log("Main scene loaded, starting record") ;      
            allowRestart = true ; 
        }

        //I believe by default, unless otherwise specified, we are assuming interface mode 2.
        interfaceMode = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionBoolean.stateDown && permitProMode) {
            if(!mainMenu.activeSelf) {
                invokeMainMenu() ; 
            } else {
                closeMainMenu() ; 
            }
        }
        if(allowRestart) {
            if(Input.GetKeyDown (KeyCode.R)) {
                if(demoMode){
                    restartLocal();
                } else {
                    restartExperience() ;
                }
            } 
            if(Input.GetKeyDown (KeyCode.L)) {
                if(dataCaller.isInLocalMode()) {
                    PlayerPrefs.SetInt("LocalMode",0) ; 
                } else {
                    PlayerPrefs.SetInt("LocalMode",1) ; 
                }
                PlayerPrefs.Save() ;  
                dataCaller.switchLocalMode() ; 
            } 
            if(Input.GetKeyDown (KeyCode.Space)) {
                restartLocal() ;
            }
            if(Input.GetKeyDown (KeyCode.D)) {
                Debug.Log("Resetting Demo Mode to player 0") ; 
                demoPlayerID = 1 ; 
                PlayerPrefs.SetInt("PlayerID",demoPlayerID) ; 
                PlayerPrefs.SetString("UserName",demoPlayerID.ToString()) ;
                PlayerPrefs.SetString("StartDate",System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) ;
                PlayerPrefs.Save() ; 
            } 

            if(Input.GetKeyDown (KeyCode.T)) {
                //dataCaller.debugTestSend() ; 
            }
        }

        //if the user hits the 'r' key, reset the experience.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    //Pro mode management:
    //Gestion du mode pro : 

    public void switchProMode() {
        proModeActive = !proModeActive ; 
        if(proModeActive) { proButtonText.text = "DISABLE PRO MODE" ; } else { proButtonText.text = "ACTIVATE PRO MODE" ; }
        omekaPad.enableProMode(proModeActive) ; 
    }

    public bool isProModeActive() {
        return proModeActive ; 
    }

    //Display of menus and panels:
    //Affichage des menus et panneaux : 

    public void placeMenu() {
        mainMenu.transform.rotation = playerCamera.transform.rotation ; 
        mainMenu.transform.SetParent(playerCamera.transform) ; 
        mainMenu.transform.localPosition = new Vector3(0f,0f,2.0f) ; 
    }

    public bool isPadVisible() {
        return omekaPad.padState() ; 
    }

    public void placePad(Vector3 objectPosition) {
        if(interfaceMode == 0) {
            if(OmekaInterface.transform.parent != playerCamera.transform) {
                OmekaInterface.transform.rotation = playerCamera.transform.rotation ; 
                OmekaInterface.transform.SetParent(playerCamera.transform) ; 
                OmekaInterface.transform.localPosition = new Vector3(0f,-0.174f,1.43f);  
            }  
        }
        if(interfaceMode == 1) {
            Vector3 intermediate = Vector3.MoveTowards(objectPosition,playerCamera.transform.position,0.5f) ; 
            Vector3 ideal_position = new Vector3(intermediate.x,interfaceHeight,intermediate.z) ; 
            OmekaInterface.transform.position = ideal_position ; 
            Vector3 ideal_rotation = new Vector3(0,playerCamera.transform.eulerAngles.y,0) ;
            OmekaInterface.transform.eulerAngles = ideal_rotation ; 
        }
        if(interfaceMode == 2) {
            if(OmekaInterface.transform.parent != leftHand.transform) {
                Vector3 ideal_rotation = new Vector3(45,0,0) ;
                //OmekaInterface.transform.eulerAngles = leftHand.transform.eulerAngles + ideal_rotation; 
                OmekaInterface.transform.SetParent(leftHand.transform) ; 
                OmekaInterface.transform.localEulerAngles = new Vector3(45f,0f,0f) ; 
                OmekaInterface.transform.localPosition = new Vector3(0.2f,0.3f,0.2f) ; 
                OmekaInterface.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ; 
            }
        }
        if(interfaceMode == 3) {
            Vector3 intermediate = Vector3.MoveTowards(objectPosition,playerCamera.transform.position,0.5f) ; 
            Debug.Log(playerCamera.transform.position) ; 
            Vector3 ideal_position = new Vector3(intermediate.x,interfaceHeight,intermediate.z) ; 
            OmekaInterface.transform.position = ideal_position ; 
            Vector3 ideal_rotation = new Vector3(0,playerCamera.transform.eulerAngles.y,0) ;
            OmekaInterface.transform.eulerAngles = ideal_rotation ; 
        }
        if(interfaceMode == 4) {
            OmekaInterface.transform.SetParent(fallBackCamera.transform) ; 
            OmekaInterface.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            OmekaInterface.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            OmekaInterface.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

            // OmekaInterface.transform.localEulerAngles = new Vector3(45f,0f,0f) ; 
            // OmekaInterface.transform.localPosition = new Vector3(0.2f,0.3f,0.2f) ; 
            // OmekaInterface.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ; 
        }
    }


    public void hidePad() {
        OmekaInterface.SetActive(false) ;
    }

    public void invokeMainMenu() {
        mainMenu.SetActive(true) ; 
        mainMenuText.text = PlayerPrefs.GetString("UserName") ; 
    }

    public void closeMainMenu() {
        mainMenu.SetActive(false) ; 

    }

    //Suggestions : 

    public void suggestItem(int itemID) {
        hapticVibration.Execute(0.0f,2.0f,5.0f,1.0f,SteamVR_Input_Sources.LeftHand) ; 
        placePad(new Vector3(0,0,0)) ; 
        omekaPad.displayItem(itemID) ; 
        if(!proModeActive) {
            VirtualOmekaObject[] targetObjects = findOmekaObjectInScene(itemID) ; 
            //Debug.Log("Number of objects found : "+targetObjects.Length) ; 
            foreach(VirtualOmekaObject target_object in targetObjects) {
                target_object.drawLineFromObject(leftHand,10.0f) ; 
            }
        }
    }

    public VirtualOmekaObject[] findOmekaObjectInScene(int omekaID) {
        List<VirtualOmekaObject> liste = new List<VirtualOmekaObject>() ; 
        VirtualOmekaObject[] objects = FindObjectsOfType<VirtualOmekaObject>() ; 
        for(int i=0; i<objects.Length; i++) {
            if(objects[i].OmekaVirtualObjectID == omekaID) {
                liste.Add(objects[i]) ; 
            }
        }
        return liste.ToArray() ; 
    }

    //Save and Restart:
    //Sauvegarde et Redémarrage : 

    public void restartExperience() {
        Debug.Log("Restarting Experience") ; 
        experienceManager.Save() ; 
        StartCoroutine(waitBeforeLoadScene("Intro")) ; 
        //SceneManager.LoadScene("Intro") ; 
    }

    IEnumerator waitBeforeLoadScene(string sceneName) {
        while(dataCaller.isWorking()) {
            yield return null; 
        }
        SceneManager.LoadScene(sceneName) ; 
    }

    public void restartLocal() {

        if(demoMode) { 
            experienceManager.Save() ; 
        } else  { 
            experienceManager.Stop() ; //Comportement par défaut : pas d'enregistrement lors de la reprise.
        }
        StartCoroutine(waitBeforeLoadScene(mainSceneName)) ; 
        //SceneManager.LoadScene(mainSceneName) ; 
    }

    public void setAllowRestart(bool flag) {
        allowRestart = flag ; 
    }

    //Mouse click support:
    //Support du click de souris : 

    public void clickOnObject(int objectID, Vector3 position)  
    {
        Debug.Log("Clicked on object " + objectID) ; 
        //invokeMainMenu() ;  
        //interfaceMode = 4 ; set this at the beginning when we first detect headset existence.
        placePad(position) ; 
        omekaPad.displayItem(objectID) ; 
    }



    //LaserPointer functions overload:
    //Surcharge des fonctions de LaserPointer : 

    public void PointerClick(object sender, PointerEventArgs e) {
        VirtualOmekaObject voo = e.target.GetComponent<VirtualOmekaObject>() ; 
        if (voo != null) {
            //The object is an OMEKA object
            //L'objet est un objet OMEKA
            placePad(e.target.transform.position) ; 
            omekaPad.displayItem(voo.OmekaVirtualObjectID) ; 
        }
        if(e.target.gameObject.layer == 5)
        {
            //layer of the UI: 5
            //layer de l'UI : 5 
            Button bouton = e.target.GetComponent<Button>() ; 
            if(bouton != null) {
                bouton.onClick.Invoke() ; 
            }
        }
    }

    public void PointerInside(object sender, PointerEventArgs e) {
        laserPointer.color = hit_color ; 
    }

    public void PointerOutside(object sender, PointerEventArgs e) {
        laserPointer.color = miss_color ; 
    }

    //Overload of InterfaceController functions:
    //Surcharge des fonctions de InterfaceController : 

    public void SwipingHorizontal(object sender, SwipeEventArgs e) {
        //Debug.Log("Swiping Right with velocity : "+e.velocity) ; 
        //descriptionScroll.horizontalNormalizedPosition = 1.0f ; 
        if(relationsScroll != null) { 
            float position = e.velocity*0.55f+0.5f ; 
            relationsScroll.horizontalNormalizedPosition = position ;
        }
    }

    public void SwipingVertical(object sender, SwipeEventArgs e) {
        //Debug.Log("Swiping Down with velocity : "+e.velocity) ; 
        //Debug.Log("Position : "+descriptionScroll.verticalNormalizedPosition) ; 
        if(descriptionScroll != null) {
            float position = e.velocity*0.55f+0.5f ; 
            descriptionScroll.verticalNormalizedPosition = position ;  
        }
    }

    public void SetInterfaceMode(int newMode){
        this.interfaceMode = newMode;
    }


}
