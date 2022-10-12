using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SuperTitles : MonoBehaviour
{

    private class SuperTitleHelper
    {
        public float time;
        public string dialogue;

        public SuperTitleHelper(float time, string dialogue){
            this.time = time;
            this.dialogue = dialogue;
        }
    }

    float currentTimeElapsed;
    bool timerStarted;
    public Text textField;
    List<SuperTitleHelper> linesOfDialogue = new List <SuperTitleHelper>();
    SuperTitleHelper currentLine;
    SuperTitleHelper nextLine;
    int nextLineIndex;

    // Start is called before the first frame update
    void Start()
    {
        linesOfDialogue.Add(new SuperTitleHelper(0f, "You see me first"));
        linesOfDialogue.Add(new SuperTitleHelper(5f, "See me at five seconds"));
        linesOfDialogue.Add(new SuperTitleHelper(10f, "See me at 10 seconds"));


        nextLineIndex = 1;
        currentLine = linesOfDialogue[0];
        nextLine = linesOfDialogue[nextLineIndex];
        timerStarted = false;   
        currentTimeElapsed = 0f;
        StartTimer();

    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted){
            currentTimeElapsed += Time.deltaTime;
        }

        textField.text = "time elapsed: " + currentTimeElapsed;

        textField.text += "\n\n\n" + currentLine.dialogue;

        if(currentTimeElapsed >= nextLine.time){
            nextLineIndex++;
            currentLine = nextLine;
            if(nextLineIndex < linesOfDialogue.Count){
                nextLine = linesOfDialogue[nextLineIndex];
            }
        }
    }

    public void StartTimer(){
        timerStarted = true;
    }

}
