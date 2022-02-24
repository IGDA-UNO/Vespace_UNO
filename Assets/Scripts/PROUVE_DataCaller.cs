//PROUVE (Extensions Omeka et Unity pour le Patrimoine en Réalité Virtuelle)
//Version 2.0
//Data Caller, link between OMEKA and Unity system.
//Paul François - VESPACE

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PROUVE_DataCaller : MonoBehaviour
{
    private PROUVE_SceneHandler sceneHandler ; 

    private string omeka_key = "5bd7dff39253e822352f22d6b2370f7b06887b9b" ;
	private string omekaBaseURL = "http://vespace.univ-nantes.fr/" ; 
	private string omekaAPIlocation = "api/" ;
	private string localDBfolder = "omeka/" ;

    private int dataCallerWorking ; 
    private OmekaItem CurrentItem ; 
    private bool loadObjectRelations = false ; 
    private bool loadSubjectRelations = true ; 
	private PROUVE_OmekaPad omekaPad ; 

	public bool refreshLocalDB = false ; 
	private bool useLocalDB = false ; 

    //Initialization and state

    public void initDataCaller(PROUVE_SceneHandler handler, PROUVE_OmekaPad pad, string baseURL, string key) {
        sceneHandler = handler ; 
		omekaPad = pad ; 
        omeka_key = key ; 
        omekaBaseURL = baseURL; 
		if(refreshLocalDB) {
			retrieveOnlineDB() ; 
		} else {
			if(useLocalDB && !localDBExists()) {
				retrieveOnlineDB() ;  
			}
		}
		Debug.Log("Local Mode Activated") ; 
    }

    public bool isWorking() {
        if(dataCallerWorking > 0) {
            return true; 
        }
        return false ; 
    }

	private void addWorkLoad() {
		dataCallerWorking++ ; 
 	}

	private void removeWorkLoad() {
		dataCallerWorking-- ; 
		if(dataCallerWorking == 0) {
			Debug.Log("Work Completed.") ; 
			omekaPad.finishedLoading() ; 
		}
	}

    //Creating web requests : 

    private string omekaWebRequest(int id, string key) {
		string omekaTypeLookingFor = "items/" ; 
		return omekaBaseURL + omekaAPIlocation + omekaTypeLookingFor + id.ToString() + "?key=" + key ; 
	}

	private string omekaItemsForTagRequest(string tag, int item_type, string key) {
		string type = "" ; 
		if(item_type > -1) {
			type = "&item_type=" + item_type ; 
		}
		string omekaTypeLookingFor = "items" ; 
		return omekaBaseURL + omekaAPIlocation + omekaTypeLookingFor + "?tags=" + tag + type +  "&key=" + key ; 
	}

    private string buildRequestJSONforItem(int id) {
		string omekaTypeLookingFor = "items/" ; 
		return omekaBaseURL + omekaAPIlocation + omekaTypeLookingFor + id.ToString() + "?key=" + omeka_key ; 
	}

    private string buildRequestJSONfromURL(string URL) {
		return URL+ "&key=" + omeka_key;
	}

	private string buildRequestFilesForItem(int id) {
		string omekaTypeLookingFor = "files?item=" ; 
		return omekaBaseURL + omekaAPIlocation + omekaTypeLookingFor + id.ToString() + "&key=" + omeka_key ; 
	}

	private string omekaAllItemsRequest() {
		return omekaBaseURL + omekaAPIlocation + "items" + "?key=" + omeka_key ; 
	}

	private string postItemRequest() {
		return omekaBaseURL + omekaAPIlocation + "items" + "?key=" + omeka_key  ; 
	}

	private string postFileRequest() {
		return omekaBaseURL + omekaAPIlocation + "files" + "?key=" + omeka_key  ; 
	}
    //Actions :

	public void lookForItemWithID(int itemID) {
		if(useLocalDB) {
			StartCoroutine(openDataForItem(itemID)) ; 
		} else {
			StartCoroutine(buildDataForItem(itemID)) ; 
		}
	}

	public void lookForThumbnailForButton(ItemRelationButton button) {
		if(useLocalDB) {
			StartCoroutine(openFilesForRelation(button.OmekaItemID,button)) ; 
		} else {
			StartCoroutine(requestFilesForRelation(button.OmekaItemID,button)) ; 
		}
	}

	public void loadCurrentItemImage(int imageID) {
		if(useLocalDB) {
			StartCoroutine(openItemMainImage(CurrentItem,imageID)) ; 
		} else  {
			StartCoroutine(loadItemMainImage(CurrentItem,imageID)) ; 
		}
	}


    private void askDisplayOfItem(OmekaItem item) {
        PROUVE_TextComponent textComponent = new PROUVE_TextComponent() ;
        textComponent.createFromItem(item) ; 
		omekaPad.updateWithTextContent(textComponent) ; 
    }

	public void searchForItemsWithTags(PROUVETagsHandler tagsHandler, PROUVEExperience currentExperience) {
		if(!useLocalDB) {
			if(!sceneHandler.isProModeActive()) {
				StartCoroutine(loadItemsWithTag(tagsHandler,currentExperience,18)) ; //Chargement des objets virtuels présents dans la scène
			} else {
				StartCoroutine(loadItemsWithTag(tagsHandler,currentExperience,-1)) ; //Chargement de tous les items de la DB
			}
		}
	}

	//Envoi des données 

	public void debugTestSend() {
	}

	public void sendExperienceAndFiles(string userName, string[] filePaths) {
		if(!useLocalDB) {
			OmekaItem newItem = new OmekaItem() ;
			newItem.addTitle("Experience "+userName); 
			newItem.addCreator(userName) ; 
			newItem.addDate(System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")) ; 
			newItem.addPublisher("PROUVE 2.0 on VESPACE") ; 
			newItem.setCollection(16) ; 
			newItem.setType(21) ; 
			StartCoroutine(SendOmekaItemAndFilesToServer(newItem,filePaths)) ; 
		} else {
			Debug.Log("Can't send files to server when in local mode.") ; 
		}
	}


	//Mode hors ligne : 

	public void switchLocalMode() {
		useLocalDB = !useLocalDB ; 
		StopAllCoroutines() ; 
		if(useLocalDB) { Debug.Log("Local Mode activated.") ; }
		else { Debug.Log("Local Mode disabled.") ; }
	}

	public bool isInLocalMode() {
		return useLocalDB ; 
	}

	public bool localDBExists() {
		string localpath = "omeka/localDB.json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		if(File.Exists(dataPath)) {
			//Debug.Log("Local DB exists") ; 
			return true; 
		} 
		return false;	
	}

	public void retrieveOnlineDB() {
		Debug.Log("Retrieving full database") ; 
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "omeka" ));
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, localDBfolder+"items" ));
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, localDBfolder+"itemfiles" ));
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, localDBfolder+"thumbnails" ));
		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, localDBfolder+"fullsize" ));
		StartCoroutine(loadAsyncAllItems()) ; 

	}

	private void saveItemToLocalDB(OmekaItem item) {
		string localpath = localDBfolder + "items/" + item.id + ".json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
        string JSONString = JsonUtility.ToJson(item) ; 
		using (StreamWriter streamWriter = File.CreateText (dataPath))
		{
			streamWriter.Write (JSONString);
		}
		//Debug.Log("Writing file : "+localpath) ;
	}

	private void saveFilesToLocalDB(OmekaFalseFile fileslist, int omekaid) {
		string localpath = localDBfolder + "itemfiles/" + omekaid + ".json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
        string JSONString = JsonUtility.ToJson(fileslist) ; 
		using (StreamWriter streamWriter = File.CreateText (dataPath))
		{
			streamWriter.Write (JSONString);
		}
		//Debug.Log("Writing file : "+localpath) ;
	}

	private void saveImageThumbnailToDisk(Texture2D image, string imageName) {
		string localpath = localDBfolder + "thumbnails/" + imageName ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		byte[] imagebytes = image.EncodeToJPG() ;
		File.WriteAllBytes(dataPath,imagebytes) ; 
		//Debug.Log("Writing file : "+localpath) ;
	}

	private void saveImageFullSizeToDisk(Texture2D image, string imageName) {
		string localpath = localDBfolder + "fullsize/" + imageName ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		byte[] imagebytes = image.EncodeToJPG() ;
		File.WriteAllBytes(dataPath,imagebytes) ; 
		//Debug.Log("Writing file : "+localpath) ;
	}

	private void saveDBRef(PROUVE_LocalDB localDB) {
		string localpath = localDBfolder + "localDB.json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
        string JSONString = JsonUtility.ToJson(localDB) ; 
		using (StreamWriter streamWriter = File.CreateText (dataPath))
		{
			streamWriter.Write (JSONString);
		}
		//Debug.Log("Writing file : "+localpath) ;
	}

	IEnumerator openDataForItem(int item) {
		addWorkLoad() ; 
		string localpath = localDBfolder + "items/" + item +".json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		using (StreamReader streamReader = File.OpenText (dataPath))
        {
			string jsonString ; 
            yield return jsonString = streamReader.ReadToEnd ();
			CurrentItem = JsonUtility.FromJson<OmekaItem> (jsonString);
			askDisplayOfItem(CurrentItem) ; 
			StartCoroutine(openFilesForItem(CurrentItem)) ; 
			if(CurrentItem.extended_resources.item_relations.relationsCount()>0) {
				if(loadObjectRelations) {
					foreach (OmekaItemRelationsObject relation in CurrentItem.extended_resources.item_relations.object_relations) {
						omekaPad.addRelationButtonObject(relation) ; 
					}
				}
				if(loadSubjectRelations) {
					foreach (OmekaItemRelationsSubject relation in CurrentItem.extended_resources.item_relations.subject_relations) {
						omekaPad.addRelationButtonSubject(relation) ; 
					}
				}
			}
        }
		removeWorkLoad() ; 
	}

	IEnumerator openFilesForItem(OmekaItem item) {
		addWorkLoad() ; 
		string localpath = localDBfolder + "itemfiles/" + item.id +".json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		using (StreamReader streamReader = File.OpenText (dataPath))
        {
			string jsonString ; 
			yield return jsonString = streamReader.ReadToEnd ();
			OmekaFalseFile omekafile = JsonUtility.FromJson<OmekaFalseFile>(jsonString) ;
			item.files.itemfiles = omekafile.filelist;
			if(item.maxFilesNumber() > 0) {
				StartCoroutine(openItemMainImage(item,0)) ; 
			} 
		}
		removeWorkLoad() ; 
	}

	IEnumerator openItemMainImage(OmekaItem Item, int imageID) {
		addWorkLoad() ; 
		string localpath = localDBfolder + "fullsize/" + Item.files.itemfiles[imageID].filename ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		byte[] byteArray ; 
		yield return byteArray = File.ReadAllBytes(dataPath) ; 
		Texture2D texture = new Texture2D(2,2) ; 
		texture.LoadImage(byteArray) ; 
		PROUVE_ImageComponent imageComponent = new PROUVE_ImageComponent() ; 
		imageComponent.id = imageID ; 
		imageComponent.title = Item.files.itemfiles[imageID].getElement("Title") ; 
		imageComponent.texture = texture ;
		omekaPad.updateImageContent(imageComponent) ; 
		removeWorkLoad() ;  
	}

	IEnumerator openFilesForRelation(int omekaID, ItemRelationButton button) {
		addWorkLoad() ; 
		string localpath = localDBfolder + "itemfiles/" + omekaID +".json" ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		using (StreamReader streamReader = File.OpenText (dataPath))
        {
			string jsonString ; 
			yield return jsonString = streamReader.ReadToEnd ();
			OmekaFalseFile omekafile = JsonUtility.FromJson<OmekaFalseFile>(jsonString) ;
			if(omekafile.filelist.Length > 0) {
				StartCoroutine(openItemThumbnail(omekafile.filelist[0],button)) ;
			} 
		}
		removeWorkLoad() ; 
	}

	IEnumerator openItemThumbnail(OmekaFile file, ItemRelationButton button) {
		addWorkLoad() ; 
		string localpath = localDBfolder + "thumbnails/" + file.filename ; 
		string dataPath = Path.Combine(Application.persistentDataPath, localpath);
		byte[] byteArray ; 
		yield return byteArray = File.ReadAllBytes(dataPath) ; 
		Texture2D texture = new Texture2D(2,2) ; 
		texture.LoadImage(byteArray) ; 
		PROUVE_ImageComponent imageComponent = new PROUVE_ImageComponent() ; 
		imageComponent.id = 0 ; 
		imageComponent.title = file.getElement("Title") ; 
		imageComponent.texture = texture ;
		button.displayThumbnail(imageComponent) ; 
		removeWorkLoad() ; 
	}

    //Web Requests : 

    IEnumerator buildDataForItem(int item) {
		addWorkLoad() ; 
		string request = buildRequestJSONforItem(item) ;
		UnityWebRequest webRqst = UnityWebRequest.Get(request) ; 
		yield return webRqst.SendWebRequest() ; 

		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Erreur buidling data for item "+item) ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {
			CurrentItem = JsonUtility.FromJson<OmekaItem>(webRqst.downloadHandler.text) ;
			askDisplayOfItem(CurrentItem) ; 
			StartCoroutine(requestFilesForItem(CurrentItem,"main")) ; 
			if(CurrentItem.extended_resources.item_relations.relationsCount()>0) {
				if(loadObjectRelations) {
					foreach (OmekaItemRelationsObject relation in CurrentItem.extended_resources.item_relations.object_relations) {
						omekaPad.addRelationButtonObject(relation) ; 
					}
				}
				if(loadSubjectRelations) {
					foreach (OmekaItemRelationsSubject relation in CurrentItem.extended_resources.item_relations.subject_relations) {
						omekaPad.addRelationButtonSubject(relation) ; 
					}
				}
			}
		}
		removeWorkLoad() ; 
	}

	IEnumerator requestFilesForItem(OmekaItem Item, string mode) {
		addWorkLoad() ; 
 		UnityWebRequest webRqst = UnityWebRequest.Get(buildRequestJSONfromURL(Item.files.url)) ; 
		yield return webRqst.SendWebRequest() ; 
		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Erreur loading files for item "+ Item.id) ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {
			string falseJSON = "{\"filelist\" : ";
			OmekaFalseFile omekafile = JsonUtility.FromJson<OmekaFalseFile>(falseJSON + webRqst.downloadHandler.text + "}") ;
			Item.files.itemfiles = omekafile.filelist;
			if(Item.maxFilesNumber() > 0) {
				StartCoroutine(loadItemMainImage(Item,0)) ;
			} 
		}
		removeWorkLoad() ; 
	}

	IEnumerator requestFilesForRelation(int omekaID, ItemRelationButton button) {
		addWorkLoad() ; 
 		UnityWebRequest webRqst = UnityWebRequest.Get(buildRequestFilesForItem(omekaID)) ; 
		yield return webRqst.SendWebRequest() ; 
		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Erreur loading files for item "+ omekaID) ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {
			if(webRqst.downloadHandler.text != "[]") {
				string falseJSON = "{\"filelist\" : ";
				OmekaFalseFile omekafile = JsonUtility.FromJson<OmekaFalseFile>(falseJSON + webRqst.downloadHandler.text + "}") ;
				
				StartCoroutine(loadItemThumbnail(omekafile.filelist[0],button)) ;
			}
		}
		removeWorkLoad() ; 
	}

	IEnumerator loadItemMainImage(OmekaItem Item, int imageID) {
		addWorkLoad() ; 
		string url = Item.files.itemfiles[imageID].file_urls.fullsize ;
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url)) 
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
				Debug.Log("Error loading image "+imageID+" for item "+Item.id+" at location : "+url) ; 
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr); 
				PROUVE_ImageComponent imageComponent = new PROUVE_ImageComponent() ; 
				imageComponent.id = imageID ; 
				imageComponent.title = Item.files.itemfiles[imageID].getElement("Title") ; 
				imageComponent.texture = texture ;
				omekaPad.updateImageContent(imageComponent) ; 
            }
        }
		removeWorkLoad() ;  
	}

	IEnumerator loadItemThumbnail(OmekaFile file, ItemRelationButton button) {
		addWorkLoad() ; 
		string url = file.file_urls.square_thumbnail ;
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url)) 
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
				Debug.Log("Error loading image 0 for item at location : "+url) ; 
                Debug.Log(uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr); 
				PROUVE_ImageComponent imageComponent = new PROUVE_ImageComponent() ; 
				imageComponent.id = 0 ; 
				imageComponent.title = file.getElement("Title") ; 
				imageComponent.texture = texture ;
				//Affichage des images pour les relations
				button.displayThumbnail(imageComponent) ; 
            }
        }
		removeWorkLoad() ;  
	}

	//Requête pour trouver l'item le plus pertinent en fonction d'une liste de tags

	IEnumerator loadItemsWithTag(PROUVETagsHandler tagsHandler, PROUVEExperience currentExperience, int type) {
		addWorkLoad() ;
		//Recherche avec les deux tags les plus utilisés
		OmekaTag[] prefsTags = tagsHandler.returnPrefTags(2) ; 
		string tagString = prefsTags[0].name +","+prefsTags[1].name ; 
		string request = omekaItemsForTagRequest(tagString,type,omeka_key) ; 
		bool send_suggestion = false ; 

		UnityWebRequest webRqst = UnityWebRequest.Get(request) ; 
		yield return webRqst.SendWebRequest() ; 

		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Error retrieving items for tags : "+tagString) ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {
			int suggestID = returnPossibleSuggestionID(webRqst.downloadHandler.text, currentExperience) ; 
			if(suggestID != -1) {
				send_suggestion = true ; 
				removeWorkLoad() ; 
				sceneHandler.suggestItem(suggestID) ;
			} else {
				//Debug.Log("Pas de résultats pour deux mots clés, essais avec un seul") ; 
				//Aucune suggestion n'a pu être faite : cherchons les items correspondant à un seul mot clé, le premier, puis le deuxième, etc. 
				prefsTags = tagsHandler.returnPrefTags(-1) ; 
				foreach(OmekaTag prefTag in prefsTags) {
					tagString = prefTag.name ; 
					request = omekaItemsForTagRequest(tagString,type,omeka_key) ; 
					webRqst = UnityWebRequest.Get(request) ; 
					yield return webRqst.SendWebRequest() ; 
					if(webRqst.isNetworkError || webRqst.isHttpError) {
						Debug.Log("Error retrieving items for tags : "+tagString) ; 
						Debug.Log(webRqst.error) ; 
					} else {
						suggestID = returnPossibleSuggestionID(webRqst.downloadHandler.text, currentExperience) ; 
						if(suggestID != -1) {
							send_suggestion = true ;
							removeWorkLoad() ;
							sceneHandler.suggestItem(suggestID) ;  
							break;
						}
					}
				}
			}
		}
		if(!send_suggestion) { 
			//Aucune correspondance n'a été trouvée
			removeWorkLoad() ;
		}
	}

	private int returnPossibleSuggestionID(string requestAnswer, PROUVEExperience currentExperience) {
		string falseJSON = "{\"itemlist\" : ";
		OmekaItemList allItems = JsonUtility.FromJson<OmekaItemList>(falseJSON + requestAnswer + "}") ;
		if(allItems.itemlist.Length > 0) {
			foreach(OmekaItem thisItem in allItems.itemlist) {
				if(!currentExperience.isElementVisited(thisItem.id)) {
					return thisItem.id; 
				}
			}
		}
		return -1 ; 
	}

	//Chargement des fichiers pour la création de la base hors ligne

	IEnumerator loadAsyncAllItems() {
		addWorkLoad() ;
		string request = omekaAllItemsRequest() ;
		UnityWebRequest webRqst = UnityWebRequest.Get(request) ; 
		yield return webRqst.SendWebRequest() ; 

		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Error retrieving full items list") ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {

			string falseJSON = "{\"itemlist\" : ";
			OmekaItemList allItems = JsonUtility.FromJson<OmekaItemList>(falseJSON + webRqst.downloadHandler.text + "}") ;
			PROUVE_LocalDB localDB = new PROUVE_LocalDB() ; 
			localDB.dateOfDownload = System.DateTime.Now.ToString("dd_MM_yyyy__HH_mm") ;
			localDB.numberOfElements = allItems.itemlist.Length ; 
			saveDBRef(localDB) ; 
			for(int i=0; i<allItems.itemlist.Length; i++) {
				OmekaItem thisItem = allItems.itemlist[i] ; 
				saveItemToLocalDB(thisItem) ; 
				StartCoroutine(loadAsyncFilesForItem(thisItem)) ; //Va chercher les fichiers associés à un élément
			}
		}
		removeWorkLoad() ; 
	}

	IEnumerator loadAsyncFilesForItem(OmekaItem item) {
		addWorkLoad() ; 
 		UnityWebRequest webRqst = UnityWebRequest.Get(buildRequestFilesForItem(item.id)) ; 
		yield return webRqst.SendWebRequest() ; 
		if(webRqst.isNetworkError || webRqst.isHttpError) {
			Debug.Log("Erreur loading files for item "+ item.id) ; 
			Debug.Log(webRqst.error) ; 
		} 
		else {
			string falseJSON = "{\"filelist\" : ";
			OmekaFalseFile omekafile = JsonUtility.FromJson<OmekaFalseFile>(falseJSON + webRqst.downloadHandler.text + "}") ;
			saveFilesToLocalDB(omekafile,item.id) ; 
			item.files.itemfiles = omekafile.filelist;
			if(item.maxFilesNumber() > 0) {
				for(int i=0;i<item.maxFilesNumber();i++) {
					if(item.files.itemfiles[i].mime_type == "image/jpeg" | item.files.itemfiles[i].mime_type == "image/png"){
						StartCoroutine(loadAsincImage(item.files.itemfiles[i].file_urls.fullsize,item.files.itemfiles[i].filename,0)) ;
						StartCoroutine(loadAsincImage(item.files.itemfiles[i].file_urls.thumbnail,item.files.itemfiles[i].filename,1)) ;
					} else { 
						Debug.Log(item.files.itemfiles[i].mime_type); 
					}
				}
			} 
		}
		removeWorkLoad() ;
	}

	IEnumerator loadAsincImage(string imageURL, string filename, int type) {
		addWorkLoad() ; 
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL)) 
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
				Debug.Log("Unable to load image at url : "+imageURL) ; 
                Debug.Log(uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr); 
				if(type == 0) {saveImageFullSizeToDisk(texture,filename) ; }
				else { saveImageThumbnailToDisk(texture,filename) ; } 
            }
        }
		removeWorkLoad() ;  
	}

	//Sending POST Requests 

	IEnumerator SendOmekaItemAndFilesToServer(OmekaItem item, string[] filesPaths) {
		addWorkLoad() ; 
		string testJSON = item.getJSONRepresentation() ; 
		var request = new UnityWebRequest(postItemRequest(), "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(testJSON);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
			Debug.Log(request.downloadHandler.text);
        }
        else
        {
			OmekaItem new_item = JsonUtility.FromJson<OmekaItem>(request.downloadHandler.text) ;
            foreach(string filePath in filesPaths) {
				string extension = Path.GetExtension(filePath) ; 
				string ContentType = "text/plain" ; 
				if(extension == ".wav") {
					ContentType = "audio/x-wav" ; 
				}
				StartCoroutine(SendFile(filePath,ContentType,new_item.id)) ; 
			}
        }
		removeWorkLoad() ; 
	}

	IEnumerator SendFile(string filePath, string ContentType, int itemID) {
		addWorkLoad() ; 
		Debug.Log("Sending local file at path : "+filePath) ; 
		string JSONFile = "{\"order\": 2,\"item\": {\"id\": "+itemID+"}}";
		byte[] fileByte = File.ReadAllBytes(filePath) ; 

		WWWForm form = new WWWForm();
        form.AddField("data", JSONFile);
		form.AddBinaryData("file",fileByte,Path.GetFileName(filePath),ContentType) ; 

		UnityWebRequest request = UnityWebRequest.Post(postFileRequest(), form) ;
		//Debug.Log("Requete envoyée : "+System.Text.Encoding.UTF8.GetString(request.uploadHandler.data)) ;  
		yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
			//Debug.Log(request.downloadHandler.text);
        }
        else
        {
			Debug.Log("Uploaded Successfully") ; 
            //Debug.Log(request.downloadHandler.text);
        }
		removeWorkLoad() ; 
	}
}

