using System.IO ; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class PROUVE_introduction : MonoBehaviour
{
    public GameObject firstCanvas ; 
    public GameObject secondCanvas ; 
    public GameObject thirdCanvas ;
    public GameObject fourthCanvas;
    public GameObject demoCube ;
    public InputField userName ; 
    public PROUVE_SceneHandler sceneHandler ;

    public Text IntroCanvasTitleText;

    public Button IntroCanvasPrevButton;
    public Button ControlCanvasPrevButton;
    public Button ObjectCanvasPrevButton;
    public Button FinalCanvasPrevButton;

    public Button IntroCanvasNextButton;
    public Text IntroCanvasNextButtonText;
    public Button ControlCanvasNextButton;
    public Button ObjectCanvasNextButton;
    public Button FinalCanvasNextButton;


    private const string VespaceUrl = "https://vespace.cs.uno.edu/";
    public TextMeshProUGUI welcomeCanvasText;
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
        sceneHandler = GetComponent<PROUVE_SceneHandler>() ; 
        firstCanvas.SetActive(true) ; 
        secondCanvas.SetActive(false) ; 
        demoCube.SetActive(false) ;
        thirdCanvas.SetActive(false) ;  
        sceneHandler.setAllowRestart(false) ; 
        userName.ActivateInputField();
        userName.Select();


        introTextList.Add("This virtual reality playable experience is one of the primary research outputs of a five-year collaborative research project involving scholars in literature, theatre, history, architecture, and computer science, primarily sponsored by the National Endowment for the Humanities Digital Humanities Advancement Grants HAA-255998-17 (Phase I) and HAA-266501-19 (Phase II). Don’t forget to visit our project webpage, <link=\"VespaceLink\">vespace.cs.uno.edu</link>, to learn more about the people and ideas behind this experience!");
        titleTextList.Add("Welcome to VESPACE!");

        introTextList.Add("In this experience, you will have the opportunity to choose between three different characters – a male noble, a female noble, and a male servant – and to embark on a mission to steal the plans for a new theatre to be built at the Foire Saint-Germain, which have been poorly hidden backstage at Bienfait’s marionette theatre! In order to accomplish this goal, you will have to: <br><indent=15%>enter the theatre</indent> <br><indent=15%>try to convince someone to help you get backstage</indent> <br><indent=15%>obtain the plans</indent> <br><indent=15%>and then leave the theatre without attracting too much notice.</indent>");
        titleTextList.Add("Game Objective");

        introTextList.Add("The virtual environment is modeled on a miniature painting that decorates the top of a jeweled snuffbox, currently in the collection of the Metropolitan Museum in New York. You can examine this source in this introductory space, and find more information about our process of transforming this 3cm x 6cm retrospective depiction of a now-lost performance space into the setting for an immersive playable experience in the articles on the VESPACE website. The characters in the space are all derived from period artworks, and are intended to represent a selection of the different social types one might meet at an eighteenth-century Fair theatre, which was one of the most socially mixed public environments of Ancien Régime France.");
        titleTextList.Add("Historical Context");
        
        introTextList.Add("This sensory-immersive environment, and the interactions with non-player characters(NPCs) are also one way that our research team has attempted to present high-level scholarship in an accessible, engaging, non-print format. To that end, there are objects throughout the space that link to a database of explanatory materials, allowing this sensory experience to serve as the platform for further exploration of eighteenth-century French public culture.");
        titleTextList.Add("Objects and NPCs");
        
        introTextList.Add("This phase of the VESPACE project represents a proof-of-concept for many of our experimental ambitions for re-thinking historically-oriented scholarship for the twenty-first century. We hope you enjoy this experience, which represents the work of dozens of scholars across the US and Europe. We would be delighted to hear from you with feedback or suggestions – please contact us through our website, vespace.cs.uno.org – and enjoy the game!");
        titleTextList.Add("Re-thinking Historical Scholarship");

        IntroCanvasTitleText.text = titleTextList[0];
        welcomeCanvasText.text = introTextList[0];
    }

    public void advanceIntroText(){
        textIndex++;
        IntroCanvasPrevButton.gameObject.SetActive(true);

        //update the 'next' button text.
        if(textIndex == introTextList.Count-1){
            IntroCanvasNextButtonText.text = "To Controls";
        }
        else{
            IntroCanvasNextButtonText.text = "Next";
        }

        if(textIndex >= introTextList.Count){
            closeFirstCanvas();
        }
        else{
            welcomeCanvasText.text = introTextList[textIndex];
            IntroCanvasTitleText.text = titleTextList[textIndex];
        }
    }

    public void retreatIntroText()
    {
        textIndex--;
        if (textIndex <= 0)
        {
            textIndex = 0;
            welcomeCanvasText.text = introTextList[textIndex];
            IntroCanvasTitleText.text = titleTextList[textIndex];
            IntroCanvasPrevButton.gameObject.SetActive(false);
        }
        else
        {
            IntroCanvasPrevButton.gameObject.SetActive(true);
            welcomeCanvasText.text = introTextList[textIndex];
            IntroCanvasTitleText.text = titleTextList[textIndex];
        }

        //update the 'next' button text.
        if (textIndex == introTextList.Count - 1)
        {
            IntroCanvasNextButtonText.text = "To Controls";
        }
        else
        {
            IntroCanvasNextButtonText.text = "Next";
        }
    }

    public void closeFirstCanvas() {
        saveUserName(userName.text) ; 
        firstCanvas.SetActive(false) ; 
        secondCanvas.SetActive(true) ; 
        demoCube.SetActive(true) ; 
        sceneHandler.setAllowRestart(true) ; 
    }

    public void returnToFirstCanvas() {
        secondCanvas.SetActive(false);
        firstCanvas.SetActive(true);
        demoCube.SetActive(false);
        retreatIntroText();
    }

    public void closeSecondCanvas() {
        secondCanvas.SetActive(false) ; 
        demoCube.SetActive(false) ;
        thirdCanvas.SetActive(true) ; 
    }

    public void returnToSecondCanvas()
    {
        thirdCanvas.SetActive(false);
        secondCanvas.SetActive(true);
        demoCube.SetActive(true);
    }

    public void closeThirdCanvas()
    {
        thirdCanvas.SetActive(false);     
        fourthCanvas.SetActive(true);
    }

    public void returnToThirdCanvas()
    {
        thirdCanvas.SetActive(true);
        fourthCanvas.SetActive(false);
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
