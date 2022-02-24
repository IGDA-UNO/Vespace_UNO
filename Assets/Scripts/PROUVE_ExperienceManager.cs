//PROUVE (Extensions Omeka et Unity pour le Patrimoine en Réalité Virtuelle)
//Version 2.0
//Experience Manager, saving actions and suggestions.
//Paul François - VESPACE

using System.Collections;
using System.Collections.Generic;
using System.IO ; 
using UnityEngine;

public class PROUVE_ExperienceManager : MonoBehaviour
{
    PROUVE_SceneHandler SceneHandler ; 
    PROUVE_audioRecorder audioRecorder; 
    PROUVE_DataCaller dataCaller; 

    private List <Vector3> playerPath = new List<Vector3>() ; 
    private List <Vector3> playerView = new List<Vector3>() ; 
    private List <PROUVEOmekaEvent> userOmekaEvents = new List<PROUVEOmekaEvent>() ; 
    private PROUVETagsHandler tagHandler = new PROUVETagsHandler() ; 
    private bool isAudioRecorded ; 

    private Camera playerCamera ; 

    private string startDateTime ; 

    private System.DateTime lastInteraction ; 
    private float inactivityBeforeAction = 600.0f ; //Default is 60.

    // Start is called before the first frame update
    void Awake()
    {
        audioRecorder = GetComponent<PROUVE_audioRecorder>() ; 
        dataCaller = GetComponent<PROUVE_DataCaller>() ; 
    }

    //Initialization : 

    public void setSceneHandler(PROUVE_SceneHandler newsSeneHandler, Camera camera) {
        SceneHandler = newsSeneHandler ;
        playerCamera = camera;  
    }

    //Suivi de l'activité : 

    private void inactivityDetected() {
        System.TimeSpan span = System.DateTime.Now.Subtract(lastInteraction) ; 
        //Debug.Log("Inactivity check : "+span) ; 
        if(span.TotalSeconds >= inactivityBeforeAction-1.0f) {
            if(!SceneHandler.isPadVisible()) {
                startSuggestion() ; 
            } else {
                Invoke("inactivityDetected",inactivityBeforeAction) ; 
            }
        }
    }

    private void startSuggestion() {
        //Debug.Log("Inactivity detected : action pending.") ; 
        if(tagHandler.tagNumber() == 0) {
            //Aucune visite n'a été réalisée depuis le début ==> recherche de l'item par défaut
            SceneHandler.suggestItem(5) ;     
        } else {
            PROUVEExperience experience = new PROUVEExperience() ; 
            experience.omekaEvents = userOmekaEvents.ToArray() ; 
            dataCaller.searchForItemsWithTags(tagHandler,experience) ; 
            //Des activités ont déjà été enregistrées : suggestions sur la base de ces activités
        }
    }

    public void suggestItem(int itemID) {
        SceneHandler.suggestItem(itemID) ; 
    }

    //Recording mechanics : 

    public void startRecord(float recordingInterval, bool audio) {
        startDateTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") ; 
        lastInteraction = System.DateTime.Now ; 
        string mic = returnViveMic() ; 
        isAudioRecorded = audio ; 
        if(isAudioRecorded) {
            audioRecorder.setRecordingDevice(mic) ; 
            audioRecorder.StartRecording() ; 
        }
        InvokeRepeating("recordPositions",1.0f, recordingInterval) ; 
        Invoke("inactivityDetected",inactivityBeforeAction) ; 
    } 

    public void recordPositions() {
        playerPath.Add(playerCamera.transform.position) ;  
        playerView.Add(playerCamera.transform.eulerAngles) ; 
    }

    public void Save() {
        string baseString = System.DateTime.Now.ToString("dd_MM_yyyy__HH_mm") ;
        List <string> filesPaths = new List<string>() ;
        baseString = baseString + "__" + PlayerPrefs.GetString("UserName");
        PROUVEExperience currentExp = new PROUVEExperience() ; 
        currentExp.userName = PlayerPrefs.GetString("UserName") ; 
        //currentExp.beginDate = PlayerPrefs.GetString("StartDate") ;
        currentExp.beginDate = startDateTime ; 
        currentExp.endDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") ; 
        currentExp.position = playerPath.ToArray() ;
        currentExp.rotation = playerView.ToArray() ; 
        currentExp.omekaEvents = userOmekaEvents.ToArray() ; 
        currentExp.tagStats = tagHandler.printOrderedList() ; 
        if(isAudioRecorded) {
            currentExp.audioFile = baseString + ".wav" ; 
            audioRecorder.StopRecording() ; 
            audioRecorder.Save(Path.Combine(Application.persistentDataPath, baseString)) ; 
            filesPaths.Add(Path.Combine(Application.persistentDataPath, baseString + ".wav")) ; 
        }
        string fileName = baseString + ".txt" ;
        string dataPath = Path.Combine(Application.persistentDataPath, fileName);
        filesPaths.Add(Path.Combine(Application.persistentDataPath, fileName)) ; 
        string JSONString = JsonUtility.ToJson(currentExp) ; 
        using (StreamWriter streamWriter = File.CreateText (dataPath))
        {
            streamWriter.Write (JSONString);
        }
        Debug.Log(dataPath) ;
        dataCaller.sendExperienceAndFiles(currentExp.userName,filesPaths.ToArray()) ; 
    }