//Classe de base de donnée locale :

[System.Serializable]
public class PROUVE_LocalDB {
	public string dateOfDownload ; 
	public int numberOfElements ; 
}


//Classes d'échange rapide : 

public class PROUVE_TextComponent { //Is a simplified class of an omeka object suitable for unity display
    public int id ; 
    public string title ; 
    public string description ; 
	public int filesCount ; 
    public OmekaTag[] keywords ;
	public PROUVE_QuickElement[] elements ;  

    public void createFromItem(OmekaItem item) {
        id = item.id ; 
        title = item.getElement("Title") ; 
        description = item.getElement("Description") ; 
        keywords = item.tags ; 
		filesCount = item.files.count ; 
		List<PROUVE_QuickElement> liste = new List<PROUVE_QuickElement>() ; 
		foreach (OmekaElementTexts elementText in item.element_texts) {
			if(elementText.element.name != "Title" && elementText.element.name != "Description") {
				PROUVE_QuickElement nouveau = new PROUVE_QuickElement() ; 
				nouveau.title = elementText.element.name ; 
				nouveau.description = elementText.text ; 
				liste.Add(nouveau) ; 
			}
		}
		elements = liste.ToArray() ; 
    }


	public string tagString() {
		string tagstring = "" ; 
		foreach(OmekaTag tag in keywords) {
			tagstring = tagstring + tag.name + ", " ; 
		}
		return tagstring ; 
	}
}

