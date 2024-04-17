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
using Unity.Profiling;

public class ENSEMBLE_UIHandler : MonoBehaviour
{
    public GameObject ensembleUI;
    public bool mainActiveUI;

    public GameObject actionsUI;
    public bool actionsActiveUI;

    public GameObject attributesUI;
    public bool attributesActiveUI;

    public GameObject historyUI;
    public bool historyActiveUI;

    public GameObject omekaUI;
    public bool omekaActiveUI;

    public GameObject leftHand;

    public SteamVR_LaserPointer laserPointer;
    public Color hit_color;
    public Color miss_color;

    public EnsembleData data;
    public EnsemblePlayer player;
    public Player playerObject;
    public GameObject character;

    public Button currentActionButton;
    public Button[] actionButtons;

    public GameObject characterMenu;
    public GameObject attributesMenu;
    public GameObject traitsMenu;
    // public GameObject clothingMenu;
    // public GameObject professionMenu;
    // public GameObject directedStatusMenu;
    // public GameObject networkMenu;
    // public GameObject nonActionableRelationshipMenu;
    // public GameObject relationshipMenu;
    // public GameObject socialRecordLabelMenu;
    // public GameObject statusMenu;
    public GameObject historyMenu;
    public GameObject omekaMenu;
    public GameObject actionsMenu;
    public Transform actionsMenuImageZone;
    public GameObject prefabButton;

    public StringBuilder attributesBuilder;
    public StringBuilder traitsBuilder;
    public StringBuilder clothingBuilder;
    public StringBuilder professionBuilder;
    public StringBuilder directedStatusBuilder;
    public StringBuilder networkBuilder;
    public StringBuilder nonActionableRelationshipBuilder;
    public StringBuilder relationshipBuilder;
    public StringBuilder socialRecordLabelBuilder;
    public StringBuilder statusBuilder;
    public StringBuilder historyBuilder;
    public StringBuilder actionsBuilder;
    private int currentOmekaIDOfClickedCharacter;
    private Vector3 currentPositionOfClickedCharacter;

    List<GameObject> actionButtonRefs = new List<GameObject>();
    private bool exitClick;

    public PROUVE_OmekaPad prouve;

    public HUD hud;
    public ArtGallery artGallery;

    public GameObject dialogueHUD;
    public GameObject resultsPanel;
    public GameObject progressPanel;
    public Camera playerCamera;
    public Camera fallBackCamera;
    public GameObject SteamVRObjects;
    public Text CharacterNameText;
    public Text DialogueText;
    public GameObject dialogueIcon;

    public Vector3 dialogueOffset;
    public Vector3 resultsOffset;

    public VideoPlayer marionetteVideoFront;
    public VideoPlayer marionetteVideoBack;
    public AudioSource marionnettisteAudio;

    public Texture MarieCatherineBienfait;
    public Texture BrunoDufort;
    public Texture MonsieurdAindYgeste;
    public Texture MonsieurdHautainville;
    public Texture MonsieurdeGentilly;
    public Texture MonsieurdesTroisLandes;
    public Texture MonsieurdIssy;
    public Texture MadamedeBlaselEveque;
    public Texture Cherubin;
    public Texture Valere;
    public Texture MadameduPuydesGougeres;
    public Texture MadamedeChersurTendre;
    public Texture MademoiselleEloisedeHauteclaire;
    public Texture Ninon;
    public Texture Toinette;
    public Texture LaFruitiere;
    public Texture Cellist;
    public Texture Lanimateur;
    public Texture FanchonLaPoche;
    public Texture HenrietteLavocat;
    public Texture JeannotLaPanse;
    public Texture Scaramouche;
    public Texture MadameArgant;

    public AudioSource whistleAudioSource;
    public AudioSource stompAudioSource;

    public bool displayingDialogue = false;

    public GameObject backstageRight;

    public string finalInterlocutor;
    public Transform lastInterlocutorTransform;
    public bool approachedFinalInterlocutor = false;

    private List<Action> gameActions = new List<Action>();

    private bool initiatedProgressPanel = false;
    private int negativeInteractionCount = 0;
    private int positiveInteractionCount = 0;
    private bool completedTwoInteractions = false;

    public List<string> characterInteractions = new List<string>();
    public List<string> strangerInteractions = new List<string>();
    public List<string> acquaintanceInteractions = new List<string>();

    private string finalResult;

    //For starting the play in Act I once you've entered the theatre.
    private float xCoordinateThatMeansYouHaveEnteredTheTheatre;
    public bool hasActIPlayStarted;

    //For ensuring the player can't go to places we don't want them to go yet.
    public List<GameObject> houseTeleporters = new List<GameObject>();
    public List<GameObject> backstageTeleporters = new List<GameObject>();

    //For keeping track of moments when using a headset or not.
    public bool isUsingVRHeadset;

    Transform lastInterlocutorParticleSystem;

