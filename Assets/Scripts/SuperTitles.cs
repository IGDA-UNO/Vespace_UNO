using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class SuperTitles : MonoBehaviour
{

    private class SuperTitleHelper
    {
        public float minute;
        public float second;
        public float time;
        public string dialogue;

        public SuperTitleHelper(float minute, float second, string dialogue){
            this.minute = minute;
            this.second = second;
            this.time = minute * 60 + second;
            this.dialogue = dialogue;
        }
    }

    public float currentTimeElapsed;
    public static bool timerStarted;
    public Text textField;
    List<SuperTitleHelper> linesOfDialogue = new List <SuperTitleHelper>();
    SuperTitleHelper currentLine;
    SuperTitleHelper nextLine;
    int nextLineIndex;

    // Start is called before the first frame update
    void Start()
    {
        linesOfDialogue.Add(new SuperTitleHelper(0f, 0f, ""));
        linesOfDialogue.Add(new SuperTitleHelper(0f, 5f, "I've got the floor!"));
        linesOfDialogue.Add(new SuperTitleHelper(0f, 10f, "Cupid decided to critique all the theatres last week, \n and I in turn will do the same..."));
        linesOfDialogue.Add(new SuperTitleHelper(0f, 23f, "Polichinelle's theatre is the noblest of the Fair, \n and that's a fact!"));
        linesOfDialogue.Add(new SuperTitleHelper(0f, 29f, "The rope dancers shut their trap, they did well \n to keep quiet. The Opéra-Comique blabbered \n on and on, and he should have just shut up."));
        linesOfDialogue.Add(new SuperTitleHelper(0f, 47f, "My fellow theatres, the other Polichinelles, \n made way for me.  Thus, it is fair \n that I make the decisions as sovereign."));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 6f, "Here comes the Opéra. Let's start with him."));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 11f, "Hello, Polichinelle my friend. \n They say that your parody of <i>Persée</i> is quite funny."));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 21f, "I just saw it. Are you roasting me too?"));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 28f, "I've been doing it for a long time. \n You provide me with plenty of sticks \n to beat you with."));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 36f, "What's that piece of paper you have? \n Is it a play for the new season?"));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 47f, "These are the bills from <i>Persée</i>. \n The expenses from the tailors alone \n are bankrupting me."));
        linesOfDialogue.Add(new SuperTitleHelper(1f, 59f, "The ball made you rich. \n Those who danced at your ball paid for the violins."));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 07f, "My parody cost just as much as your play to put on. \n My monster is more beautiful than yours."));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 27f, "Why let me boast about it. \n My actresses are well dressed \n and deliciously beautiful."));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 38f, "As much as an actress made of wood can be, \n I suppose."));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 43f, "If my actresses were made of flesh like yours, \n say goodbye to their figure! There's always something \n to improve in a real woman."));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 54f, "Is the show starting soon?"));
        linesOfDialogue.Add(new SuperTitleHelper(2f, 59f, "My opera resembles your own. \n Get yourself a good seat, \n the Opéra is the prince of all performances."));
        linesOfDialogue.Add(new SuperTitleHelper(3f, 11f, "And the performance of princes."));
        linesOfDialogue.Add(new SuperTitleHelper(3f, 17f, ""));
        linesOfDialogue.Add(new SuperTitleHelper(3f, 32f, "What does this fat queen want from me?"));
        linesOfDialogue.Add(new SuperTitleHelper(3f, 39f, "Ah, It's the Comédie-Française!\n We all know that she makes her dough \n with the public."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 01f, "I have come to relax after my noble labors."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 07f, "The Opéra is much more popular, it is openly \n called the cousin of the Opéra-Comique. The Opéra \n is even paid to help handle the embarrassment."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 20f, "So, are you coming to see my parody?"));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 25f, "I'm counting on laughing at it."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 30f, "You seem to be very pleased with yourself."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 33f, "I had a good haul. Despite my arrogance, \n the finest poets bring me their work with respect. \n That's what's ruining you."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 46f, "Go take a seat near the Opéra, \n you two can go hand in hand."));
        linesOfDialogue.Add(new SuperTitleHelper(4f, 52f, "If I'm pleased, \n I will grant you my protection \n and you can count on my friendship."));
        linesOfDialogue.Add(new SuperTitleHelper(5f, 02f, "Seems like you learned your lesson \n from <i>The School of Friends</i>. \n We will see about that."));
        linesOfDialogue.Add(new SuperTitleHelper(5f, 09f, "Here comes the Comédie-Italienne. \n The gang will all be here now."));
        linesOfDialogue.Add(new SuperTitleHelper(5f, 15f, ""));

        nextLineIndex = 1;
        currentLine = linesOfDialogue[0];
        nextLine = linesOfDialogue[nextLineIndex];
        timerStarted = false;   
        currentTimeElapsed = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted){
            currentTimeElapsed += Time.deltaTime;
        }
        textField.text = "";
        //textField.text = "time elapsed: " + currentTimeElapsed;
        //textField.text += "\n\n\n";
        textField.text += currentLine.dialogue;

        if(currentTimeElapsed >= nextLine.time){
            AdvanceDialogue();
        }

        /*
        //Useful for testing but we probably don't want this all the time.
        if(Input.GetKeyDown(KeyCode.N)){
            AdvanceDialogue();
        }
        */
    }

    private void AdvanceDialogue(){
        nextLineIndex++;
        currentLine = nextLine;
        if (nextLineIndex < linesOfDialogue.Count)
        {
            nextLine = linesOfDialogue[nextLineIndex];
        }
    }

    public static void StartTimer(){
        timerStarted = true;
    }

    public void ShowNextLine(){
        nextLineIndex++;
    }

}