public class PROUVE_QuickElement {
	public string title ; 
	public string description ; 
}

public class PROUVE_ImageComponent {
	public int id ; 
	public string title ;
	public float width ; 
	public float height ;  
	public Sprite image ; 
	public Texture2D texture ; 

	public Sprite returnSpriteToFit(float maxWidth, float maxHeight) {
		float newWidth = texture.width ; 
		float newHeight = texture.height ; 
		float ratio = 1.0f;
		if(newWidth >= newHeight) {
			ratio = newWidth/maxWidth; 
			newWidth = maxWidth; 
			newHeight = newHeight/ratio ; 
			if(newHeight > maxHeight) {
				ratio = newHeight/maxHeight ;
				newHeight = maxHeight ; 
				newWidth = newWidth/ratio ; 
			}
		} else {
			ratio = newHeight/maxHeight ; 
			newHeight = maxHeight ; 
			newWidth = newWidth/ratio ; 
		}
		width = newWidth ; 
		height = newHeight ; 
		Sprite mySprite = Sprite.Create(texture,new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)) ; 
		return mySprite ; 
	}
}



//Definition of Omeka Classes as shown in JSON files : 

[System.Serializable]
public class OmekaFalseFile { 
	public OmekaFile[] filelist ; 
}

[System.Serializable]
public class OmekaItemList {
	public OmekaItem[] itemlist ; 
}