    public void Stop() {
        audioRecorder.StopRecording() ; 
        CancelInvoke() ; 
    }

    //Registering Omeka Actions : 

    public void registerOmekaAction(int OmekaID, string omekaActionType, OmekaTag[] tags) {
        Invoke("inactivityDetected",inactivityBeforeAction) ; 
        lastInteraction = System.DateTime.Now ; 
        string tagstring = tagHandler.computeTags(tags);
        //Debug.Log("Registering complete action _ " + omekaActionType + " _ ID : " + OmekaID + "tags : " + tagstring) ;
        PROUVEOmekaEvent newEvent = new PROUVEOmekaEvent() ; 
        newEvent.eventDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") ; 
        newEvent.actionType = omekaActionType ; 
        newEvent.omekaID = OmekaID ;
        newEvent.tags = tagstring ;  
        userOmekaEvents.Add(newEvent) ; 
    }

    public void registerSimpleActivity() {
        Invoke("inactivityDetected",inactivityBeforeAction) ; 
        lastInteraction = System.DateTime.Now ; 
    }

    //Utilitaires : 

    public string returnViveMic() {
        string ViveIndicator = "" ; 
        foreach (var device in Microphone.devices)
        {
            if (device.Contains("Vive")) {
                ViveIndicator = device ;
                //Debug.Log("Vive Microphone is: " + device); 
            }
        }   
        return ViveIndicator ; 
    }
}

[System.Serializable]
public class PROUVEExperience {
    public string userName ; 
    public string beginDate ; 
    public string endDate ; 
    public string audioFile ; 
    public Vector3[] position ; 
    public Vector3[] rotation ; 
    public PROUVEOmekaEvent[] omekaEvents; 
    public string tagStats ; 

    public bool isElementVisited(int itemID) {
        foreach(PROUVEOmekaEvent omekaEvent in omekaEvents) {
            if(omekaEvent.omekaID == itemID) {
                return true ; 
            } 
        }
        return false ; 
    }
}

[System.Serializable]
public class PROUVEOmekaEvent {
    public string eventDate ; 
    public string actionType ;
    public string tags ; 
    public int omekaID ; 
}

public class PROUVETagsHandler {
    private List <OmekaTag> tagList = new List<OmekaTag>() ;

    public int tagNumber() {
        return tagList.Count ; 
    }

    public string computeTags(OmekaTag[] newBatch) {
        string tagstring = "";
        for(int i = 0; i< newBatch.Length; i++) {
            tagstring = tagstring + newBatch[i].name + ", "; 
            int index = searchForTag(newBatch[i]) ;
            if(index > -1) {
                tagList[index].increment() ;
            } else {
                tagList.Add(newBatch[i]) ;  
            }
        }
        return tagstring; 
    }  

    public string printOrderedList() {
        string orderedList ="";
        orderList();
        for(int i=0; i<tagList.Count;i++) {
            orderedList = orderedList + tagList[i].name + "("+ tagList[i].getOccurrences() +")," ;
        }
        return orderedList;
    }

    public int searchForTag(OmekaTag tag) {
        for(int i=0; i< tagList.Count; i++) {
            if(tag.id == tagList[i].id) {
                return i; 
            }
        }
        return -1; 
    }

    public OmekaTag[] returnPrefTags(int limit) {
        List <OmekaTag> returnList = new List<OmekaTag>() ;
        orderList() ; 
        if(limit > tagList.Count) { limit =  tagList.Count ;}
        if(limit == -1) { limit = tagList.Count; } 
        for(int i=0; i<limit; i++) {
            returnList.Add(tagList[i]) ; 
        }
        return returnList.ToArray() ; 
    }

    private void orderList() {
        tagList.Sort(SortByOccurences) ; 
    }

    private int SortByOccurences(OmekaTag t1, OmekaTag t2) {
        return t2.getOccurrences().CompareTo(t1.getOccurrences()) ; 
    }
}