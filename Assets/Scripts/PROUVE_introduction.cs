using System.IO ; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR; 
using TMPro;
using UnityEngine.SceneManagement;

public class PROUVE_introduction : MonoBehaviour
{
    public GameObject firstCanvas ; 
    public GameObject secondCanvas ; 
    public GameObject thirdCanvas ;
    public GameObject fourthCanvas;
    public GameObject demoNPC ;
    public InputField userName ; 
    public PROUVE_SceneHandler sceneHandler ;

    //Wallpaper Canvas
    public Text WallpaperCanvasTitleText;

    public Text IntroCanvasTitleText;

    public GameObject snuffBoxImage;

    public Button SkipIntroductionButton;

    public Button WallpaperCanvasPrevButton;
    public Button IntroCanvasPrevButton;
    public Button ControlCanvasPrevButton;
    public Button ObjectCanvasPrevButton;
    public Button FinalCanvasPrevButton;

    public Button WallpaperCanvasNextButton;
    public Button IntroCanvasNextButton;
    public Text IntroCanvasNextButtonText;
    public Button ControlCanvasNextButton;
    public Button ObjectCanvasNextButton;
    public Button FinalCanvasNextButton;

    public Button projectWebsiteButton;

    public string historicalScholarshipTitle;
    public string historicalScholarshipText;


    private const string VespaceUrl = "https://vespace.cs.uno.edu/";
    public TextMeshProUGUI welcomeCanvasText;
    public TextMeshProUGUI wallpaperCanvasText;
    public int textIndex = 0;
    public List<string> introTextList = new List<string>();
    public List<string> titleTextList = new List<string>();

/*
    public string introText1 = "This virtual reality playable experience is one of the primary research outputs of a five-year collaborative research project involving scholars in literature, theatre, history, architecture, and computer science, primarily sponsored by the National Endowment for the Humanities Digital Humanities Advancement Grants HAA-255998-17 (Phase I) and HAA-266501-19 (Phase II). Don’t forget to visit our project webpage, <link=\"VespaceLink\">vespace.cs.uno.edu</link>, to learn more about the people and ideas behind this experience!";
    public string introText2 = "In this experience, you will have the opportunity to choose between three different characters – a male noble, a female noble, and a male servant – and to embark on a mission to steal the plans for a new theatre to be built at the Foire Saint-Germain, which have been poorly hidden backstage at Bienfait’s marionette theatre! In order to accomplish this goal, you will have to: <br><indent=15%>enter the theatre</indent> <br><indent=15%>try to convince someone to help you get backstage</indent> <br><indent=15%>obtain the plans</indent> <br><indent=15%>and then leave the theatre without attracting too much notice.</indent>";
    public string introText3 = "The virtual environment is modeled on a miniature painting that decorates the top of a jeweled snuffbox, currently in the collection of the Metropolitan Museum in New York.You can examine this source in this introductory space, and find more information about our process of transforming this 3cm x 6cm retrospective depiction of a now-lost performance space into the setting for an immersive playable experience in the articles on the VESPACE website. The characters in the space are all derived from period artworks, and are intended to represent a selection of the different social types one might meet at an eighteenth-century Fair theatre, which was one of the most socially mixed public environments of Ancien Régime France.";
    public string introText4 = "This sensory-immersive environment, and the interactions with non-player characters(NPCs) are also one way that our research team has attempted to present high-level scholarship in an accessible, engaging, non-print format.To that end, there are objects throughout the space that link to a database of explanatory materials, allowing this sensory experience to serve as the platform for further exploration of eighteenth-century French public culture.";
    public string introText5 = "This phase of the VESPACE project represents a proof-of-concept for many of our experimental ambitions for re-thinking historically-oriented scholarship for the twenty-first century.We hope you enjoy this experience, which represents the work of dozens of scholars across the US and Europe.We would be delighted to hear from you with feedback or suggestions – please contact us through our website, vespace.cs.uno.org – and enjoy the game!";
*/