[System.Serializable]
public class OmekaExhibitList {
	public OmekaExhibit[] exhibitlist ;
}

[System.Serializable]
public class OmekaExhibit {
	public int id;
	public string title; 
	public OmekaExhibitPagesInfo pages;

}

[System.Serializable]
public class OmekaExhibitPagesInfo {
	public int count;
	public string url ;
}

[System.Serializable]
public class OmekaExhibitPageList {
	public OmekaExhibitPage[] exhibitpage ;
}

[System.Serializable]
public class OmekaExhibitPage {
	public int id ; 
	public string url;
	public string title;
	public OmekaExhibitBlock[] page_blocks;
}

[System.Serializable]
public class OmekaExhibitBlock {
	public int id;
	public int page_id;
	public string layout;
	public string text;
	public int order;
	public OmekaExhibitPageAttachment[] attachments;
}

[System.Serializable]
public class OmekaExhibitPageAttachment {
	public int id;
	public string caption;
	public OmekaExhibitItem item;
}

[System.Serializable]
public class OmekaExhibitItem {
	public int id;
	public string resource;
	public string url;
}

[System.Serializable]
public class OmekaItemRelationsSubject   {
	public int item_relation_id;
	public int object_item_id;
	public string object_item_title;
	public string relation_text;
	public string relation_description;
}

