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
    public GameObject directedStatusMenu;
    public GameObject networkMenu;
    public GameObject nonActionableRelationshipMenu;
    public GameObject relationshipMenu;
    public GameObject socialRecordLabelMenu;
    public GameObject statusMenu;
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
    public Texture Perso2;
    public Texture SeatedFemaleNoble;

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

    private string finalResult;

    //For starting the play in Act I once you've entered the theatre.
    private float xCoordinateThatMeansYouHaveEnteredTheTheatre;
    private bool hasActIPlayStarted;

    //For ensuring the player can't go to places we don't want them to go yet.
    public List<GameObject> houseTeleporters = new List<GameObject>();
    public List<GameObject> backstageTeleporters = new List<GameObject>();

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

    public Dictionary<string, bool> characterAvailable = new Dictionary<string, bool>();

    void Start()
    {
        hud.UpdateQuestProgress(HUD.NO_TICKET);

        dialogueOffset = new Vector3(-0.06f, -0.07f, 0.51f);
        resultsOffset = new Vector3(-0.06f, -0.03f, 0.5f);

        if (!SteamVRObjects.activeSelf)
        {
            playerCamera = fallBackCamera;
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
        directedStatusMenu = GameObject.Find("DirectedStatusList");
        networkMenu = GameObject.Find("NetworkList");
        nonActionableRelationshipMenu = GameObject.Find("NonActionableRelationshipList");
        relationshipMenu = GameObject.Find("RelationshipList");
        socialRecordLabelMenu = GameObject.Find("SocialRecordLabelList");
        statusMenu = GameObject.Find("StatusList");
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
        SetUsabilityOfBackstageTeleporters(true);

        foreach (string character in cast) {
            characterAvailable.Add(character, false);
        }

        StartCoroutine(SetCharacterAvailability());
        StartCoroutine(StartCountdown());
    }

    void Update(){
        //Debug.Log("The position of the player is: " + playerCamera.transform.position);

        if(!hasActIPlayStarted){
            if (playerCamera.transform.position.x > xCoordinateThatMeansYouHaveEnteredTheTheatre)
            {
                Debug.Log("IN THE THEATRE!");
                hasActIPlayStarted = true;

                //start the show!
                marionetteVideoFront.gameObject.SetActive(true);
                marionnettisteAudio.gameObject.SetActive(true);
                SuperTitles.StartTimer();
            }
            else
            {
                Debug.Log("NOT IN THEATRE!");
            }
        }


    }

    private IEnumerator<object> SetCharacterAvailability()
    {
        yield return null;
        VolitionInterface volitionInterface = data.ensemble.calculateVolition(cast);

        foreach (string character in cast) {
            string initiator = EnsemblePlayer.GetSelectedCharacter();
            string responder = character;

            List<Action> actions = data.ensemble.getActions(initiator, responder, volitionInterface, cast, 999, 999, 999);
            characterAvailable[character] = actions.Count > 0;
        }
    }

    private IEnumerator<object> StartCountdown()
    {
        yield return new WaitForSeconds(360);

        if (hud.GetQuestProgress() == HUD.RECEIVED_MARK) {
            hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
            StartCoroutine(ShowProgress(1, "You are running out of time... the play is almost over, but you see an opening, so you head backstage!"));
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

            directedStatusMenu.GetComponent<UnityEngine.UI.Text>().text = directedStatusBuilder.ToString();
            networkMenu.GetComponent<UnityEngine.UI.Text>().text = networkBuilder.ToString();
            nonActionableRelationshipMenu.GetComponent<UnityEngine.UI.Text>().text = nonActionableRelationshipBuilder.ToString();
            relationshipMenu.GetComponent<UnityEngine.UI.Text>().text = relationshipBuilder.ToString();
            socialRecordLabelMenu.GetComponent<UnityEngine.UI.Text>().text = socialRecordLabelBuilder.ToString();
            statusMenu.GetComponent<UnityEngine.UI.Text>().text = statusBuilder.ToString();
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
            GameObject.Find("PROUVE/SceneHandler").GetComponent<PROUVE_SceneHandler>().clickOnObject(currentOmekaIDOfClickedCharacter, currentPositionOfClickedCharacter);
        }
    }

    public void CloseMenu()
    {

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
            StartCoroutine(TransportHeadBackstage());
        } else if (hud.GetQuestProgress() == HUD.POSSESS_PLANS) {
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
        ensembleUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        ensembleUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        actionsUI.transform.SetParent(leftHand.transform);
        actionsUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        actionsUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        actionsUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        attributesUI.transform.SetParent(leftHand.transform);
        attributesUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        attributesUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        attributesUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        historyUI.transform.SetParent(leftHand.transform);
        historyUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        historyUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        historyUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);

        omekaUI.transform.SetParent(leftHand.transform);
        omekaUI.transform.localEulerAngles = new Vector3(45f, 0f, 0f);
        omekaUI.transform.localPosition = new Vector3(0.2f, 0.3f, 0.2f);
        omekaUI.transform.localScale = new Vector3(0.0006f, 0.0006f, 0.0006f);
    }

    //LaserPointer functions overload:
    public void PointerClick(object sender, PointerEventArgs e)
    {
        EnsembleObject ensemble = e.target.GetComponent<EnsembleObject>();

        if (ensemble != null)
        {
            //The object is an Ensemble object

            if (ensemble.name == "TheatrePlans") {
                OpenPlans();
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

        VolitionInterface volitionInterface = data.ensemble.calculateVolition(cast);
        List<Action> actions = data.ensemble.getActions(initiator, responder, volitionInterface, cast, 999, 999, 999);

        if (objectName == "Cellist") {
            CloseMenu();
            StartCoroutine(DisplayDialogue(objectName, "The cellist is busy playing."));
        } else if (hud.GetQuestProgress() == HUD.BACKSTAGE_ACCESS) {
            CloseMenu();

            if (!suppressResponse) {
                StartCoroutine(DisplayDialogue(objectName, "Please, I'm trying to watch the performance!"));
            }
        } else if (hud.GetQuestProgress() == HUD.POSSESS_PLANS) {
            if (finalInterlocutor == objectName) {
                if (approachedFinalInterlocutor != true) {
                    approachedFinalInterlocutor = true;
                    TakeAction(objectName, actions[0]);
                    volitionInterface = data.ensemble.calculateVolition(cast);
                    actions = data.ensemble.getActions(initiator, responder, volitionInterface, cast, 999, 999, 999);
                }
            } else {
                CloseMenu();
                StartCoroutine(DisplayDialogue(objectName, "Did I just see you sneak backstage?"));
            }
        } else if (actions.Count > 0 && actions[0].Name.Contains("interaction")) {
            actions = new List<Action>();
        } else if (hud.GetQuestProgress() == HUD.FINAL_INTERACTION) {
            if (objectName == "Marie-Catherine Bienfait, ticket taker") {
                if (actions.Count > 0) {
                    CloseMenu();
                    ShowFinalText(objectName, actions[0]);
                }
            } else if (objectName == finalInterlocutor) {
                CloseMenu();
                StartCoroutine(DisplayDialogue(objectName, "Shouldn't you be on your way out by now?"));
            } else {
                CloseMenu();
                StartCoroutine(DisplayDialogue(objectName, "Stop, thief!"));
                StartCoroutine(GameOver());
            }
        }
        
        float x = 0;
        float y = -0.05f;
        float z = 5602.218f;

        if (actions.Count > 0) {
            foreach (Action action in actions)
            {
                if (!action.Name.Contains("interaction")) {
                    string actionName = action.Name;

                    if (actionName.Contains("(") && actionName.Contains(")")) {
                        int parensStart = actionName.LastIndexOf("(");
                        int parensEnd = actionName.LastIndexOf(")") + 1;
                        actionName = actionName.Remove(parensStart, parensEnd - parensStart);
                    }

                    GameObject goButton = (GameObject)Instantiate(prefabButton);
                    goButton.transform.SetParent(actionsMenuImageZone, false);

                    
                    goButton.GetComponent<RectTransform>().transform.localPosition = new Vector3(x, y, z);
                    goButton.GetComponent<RectTransform>().transform.rotation = new Quaternion(0, 0, 0, 0);
                    y -= 0.1f;

                    Button tempButton = goButton.GetComponent<Button>();
                    tempButton.GetComponentInChildren<Text>().text = actionName;
                    tempButton.onClick.AddListener(() => TakeAction(objectName, action));

                    actionButtonRefs.Add(goButton);
                }
            }
        } else {
            CloseMenu();
            if (!suppressResponse) {
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

        actionsBuilder.Append("You impressed this many people: " + positiveInteractionCount + "\n\n");
        actionsBuilder.Append("You annoyed this many people: " + negativeInteractionCount + "\n\n");
        actionsBuilder.Append("You talked to these people you knew: \n\n");
        actionsBuilder.Append("You talked to these people you didn't know: \n\n");

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
        StartCoroutine(ShowProgress(3, finalResult));
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
        SteamVR_Fade.Start(Color.black, 10);
        yield return new WaitForSeconds(3);
        playerObject.transform.position = new Vector3(4f, 1.015f, 2f);
        SteamVR_Fade.Start(Color.clear, 10);
    }

    private IEnumerator<object> TransportReturnToTheater()
    {
        // vm.PositionAssigner();
        GameObject lastInterlocutor = GameObject.Find(finalInterlocutor);
        Transform lastInterlocutorParent = lastInterlocutor.transform.parent;
        lastInterlocutorParent.GetComponent<NPCNavMesh>().myViewingTransform = lastInterlocutorTransform;
        lastInterlocutorParent.transform.position = new Vector3(2.5f, 0f, 2f);

        SteamVR_Fade.Start(Color.black, 10);
        yield return new WaitForSeconds(3);
        playerObject.transform.position = new Vector3(3f, 0f, 2f);
		SteamVR_Fade.Start(Color.clear, 10);
    }

    public IEnumerator<object> ShowProgress(int delay, string progressText)
    {
        yield return new WaitForSeconds(delay);

        dialogueHUD.SetActive(false);
        hud.removeHud();
        artGallery.gameObject.SetActive(false);

        if (!initiatedProgressPanel) {
            progressPanel.transform.rotation = playerCamera.transform.rotation;
            progressPanel.transform.SetParent(playerCamera.transform);
            progressPanel.transform.localPosition = resultsOffset;
        }

        progressPanel.SetActive(true);
        progressPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = progressText;
    }

    private void TakeAction(string objectName, Action action)
    {
        data.ensemble.takeAction(action);
        gameActions.Add(action);

        string dialogueResponse = "";
        bool backstageAccess = false;

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

                    StartCoroutine(ShowProgress(3, "Good job! You've received a mark, which you needed in order to enter the theatre. Now make your way into the theater and start talking to people in order to find a way backstage. You will need to speak to at least two people in order to proceed."));

                    //make it so that now you can teleport into the theatre.
                    SetUsabilityOfHouseTeleporters(true);
                }

                if (e.Type == "StompAndWhistle" && e.Value is bool && e.Value is true) {
                    GameObject characterInQuestion = GameObject.Find(objectName);
                    Animator anim = characterInQuestion.GetComponent<Animator>();

                    if (anim != null) {
                        anim.SetTrigger("Active");
                        whistleAudioSource.Play();
                        stompAudioSource.Play();
                    }

                    finalInterlocutor = objectName;
                    hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                    StartCoroutine(ShowProgress(7, "All of that whistling and stomping is seriously distracting! It looks like now is your chance to slip backstage..."));
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

                    finalInterlocutor = objectName;
                    hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                    StartCoroutine(ShowProgress(3, "Your friend has really caused a scene! It seems to have distracted the crowd enough that you can slip backstage..."));
                }

                if (e.Type == "FinalInteraction" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.FINAL_INTERACTION);
                }

                if (e.Type == "ThrownOut" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.THROWN_OUT);
                    StartCoroutine(ShowProgress(3, "Unfortunately you've drawn too much attention to yourself. The bouncer grabs you by the collar and begins to drag you out of the theatre."));
                }

                if (e.Type == "PositiveInteraction" && e.Value is bool && e.Value is true) {
                    positiveInteractionCount += 1;
                    Debug.Log("had a positive interaction!");
                }

                if (e.Type == "NegativeInteraction" && e.Value is bool && e.Value is true) {
                    negativeInteractionCount += 1;
                    Debug.Log("had a negative interaction!");

                    if (negativeInteractionCount == 2) {
                        string role = EnsemblePlayer.GetSelectedCharacter().ToLower();
                        StartCoroutine(ShowProgress(3, "It looks like you're drawing attention to yourself. Try to find a fellow " + role + " to talk to who may be more willing to help."));
                    }
                }

                bool isCompleteInteraction = e.Value is bool && e.Value is true &&
                    (e.Type == "AcquaintanceSecondInteraction" || 
                    e.Type == "NegativeInteraction" || 
                    e.Type == "PositiveInteraction");

                if (isCompleteInteraction) {
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

        if (backstageAccess && hud.GetQuestProgress() != HUD.BACKSTAGE_ACCESS) {
            // if (completedTwoInteractions) {
                finalInterlocutor = objectName;
                hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                StartCoroutine(ShowProgress(3, "Good job! You've managed to create an opening to slip backstage! You will be transported behind the curtains, where you should look for the plans."));
            // } else {
            //     StartCoroutine(ShowProgress(3, "Great, you're certainly being persuasive! However, you need to speak to at least one more person to gain further intel before going backstage."));
            // }
        }

        if (hud.GetQuestProgress() == HUD.RECEIVED_MARK) {
            finalInterlocutor = objectName;
        }

        dialogueResponse = getDialogueResponse(action);

        CloseMenu();
        DisplayActions();
        getCharacterActions(objectName, true);

        StartCoroutine(DisplayDialogue(objectName, dialogueResponse));
        StartCoroutine(SetCharacterAvailability());
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
        } else if (characterName == "Perso 2") {
            dialogueIcon.GetComponent<RawImage>().texture = Perso2;
        } else if (characterName == "Seated Female Noble") {
            dialogueIcon.GetComponent<RawImage>().texture = SeatedFemaleNoble;
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

        if (displayingDialogue == false) {
            displayingDialogue = true;
            yield return new WaitForSeconds(10);
            dialogueHUD.SetActive(false);
            displayingDialogue = false;
        }
    }

    public void OpenPlans()
    {
        hud.UpdateQuestProgress(HUD.POSSESS_PLANS);
        artGallery.gameObject.SetActive(true);
        artGallery.SetPlayer(playerObject);
    }

    public void clickOnObject(string objectName, Vector3 position)  
    {
        if (objectName == "TheatrePlans") {
            OpenPlans();
        } else {
            DisplayMain();

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
            getCharacterActions(objectName, false);
            getCharacterOmeka(objectName);
        }
    }

    public void SetUsabilityOfHouseTeleporters(bool canBeUsed){
        foreach(GameObject teleporter in houseTeleporters){
            teleporter.SetActive(canBeUsed);
        }
    }

    public void SetUsabilityOfBackstageTeleporters(bool canBeUsed){
        foreach (GameObject teleporter in backstageTeleporters)
        {
            teleporter.SetActive(canBeUsed);
        }
    }

}
