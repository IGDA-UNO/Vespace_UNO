using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{

    public Camera playerCamera;
    public Camera fallBackCamera;
    private GameObject characterSelectionMenu;
    public GameObject SteamVRObjects;
    public string characterStoryText;
    public int currentlySelectedCharacterIndex;
    public List<SelectableCharacterHelper> selectableCharacters;
    public Material maleNobleMaterial;
    public Material femaleNobleMaterial;
    public Material servantMaterial;
    public SelectableCharacterHelper maleNoble;
    public SelectableCharacterHelper femaleNoble;
    public SelectableCharacterHelper servant;
    private string actOneScene = "Theatre_late_august";
    public RawImage characterImage;
    public Text currentlySelectedCharacterText;
    public Text characterDescription;


    void Awake()
    {
        //Initialize all interface references needed to display information
        //Initialisation de toutes les références à l'interface nécessaires pour l'affichage des informations
        characterSelectionMenu = GameObject.Find("CharacterSelection");
        currentlySelectedCharacterIndex = 0;
        
        //make a list of all of the selectable characters
        selectableCharacters = new List<SelectableCharacterHelper>();
        maleNoble = new SelectableCharacterHelper();
        maleNoble.characterName = EnsemblePlayer.MALE_NOBLE;
        maleNoble.characterDescription = "Your monthly tailor’s bill has gone unpaid for the fourth time, and your banker tells you there’s " +
                                         "simply no money for your late-night soirées with tout Paris – the fact is, at age 24, you’ve " +
                                         "squandered generations of family wealth in two wild years at court and in town, and now you " +
                                         "need some cash.You’ve been tipped off to plans for a theatre that interested investors would " +
                                        "pay good money for, and despite the indignity you’ll have to go fetch them backstage at " +
                                        "Bienfait’s marionette theatre. \n\n" +
                                        "First, give your ticket to the ticket taker. Next, interact with at least two people in the theatre, " +
                                        "trying to convince them to help you get backstage. Once backstage, get the plans on the other " +
                                        "side by crossing over the catwalk without being seen by the audience – don’t forget to look at " +
                                        "the drawings before you put them in your pocket! Once you’re back in the theatre, find the last " +
                                        "person you spoke to, and make them help you escape!";
        maleNoble.characterImage = maleNobleMaterial;
        maleNoble.titleText = "The Nobleman";

        femaleNoble = new SelectableCharacterHelper();
        femaleNoble.characterName = EnsemblePlayer.FEMALE_NOBLE;
        femaleNoble.characterDescription = "For weeks you’ve endured the humiliating rumors that your husband, the Marquis, was running " +
                                            "around with some actress, and then you overheard he’s spending a fortune to have a theatre " +
                                            "built for the trollop at the Faoire Saint - Germain! Throwing caution to the wind, you decide on a " +
                                            "course of vengeance and set out to steal the theatre plans at the risk of seeming mad – because " +
                                            "they’re hidden backstage in, of all places, Bienfait’s marionette theatre! \n\n" +
                                            "First, give your ticket to the ticket taker. Next, interact with at least two people in the theatre, " +
                                            "trying to convince them to help you get backstage. Once backstage, get the plans on the other " +
                                            "side by crossing over the catwalk without being seen by the audience – don’t forget to look at " +
                                            "the drawings before you put them in your pocket! Once you’re back in the theatre, find the last " +
                                            "person you spoke to, and try to make as discreet an exit as possible!";
        femaleNoble.characterImage = femaleNobleMaterial;
        femaleNoble.titleText = "The Noblewoman";

        servant = new SelectableCharacterHelper();
        servant.characterName = EnsemblePlayer.SERVANT;
        servant.characterDescription = "You’ve only been in Paris for a few weeks, having come from your native province with a letter " +
                                        "of recommendation to enter into the service of a powerful noble – and already your master has " +
                                        "sent you to the Foire Saint-Germain with a strange request. Having invested a large amount of " +
                                        "money in a proposed theatre at the Fair, he tells you the plans have been stolen and hidden " +
                                        "somewhere backstage at Bienfait’s marionette theatre by a rival. He’s never really taken notice " +
                                        "of you before – here’s an opportunity to make an impression!\n\n" +
                                        "First, give your ticket to the ticket taker. Next, interact with at least two people in the theatre, " +
                                        "trying to convince them to help you get backstage. Once backstage, get the plans on the other " +
                                        "side by crossing over the catwalk without being seen by the audience – don’t forget to look at " +
                                        "the drawings before you put them in your pocket! Once you’re back in the theatre, find the last " +
                                        "person you spoke to, and try to get out without causing a scandal!";
        servant.characterImage = servantMaterial;
        servant.titleText = "The Servant";

        //add these characters to the list of playable characters!
        selectableCharacters.Add(maleNoble);
        selectableCharacters.Add(femaleNoble);
        selectableCharacters.Add(servant);

        UpdateMenu();
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
        placeMenu();
    }



    public void OnSelectThisCharacterButtonPushed()
    {
        Debug.Log(selectableCharacters[currentlySelectedCharacterIndex].characterName + " Selected!");
        EnsemblePlayer.SetSelectedCharacter(selectableCharacters[currentlySelectedCharacterIndex].characterName);

        // "Male Noble Player"
        // "Female Noble Player"
        // "Servant Player" 

        SceneManager.LoadScene(actOneScene);
    }

    public void OnPreviousCharacterButtonPushed(){
        currentlySelectedCharacterIndex -= 1;
        if(currentlySelectedCharacterIndex < 0){
            currentlySelectedCharacterIndex = selectableCharacters.Count - 1;
        }
        Debug.Log("About to display character at index: " + currentlySelectedCharacterIndex);
        UpdateMenu();
    }

    public void OnNextCharacterButtonPushed()
    {
        currentlySelectedCharacterIndex += 1;
        if (currentlySelectedCharacterIndex >= selectableCharacters.Count)
        {
            currentlySelectedCharacterIndex = 0;
        }
        Debug.Log("About to display character at index: " + currentlySelectedCharacterIndex);
        UpdateMenu();
    }

    public void UpdateMenu(){
        //update character title text name
        this.currentlySelectedCharacterText.text = selectableCharacters[currentlySelectedCharacterIndex].titleText;

        //update character image
        this.characterImage.material = selectableCharacters[currentlySelectedCharacterIndex].characterImage;

        //update character description
        this.characterDescription.text = selectableCharacters[currentlySelectedCharacterIndex].characterDescription;

        Debug.Log("Updating the menu based on the new character! Which is " + selectableCharacters[currentlySelectedCharacterIndex].characterName);
    }

    public void placeMenu()
    {
        characterSelectionMenu.transform.rotation = playerCamera.transform.rotation;
        characterSelectionMenu.transform.SetParent(playerCamera.transform);
        characterSelectionMenu.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }

    public class SelectableCharacterHelper{
        public string characterName;
        public Material characterImage;
        public string titleText;
        public string characterDescription;
    }


    /*
        public void OnServantSelected(){
            Debug.Log("Servant is selected!!!");
            EnsemblePlayer.SetSelectedCharacter(EnsemblePlayer.SERVANT);
            SceneManager.LoadScene(actOneScene);
        }

        public void OnMaleNobleSelected(){
            Debug.Log("Male Noble Selected!");
            EnsemblePlayer.SetSelectedCharacter(EnsemblePlayer.MALE_NOBLE);
            SceneManager.LoadScene(actOneScene);
        }

        public void OnFemaleNobleSelected(){
            Debug.Log("Female Noble Selected!");
            EnsemblePlayer.SetSelectedCharacter(EnsemblePlayer.FEMALE_NOBLE);

            // "Male Noble Player"
            // "Female Noble Player"
            // "Servant Player" 

            SceneManager.LoadScene(actOneScene);
        }
    */

}