[System.Serializable]
public class OmekaItemRelationsObject  {
	public int item_relation_id;
	public int subject_item_id;
	public string subject_item_title;
	public string relation_text;
	public string relation_description;
}

[System.Serializable]
public class OmekaItemRelations   {
	public int id;
	public string url;
	public OmekaItemRelationsSubject[] subject_relations;
	public OmekaItemRelationsObject[] object_relations;

	public int relationsCount() {
		return subjectRelationsCount() + objectRelationsCount() ;  
	}
	public int subjectRelationsCount() {
		return subject_relations.Length ; 
	}

	public int objectRelationsCount() {
		return object_relations.Length ; 
	}
}

[System.Serializable]
public class OmekaExtendedResources   {
	public OmekaItemRelations item_relations ; 
	public string exhibit_pages;
}

[System.Serializable]
public class OmekaElementDefinition {
	public string name;
	public int id;
	public string url; 
	public string resource; 
}

[System.Serializable]
public class OmekaElementSetDefinition {
	public int id; 
	public string url; 
	public string name; 
	public string resource ; 
}


[System.Serializable]
public class OmekaElementTexts {
	public bool html;
	public string text;
	public OmekaElementDefinition element;
	public OmekaElementSetDefinition element_set ; 

	public string getJSONRepresentation() {
		string JSONRep = "{\"html\": false,\"text\": \""+text+"\"," ; 
		JSONRep = JSONRep + "\"element\": {\"id\": "+element.id+"}," ;
		JSONRep = JSONRep + "\"element_set\": {\"id\": "+element_set.id+"}}" ;
		return JSONRep ; 
	}

