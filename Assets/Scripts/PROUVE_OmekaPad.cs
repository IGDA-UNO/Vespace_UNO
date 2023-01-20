//PROUVE (Extensions Omeka et Unity pour le Patrimoine en Réalité Virtuelle)
//Version 2.0
//OmekaPad, virtual UI for displaying OMEKA content
//Paul François - VESPACE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PROUVE_OmekaPad : MonoBehaviour
{

    public GameObject omekaPad ; 
    public GameObject extendedPad ; 
    public GameObject imageZone ; 
    public GameObject nextImageButton ; 
    public GameObject previousImageButton ;
    public GameObject backButton ; 
    public GameObject closeButton ; 
    public GameObject loadingText ;  
    public GameObject mainView;
    public GameObject fullSizeImageView ; 
    public Text omekaTitleText ; 
    public Text omekaDescriptionText ; 
    public Text omekaImageDescription ;
    public Text fullSizeImageTitle ; 
    public Text omekaKeywordsText ; 
    public Image mainImageView ; 
    public Image fullSizeImage ; 
    public RectTransform relationZone ; 
    public RectTransform elementsZone ;

    private SimpleObjectPool objectPool ; 
    private SimpleObjectPool elementObjectPool ; 
    private RectTransform imageViewRectTransform ; 
    private RectTransform fullSizeImageRectTransform; 
    private int currentOmekaItem ; 
    private int currentImageNumber ; 
    private int maxFileNumber ; 
    private string currentActionType ; 
    private string currentLanguage = "fra" ; //language de l'interface
    private PROUVE_ImageComponent currentImageComponent ; 

    private List <int> arborescenceOmekaID = new List<int>() ; 

    private PROUVE_DataCaller dataCaller ; 
    private PROUVE_ExperienceManager experienceManager; 

    private float mainImageMaxWidth = 500 ; 
    private float mainImageMaxHeight = 300 ; 
    private float fullSizeImageMaxWidth = 1400 ; 
    private float fullSizeImageMaxHeight = 1000 ; 

    private bool proModeActivated = false ;

    public ENSEMBLE_UIHandler ensemble;
    public ENSEMBLE_UIHandler_Introduction ensembleIntro;

    //Initialization : 

    public void initWithDataCallerAndExperienceManager(PROUVE_DataCaller DC, PROUVE_ExperienceManager EM, string language) {
        dataCaller = DC ; 
        experienceManager = EM ; 
        currentLanguage = language ; 
    }

    public void enableProMode(bool flag) {
        proModeActivated = flag ; 
    }

    //Self interface control :

    public void Display() {
        omekaPad.SetActive(true);

        if (ensemble != null) {
            ensemble.CloseMenu();
        } else if (ensembleIntro != null) {
            ensembleIntro.CloseMenu();
        }
    }
    
    public void closePad() {
        omekaPad.SetActive(false) ; 
        experienceManager.registerSimpleActivity() ; 
    }

    public bool padState() {
        return omekaPad.activeSelf ; 
    }

    private void hideImageZone() {
        imageZone.SetActive(false) ; 
    }

    private void showImageZone() {
        imageZone.SetActive(true) ; 
    }

    private void hideFullSizeImageView() {
        fullSizeImageView.SetActive(false) ; 
    }

    private void showFullSizeImageView() {
        fullSizeImageView.SetActive(true) ; 
    }

    private void hideMainView() {
        mainView.SetActive(false) ; 
    }

    private void showMainView() {
        mainView.SetActive(true) ; 
    }

    private void hideExtendedPad() {
        extendedPad.SetActive(false) ;
    }

    private void showExtendedPad() {
        extendedPad.SetActive(true) ; 
    }

    public void nextImage() {
        if(currentImageNumber + 1 < maxFileNumber) {
            dataCaller.loadCurrentItemImage(currentImageNumber+1) ; 
        }
    }

    public void previousImage() {
        if(currentImageNumber > 0) {
            dataCaller.loadCurrentItemImage(currentImageNumber-1) ; 
        }

    }

    private void updateImageControls() {
        if(currentImageNumber == 0) {
            previousImageButton.SetActive(false) ; 
        } else { previousImageButton.SetActive(true) ; }
        if(currentImageNumber + 1 == maxFileNumber) {
            nextImageButton.SetActive(false) ; 
        } else{ nextImageButton.SetActive(true) ; }
    }

    private void RemoveRelationButtons() {
		while (relationZone.childCount > 0) 
        {
            GameObject toRemove = relationZone.transform.GetChild(0).gameObject;
            objectPool.ReturnObject(toRemove);
        }
	}

    private void flushElements() {
        while (elementsZone.childCount > 0) 
        {
            GameObject toRemove = elementsZone.transform.GetChild(0).gameObject;
            elementObjectPool.ReturnObject(toRemove);
        }
    }

    private void updateBackButton() {
        if(arborescenceOmekaID.Count > 1) {
            backButton.SetActive(true) ;
        } else {backButton.SetActive(false);}
    }

    private void refreshUI() {
        Display() ; 
        RemoveRelationButtons() ; 
        flushElements() ; 
        hideImageZone() ; 
        hideFullSizeImageView() ; 
        showMainView() ; 
        if(proModeActivated) { showExtendedPad() ; } else { hideExtendedPad() ; }
        loadingText.SetActive(true) ; 
    }

    public void finishedLoading() {
        loadingText.SetActive(false) ; 
    }

    //Methodes principales : 

    public void displayItem(int itemID) {
        if(!dataCaller.isWorking()) {
            refreshUI() ; 
            currentActionType = "main" ; 
            dataCaller.lookForItemWithID(itemID) ; 
            arborescenceOmekaID.Clear() ;
            arborescenceOmekaID.Add(itemID) ;  
        } else {
            //Debug.Log("Data Caller is still working !") ; 
        }
    }

    public void displayItemInRelation(int itemID) {
        if(!dataCaller.isWorking()) {
            refreshUI() ; 
            currentActionType = "relation" ; 
            dataCaller.lookForItemWithID(itemID) ; 
            arborescenceOmekaID.Add(itemID) ; 
        }
    }

    public void previousItem() {
        if(fullSizeImageView.activeSelf) {
            hideFullSizeImageView() ; 
            showMainView() ; 
        } else {
            if(!dataCaller.isWorking()) {
                refreshUI() ;
                arborescenceOmekaID.RemoveAt(arborescenceOmekaID.Count-1) ; 
                currentActionType = "back" ; 
                dataCaller.lookForItemWithID(arborescenceOmekaID[arborescenceOmekaID.Count -1]);
            }
        }
    }

    public void updateWithTextContent(PROUVE_TextComponent textComponent) {
        //updateBackButton() ; 
        omekaTitleText.text = selectStringLanguage(textComponent.title) ; 
        omekaDescriptionText.text = selectStringLanguage(textComponent.description) ; 
        currentOmekaItem = textComponent.id ; 
        maxFileNumber = textComponent.filesCount ; 
        currentImageNumber = 0 ; 
        experienceManager.registerOmekaAction(currentOmekaItem,currentActionType,textComponent.keywords) ; 
        if(proModeActivated) {
            omekaKeywordsText.text = textComponent.tagString() ; 
            foreach(PROUVE_QuickElement element in textComponent.elements) {
                addElementObject(element) ; 
            }
        }
    }

    public void updateTextWithErrorMessage(){
        omekaTitleText.text = selectStringLanguage("An Error Has Occured");
        omekaDescriptionText.text = selectStringLanguage("We apologize. This data is not available.");
    }

    public void updateImageContent(PROUVE_ImageComponent imageComponent) {
        currentImageComponent = imageComponent ; 
        mainImageView.sprite = imageComponent.returnSpriteToFit(mainImageMaxWidth,mainImageMaxHeight) ; 
        omekaImageDescription.text = imageComponent.title ; 
        currentImageNumber = imageComponent.id ; 
        imageViewRectTransform.sizeDelta = new Vector2(imageComponent.width,imageComponent.height) ; 
        showImageZone() ; 
        updateImageControls() ; 
    }

    public void showFullSizeImage() {
        fullSizeImage.sprite = currentImageComponent.returnSpriteToFit(fullSizeImageMaxWidth,fullSizeImageMaxHeight) ; 
        fullSizeImageTitle.text = currentImageComponent.title;
        fullSizeImageRectTransform.sizeDelta = new Vector2(currentImageComponent.width,currentImageComponent.height) ; 
        hideMainView() ; 
        showFullSizeImageView() ; 
    }

    public void hideFullSizeImage() {
        hideFullSizeImageView() ;
        showMainView() ; 
    }

    //Affichage des éléments : 

    public void addElementObject(PROUVE_QuickElement elementObject) {
        GameObject newElement = elementObjectPool.GetObject() ; 
        newElement.transform.SetParent(elementsZone, false) ; 
        newElement.transform.eulerAngles = elementsZone.transform.eulerAngles ; 
        newElement.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) ; 
        PROUVE_elementInterfaceController element = newElement.GetComponent<PROUVE_elementInterfaceController>() ; 
        element.setInfos(selectStringLanguage(elementObject.title), selectStringLanguage(elementObject.description)) ; 
    }

    //Affichage des relations : 

    public void addRelationButtonObject(OmekaItemRelationsObject relation){
		GameObject newButton = objectPool.GetObject();
		newButton.transform.SetParent(relationZone, false) ;
        newButton.transform.eulerAngles = relationZone.transform.eulerAngles ;
        newButton.transform.localScale = new Vector3(1.0f,1.0f,1.0f) ; 
		ItemRelationButton irButton = newButton.GetComponent<ItemRelationButton>() ; //relation au script attaché au prefab;
		irButton.SetupObject(relation, this) ; 
        dataCaller.lookForThumbnailForButton(irButton) ; 
	}

	public void addRelationButtonSubject(OmekaItemRelationsSubject relation){
		GameObject newButton = objectPool.GetObject();
		newButton.transform.SetParent(relationZone, false) ;
        newButton.transform.eulerAngles = relationZone.transform.eulerAngles ;
        newButton.transform.localScale = new Vector3(1.0f,1.0f,1.0f) ; 
		ItemRelationButton irButton = newButton.GetComponent<ItemRelationButton>() ; //relation au script attaché au prefab;
		irButton.SetupSubject(relation, this) ; 
		dataCaller.lookForThumbnailForButton(irButton) ; 
	}


    //Selection de la langue des éléments affichés

    private string selectStringLanguage(string multiLanguageString){
        string ISO_CODE;
        string OTHER_ISO;
        if(currentLanguage == "fra") {
            ISO_CODE = "_fra:";
            OTHER_ISO = "_eng:";
        } else {
            ISO_CODE = "_eng:";
            OTHER_ISO = "_fra:";
        }
        int beginposition = multiLanguageString.IndexOf(ISO_CODE) ;
        if(beginposition > -1){
            beginposition = beginposition +5;
            int endposition = multiLanguageString.IndexOf(OTHER_ISO);
            if(endposition < beginposition) {
                endposition = multiLanguageString.Length;
            }
            return multiLanguageString.Substring(beginposition,endposition-beginposition).Trim();
        } else {
            beginposition = multiLanguageString.IndexOf(OTHER_ISO) ;
            if(beginposition > -1) {
                beginposition= beginposition + 5;
                int endposition = multiLanguageString.Length;
                return multiLanguageString.Substring(beginposition,endposition-beginposition).Trim();

            } else {
                return multiLanguageString.Trim();
            }
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake() {
        objectPool = GetComponent<SimpleObjectPool>() ; 
        elementObjectPool = extendedPad.GetComponent<SimpleObjectPool>() ; 
        imageViewRectTransform = mainImageView.GetComponent<RectTransform>() ; 
        fullSizeImageRectTransform = fullSizeImage.GetComponent<RectTransform>() ; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
