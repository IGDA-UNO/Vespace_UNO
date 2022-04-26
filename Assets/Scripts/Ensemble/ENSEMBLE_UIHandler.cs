using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    public GameObject leftHand;
 
    public SteamVR_LaserPointer laserPointer;
    public Color hit_color;
    public Color miss_color;

    //public string objectName;
    public EnsembleData data;
    public EnsemblePlayer player;

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
    public GameObject actionsMenu;

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

    public PROUVE_OmekaPad prouve;

    void Start()
    {
        mainActiveUI = false;
        actionsActiveUI = false;
        attributesActiveUI = false;
        historyActiveUI = false;

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

            FindObjectsGetStrings();

            actionsMenu = GameObject.Find("ActionsList");
            actionsMenu.GetComponent<UnityEngine.UI.Text>().text = actionsBuilder.ToString();
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

            FindObjectsGetStrings();

            directedStatusMenu = GameObject.Find("DirectedStatusList");
            networkMenu = GameObject.Find("NetworkList");
            nonActionableRelationshipMenu = GameObject.Find("NonActionableRelationshipList");
            relationshipMenu = GameObject.Find("RelationshipList");
            socialRecordLabelMenu = GameObject.Find("SocialRecordLabelList");
            statusMenu = GameObject.Find("StatusList");

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

            FindObjectsGetStrings();

            historyMenu = GameObject.Find("HistoryList");
            historyMenu.GetComponent<UnityEngine.UI.Text>().text = historyBuilder.ToString();
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
    }

    private void FindObjectsGetStrings()
    {
        attributesMenu = GameObject.Find("AttributesList");
        traitsMenu = GameObject.Find("TraitsList");
        clothingMenu = GameObject.Find("TraitClothingList");
        professionMenu = GameObject.Find("ProfessionList");

        attributesMenu.GetComponent<UnityEngine.UI.Text>().text = attributesBuilder.ToString();
        traitsMenu.GetComponent<UnityEngine.UI.Text>().text = traitsBuilder.ToString();
        clothingMenu.GetComponent<UnityEngine.UI.Text>().text = clothingBuilder.ToString();
        professionMenu.GetComponent<UnityEngine.UI.Text>().text = professionBuilder.ToString();
    }

    private void Orientation()
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
    }

    //LaserPointer functions overload:
    public void PointerClick(object sender, PointerEventArgs e)
    {
        EnsembleObject ensemble = e.target.GetComponent<EnsembleObject>();
        if (ensemble != null)
        {
            //The object is an Ensemble object
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

            characterMenu = GameObject.Find("Character_Name");
            characterMenu.GetComponent<UnityEngine.UI.Text>().text = ensemble.name;

            getCharacterData(ensemble.name);
            getCharacterHistory(ensemble.name);
            getCharacterActions(ensemble.name);
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

    public void getCharacterActions(string objectName)
    {
        List<Action> actions = data.ensemble.getAllActions();

        foreach(Action action in actions)
        {
            actionsBuilder.Append(action.Name + "\n");
        }
    }

}