	public OmekaElementTexts(string elementText, int elementID, int element_setID) {
		this.text = elementText ; 
		OmekaElementDefinition elementTitleDef = new OmekaElementDefinition() ; 
		OmekaElementSetDefinition elementTitleSetDef = new OmekaElementSetDefinition() ; 
		elementTitleSetDef.id = element_setID ; 
		elementTitleDef.id = elementID ; 
		this.element = elementTitleDef ; 
		this.element_set = elementTitleSetDef ; 
	}
}

[System.Serializable]
public class OmekaCollection {
	public int id ; 
	public string url ; 
	public string resource ; 
}

[System.Serializable]
public class OmekaFileURLs {
	public string original;
	public string fullsize;
	public string thumbnail;
	public string square_thumbnail;
}

[System.Serializable]
public class OmekaFile {
	public int id; 
	public string url;
	public string filename; 
	public string mime_type;
	public OmekaFileURLs file_urls;
	public OmekaElementTexts[] element_texts;

	public int IndexOfElementDefinition(string elementdef) {
	int tablelength = element_texts.Length;
	for(int i=0; i<tablelength ; i++) {
		if(element_texts[i].element.name == elementdef) {
			return i ;
		}
	}
	return -1 ;
	}

	public string getElement(string elementdef) {
		int elt_index = IndexOfElementDefinition(elementdef) ; 
		if(elt_index > -1) {
			return element_texts[elt_index].text ; 
		}
		else return "" ; 
	}
}