    //The full cast
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
        "Valère",
        "Madame du Puy-des-Gougères",
		"Madame de Cher-sur-Tendre",
		"Mademoiselle Eloïse de Hauteclaire",
		"Ninon",
		"Toinette",
		"Fruit-seller / la Fruitière",
		"Cellist",
		"L'Animateur",
		"Fanchon la Poche",
		"Henriette Lavocat",
		"Jeannot la Panse",
		"Scaramouche",
		"Madame Argant"
    };


    private Cast castWithoutInteractableCharacters = new Cast {
        "Male Noble Player",
        "Female Noble Player",
        "Servant Player",
        "Marie-Catherine Bienfait, ticket taker",
        "Monsieur d'Ain d'Ygeste",
        "Monsieur d'Hautainville",
        "Monsieur de Gentilly",
        "Monsieur des Trois-Landes",
        "Monsieur d'Issy",
        "Madame de Blasé-l'Evêque",
        "Chérubin",
        "Valère",
        "Madame du Puy-des-Gougères",
        "Madame de Cher-sur-Tendre",
        "Mademoiselle Eloïse de Hauteclaire",
        "Ninon",
        "Toinette",
        "Fruit-seller / la Fruitière",
        "Henriette Lavocat",
        "Madame Argant"
    };

    private Cast castForServant = new Cast {
        "Servant Player",
        "Marie-Catherine Bienfait, ticket taker",
        "Monsieur d'Ain d'Ygeste",
        "Monsieur d'Hautainville",
        "Monsieur de Gentilly",
        "Monsieur des Trois-Landes",
        "Monsieur d'Issy",
        "Madame de Blasé-l'Evêque",
        "Chérubin",
        "Valère",
        "Madame du Puy-des-Gougères",
        "Madame de Cher-sur-Tendre",
        "Mademoiselle Eloïse de Hauteclaire",
        "Ninon",
        "Toinette",
        "Fruit-seller / la Fruitière",
        "Henriette Lavocat",
        "Madame Argant"
    };

    public Dictionary<string, bool> characterAvailable = new Dictionary<string, bool>();

    private VolitionInterface volitionInterface;

    //Testing threads
    private TestJob threadTestJob;
    private EnsembleCalculateVolitionsJob ensembleJob;
    bool volitionInterfaceIsLocked = true; //when the thread to calculate volitions is running, set this flag so that we don't try to use it.
    EnsembleCalculateVolitionForSetCharacterAvailability ensembleCalculateVolitionsForSetCharacterAvailability;
    EnsembleCalculateVolitionsJobForGetCharacterActions ensembleCalculateVolitionsJobForGetCharacterActions;
    EnsembleCalculateVolitionsJobForFinalInterlocutor ensembleCalculateVolitionsJobForFinalInterlocutor;


    //The 'quest arrow'
    GameObject arrowTarget;
    public GameObject arrow;

    void Start()
    {
        Debug.Log("TURNING OFF DEBUG MESSAGES -- set Debug.unityLogger.logEnabled to true in ENSEMBLE_UIHandler.cs to re-enable them");
        //Debug.unityLogger.logEnabled = false;
        Debug.Log("LA LAL A LA CAN YOU SEE ME?");
        hud.UpdateQuestProgress(HUD.NO_TICKET);

        arrowTarget = GameObject.Find("Marie-Catherine Bienfait, ticket taker");

        dialogueOffset = new Vector3(-0.06f, 0.04f, 0.51f);
        resultsOffset = new Vector3(0f, 0f, 0.5f);

        isUsingVRHeadset = true;
        if (!SteamVRObjects.activeSelf)
        {
            playerCamera = fallBackCamera;
            isUsingVRHeadset = false;
        }

        //Turn off the play on start...
        marionetteVideoFront.gameObject.SetActive(false);
        marionetteVideoBack.gameObject.SetActive(false);
        marionnettisteAudio.gameObject.SetActive(false);
        //GameObject.Find("Marionettes").transform.Find("Marionette Video Front").gameObject.SetActive(false);
        //GameObject.Find("Marionettes").transform.Find("Marionette Video Back").gameObject.SetActive(false);

        //Testing jumping to specific places in the video...
        //marionetteVideoBack.frame = 4000;
        //Debug.Log("Total frames of back video: " + marionetteVideoBack.frameCount);

        mainActiveUI = false;
        actionsActiveUI = false;
        attributesActiveUI = false;
        historyActiveUI = false;
        omekaActiveUI = false;

        laserPointer.color = miss_color;
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;

        attributesBuilder = new StringBuilder();
        traitsBuilder = new StringBuilder();
        clothingBuilder = new StringBuilder();
        professionBuilder = new StringBuilder();
        directedStatusBuilder = new StringBuilder();
        networkBuilder = new StringBuilder();
        nonActionableRelationshipBuilder = new StringBuilder();
        relationshipBuilder = new StringBuilder();
        socialRecordLabelBuilder = new StringBuilder();
        statusBuilder = new StringBuilder();
        historyBuilder = new StringBuilder();
        actionsBuilder = new StringBuilder();

        actionsMenu = GameObject.Find("ActionsMenu");
        actionsMenuImageZone = actionsMenu.transform.Find("MainView/ScrollViewActions/Viewport/Content/ImageZone");
        // directedStatusMenu = GameObject.Find("DirectedStatusList");
        // networkMenu = GameObject.Find("NetworkList");
        // nonActionableRelationshipMenu = GameObject.Find("NonActionableRelationshipList");
        // relationshipMenu = GameObject.Find("RelationshipList");
        // socialRecordLabelMenu = GameObject.Find("SocialRecordLabelList");
        // statusMenu = GameObject.Find("StatusList");
        historyMenu = GameObject.Find("HistoryList");
        attributesMenu = GameObject.Find("AttributesList");
        omekaMenu = GameObject.Find("OmekaList");
        traitsMenu = GameObject.Find("TraitsList");
        // clothingMenu = GameObject.Find("TraitClothingList");
        // professionMenu = GameObject.Find("ProfessionList");
        characterMenu = GameObject.Find("Character_Name");

        //For starting the play in Act I once you've entered the theatre.
        xCoordinateThatMeansYouHaveEnteredTheTheatre = -1.5f;
        hasActIPlayStarted = false;

        //Initialize all teleporters to inactive
        SetUsabilityOfHouseTeleporters(false);
        SetUsabilityOfBackstageTeleporters(false);

        foreach (string character in cast) {
            characterAvailable.Add(character, false);
        }

        //Moving this to the 'finished' of the thread of calculating volitions.
        //SetCharacterAvailability();
        //and instead, we'll start by calculating volitions
        ensembleJob = new EnsembleCalculateVolitionsJob();
        ensembleJob.InDataEnsemble = this.data.ensemble;
        ensembleJob.InDataCharactersToCalculateVolitionsFor = this.cast;
        volitionInterfaceIsLocked = true; // whenever we start ensemble job we should lock the interface.
        ensembleJob.Start();
    }

    void Update(){
        //Debug.Log("The position of the player is: " + playerCamera.transform.position);

        if(!hasActIPlayStarted){
            if (playerCamera.transform.position.x > xCoordinateThatMeansYouHaveEnteredTheTheatre)
            {
                //Debug.Log("IN THE THEATRE!");
                hasActIPlayStarted = true;

                //start the show!
                marionetteVideoFront.gameObject.SetActive(true);
                marionetteVideoBack.gameObject.SetActive(true);
                marionnettisteAudio.gameObject.SetActive(true);
                SuperTitles.StartTimer();

                StartCoroutine(StartCountdown());
            }
            else
            {
                //Debug.Log("NOT IN THEATRE!");
            }
        }
/*
        if(Input.GetKeyDown(KeyCode.N)){
            Debug.Log("STARTING LONG DUMB THING SHOULD FREEZE THE GAME");
            expensiveDumbOperationForTesting();
            Debug.Log("ENDING DUMB THING GAME IS BACK!");
        }

        if(Input.GetKeyDown(KeyCode.M)){
            Debug.Log("STARTING SMART THREAD FOR DUMMY DATA GAME SHOULD NOT FREEZE");
            threadTestJob = new TestJob();
            threadTestJob.InData = new Vector3[3];
            threadTestJob.OutData = new Vector3[5];

            threadTestJob.InData[0] = new Vector3(1f, 0f, 0f);
            threadTestJob.InData[1] = new Vector3(0f, 1f, 0f);
            threadTestJob.InData[2] = new Vector3(0f, 0f, 1f);
            threadTestJob.Start();
            Debug.Log("SMART THREAD THING FOR DUMMY DATA ENDED");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("STARTING SMART THREAD FOR ENSEMBLE SHOULD NOT FREEZE");
            ensembleJob = new EnsembleCalculateVolitionsJob();
            ensembleJob.InDataEnsemble = this.data.ensemble;
            ensembleJob.InDataCharactersToCalculateVolitionsFor = this.cast;
            volitionInterfaceIsLocked = true;
            ensembleJob.Start();
            Debug.Log("SMART TREAD THING FOR ENSEMBLE ENDED");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("STARTING SMART THREAD FOR ENSEMBLE GRAND FINALE SHOULD NOT FREEZE");
            ensembleCalculateVolitionsJobForFinalInterlocutor = new EnsembleCalculateVolitionsJobForFinalInterlocutor();
            ensembleCalculateVolitionsJobForFinalInterlocutor.InDataEnsemble = this.data.ensemble;
            ensembleCalculateVolitionsJobForFinalInterlocutor.InDataCharactersToCalculateVolitionsFor = this.cast;
            volitionInterfaceIsLocked = true;
            ensembleCalculateVolitionsJobForFinalInterlocutor.Start();
            Debug.Log("SMART TREAD THING FOR ENSEMBLE ENDED");
        }
*/

        //GameObject ticketTaker = GameObject.Find("TicketTakerTest");
        

        if(arrowTarget == null){
            //if there is no arrow target, turn off the arrow
            arrow.SetActive(false);
        }
        else{
            arrow.SetActive(true);
            arrow.transform.LookAt(arrowTarget.transform.position);
        }
        

        //Vector3 dir = arrow.transform.InverseTransformPoint(ticketTaker.transform.position);

        if(Input.GetKeyDown(KeyCode.J)){
            Debug.Log("rotating...");
            //playerCamera.transform.localRotation = Quaternion.Euler(0,180,0);
            //playerCamera.transform.rotation = Quaternion.Euler(0,180,0);
            Debug.Log("Payer camera is: " + playerCamera.name);
            Debug.Log("player camera parent is: " + playerCamera.transform.parent.name); 
        }

        if(threadTestJob != null){
            if(threadTestJob.Update()){
                threadTestJob = null;
            }
        }

        if(ensembleJob != null){
            if(ensembleJob.Update()){
                this.volitionInterface = ensembleJob.OutDataVolitionInterface;
                volitionInterfaceIsLocked = false; // we can use volitions again.
                SetCharacterAvailability();
                ensembleJob = null;
            }
        }
        if(ensembleCalculateVolitionsForSetCharacterAvailability != null){
            if(ensembleCalculateVolitionsForSetCharacterAvailability.Update()){
                this.volitionInterface = ensembleCalculateVolitionsForSetCharacterAvailability.OutDataVolitionInterface;
                ensembleCalculateVolitionsForSetCharacterAvailability = null;
                foreach (string character in cast)
                {
                    string initiator = EnsemblePlayer.GetSelectedCharacter();
                    string responder = character;

                    List<Action> actions = data.ensemble.getActions(initiator, responder, this.volitionInterface, cast, 999, 999, 999);
                    characterAvailable[character] = actions.Count > 0;
                }
            }
        }

        //Thread that runs when we are populating the actions of a character.
        if (ensembleCalculateVolitionsJobForGetCharacterActions != null)
        {
            if (ensembleCalculateVolitionsJobForGetCharacterActions.Update())
            {
                bool isGrandFinale = false;
                this.volitionInterface = ensembleCalculateVolitionsJobForGetCharacterActions.OutDataVolitionInterface;
                volitionInterfaceIsLocked = false; // we can safely use it again!
                SetCharacterAvailability();
                
                Debug.Log("About to compute actions for " + ensembleCalculateVolitionsJobForGetCharacterActions.InDataInitiator + " and " + ensembleCalculateVolitionsJobForGetCharacterActions.InDataResponder);
                List<Action> actions = data.ensemble.getActions(ensembleCalculateVolitionsJobForGetCharacterActions.InDataInitiator, ensembleCalculateVolitionsJobForGetCharacterActions.InDataResponder, volitionInterface, cast, 999, 999, 999);
                Debug.Log("Action count is: " + actions.Count);

                if (ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName == "Cellist")
                {
                    CloseMenu();
                    StartCoroutine(DisplayDialogue(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, "The cellist is busy playing."));
                }
                else if (hud.GetQuestProgress() == HUD.BACKSTAGE_ACCESS)
                {
                    CloseMenu();

                    if (!ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse)
                    {
                        StartCoroutine(DisplayDialogue(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, "Please, I'm trying to watch the performance!"));
                    }
                }
                else if (hud.GetQuestProgress() == HUD.POSSESS_PLANS)
                {
                    if (finalInterlocutor == ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName)
                    {
                        Debug.Log("clicked on finalInterlocutor");
                        if (approachedFinalInterlocutor != true)
                        {
                            approachedFinalInterlocutor = true;

                            foreach (Action action in actions)
                            {
                                if (action.Name.Contains("interaction"))
                                {
                                    TakeAction(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, action);
                                    ensembleCalculateVolitionsJobForFinalInterlocutor = new EnsembleCalculateVolitionsJobForFinalInterlocutor();
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDataCharactersToCalculateVolitionsFor = this.cast;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDataEnsemble = this.data.ensemble;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDataInitiator = ensembleCalculateVolitionsJobForGetCharacterActions.InDataInitiator;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDataObjectName = ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDataResponder = ensembleCalculateVolitionsJobForGetCharacterActions.InDataResponder;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.InDatasuppressResponse = ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse;
                                    volitionInterfaceIsLocked = true;
                                    isGrandFinale = true;
                                    ensembleCalculateVolitionsJobForFinalInterlocutor.Start();
                                    /*
                                    volitionInterface = calculateVolition(cast, null, () =>
                                    {
                                        actions = data.ensemble.getActions(ensembleCalculateVolitionsJobForGetCharacterActions.InDataInitiator, ensembleCalculateVolitionsJobForGetCharacterActions.InDataResponder, volitionInterface, cast, 999, 999, 999);
                                        ShowActionsList(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, actions, ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse);
                                        return;
                                    });
                                    */
                                }
                            }
                        }
                    }
                    else
                    {
                        CloseMenu();
                        StartCoroutine(DisplayDialogue(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, "Did I just see you sneak backstage?"));
                    }
                }
                else if (actions.Count > 0 && actions[0].Name.Contains("interaction") && hud.GetQuestProgress() != HUD.FINAL_INTERACTION)
                {
                    //Debug.Log("I think I'm getting here...? What is this interaction thingy? " + actions[0].Name);
                    Debug.Log("In data object name BUG? actions.count is " + actions.Count);
                    actions = new List<Action>();
                }
                else if (hud.GetQuestProgress() == HUD.FINAL_INTERACTION)
                {
                    Debug.Log("In data object name " + ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName + " final interlocutor: " + finalInterlocutor);
                    if (ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName == "Marie-Catherine Bienfait, ticket taker")
                    {
                        if (actions.Count > 0)
                        {
                            CloseMenu();
                            ShowFinalText(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, actions[0]);
                        }
                    }
                    else if (ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName == finalInterlocutor)
                    {
                        CloseMenu();
                        StartCoroutine(DisplayDialogue(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, "Shouldn't you be on your way out by now?"));
                    }
                    else
                    {
                        CloseMenu();
                        StartCoroutine(DisplayDialogue(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, "Stop, thief!"));
                        //StartCoroutine(GameOver());
                    }
                }

                //if it is the grand finale, we have a separate thread for that.
                if(!isGrandFinale){
                    Debug.Log("*** ABOUT TO SHOW ACTION LIST AFTER THREAD COMPLETED FOR: " + ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName + " number of actions: " + actions.Count + " supress response: " + ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse);
                    ShowActionsList(ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName, actions, ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse);
                }
                //zero this out for next time!
                ensembleCalculateVolitionsJobForGetCharacterActions = null;

            }
        }



        //For the very end!
        if (ensembleCalculateVolitionsJobForFinalInterlocutor != null)
        {
            if (ensembleCalculateVolitionsJobForFinalInterlocutor.Update())
            {
                this.volitionInterface = ensembleCalculateVolitionsJobForFinalInterlocutor.OutDataVolitionInterface;
                this.volitionInterfaceIsLocked = false;

                List<Action> actions = data.ensemble.getActions(ensembleCalculateVolitionsJobForFinalInterlocutor.InDataInitiator, ensembleCalculateVolitionsJobForFinalInterlocutor.InDataResponder, this.volitionInterface, cast, 999, 999, 999);
                ShowActionsList(ensembleCalculateVolitionsJobForFinalInterlocutor.InDataObjectName, actions, ensembleCalculateVolitionsJobForFinalInterlocutor.InDatasuppressResponse);


                ensembleCalculateVolitionsJobForFinalInterlocutor = null;
            }
        }

        


    }

    public void SetCharacterAvailability()
    {

        if(!volitionInterfaceIsLocked && this.volitionInterface != null){
            Debug.Log("**** about to set character availability");
            foreach (string character in cast)
            {
                string initiator = EnsemblePlayer.GetSelectedCharacter();
                string responder = character;

                List<Action> actions = data.ensemble.getActions(initiator, responder, this.volitionInterface, cast, 999, 999, 999);
                bool areTheyAvailable = actions.Count > 0;

                //I also think we need to do something special so that all of them being angry at you at the end marks them as red.
                /*
                if(actions.Count > 0 && (finalInterlocutor.Length > 0)){
                    Debug.Log("Final interlocutor is: " + finalInterlocutor);
                    Debug.Log("Final INterlocturo is " + finalInterlocutor.Length);
                    areTheyAvailable = false;
                }
                */
                //I don't fully understand this, but checking to see if the first action has "interaction" in it seems to be code that 
                //keeps them from wanting to takl to you -- mark it so that they are unavailable if this is the case.


                if (actions.Count > 0 && actions[0].Name.Contains("interaction"))
                {
                    areTheyAvailable = false;
                }

                Debug.Log("hud qeust progress is: " + hud.GetQuestProgress() + " and final interloctur is: " + finalInterlocutor + " and character is: " + character);
                if (hud.GetQuestProgress() == HUD.POSSESS_PLANS)
                {
                    areTheyAvailable = false;
                    if(character == finalInterlocutor){
                        areTheyAvailable = true;
                    }
                }
                
                if(hud.GetQuestProgress() == HUD.FINAL_INTERACTION){
                    areTheyAvailable = false;
                    if(character == "Marie-Catherine Bienfait, ticket taker"){
                        areTheyAvailable = true;
                    }
                }




                Debug.Log("SETCHARACTERAVAILABILITY: " + character + " is available: " + areTheyAvailable + " (action count was " + actions.Count);
                characterAvailable[character] = areTheyAvailable;
            }
        }
        else{
            Debug.Log("**** Tried to set character availability but either volition interface was locked or volition interface was null");
        }
        
        /*
        if(ensembleCalculateVolitionsForSetCharacterAvailability == null){
            //only do this if it isn't null!
            Debug.Log("trying to calculate volition...");
            ensembleCalculateVolitionsForSetCharacterAvailability = new EnsembleCalculateVolitionForSetCharacterAvailability();
            ensembleCalculateVolitionsForSetCharacterAvailability.InDataEnsemble = data.ensemble;
            Debug.Log("Ensemble API: " + ensembleCalculateVolitionsForSetCharacterAvailability.InDataEnsemble);
            ensembleCalculateVolitionsForSetCharacterAvailability.InDataCharactersToCalculateVolitionsFor = cast;
            ensembleCalculateVolitionsForSetCharacterAvailability.Start();
        }
        */

        /*
        volitionInterface = calculateVolition(cast, null, () => {

        });
        */
    }

    private IEnumerator<object> StartCountdown()
    {
        yield return new WaitForSeconds(300);

        if (hud.GetQuestProgress() == HUD.RECEIVED_MARK) {
            if (finalInterlocutor != null) {
                string initiator = EnsemblePlayer.GetSelectedCharacter();
                Predicate accessPred = new Predicate("progress", "BackstageAccess", initiator, true);
                Predicate greetedPred = new Predicate("conversation", "GreetedStranger", initiator, finalInterlocutor, true);
                Predicate greetPred = new Predicate("conversation", "GreetStranger", initiator, finalInterlocutor, false);
                Predicate negativeInteractionPred = new Predicate("conversation", "NegativeInteraction", initiator, finalInterlocutor, true);

                data.ensemble.set(accessPred);
                data.ensemble.set(greetedPred);
                data.ensemble.set(greetPred);
                data.ensemble.set(negativeInteractionPred);

                hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                StartCoroutine(ShowProgress(1, "You are running out of time... the play is almost over, but you see an opening, so you head backstage! Hold still..."));
            } else {
                hud.UpdateQuestProgress(HUD.THROWN_OUT);
                StartCoroutine(ShowProgress(1, "You are running out of time... the play is almost over, and unfortunately you haven't spoken to enough people to create a diversion!"));
            }
        }
    }

    public void DisplayMain()
    {
        if (!mainActiveUI)
        {
            ensembleUI.SetActive(true);
            mainActiveUI = true;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            attributesUI.SetActive(false);
            attributesActiveUI = false;

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

            attributesUI.SetActive(false);
            attributesActiveUI = false;

            historyUI.SetActive(false);
            historyActiveUI = false;

            omekaUI.SetActive(false);
            omekaActiveUI = false;

            FindObjectsGetStrings();
            
            //actionsMenu.GetComponent<UnityEngine.UI.Text>().text = actionsBuilder.ToString();
        }
        
    }

    public void DisplayAttributes()
    {
        if (!attributesActiveUI)
        {
            ensembleUI.SetActive(false);
            mainActiveUI = false;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            attributesUI.SetActive(true);
            attributesActiveUI = true;

            historyUI.SetActive(false);
            historyActiveUI = false;

            omekaUI.SetActive(false);
            omekaActiveUI = false;

            FindObjectsGetStrings();

            // directedStatusMenu.GetComponent<UnityEngine.UI.Text>().text = directedStatusBuilder.ToString();
            // networkMenu.GetComponent<UnityEngine.UI.Text>().text = networkBuilder.ToString();
            // nonActionableRelationshipMenu.GetComponent<UnityEngine.UI.Text>().text = nonActionableRelationshipBuilder.ToString();
            // relationshipMenu.GetComponent<UnityEngine.UI.Text>().text = relationshipBuilder.ToString();
            // socialRecordLabelMenu.GetComponent<UnityEngine.UI.Text>().text = socialRecordLabelBuilder.ToString();
            // statusMenu.GetComponent<UnityEngine.UI.Text>().text = statusBuilder.ToString();
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

            attributesUI.SetActive(false);
            attributesActiveUI = false;

            historyUI.SetActive(true);
            historyActiveUI = true;

            omekaUI.SetActive(false);
            omekaActiveUI = false;

            FindObjectsGetStrings();

            historyMenu.GetComponent<UnityEngine.UI.Text>().text = historyBuilder.ToString();
        }
    }

    public void DisplayOmeka()
    {
        if (!omekaActiveUI)
        {
            ensembleUI.SetActive(false);
            mainActiveUI = false;

            actionsUI.SetActive(false);
            actionsActiveUI = false;

            attributesUI.SetActive(false);
            attributesActiveUI = false;

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
    }

    public void CloseMenu()
    {

        Debug.Log("CALLING CLOSE MENU");

        ensembleUI.SetActive(false);
        mainActiveUI = false;

        actionsUI.SetActive(false);
        actionsActiveUI = false;

        attributesUI.SetActive(false);
        attributesActiveUI = false;

        historyUI.SetActive(false);
        historyActiveUI = false;

        omekaUI.SetActive(false);
        omekaActiveUI = false;

        foreach(GameObject bt in actionButtonRefs) {
            Destroy(bt);
        }

        actionButtonRefs.Clear();
    }

    public void CloseProgress()
    {
        progressPanel.SetActive(false);
        hud.replaceHud();

        if (hud.GetQuestProgress() == HUD.BACKSTAGE_ACCESS) {
            //start the 'backwards play'
            // marionetteVideoFront.gameObject.SetActive(false);
            // marionetteVideoBack.gameObject.SetActive(true);
            // marionetteVideoBack.isLooping = true;
            // marionnettisteAudio.gameObject.SetActive(true);
            // marionnettisteAudio.Play();
            // marionnettisteAudio.loop = true;

            StartCoroutine(TransportHeadBackstage());
        } else if (hud.GetQuestProgress() == HUD.POSSESS_PLANS) {
            //start the 'forwards' play again.
            // marionetteVideoFront.gameObject.SetActive(true);
            // marionetteVideoFront.frame = 0;
            // SuperTitles.ResetSupertitles();
            // marionetteVideoBack.gameObject.SetActive(false);
            // marionnettisteAudio.gameObject.SetActive(true);
            // marionnettisteAudio.Play();
            
            StartCoroutine(TransportReturnToTheater());
        } else if (hud.GetQuestProgress() == HUD.THROWN_OUT) {
            StartCoroutine(GameOver());
        } else if (hud.GetQuestProgress() == HUD.GAME_COMPLETED) {
            HandleFinalResults();
        }
    }

    private void FindObjectsGetStrings()
    {
        attributesMenu.GetComponent<UnityEngine.UI.Text>().text = attributesBuilder.ToString();
        traitsMenu.GetComponent<UnityEngine.UI.Text>().text = traitsBuilder.ToString();
        //clothingMenu.GetComponent<UnityEngine.UI.Text>().text = clothingBuilder.ToString();
        //professionMenu.GetComponent<UnityEngine.UI.Text>().text = professionBuilder.ToString();
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

        attributesUI.transform.SetParent(leftHand.transform);
        attributesUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        attributesUI.transform.localPosition = new Vector3(0.1f, 0.0f, 0.2f);
        attributesUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

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
        EnsembleObject ensemble = e.target.GetComponent<EnsembleObject>();
        Debug.Log("CLICK CLICK CLICK");
        if (ensemble != null)
        {
            //The object is an Ensemble object
            
            if (ensemble.name == "TheatrePlans") {
                GameObject.Find("TheatrePlans").SetActive(false);

                artGallery.transform.SetParent(GameObject.Find("LeftHand").transform);
                artGallery.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
                artGallery.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
                artGallery.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
            } else {
                Orientation();
                DisplayMain();

                attributesBuilder.Clear();
                traitsBuilder.Clear();
                clothingBuilder.Clear();
                professionBuilder.Clear();
                directedStatusBuilder.Clear();
                networkBuilder.Clear();
                nonActionableRelationshipBuilder.Clear();
                relationshipBuilder.Clear();
                socialRecordLabelBuilder.Clear();
                statusBuilder.Clear();
                historyBuilder.Clear();
                actionsBuilder.Clear();

                characterMenu.GetComponent<UnityEngine.UI.Text>().text = ensemble.name;

                getCharacterData(ensemble.name);
                getCharacterHistory(ensemble.name);
                getCharacterActions(ensemble.name, false);
                getCharacterOmeka(ensemble.name);
            }
        }
        if (e.target.gameObject.layer == 5)
        {
            //layer of the UI: 5 
            Button button = e.target.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.Invoke();
            }
        }
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        laserPointer.color = hit_color;
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        laserPointer.color = miss_color;
    }

    public void getCharacterData(string objectName)
    {
        Predicate searchPredicate = new Predicate();
        searchPredicate.First = objectName;

        List<Predicate> socialRecordData = data.ensemble.getSocialRecordCopyAtTimestep(0);
        List<Predicate> characterData = socialRecordData.FindAll(predicate => predicate.First == objectName).ToList();

        foreach (Predicate datum in characterData)
        {
            string[] predicateDebug = new string[] { datum.First, datum.Second, datum.Category, datum.Type };
            string predicateToString = string.Format("First: {0}, Second: {1}, Category: {2}, Type: {3}", predicateDebug);

            if (datum.Category == "trait")
            {
                traitsBuilder.Append(datum.Type + "\n");
            }
            else if (datum.Category == "role" && datum.Type != "player")
            {
                attributesBuilder.Append(datum.Type + "\n");
            }

            // if(datum.Category == "Attribute")
            // {
            //     attributesBuilder.Append(datum.Type + "\n");
            // }else if (datum.Category == "Trait")
            // {
            //     traitsBuilder.Append(datum.Type + "\n");
            // }
            // else if (datum.Category == "TraitClothing")
            // {
            //     clothingBuilder.Append(datum.Type + "\n");
            // }
            // else if (datum.Category == "TraitConditionProfession")
            // {
            //     professionBuilder.Append(datum.Type + "\n");
            // }
            // else if (datum.Category == "DirectedStatus")
            // {
            //     directedStatusBuilder.Append(predicateToString + "\n");
            // }
            // else if (datum.Category == "Network")
            // {
            //     networkBuilder.Append(predicateToString + "\n");
            // }
            // else if (datum.Category == "NonActionableRelationship")
            // {
            //     nonActionableRelationshipBuilder.Append(predicateToString + "\n");
            // }
            // else if (datum.Category == "Relationship")
            // {
            //     relationshipBuilder.Append(predicateToString + "\n");
            // }
            // else if (datum.Category == "SocialRecordLabel")
            // {
            //     socialRecordLabelBuilder.Append(predicateToString + "\n");
            // }
            // else if (datum.Category == "Status")
            // {
            //     statusBuilder.Append(predicateToString + "\n");
            // }
        }
    }

    public void getCharacterHistory(string objectName)
    {
        Predicate searchPredicate = new Predicate();
        searchPredicate.First = objectName;

        List<List<Predicate>> socialRecordDataByTimestep = data.ensemble.getSocialRecordCopy();

        // int timestep = 0;

        foreach (List<Predicate> predicatesForTimestep in socialRecordDataByTimestep)
        {
            List<Predicate> characterData = predicatesForTimestep.FindAll(predicate => predicate.First == objectName).ToList();
            // historyBuilder.Append("\nTimestep " + timestep.ToString() + ": " + "\n\n");

            foreach (Predicate datum in characterData)
            {
                string predicateToString = data.ensemble.predicateToEnglish(datum).Text;
                //string[] predicateDebug = new string[] { datum.First, datum.Second, datum.Category, datum.Type };
                //string predicateToString = string.Format("First: {0}, Second: {1}, Category: {2}, Type: {3}", predicateDebug);

                historyBuilder.Append(predicateToString + "\n");
            }
        }
    }

    public void getCharacterOmeka(string objectName){
        GameObject characterInQuestion = GameObject.Find(objectName);
        currentOmekaIDOfClickedCharacter = characterInQuestion.GetComponent<EnsembleObject>().omekaDatabaseID;
        currentPositionOfClickedCharacter = characterInQuestion.transform.position;
    }

    public void getCharacterActions(string objectName, bool suppressResponse)
    {
        string initiator = EnsemblePlayer.GetSelectedCharacter();
        string responder = objectName;

        if(ensembleCalculateVolitionsJobForGetCharacterActions == null){
            ensembleCalculateVolitionsJobForGetCharacterActions = new EnsembleCalculateVolitionsJobForGetCharacterActions();
            ensembleCalculateVolitionsJobForGetCharacterActions.InDataEnsemble = data.ensemble;
            ensembleCalculateVolitionsJobForGetCharacterActions.InDataCharactersToCalculateVolitionsFor = cast;
            ensembleCalculateVolitionsJobForGetCharacterActions.InDatasuppressResponse = suppressResponse;
            ensembleCalculateVolitionsJobForGetCharacterActions.InDataInitiator = initiator;
            ensembleCalculateVolitionsJobForGetCharacterActions.InDataObjectName = objectName;
            ensembleCalculateVolitionsJobForGetCharacterActions.InDataResponder = responder;
            volitionInterfaceIsLocked = true; // should be locked before we ever try to do something with it.
            ensembleCalculateVolitionsJobForGetCharacterActions.Start();
        }


        
        /*
        volitionInterface = calculateVolition(cast, null, () => {
            
        });
        */
    }

    private void ShowActionsList(string objectName, List<Action> actions, bool suppressResponse)
    {

        Debug.Log("In show action list object name is: " + objectName + " number of actions is " + actions.Count);

        float x = 0;
        float y = -0.05f;
        float z = 5602.218f;

        //Always clear out all of the action buttons so we are working with a blank slate.
        foreach (GameObject bt in actionButtonRefs)
        {
            Destroy(bt);
        }

        actionButtonRefs.Clear();

        if (actions.Count > 0) {
            foreach (Action action in actions)
            {
                Debug.Log("*** Loop! Action is " + action.DisplayName);
                if (!action.Name.Contains("interaction")) {
                    string actionName = action.Name;

                    if (actionName.Contains("(") && actionName.Contains(")")) {
                        int parensStart = actionName.LastIndexOf("(");
                        int parensEnd = actionName.LastIndexOf(")") + 1;
                        actionName = actionName.Remove(parensStart, parensEnd - parensStart);
                    }

                    ProfilerMarker s_PreparePerfMarker5 = new ProfilerMarker("about to instantiate a button");
                    s_PreparePerfMarker5.Begin();
                    GameObject goButton = (GameObject)Instantiate(prefabButton);
                    s_PreparePerfMarker5.End();
                    goButton.transform.SetParent(actionsMenuImageZone, false);

                    
                    goButton.GetComponent<RectTransform>().transform.localPosition = new Vector3(x, y, z);
                    goButton.GetComponent<RectTransform>().transform.rotation = new Quaternion(0, 0, 0, 0);
                    y -= 0.1f;

                    Button tempButton = goButton.GetComponent<Button>();
                    tempButton.GetComponentInChildren<Text>().text = actionName;

                    Debug.Log("***About to give button listener for: " + objectName + " " + action.DisplayName);
                    tempButton.onClick.AddListener(() => {Debug.Log("***CLICK CLICK MAYBE???***");  TakeAction(objectName, action);});
                    Debug.Log("***JUST GAVE BUTTON LISTENER FOR: " + objectName + " " + action.DisplayName);

                    actionButtonRefs.Add(goButton);
                }
            }
        } else {
            CloseMenu();
            if (!suppressResponse) {
                Debug.Log("wait maybe action count is actually zero?");
                StartCoroutine(DisplayDialogue(objectName, "This person seems busy or uninterested in talking to you."));
            }
        }
    }

    private IEnumerator<object> postRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }

    private IEnumerator<object> ShowFinalResults(string actions, string results)
    {
        yield return new WaitForSeconds(6);
        dialogueHUD.SetActive(false);
        hud.removeHud();

        resultsPanel.SetActive(true);
        resultsPanel.transform.rotation = playerCamera.transform.rotation;
        resultsPanel.transform.SetParent(playerCamera.transform);
        resultsPanel.transform.localPosition = resultsOffset;
        resultsPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = actions;

        StartCoroutine(postRequest("http://ensemble-tool.herokuapp.com/gamelog", results));
    }

    private void HandleFinalResults()
    {
        string title = "Results";
        actionsBuilder.Clear();
        actionsBuilder.Append(title + "\n\n");

        StringBuilder resultsJsonBuilder = new StringBuilder();
        resultsJsonBuilder.Append("{\"actions\":[");

        actionsBuilder.Append("Annoying interactions: " + positiveInteractionCount + "\n\n");
        actionsBuilder.Append("Impressive interactions: " + negativeInteractionCount + "\n\n");

        actionsBuilder.Append("You talked to these people you knew: \n");
        foreach (string acquaintance in acquaintanceInteractions) {
            actionsBuilder.Append(acquaintance + "\n");
        }

        actionsBuilder.Append("\n\nYou talked to these people you didn't know: \n");
        foreach (string stranger in strangerInteractions) {
            actionsBuilder.Append(stranger + "\n");
        }

        int i = 1;
        foreach(Action act in gameActions)
        {
            // actionsBuilder.Append(i.ToString() + ": " + act.Name.ToString() + "\n");
            resultsJsonBuilder.Append("{ \"name\": \"" + act.Name.ToString() + "\"}");

            if (i < gameActions.Count) {
                resultsJsonBuilder.Append(",");
            }

            i++;
        }

        resultsJsonBuilder.Append("]}");

        // List<List<Predicate>> socialRecordDataByTimestep = data.ensemble.getSocialRecordCopy();

        StartCoroutine(ShowFinalResults(actionsBuilder.ToString(), resultsJsonBuilder.ToString()));
    }

    private void ShowFinalText(string objectName, Action action)
    {
        StartCoroutine(DisplayDialogue(objectName, action.Name));
        hud.UpdateQuestProgress(HUD.GAME_COMPLETED);

        data.ensemble.takeAction(action);
        finalResult = getDialogueResponse(action);
        StartCoroutine(ShowProgress(2, finalResult));
    }

    private string getDialogueResponse(Action action) 
    {
        string dialogueResponse = "";
        if (action != null && action.Performance != null) {
            foreach(List<Performance> performanceQueue in action.Performance) {
                foreach(Performance p in performanceQueue) {
                    if (p.Type == "dialogue" && p.Text != null) {
                        dialogueResponse = p.Text;
                    }
                }
            }
        }
        return dialogueResponse;
    }

    private IEnumerator<object> TransportHeadBackstage()
    {
        if (SteamVRObjects.activeSelf)
        {
            //SteamVR_Fade.Start(Color.black, 10);
            //SteamVR_Fade.View(Color.black, 10);
            yield return new WaitForSeconds(1);
            playerObject.transform.position = new Vector3(4f, 1f, 2f);
            //SteamVR_Fade.Start(Color.clear, 10);
            //SteamVR_Fade.View(Color.clear, 10);
        } else {
            playerObject.transform.position = GameObject.Find("Backstage Right Tele").transform.localPosition;
        }
        playerCamera.transform.localPosition = new Vector3(0f, 1f, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(0, 90, 0);
        arrowTarget = GameObject.Find("TheatrePlans");
        Debug.Log("ARROW: target is now " + arrowTarget.name);
    }

    private IEnumerator<object> TransportReturnToTheater()
    {
        if (SteamVRObjects.activeSelf)
        {
            // vm.PositionAssigner();

            if (finalInterlocutor == "Valère") {
                GameObject valere = GameObject.Find(finalInterlocutor);
                GameObject madameDuPuyDesGougeres = GameObject.Find("Madame du Puy-des-Gougères");
                arrowTarget = valere;
                Transform madameParent = madameDuPuyDesGougeres.transform.parent;
                madameParent.GetComponent<NPCNavMesh>().myViewingTransform = lastInterlocutorTransform;
                madameParent.transform.position = new Vector3(2.5f, 0f, 2f);
                lastInterlocutorParticleSystem = valere.transform.GetChild(0).GetChild(0);
                Debug.Log("the game object we found was: " + lastInterlocutorParticleSystem);
                lastInterlocutorParticleSystem.gameObject.SetActive(true); // turn on the particle system!
            } else {
                GameObject lastInterlocutor = GameObject.Find(finalInterlocutor);
                arrowTarget = lastInterlocutor;
                Transform lastInterlocutorParent = lastInterlocutor.transform.parent;
                //if they have a nav mesh, it means they can walk -- teleport them close to the player.
                //otherwise, they are seated, so don't bother moving them.
                if(lastInterlocutorParent.GetComponent<NPCNavMesh>() != null){
                    lastInterlocutorParent.GetComponent<NPCNavMesh>().myViewingTransform = lastInterlocutorTransform;
                    lastInterlocutorParent.transform.position = new Vector3(2.5f, 0f, 2f);
                }
                lastInterlocutorParticleSystem = lastInterlocutor.transform.GetChild(0).GetChild(0);
                Debug.Log("the game object we found was: " + lastInterlocutorParticleSystem);
                lastInterlocutorParticleSystem.gameObject.SetActive(true); // turn on the particle system!
            }

            //SteamVR_Fade.Start(Color.black, 10);
            yield return new WaitForSeconds(1);
            playerObject.transform.position = new Vector3(3f, 0f, 2f);
            playerCamera.transform.localRotation = Quaternion.Euler(0, 180, 0);
            //SteamVR_Fade.Start(Color.clear, 10);
        } else {
            //playerObject.transform.position = GameObject.Find("Backstage Right Tele").transform.position;
            GameObject lastInterlocutor = GameObject.Find(finalInterlocutor);
            arrowTarget = lastInterlocutor;
            lastInterlocutorParticleSystem = lastInterlocutor.transform.GetChild(0).GetChild(0);
            Debug.Log("the game object we found was: " + lastInterlocutorParticleSystem);
            lastInterlocutorParticleSystem.gameObject.SetActive(true); // turn on the particle system!
            
        }
    }

    public IEnumerator<object> ShowProgress(int delay, string progressText)
    {
        yield return new WaitForSeconds(delay);

        dialogueHUD.SetActive(false);
        hud.removeHud();

        if (hud.GetQuestProgress() == HUD.POSSESS_PLANS) {
            artGallery.gameObject.SetActive(false);
        }

        if (!initiatedProgressPanel) {
            progressPanel.transform.rotation = playerCamera.transform.rotation;
            progressPanel.transform.SetParent(playerCamera.transform);
            progressPanel.transform.localPosition = resultsOffset;
        }

        progressPanel.SetActive(true);
        progressPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = progressText;
    }

    public IEnumerator<object> WaitToUpdateQuestProgress(float secondsToWait, int hudProgressToUpdateTo){
        yield return new WaitForSeconds(secondsToWait);

        hud.UpdateQuestProgress(hudProgressToUpdateTo);

        //character availability is directly connected to quest progress!
        SetCharacterAvailability();

        //if the action menu is open, close it
        if(actionsUI.activeSelf){
            actionsUI.SetActive(false);
        }
    }

    public void TakeAction(string objectName, Action action)
    {
        Debug.Log("**** Inside of Take Action");
        data.ensemble.takeAction(action);
        gameActions.Add(action);

        string dialogueResponse = "";
        bool backstageAccess = false;
        bool successfulDistraction = false;
        bool whistleAndStomp = false;

        List<Predicate> socialRecordData = data.ensemble.getSocialRecordCopyAtTimestep(0);
        List<Predicate> characterData = socialRecordData.FindAll(predicate => predicate.First == objectName).ToList();

        foreach(Predicate pred in characterData) {
            if (pred.Category == "role" && pred.Type == "stranger")
            {
                if (!strangerInteractions.Contains(objectName)) {
                    strangerInteractions.Add(objectName);
                }
            }

            if (pred.Category == "role" && pred.Type == "acquaintance") 
            {
                if (!acquaintanceInteractions.Contains(objectName)) {
                    acquaintanceInteractions.Add(objectName);
                }
            }
        }

        if (action.Effects != null) {
            foreach(Effect e in action.Effects) {
                if (e.Type == "BackstageAccess" && e.Value is bool && e.Value is true) {
                    backstageAccess = true;
                }
            }

            foreach(Effect e in action.Effects) {
                if (e.Type == "HasTicket" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.POSSESS_TICKET);
                }
                
                if (e.Type == "GaveTicket" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.HANDED_TICKET_TO_TICKET_TAKER);
                }

                if (e.Type == "HasMark" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.RECEIVED_MARK);

                    //start the show -- not sure if this is where people start taking their seats?
                    //GameObject.Find("Marionettes").transform.Find("Marionette Video Front").gameObject.SetActive(true);
                    //marionetteVideoFront.gameObject.SetActive(true);
                    //marionnettisteAudio.gameObject.SetActive(true);
                    //SuperTitles.StartTimer();

                    StartCoroutine(ShowProgress(4, "Good job! You've received a mark, which you needed in order to enter the theatre. Now make your way into the theater and start talking to people in order to find a way backstage.\n\nRemember: talk to people by selecting them with the trigger button. Move through the space with the joystick or touchpad.\n\nYou will need to speak to at least two people in order to proceed."));

                    //make it so that now you can teleport into the theatre.
                    SetUsabilityOfHouseTeleporters(true);

                    //turn off the arrow for the moment!
                    arrowTarget = null;
                }

                if (e.Type == "StompAndWhistle" && e.Value is bool && e.Value is true) {
                    GameObject characterInQuestion = GameObject.Find(objectName);
                    Animator anim = characterInQuestion.GetComponent<Animator>();

                    if (anim != null) {
                        anim.SetTrigger("Active");
                        whistleAudioSource.Play();
                        stompAudioSource.Play();
                    }

                    whistleAndStomp = true;
                }

                if (e.Type == "NearStageInteraction" && e.Value is bool && e.Value is true) {

                }

                if (e.Type == "SuccessfulDistraction" && e.Value is bool && e.Value is true) {
                    GameObject characterInQuestion = GameObject.Find(objectName);
                    Animator anim = characterInQuestion.GetComponent<Animator>();

                    if (anim != null) {
                        anim.SetTrigger("Active");
                        // yellingAudioSource.Play();
                    }

                    successfulDistraction = true;
                }

                if (e.Type == "FinalInteraction" && e.Value is bool && e.Value is true) {
                    Debug.Log("We are in final interaction!");
                    StartCoroutine(WaitToUpdateQuestProgress(3.5f, HUD.FINAL_INTERACTION));
                    //hud.UpdateQuestProgress(HUD.FINAL_INTERACTION);

                    //Turn off previous person Halo, turn on ticket taker halo.
                    lastInterlocutorParticleSystem.gameObject.SetActive(false);
                    GameObject ticketTaker = GameObject.Find("Marie-Catherine Bienfait, ticket taker");
                    arrowTarget = ticketTaker;
                    Transform ticketTakerParticleSystem = ticketTaker.transform.GetChild(0).GetChild(0);
                    ticketTakerParticleSystem.gameObject.SetActive(true);

                }

                if (e.Type == "ThrownOut" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.THROWN_OUT);
                    CloseMenu();
                    StartCoroutine(ShowProgress(1, "Unfortunately you've drawn too much attention to yourself. The bouncer grabs you by the collar and begins to drag you out of the theatre."));
                }

                if (e.Type == "PositiveInteraction" && e.Value is bool && e.Value is true) {
                    positiveInteractionCount += 1;
                    Debug.Log("had a positive interaction!");
                }

                if (e.Type == "NegativeInteraction" && e.Value is bool && e.Value is true) {
                    negativeInteractionCount += 1;
                    Debug.Log("had a negative interaction!");

                    if (negativeInteractionCount == 2) {
                        CloseMenu();
                        string role = EnsemblePlayer.GetSelectedCharacter().ToLower();
                        StartCoroutine(ShowProgress(1, "It looks like you're drawing attention to yourself. Try to find a fellow " + role + " to talk to who may be more willing to help."));
                    }
                }

                // bool isCompleteInteraction = e.Value is bool && e.Value is true &&
                //     (e.Type == "AcquaintanceSecondInteraction" || 
                //     e.Type == "NegativeInteraction" || 
                //     e.Type == "PositiveInteraction");
                bool isCompleteInteraction = true;

                if (isCompleteInteraction && objectName != "Marie-Catherine Bienfait, ticket taker") {
                    if (!characterInteractions.Contains(objectName)) {
                        characterInteractions.Add(objectName);

                        if (characterInteractions.Count == 2) {
                            completedTwoInteractions = true;
                        }

                        if (hud.GetQuestProgress() == HUD.RECEIVED_MARK) {
                            hud.UpdateQuestProgress(HUD.RECEIVED_MARK);
                        }
                    }
                }
            }
        }

        if (whistleAndStomp && hud.GetQuestProgress() != HUD.BACKSTAGE_ACCESS) {
            if (completedTwoInteractions) {
                finalInterlocutor = objectName;
                hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                StartCoroutine(ShowProgress(1, "All of that whistling and stomping is seriously distracting! It looks like now is your chance to slip backstage... hold still!"));
            } else {
                StartCoroutine(ShowProgress(1, "Your friend is really causing a scene with the whistling and stomping! Still, you need to speak to at least one more person to gather further intel and continue your distactions."));
            }
        } else if (successfulDistraction && hud.GetQuestProgress() != HUD.BACKSTAGE_ACCESS) {
            if (completedTwoInteractions) {
                    finalInterlocutor = objectName;
                    hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                    StartCoroutine(ShowProgress(1, "Your friend has really caused a scene! It seems to have distracted the crowd enough that you can slip backstage... hold still!"));
            } else {
                StartCoroutine(ShowProgress(1, "Your friend is making a serious racket! Still, you need to speak to at least one more person to gather further intel and continue your distactions."));
            }
        } else if (backstageAccess && hud.GetQuestProgress() != HUD.BACKSTAGE_ACCESS) {
            if (completedTwoInteractions) {
                finalInterlocutor = objectName;
                hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                StartCoroutine(ShowProgress(1, "Good job! You've managed to create an opening to slip backstage! You will be transported behind the curtains, where you should look for the plans. Please hold still..."));
            } else {
                StartCoroutine(ShowProgress(1, "Great, you're certainly being persuasive! Still, you need to speak to at least one more person to gain further intel before going backstage."));
            }
        }

        if (hud.GetQuestProgress() == HUD.RECEIVED_MARK) {
            finalInterlocutor = objectName;
        }

        dialogueResponse = getDialogueResponse(action);

        Debug.Log("****About to call close menu inside of take action...");
        CloseMenu();
        DisplayActions();
        getCharacterActions(objectName, true);

        StartCoroutine(DisplayDialogue(objectName, dialogueResponse));
        SetCharacterAvailability();
    }

    public void ExitGame()
    {
        data.ensemble.clearHistory();
        SceneManager.LoadScene("Intro");
    }

    private IEnumerator<object> GameOver()
    {
        SteamVR_Fade.Start(Color.black, 10);
        yield return new WaitForSeconds(3);
        ExitGame();
    }

    public void setDialogueHudImage(string characterName)
    {
        if (characterName == "Marie-Catherine Bienfait, ticket taker") {
            dialogueIcon.GetComponent<RawImage>().texture = MarieCatherineBienfait;
        } else if (characterName == "Bruno Dufort, semainier") {
            dialogueIcon.GetComponent<RawImage>().texture = BrunoDufort;
        } else if (characterName == "Monsieur d'Ain d'Ygeste") {
            dialogueIcon.GetComponent<RawImage>().texture = MonsieurdAindYgeste;
        } else if (characterName == "Monsieur d'Hautainville") {
            dialogueIcon.GetComponent<RawImage>().texture = MonsieurdHautainville;
        } else if (characterName == "Monsieur de Gentilly") {
            dialogueIcon.GetComponent<RawImage>().texture = MonsieurdeGentilly;
        } else if (characterName == "Monsieur des Trois-Landes") {
            dialogueIcon.GetComponent<RawImage>().texture = MonsieurdesTroisLandes;
        } else if (characterName == "Monsieur d'Issy") {
            dialogueIcon.GetComponent<RawImage>().texture = MonsieurdIssy;
        } else if (characterName == "Madame de Blasé-l'Evêque") {
            dialogueIcon.GetComponent<RawImage>().texture = MadamedeBlaselEveque;
        } else if (characterName == "Chérubin") {
            dialogueIcon.GetComponent<RawImage>().texture = Cherubin;
        } else if (characterName == "Valère") {
            dialogueIcon.GetComponent<RawImage>().texture = Valere;
        } else if (characterName == "Madame du Puy-des-Gougères") {
            dialogueIcon.GetComponent<RawImage>().texture = MadameduPuydesGougeres;
        } else if (characterName == "Madame de Cher-sur-Tendre") {
            dialogueIcon.GetComponent<RawImage>().texture = MadamedeChersurTendre;
        } else if (characterName == "Mademoiselle Eloïse de Hauteclaire") {
            dialogueIcon.GetComponent<RawImage>().texture = MademoiselleEloisedeHauteclaire;
        } else if (characterName == "Ninon") {
            dialogueIcon.GetComponent<RawImage>().texture = Ninon;
        } else if (characterName == "Toinette") {
            dialogueIcon.GetComponent<RawImage>().texture = Toinette;
        } else if (characterName == "Fruit-seller / la Fruitière") {
            dialogueIcon.GetComponent<RawImage>().texture = LaFruitiere;
        } else if (characterName == "Cellist") {
            dialogueIcon.GetComponent<RawImage>().texture = Cellist;
        } else if (characterName == "L'Animateur") {
            dialogueIcon.GetComponent<RawImage>().texture = Lanimateur;
        } else if (characterName == "Fanchon la Poche") {
            dialogueIcon.GetComponent<RawImage>().texture = FanchonLaPoche;
        } else if (characterName == "Henriette Lavocat") {
            dialogueIcon.GetComponent<RawImage>().texture = HenrietteLavocat;
        } else if (characterName == "Jeannot la Panse") {
            dialogueIcon.GetComponent<RawImage>().texture = JeannotLaPanse;
        } else if (characterName == "Scaramouche") {
            dialogueIcon.GetComponent<RawImage>().texture = Scaramouche;
        } else if (characterName == "Madame Argant") {
            dialogueIcon.GetComponent<RawImage>().texture = MadameArgant;
        }
    }

    public void placeDialogueHud()
    {
        dialogueHUD.transform.rotation = playerCamera.transform.rotation;
        dialogueHUD.transform.SetParent(playerCamera.transform);
        dialogueHUD.transform.localPosition = dialogueOffset;
    }

    private IEnumerator<object> DisplayDialogue(string characterName, string dialogue)
    {
        dialogueHUD.SetActive(true);

        setDialogueHudImage(characterName);
        placeDialogueHud();

        CharacterNameText.text = characterName;
        DialogueText.text = dialogue;

        string dialogueToHide = dialogue;

        yield return new WaitForSeconds(10);

        if (DialogueText.text == dialogueToHide) {
            dialogueHUD.SetActive(false);
        }
    }

    public void clickOnObject(string objectName, Vector3 position)  
    {
        ProfilerMarker s_PreparePerfMarker = new ProfilerMarker("Click On Object");
        s_PreparePerfMarker.Begin();

        if (objectName == "TheatrePlans") {
            GameObject.Find("TheatrePlans").SetActive(false);

            artGallery.transform.SetParent(fallBackCamera.transform);
            artGallery.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            artGallery.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            artGallery.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;
        } else {
            Debug.Log("before Display main");
            ProfilerMarker s_PreparePerfMarker2 = new ProfilerMarker("DisplayMain");
            s_PreparePerfMarker2.Begin();
            DisplayMain();
            s_PreparePerfMarker2.End();
            Debug.Log("After display main");

            ensembleUI.transform.SetParent(fallBackCamera.transform);
            ensembleUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            ensembleUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            ensembleUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

            actionsUI.transform.SetParent(fallBackCamera.transform);
            actionsUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            actionsUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            actionsUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

            attributesUI.transform.SetParent(fallBackCamera.transform);
            attributesUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            attributesUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            attributesUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

            historyUI.transform.SetParent(fallBackCamera.transform);
            historyUI.transform.localEulerAngles = new Vector3(0f,0f,0f) ; 
            historyUI.transform.localPosition = new Vector3(0.0f,0.0f,0.5f) ;  
            historyUI.transform.localScale = new Vector3(0.0006f,0.0006f,0.0006f) ;

            omekaUI.transform.SetParent(fallBackCamera.transform);
            omekaUI.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            omekaUI.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
            omekaUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

            attributesBuilder.Clear();
            traitsBuilder.Clear();
            clothingBuilder.Clear();
            professionBuilder.Clear();
            directedStatusBuilder.Clear();
            networkBuilder.Clear();
            nonActionableRelationshipBuilder.Clear();
            relationshipBuilder.Clear();
            socialRecordLabelBuilder.Clear();
            statusBuilder.Clear();
            historyBuilder.Clear();
            actionsBuilder.Clear();

            characterMenu.GetComponent<UnityEngine.UI.Text>().text = objectName;

            getCharacterData(objectName);
            getCharacterHistory(objectName);
            ProfilerMarker s_PreparePerfMarker3 = new ProfilerMarker("getCharacterActions");
            s_PreparePerfMarker3.Begin();
            getCharacterActions(objectName, false);
            s_PreparePerfMarker3.End();
            getCharacterOmeka(objectName);
            Debug.Log("End of click on object");
            s_PreparePerfMarker.End();
        }
    }

    public void SetUsabilityOfHouseTeleporters(bool canBeUsed){
        Debug.Log("INSIDE SET USABILITY OF HOUSE TELEPORTERS");
        foreach(GameObject teleporter in houseTeleporters){
            teleporter.SetActive(canBeUsed);
            teleporter.GetComponent<TeleportArea>().locked = !canBeUsed;
            teleporter.GetComponent<TeleportArea>().UpdateVisuals();
        }
    }

    public void SetUsabilityOfBackstageTeleporters(bool canBeUsed){
        foreach (GameObject teleporter in backstageTeleporters)
        {
            teleporter.SetActive(canBeUsed);
            teleporter.GetComponent<TeleportArea>().locked = !canBeUsed;
            teleporter.GetComponent<TeleportArea>().UpdateVisuals();
        }
    }


    //BEGIN CUSTOM ENSEMBLE CALCULATE VOLITIONS ATTEMPT

    public VolitionInterface calculateVolition(Cast cast, int? timeStep, System.Action action)
    {

        Debug.Log("******** SLOW WAY ALERT &&&&&&&");

        VolitionSet calculatedVolitions = data.ensemble.getRuleLibrary().GetVolitionCache().newSet(cast);

        Cast charactersToSkipVolitionCalculation = new Cast();
        Cast offstageCharacters = data.ensemble.getRuleLibrary().GetSocialRecord().getOffstageCharacters();
        Cast eliminatedCharacters = data.ensemble.getRuleLibrary().GetSocialRecord().getEliminatedCharacters(); // socialRecord.getEliminatedCharacters();

        for (int i = 0; i < offstageCharacters.Count; i++)
        {
            if (!charactersToSkipVolitionCalculation.Contains(offstageCharacters[i]))
            {
                charactersToSkipVolitionCalculation.Add(offstageCharacters[i]);
            }
        }

        for (int i = 0; i < eliminatedCharacters.Count; i++)
        {
            if (!charactersToSkipVolitionCalculation.Contains(eliminatedCharacters[i]))
            {
                charactersToSkipVolitionCalculation.Add(eliminatedCharacters[i]);
            }
        }

        StartCoroutine(runRules("volitionRules", cast, "volition", timeStep, charactersToSkipVolitionCalculation, calculatedVolitions, action));
        return data.ensemble.getRuleLibrary().GetVolitionCache().register("main", calculatedVolitions);
    }

    public IEnumerator<object> runRules(string ruleSet, Cast cast, string type, int? timeStep, Cast unaffectedCharacters, VolitionSet calculatedVolitions, System.Action action)
    {
        List<Rule> rules;

        if (ruleSet == "triggerRules")
            rules = data.ensemble.getRuleLibrary().getTriggerRules();
        else
            rules = data.ensemble.getRuleLibrary().getVolitionRules();

        if (rules == null)
            yield return null;

        for (int i = 0; i < rules.Count; i++)
        {
            if (rules[i].Conditions == null)
                // throw error
            if (rules[i].IsActive == false)
                continue;
            
            //construct a list of every predicate in the rule.
            List<Predicate> allPredicates = new List<Predicate>();
            for(int conditionCounter = 0; conditionCounter < rules[i].Conditions.Count; conditionCounter++){
                allPredicates.Add(rules[i].Conditions[conditionCounter]);
            }
            for(int effectCounter = 0; effectCounter < rules[i].Effects.Count; effectCounter++){
                allPredicates.Add(rules[i].Effects[effectCounter]);
            }
            
            Binding uniqueBindings = getUniqueBindings(allPredicates);
            data.ensemble.getRuleLibrary().matchUniqueBindings(uniqueBindings, cast, type, rules[i], timeStep, unaffectedCharacters, calculatedVolitions);

            yield return null;
        }

        action();
    }

    public Binding getUniqueBindings(List<Predicate> ruleConditions)
    {
        Binding dictionary = new Binding();
        for (int i = 0; i < ruleConditions.Count; i++)
        {
            Predicate predicate = ruleConditions[i];

            if (!dictionary.ContainsKey(predicate.First) || dictionary[predicate.First] == null)
            {
                dictionary[predicate.First] = "";
            }

            if (predicate.Second != null)
            {
                if (!dictionary.ContainsKey(predicate.Second) || dictionary[predicate.Second] == null)
                {
                    dictionary[predicate.Second] = "";
                }
            }
        }
        return dictionary;
    }

    //END CUSTOM ENSEMBLE CALCULATE VOLITIONS ATTEMPT



    //BEGIN MULTI-THREADING SOLUTION
    //TREMENDOUSLY INSPIRED BY https://answers.unity.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html

    public void expensiveDumbOperationForTesting(){
        Vector3[] InData = new Vector3[3];
        Vector3[] OutData = new Vector3[5];

        InData[0] = new Vector3(1f, 0f, 0f);
        InData[1] = new Vector3(0f, 1f, 0f);
        InData[2] = new Vector3(0f, 0f, 1f);

        for (int i = 0; i < 100000000; i++)
        {
            OutData[i % OutData.Length] += InData[(i + 1) % InData.Length];
        }



        //This gets called when the job is finished!
        for (int i = 0; i < InData.Length; i++)
        {
            Debug.Log("In Data Results: (" + i + "): " + InData[i]);
        }
        //This gets called when the job is finished!
        for (int i = 0; i < OutData.Length; i++)
        {
            Debug.Log("Out Data Results: (" + i + "): " + OutData[i]);
        }     
    }

    public class ThreadedJob{
        private bool m_IsDone = false;
        private object m_Handle = new object();
        private System.Threading.Thread m_Thread = null;
        public bool IsDone{
            get{
                bool tmp;
                lock (m_Handle){
                    tmp = m_IsDone;
                }
                return tmp;
            }
            set{
                lock(m_Handle){
                    m_IsDone = value;
                }
            }
        }

        public virtual void Start(){
            m_Thread = new System.Threading.Thread(Run);
            m_Thread.Start();
        }

        public virtual void Abort(){
            m_Thread.Abort();
        }

        //To be overridden by subclass
        protected virtual void ThreadFunction() {}

        //To be overridden by subclass
        protected virtual void OnFinished() {}

        public virtual bool Update(){
            if (IsDone){
                OnFinished();
                return true;
            }
            return false;
        }
        public IEnumerator<object> WaitFor(){
            while(!Update()){
                yield return null;
            }
        }

        private void Run(){
            ThreadFunction();
            IsDone = true;
        }
    }

    public class TestJob : ThreadedJob{
        public Vector3[] InData;
        public Vector3[] OutData;

        protected override void ThreadFunction()
        {
            //Threaded task goes here! This is where your long operation belongs!
            for(int i = 0; i < 100000000; i++){
                OutData[i % OutData.Length] += InData[(i+1) % InData.Length];
            }
        }

        protected override void OnFinished()
        {
            //This gets called when the job is finished!
            for(int i = 0; i < InData.Length; i++){
                Debug.Log("In Data Results: (" + i + "): " + InData[i]);
            }
            //This gets called when the job is finished!
            for (int i = 0; i < OutData.Length; i++)
            {
                Debug.Log("Out Data Results: (" + i + "): " + OutData[i]);
            }
        }
    }

    public class EnsembleCalculateVolitionForSetCharacterAvailability : ThreadedJob{
        public EnsembleAPI InDataEnsemble;
        public Cast InDataCharactersToCalculateVolitionsFor;
        public VolitionInterface OutDataVolitionInterface;

        protected override void ThreadFunction()
        {
            Debug.Log("******STARTING ENSEMBLE JOB: EnsembleCalculateVolitionForSetCharacterAvailability");
            OutDataVolitionInterface = InDataEnsemble.calculateVolition(InDataCharactersToCalculateVolitionsFor);
        }

        protected override void OnFinished()
        {
            Debug.Log("*****FINISHED ASYNC VOLITION CALCULATION: EnsembleCalculateVolitionForSetCharacterAvailability******");

        }
    }

    public class EnsembleCalculateVolitionsJobForGetCharacterActions : ThreadedJob
    {
        public EnsembleAPI InDataEnsemble;
        public Cast InDataCharactersToCalculateVolitionsFor;
        public bool InDatasuppressResponse;
        public string InDataInitiator;
        public string InDataResponder;
        public string InDataObjectName;


        public VolitionInterface OutDataVolitionInterface;
        

        protected override void ThreadFunction()
        {
            Debug.Log("****STARTING ENSEMBLE JOB: EnsembleCalculateVolitionsJobForGetCharacterActions (actions)*****");
            OutDataVolitionInterface = InDataEnsemble.calculateVolition(InDataCharactersToCalculateVolitionsFor);
        }

        protected override void OnFinished()
        {
            Debug.Log("*****FINISHED ENSEMBLE JOB: EnsembleCalculateVolitionsJobForGetCharacterActions (actions) ******");      
        }
    }

    public class EnsembleCalculateVolitionsJobForFinalInterlocutor : ThreadedJob
    {
        public EnsembleAPI InDataEnsemble;
        public Cast InDataCharactersToCalculateVolitionsFor;
        public bool InDatasuppressResponse;
        public string InDataInitiator;
        public string InDataResponder;
        public string InDataObjectName;


        public VolitionInterface OutDataVolitionInterface;


        protected override void ThreadFunction()
        {
            Debug.Log("****STARTING ENSEMBLE JOB: EnsembleCalculateVolitionsJobForFinalInterlocutor (grand finale)*****");
            OutDataVolitionInterface = InDataEnsemble.calculateVolition(InDataCharactersToCalculateVolitionsFor);
        }

        protected override void OnFinished()
        {
            Debug.Log("*****FINISHED ENSEMBLE JOB: EnsembleCalculateVolitionsJobForFinalInterlocutor (grand finale) ******");
        }
    }

    public class EnsembleCalculateVolitionsJob : ThreadedJob
    {
        public EnsembleAPI InDataEnsemble;
        public Cast InDataCharactersToCalculateVolitionsFor;
        public VolitionInterface OutDataVolitionInterface;

        protected override void ThreadFunction()
        {
            Debug.Log("******STARTING ENSEMBLE JOB: EnsembleCalculateVolitionsJob (i.e., vanilla)");
            OutDataVolitionInterface = InDataEnsemble.calculateVolition(InDataCharactersToCalculateVolitionsFor);
        }

        protected override void OnFinished()
        {
            Debug.Log("******FINISHED ENSEMBLE JOB: EnsembleCalculateVolitionsJob (i.e., vanilla)");
        }
    }



    //END MULTI-THREADING SOLUTION

}
