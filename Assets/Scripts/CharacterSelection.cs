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
        maleNoble.characterDescription = "This character is driven by unique motivations  that are wholly unlike any of the other characters. If you pick this character, you are undoubtedly going to have the single greatest experience of your life.\n\nTruly, I'm envious of you. Would that our places could be swapped, and that *you* were writing the descriptions of these fascinating selectable characters, and *I* was the one charged with the selecting of them.\n\nAlas.";
        maleNoble.characterImage = maleNobleMaterial;
        maleNoble.titleText = "The Nobleman";

        femaleNoble = new SelectableCharacterHelper();
        femaleNoble.characterName = EnsemblePlayer.FEMALE_NOBLE;
        femaleNoble.characterDescription = "This character is driven by unique motivations  that are wholly unlike any of the other characters. If you pick this character, you are undoubtedly going to have the single greatest experience of your life.\n\nTruly, I'm envious of you. Would that our places could be swapped, and that *you* were writing the descriptions of these fascinating selectable characters, and *I* was the one charged with the selecting of them.\n\nAlas.";
        femaleNoble.characterImage = femaleNobleMaterial;
        femaleNoble.titleText = "The Noblewoman";

        servant = new SelectableCharacterHelper();
        servant.characterName = EnsemblePlayer.SERVANT;
        servant.characterDescription = "This character is driven by unique motivations  that are wholly unlike any of the other characters. If you pick this character, you are undoubtedly going to have the single greatest experience of your life.\n\nTruly, I'm envious of you. Would that our places could be swapped, and that *you* were writing the descriptions of these fascinating selectable characters, and *I* was the one charged with the selecting of them.\n\nAlas.";
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