[System.Serializable]
public class OmekaItemFiles {
	public int count ;
	public string url;
	public string resource;
	public OmekaFile[] itemfiles;
	public void loadFiles() {	

	}
}

[System.Serializable]
public class OmekaTag {
	public int id; 
	public string url;
	public string name;
	public string resource; 
	
	private int occurences = 1 ; 
	public void increment() {
		occurences++ ; 
	}
	public int getOccurrences() {
		return occurences ; 
	}
	public void initWithName(string newName) {
		name = newName ; 
		occurences = 1 ; 
	}
}

[System.Serializable]
public class OmekaItemType {
	public int id; 
	public string url; 
	public string name;
	public string resource; 
}

[System.Serializable]
public class OmekaItem {
	public int id;
	public string url;

	public OmekaItemType item_type;
	public OmekaCollection collection ; 
	public OmekaItemFiles files ; 
	public OmekaTag[] tags;
	public OmekaElementTexts[] element_texts ; 
	public OmekaExtendedResources extended_resources ; 

	public void addElementText(OmekaElementTexts elementText) {
		int length ; 
		if(element_texts != null) {length = element_texts.Length + 1; }
		else { length = 1 ; }
		OmekaElementTexts[] newTable = new OmekaElementTexts[length] ; 
		if(element_texts != null) { element_texts.CopyTo(newTable, 0) ; }
		newTable[length - 1] = elementText ;  
		element_texts = newTable ; 
	}

