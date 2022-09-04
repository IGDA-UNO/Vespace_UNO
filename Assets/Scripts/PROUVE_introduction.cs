using System.IO ; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class PROUVE_introduction : MonoBehaviour
{
    public GameObject firstCanvas ; 
    public GameObject secondCanvas ; 
    public GameObject thirdCanvas ;
    public GameObject fourthCanvas;
    public GameObject demoCube ;
    public InputField userName ; 
    public PROUVE_SceneHandler sceneHandler ;

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
        userName.Select() ; 
    }

    public void closeFirstCanvas() {
        saveUserName(userName.text) ; 
        firstCanvas.SetActive(false) ; 
        secondCanvas.SetActive(true) ; 
        demoCube.SetActive(true) ; 
        sceneHandler.setAllowRestart(true) ; 
    }

    public void closeSecondCanvas() {
        secondCanvas.SetActive(false) ; 
        demoCube.SetActive(false) ;
        thirdCanvas.SetActive(true) ; 
    }

    public void closeThirdCanvas()
    {
        thirdCanvas.SetActive(false);     
        fourthCanvas.SetActive(true);
    }

    public void closeFourthCanvas() {
        thirdCanvas.SetActive(false) ; 
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
