using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Valve.VR.Extras;
using Valve.VR;
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
    public GameObject character;

    public Button currentActionButton;
    public Button[] actionButtons;

    public GameObject characterMenu;
    public GameObject attributesMenu;
    public GameObject traitsMenu;
    public GameObject clothingMenu;
    public GameObject professionMenu;
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
    public Camera playerCamera;
    public Camera fallBackCamera;
    public GameObject SteamVRObjects;
    public Text CharacterNameText;
    public Text DialogueText;
    public GameObject dialogueIcon;

    public Vector3 dialogueOffset = new Vector3(-0.03f, -0.03f, 0.5f);
    public Vector3 resultsOffset = new Vector3(-0.03f, -0.03f, 0.5f);

    public VideoPlayer marionetteVideoFront;
    public VideoPlayer marionetteVideoBack;

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

    public string finalInterlocutor;

    private List<Action> gameActions = new List<Action>();

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

    void Start()
    {
        hud.UpdateQuestProgress(HUD.NO_TICKET);

        if (!SteamVRObjects.activeSelf)
        {
            playerCamera = fallBackCamera;
        }

        //Turn off the play on start...
        GameObject.Find("Marionettes").transform.Find("Marionette Video Front").gameObject.SetActive(false);
        GameObject.Find("Marionettes").transform.Find("Marionette Video Back").gameObject.SetActive(false);

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
        clothingMenu = GameObject.Find("TraitClothingList");
        professionMenu = GameObject.Find("ProfessionList");
        characterMenu = GameObject.Find("Character_Name");
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

    private void FindObjectsGetStrings()
    {
        attributesMenu.GetComponent<UnityEngine.UI.Text>().text = attributesBuilder.ToString();
        traitsMenu.GetComponent<UnityEngine.UI.Text>().text = traitsBuilder.ToString();
        clothingMenu.GetComponent<UnityEngine.UI.Text>().text = clothingBuilder.ToString();
        professionMenu.GetComponent<UnityEngine.UI.Text>().text = professionBuilder.ToString();
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
            //Debug.Log(predicateToString);

            if(datum.Category == "Attribute")
            {
                attributesBuilder.Append(datum.Type + "\n");
            }else if (datum.Category == "Trait")
            {
                traitsBuilder.Append(datum.Type + "\n");
            }
            else if (datum.Category == "TraitClothing")
            {
                clothingBuilder.Append(datum.Type + "\n");
            }
            else if (datum.Category == "TraitConditionProfession")
            {
                professionBuilder.Append(datum.Type + "\n");
            }
            else if (datum.Category == "DirectedStatus")
            {
                directedStatusBuilder.Append(predicateToString + "\n");
            }
            else if (datum.Category == "Network")
            {
                networkBuilder.Append(predicateToString + "\n");
            }
            else if (datum.Category == "NonActionableRelationship")
            {
                nonActionableRelationshipBuilder.Append(predicateToString + "\n");
            }
            else if (datum.Category == "Relationship")
            {
                relationshipBuilder.Append(predicateToString + "\n");
            }
            else if (datum.Category == "SocialRecordLabel")
            {
                socialRecordLabelBuilder.Append(predicateToString + "\n");
            }
            else if (datum.Category == "Status")
            {
                statusBuilder.Append(predicateToString + "\n");
            }
        }
    }

    public void getCharacterHistory(string objectName)
    {
        Predicate searchPredicate = new Predicate();
        searchPredicate.First = objectName;

        List<List<Predicate>> socialRecordDataByTimestep = data.ensemble.getSocialRecordCopy();

        int timestep = 0;

        foreach (List<Predicate> predicatesForTimestep in socialRecordDataByTimestep)
        {
            List<Predicate> characterData = predicatesForTimestep.FindAll(predicate => predicate.First == objectName).ToList();
            historyBuilder.Append("\nTimestep " + timestep.ToString() + ": " + "\n\n");

            foreach (Predicate datum in characterData)
            {
                string[] predicateDebug = new string[] { datum.First, datum.Second, datum.Category, datum.Type };
                string predicateToString = string.Format("First: {0}, Second: {1}, Category: {2}, Type: {3}", predicateDebug);

                historyBuilder.Append(predicateToString + "\n");
            }
        }
    }

    public void getCharacterOmeka(string objectName){
        // Debug.Log("Getting Omeka data for " + objectName);
        GameObject characterInQuestion = GameObject.Find(objectName);
        currentOmekaIDOfClickedCharacter = characterInQuestion.GetComponent<EnsembleObject>().omekaDatabaseID;
        // Debug.Log("Their database id is: " + currentOmekaIDOfClickedCharacter);
        currentPositionOfClickedCharacter = characterInQuestion.transform.position;
    }

    public void getCharacterActions(string objectName, bool suppressResponse)
    {
        string initiator = EnsemblePlayer.GetSelectedCharacter();
        string responder = objectName;

        // Debug.Log("getCharacterActions: " + initiator);
        // Debug.Log("getCharacterActions: " + objectName);

        VolitionInterface volitionInterface = data.ensemble.calculateVolition(cast);
        List<Action> actions = data.ensemble.getActions(initiator, responder, volitionInterface, cast, 999, 999, 999);

        if (hud.GetQuestProgress() == HUD.BACKSTAGE_ACCESS) {
            CloseMenu();

            if (!suppressResponse) {
                StartCoroutine(DisplayDialogue(objectName, "Please, I'm trying to watch the performance!"));
            }
        } else if (hud.GetQuestProgress() == HUD.POSSESS_PLANS) {
            bool finalInteraction = false;

            foreach (Action action in actions) {
                if (action.Name == "unseen male noble positive interaction" ||
                    action.Name == "unseen male noble negative interaction" ||
                    action.Name == "unseen female noble positive interaction" ||
                    action.Name == "unseen female noble negative interaction" ||
                    action.Name == "unseen servant positive interaction" ||
                    action.Name == "unseen servant negative interaction" ||
                    action.Name == "seen male noble positive interaction" ||
                    action.Name == "seen male noble negative interaction" ||
                    action.Name == "seen female noble positive interaction" ||
                    action.Name == "seen female noble negative interaction" ||
                    action.Name == "seen servant positive interaction" ||
                    action.Name == "seen servant negative interaction"
                ) {
                    finalInteraction = true;
                }
            }

            if (finalInteraction == true) {
                // Debug.Log("found last interaction: " + actions[0].Name);
                finalInterlocutor = objectName;

                TakeAction(objectName, actions[0]);
                volitionInterface = data.ensemble.calculateVolition(cast);
                actions = data.ensemble.getActions(initiator, responder, volitionInterface, cast, 999, 999, 999);
            }
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

        // Debug.Log("actions: " + actions);

        if (actions.Count > 0) {
            foreach (Action action in actions)
            {   
                // Debug.Log("action: " + action);
                
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
        } else {
            CloseMenu();
            if (!suppressResponse) {
                StartCoroutine(DisplayDialogue(objectName, "This person seems busy or uninterested in talking to you."));
            }
        }
    }

    private void HandleFinalResults(string title, string results)
    {
        actionsBuilder.Clear();
        actionsBuilder.Append(title + "\n\n");

        if (title == "Game Log" && results != "")
        {
            actionsBuilder.Append(results + "\n\n");
        }

        foreach(Action act in gameActions)
        {
            actionsBuilder.Append(act.ToString() + "\n");
        }

        resultsPanel.transform.rotation = playerCamera.transform.rotation;
        resultsPanel.transform.SetParent(playerCamera.transform);
        resultsPanel.transform.localPosition = resultsOffset;

        resultsPanel.GetComponentInChildren<UnityEngine.UI.Text>().text = actionsBuilder.ToString();
    }

    private void ShowFinalText(string objectName, Action action)
    {
        Debug.Log("ShowFinalText");
        StartCoroutine(DisplayDialogue(objectName, action.Name));
        string finalResult = getDialogueResponse(action);
        data.ensemble.takeAction(action);
        hud.UpdateQuestProgress(HUD.GAME_COMPLETED);
        HandleFinalResults("Results", finalResult);
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

    private void TakeAction(string objectName, Action action)
    {
        data.ensemble.takeAction(action);
        gameActions.Add(action);

        string dialogueResponse = "";

        if (action.Effects != null) {
            foreach(Effect e in action.Effects) {
                if (e.Type == "HasTicket" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.POSSESS_TICKET);
                    Debug.Log("Got ticket!");
                }
                
                if (e.Type == "GaveTicket" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.HANDED_TICKET_TO_TICKET_TAKER);
                    Debug.Log("Gave ticket!");
                }

                if (e.Type == "HasMark" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.RECEIVED_MARK);
                    Debug.Log("Got mark!");

                    //start the show -- not sure if this is where people start taking their seats?
                    GameObject.Find("Marionettes").transform.Find("Marionette Video Front").gameObject.SetActive(true);
                }

                if (e.Type == "StompAndWhistle" && e.Value is bool && e.Value is true) {
                    Debug.Log("Stomping and whistling!");
                }

                if (e.Type == "NearStageInteraction" && e.Value is bool && e.Value is true) {
                    Debug.Log("Near stage interaction!");
                }

                if (e.Type == "SuccessfulDistraction" && e.Value is bool && e.Value is true) {
                    Debug.Log("Successful distraction!");
                }

                if (e.Type == "BackstageAccess" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.BACKSTAGE_ACCESS);
                    Debug.Log("Got backstage access!");
                }

                if (e.Type == "FinalInteraction" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.FINAL_INTERACTION);
                    Debug.Log("Completed final interaction!");
                }

                if (e.Type == "ThrownOut" && e.Value is bool && e.Value is true) {
                    hud.UpdateQuestProgress(HUD.THROWN_OUT);
                    Debug.Log("Got thrown out!");
                    StartCoroutine(GameOver());
                }
            }
        }

        dialogueResponse = getDialogueResponse(action);

        CloseMenu();
        DisplayActions();
        getCharacterActions(objectName, true);

        StartCoroutine(DisplayDialogue(objectName, dialogueResponse));
    }

    private IEnumerator<object> GameOver()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Intro");
        HandleFinalResults("Game Over", "");
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

        Debug.Log("DisplayDialogue characterName: " + characterName);
        Debug.Log("DisplayDialogue dialogue: " + dialogue);

        setDialogueHudImage(characterName);
        placeDialogueHud();

        CharacterNameText.text = characterName;
        DialogueText.text = dialogue;

        yield return new WaitForSeconds(10);

        dialogueHUD.SetActive(false);
    }

    public void OpenPlans()
    {
        hud.UpdateQuestProgress(HUD.POSSESS_PLANS);
        Debug.Log("***PLANS CLICKED****");
        artGallery.gameObject.SetActive(true);
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

}