	public int IndexOfElementDefinition(string elementdef) {
		int tablelength = element_texts.Length;
		for(int i=0; i<tablelength ; i++) {
			if(element_texts[i].element.name == elementdef) {
				return i ;
			}
		}
		return -1 ;
	}

	public string getElement(string elementdef) {
		int elt_index = IndexOfElementDefinition(elementdef) ; 
		if(elt_index > -1) {
			return element_texts[elt_index].text ; 
		}
		else return "" ; 
	}
	public int maxFilesNumber () {
		return files.count;
	}

	public string[] tagList() {
		List <string> taglist = new List<string>() ;  
		for (int i = 0;  i<tags.Length ; i++) {
			taglist.Add(tags[i].name) ; 
		}
		return taglist.ToArray() ; 
	}

	public string getJSONRepresentation() {
		//Script permettant de créer automatiquement un JSON qui puisse être accepté par OMEKA
		//Sans les valeurs par défaut, avec les bons indicateurs, etc. 

		string JSONRep = "{\"item_type\": {\"id\": "+item_type.id+"}," ;
		JSONRep = JSONRep + "\"collection\": {\"id\": "+collection.id+"},\"public\": true,\"featured\": false," ; 
		JSONRep = JSONRep + "\"element_texts\": [" ; 
		int iterations = 0 ; 
		foreach(OmekaElementTexts elementText in element_texts) {
			if(iterations > 0) { JSONRep = JSONRep + "," ; }
			JSONRep = JSONRep + elementText.getJSONRepresentation() ; 
			iterations++ ; 
		}
		JSONRep = JSONRep + "]}" ; 
		return JSONRep ; 
	}

	public void addTitle(string title) {
		OmekaElementTexts elementTitle = new OmekaElementTexts(title,50,1) ; 
		addElementText(elementTitle) ; 
	}

	public void addCreator(string creator) {
		OmekaElementTexts elementTitle = new OmekaElementTexts(creator,39,1) ; 
		addElementText(elementTitle) ; 
	}

	public void addPublisher(string publisher) {
		OmekaElementTexts elementTitle = new OmekaElementTexts(publisher,45,1) ; 
		addElementText(elementTitle) ; 
	}
	public void addDate(string date) {
		OmekaElementTexts elementTitle = new OmekaElementTexts(date,40,1) ; 
		addElementText(elementTitle) ; 
	}

	public void setType(int typeID) {
		OmekaItemType newType = new OmekaItemType() ; 
		newType.id = typeID ; 
		item_type = newType ; 
	}

	public void setCollection(int collectionID) {
		OmekaCollection newCollection = new OmekaCollection() ; 
		newCollection.id = collectionID ; 
		collection = newCollection ; 
	}
}