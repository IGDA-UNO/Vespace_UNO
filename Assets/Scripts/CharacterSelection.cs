using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{

    public Camera playerCamera;
    public Camera fallBackCamera;
    private GameObject characterSelectionMenu;
    public GameObject SteamVRObjects;
    private string actOneScene = "Theatre_late_august";

    void Awake()
    {
        //Initialize all interface references needed to display information
        //Initialisation de toutes les références à l'interface nécessaires pour l'affichage des informations
        characterSelectionMenu = GameObject.Find("CharacterSelection");
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

    public void placeMenu()
    {
        characterSelectionMenu.transform.rotation = playerCamera.transform.rotation;
        characterSelectionMenu.transform.SetParent(playerCamera.transform);
        characterSelectionMenu.transform.localPosition = new Vector3(0f, 0f, 0.5f);
    }
}
