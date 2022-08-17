using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnsemblePlayer : MonoBehaviour
{

    public const string MALE_NOBLE = "Male Noble Player";
    public const string FEMALE_NOBLE = "Female Noble Player";
    public const string SERVANT = "Servant Player";

    //The character that the player is. Either:
    // "Male Noble Player"
    // "Female Noble Player"
    // "Servant Player" 
    private static string selectedCharacter = "NONE";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SetSelectedCharacter(string character){
        selectedCharacter = character;
    }

    public static string GetSelectedCharacter(){
        //if we ever try to get a character when it hasn't been set yet, by default set the character
        //to be the male noble.
        if(selectedCharacter == "NONE"){
            SetSelectedCharacter(MALE_NOBLE);
        }
        return selectedCharacter;
    }
}