    // Start is called before the first frame update
    void Start()
    {
        //Debug.unityLogger.logEnabled = false;
        Debug.Log("LA LAL A LA CAN YOU SEE ME?");
        Debug.Log("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
        sceneHandler = GetComponent<PROUVE_SceneHandler>(); 
        firstCanvas.SetActive(true) ; 
        secondCanvas.SetActive(false) ; 
        demoNPC.SetActive(false);
        thirdCanvas.SetActive(false) ;  
        sceneHandler.setAllowRestart(false) ; 
        userName.ActivateInputField();
        userName.Select();


        introTextList.Add("This virtual reality playable experience is one of the primary research outputs of a five-year collaborative project involving scholars in literature, theatre, history, architecture, and computer science, sponsored by the National Endowment for the Humanities Digital Humanities Advancement Grants HAA-255998-17 (Phase I) and HAA-266501-19 (Phase II). Don’t forget to visit our project webpage, <link=\"VespaceLink\">vespace.cs.uno.edu</link>, to learn more about the people and ideas behind this experience!");
        titleTextList.Add("Welcome to VESPACE!");

        introTextList.Add("In this experience, you will have the opportunity to play the role of either a male noble, a female noble, or a male servant. Your goal is to get the plans for a new theatre to be built at the Foire Saint-Germain – plans which have been poorly hidden backstage at Bienfait’s marionette theatre! In order to accomplish this goal, you will have to: <br><br><indent=15%>1. Enter the theatre</indent> <br><indent=15%>2. Try to convince someone to help you get backstage</indent> <br><indent=15%>3. Obtain the plans</indent> <br><indent=15%>4. Leave the theatre without attracting too much notice.</indent>");
        titleTextList.Add("Game Objective");

        introTextList.Add("The virtual environment is modeled on a miniature painting from the top of a jeweled snuffbox, currently in the collection of the Metropolitan Museum in New York. You can examine this source in this introductory space, and find more information about our process of transforming this tiny (3cm x 6cm!) depiction of a now-lost performance space into the setting for an immersive playable experience in the articles on the VESPACE website. Character avatars are all derived from period artworks, selected to represent different social types one might meet at an eighteenth-century Fair theatre, one of the most socially mixed public environments of Ancien Régime France.");
        titleTextList.Add("Historical Context");
        
        introTextList.Add("This sensory-immersive environment, and the interactions with non-player characters(NPCs) are also one way that our research team has attempted to present high-level scholarship in an accessible, engaging, non-print format.To that end, objects throughout the space link to a database of explanatory materials, allowing this experience to serve as the platform for further exploration of eighteenth-century French public culture.");
        titleTextList.Add("Objects and NPCs");
        
        historicalScholarshipText = "This phase of the VESPACE project represents a proof-of-concept for many of our experimental ambitions for rethinking historically-oriented scholarship for the twenty-first century.We hope you enjoy this experience, which represents the work of dozens of scholars across the US and Europe. We would be delighted to hear from you with feedback or suggestions – please contact us through our website, vespace.cs.uno.org – and enjoy the game!";
        historicalScholarshipTitle = "Re-thinking Historical Scholarship";
        

        WallpaperCanvasTitleText.text = titleTextList[0];
        wallpaperCanvasText.text = introTextList[0];

        IntroCanvasTitleText.text = historicalScholarshipTitle;
        welcomeCanvasText.text = historicalScholarshipText;
    }

    public void advanceIntroText(){
        textIndex++;


        //***testing printing things out to identify input device
        Debug.Log(SystemInfo.deviceModel);
        Debug.Log("operatingSystem: " + SystemInfo.operatingSystem);
        Debug.Log("deviceModel: " + SystemInfo.deviceModel);
        Debug.Log("deviceName: " + SystemInfo.deviceName);
        Debug.Log("deviceType: " + SystemInfo.deviceType);
        Debug.Log("platform: " + Application.platform);

        var devices = new List<InputDevice>();
        InputDevices.GetDevices(devices) ;
        foreach (var device in devices)
        {
            Debug.Log("Device connected: " +device.name);
        }

        if (SystemInfo.deviceName == "Oculus Quest 2"){
            Debug.Log("we detected speciifcally oculus quest 2");
            //DoNeededCode(); // Standalone Quest 2
        }
    //*****end testing printing things out to identify input device.

        WallpaperCanvasPrevButton.gameObject.SetActive(true);

        //update the 'next' button text.
        /*
        if(textIndex == introTextList.Count-1){
            IntroCanvasNextButtonText.text = "To Controls";
        }
        else{
            IntroCanvasNextButtonText.text = "Next";
        }
        */

        
        if(textIndex >= introTextList.Count - 1){
            textIndex = introTextList.Count - 1;
            WallpaperCanvasNextButton.gameObject.SetActive(false);
        }
        else{
            WallpaperCanvasNextButton.gameObject.SetActive(true);
        }

        //update the text
        WallpaperCanvasTitleText.text = titleTextList[textIndex];
        wallpaperCanvasText.text = introTextList[textIndex];
        

        if(textIndex == 2){
            snuffBoxImage.SetActive(true);
        }
        else{
            snuffBoxImage.SetActive(false);
        }

        //Deal with the project webpage button
        /*
        if (textIndex == 0 || textIndex == 4)
        {
            projectWebsiteButton.gameObject.SetActive(true);
        }
        else
        {
            projectWebsiteButton.gameObject.SetActive(false);
        }
        */
    }

    public void retreatIntroText()
    {
        textIndex--;
        if (textIndex <= 0)
        {
            textIndex = 0;
            WallpaperCanvasTitleText.text = titleTextList[textIndex];
            wallpaperCanvasText.text = introTextList[textIndex];
            //welcomeCanvasText.text = introTextList[textIndex];
            //IntroCanvasTitleText.text = titleTextList[textIndex];
            WallpaperCanvasPrevButton.gameObject.SetActive(false);
        }
        else
        {
            WallpaperCanvasPrevButton.gameObject.SetActive(true);
            WallpaperCanvasTitleText.text = titleTextList[textIndex];
            wallpaperCanvasText.text = introTextList[textIndex];
        }

        //We always want 'next' to be true after hitting 'previous'
        WallpaperCanvasNextButton.gameObject.SetActive(true);



        //update the 'next' button text.
        /*
        if (textIndex == introTextList.Count - 1)
        {
            IntroCanvasNextButtonText.text = "To Controls";
        }
        else
        {
            IntroCanvasNextButtonText.text = "Next";
        }
        */

        if(textIndex == 2){
            snuffBoxImage.SetActive(true);
        }
        else{
            snuffBoxImage.SetActive(false);
        }

        //Deal with the project webpage button
        /*
        if (textIndex == 0 || textIndex == 4)
        {
            projectWebsiteButton.gameObject.SetActive(true);
        }
        else
        {
            projectWebsiteButton.gameObject.SetActive(false);
        }
        */
    }

    public void openProjectWebsite(){
        Debug.Log("Opening project website");
        Application.OpenURL(VespaceUrl);
    }

    public void skipIntroduction(){
        textIndex = 0;
        firstCanvas.SetActive(false); 
        secondCanvas.SetActive(false); 
        thirdCanvas.SetActive(false); 
        demoNPC.SetActive(false);
        fourthCanvas.SetActive(false);
    }

    public void closeFirstCanvas() {
        saveUserName(userName.text) ; 
        firstCanvas.SetActive(false) ; 
        secondCanvas.SetActive(true) ; 
        sceneHandler.setAllowRestart(true) ; 
    }

    public void returnToFirstCanvas() {
        secondCanvas.SetActive(false);
        firstCanvas.SetActive(true);
        demoNPC.SetActive(false);
        welcomeCanvasText.text = historicalScholarshipText;
        IntroCanvasTitleText.text = historicalScholarshipTitle;
        //retreatIntroText();
    }

    public void closeSecondCanvas() {
        secondCanvas.SetActive(false) ; 
        demoNPC.SetActive(true) ;
        thirdCanvas.SetActive(true) ; 
    }

    public void returnToSecondCanvas()
    {
        thirdCanvas.SetActive(false);
        secondCanvas.SetActive(true);
        demoNPC.SetActive(false);
    }

    public void closeThirdCanvas()
    {
        demoNPC.SetActive(false); 
        thirdCanvas.SetActive(false);     
        fourthCanvas.SetActive(true);
    }

    public void returnToThirdCanvas()
    {
        thirdCanvas.SetActive(true);
        fourthCanvas.SetActive(false);
        demoNPC.SetActive(true);
    }

    public void closeFourthCanvas() {
        fourthCanvas.SetActive(false) ; 
    }

    static void saveUserName(string name) {
        PlayerPrefs.SetString("UserName",name) ;
        PlayerPrefs.SetInt("LocalMode",0) ; 
        PlayerPrefs.SetString("StartDate",System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")) ;
        PlayerPrefs.Save() ; 
    }

    // Update is called once per frame
    void Update()
    {
        if(firstCanvas.activeSelf)  {
            if(!userName.isFocused){
                userName.ActivateInputField();
                userName.Select() ; 
            }
        }

    }
}
