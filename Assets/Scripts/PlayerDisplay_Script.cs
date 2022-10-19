using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay_Script : MonoBehaviour
{
    /*
    class used to control the display for each individual player

    handles updating the current score, tracking high score, 
    also displays the death timer for each player,
    and when the snake isnt alive, prompt player to press a key to begin

    use flow->
        --> displays exist when game starts,
            hides/disables displays that arent in use,
            handler calls each display's (SetValues) and updates them with all info
            displays start in idle mode (displays press button to start)
        --> player presses button -> snake spawns
            displays change from idle mode to score display mode
            display updates constantly to display the player's score
        --> player changes to a temporary state
            score changes to countdown mode
            display counts down to reflect when the snake will change back to regular state
    */


    //vars gathered when script loaded
        //vars assigned by parent
    
    SpriteRenderer Background;

    TextMesh PlayerNumText;
        int playerNum = 0;

    GameObject PressKeyPrompt;

        Color DeadColour;
        Color AliveColour;

    GameObject DeathTimerDisplay;
    TextMesh DeathTimerText;
        int DeathPenaltyTime = 0;

    GameObject ScoreDisplays;
    TextMesh ScoreText;
    TextMesh HighscoreText;
        int score = 0;
        int highscore = 0;

   
    
    
    // Start is called before the first frame update
    void Start()
    {
        Background = this.transform.GetChild(0).GetComponent<SpriteRenderer>();

        PlayerNumText = this.transform.GetChild(1).GetComponent<TextMesh>();

        PressKeyPrompt = this.transform.GetChild(2).gameObject;

        DeathTimerDisplay = this.transform.GetChild(3).gameObject;
        DeathTimerText = DeathTimerDisplay.GetComponent<TextMesh>();

        ScoreDisplays = this.transform.GetChild(4).gameObject;
        ScoreText = ScoreDisplays.transform.GetChild(0).GetComponent<TextMesh>();
        HighscoreText = ScoreDisplays.transform.GetChild(1).GetComponent<TextMesh>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(int pNum, int dTime, Color cDead, Color cAlive)
    {
        //grab initial values
        playerNum = pNum + 1;//playernum starts at 0, this display starts at 1
        DeathPenaltyTime = dTime;
        DeadColour = cDead;
        AliveColour = cAlive;

        //display them
        PlayerNumText.text = playerNum.ToString();

        //setup initial display (no score, no countdown, yes player num, yes press play prompt)
        PressKeyPrompt.SetActive(true);
        ScoreDisplays.SetActive(false);
        DeathTimerDisplay.SetActive(false);

    }

    public void ShowStartPrompt(bool b){
        if (b){
            //show start prompt text, hide all other

        }
        else{
            //hide start prompt text, show all other

        }
    }

    public void StartCountdown(int time){
        //hide score , show number

        // countdown one step + change number
        //repeat x time

        //hide number, show score
    }

    public void UpdateScore(int s)
    {
        score = s;
        //displaying current score
        ScoreText.text = score.ToString();

        //checking if current score is higher than highscore
        if (highscore < score)
        {
            highscore = score;
            HighscoreText.text = highscore.ToString();
        }
    }
}
