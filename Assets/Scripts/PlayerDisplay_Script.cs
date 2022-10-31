using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplay_Script : MonoBehaviour
{
    /*
    class used to control the display for each individual player

    when game begins, displays a message prompting the player to press any key to begin
    handles updating the current score, tracking high score, 
    also displays the death timer for each player,

    use flow->
        --> displays exist when game starts,
            handler calls each display's (SetValues) and updates them with all info
            displays start in idle mode (displays press button to start)
        --> player presses button -> snake spawns
            displays change from idle mode to score display mode
            display updates constantly to display the player's score
        --> player changes to a temporary state (ghosted or invincible)
            score indicator bar displays the duration of time for the current state
            display unfills a charge bar to reflect when the snake will change back to regular state
    */


    //vars gathered when script loaded
        //vars assigned by parent
    
    Indicator_Bar_Script IndicatorScript;
    SpriteRenderer Outline;

    TextMesh PlayerNumText;
        int playerNum = 0;


    GameObject PressKeyPrompt;//the cover of the display that shows "press any key to start"
    // - which dissapears permanently after the player presses a button

    GameObject ScoreDisplays;
    TextMesh ScoreText;
    TextMesh HighscoreText;
        int score = 0;
        int highscore = 0;

    bool isCountingDown = false;
    float totalTimer;
    float timer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        PressKeyPrompt = this.transform.GetChild(0).gameObject;

        Outline = this.transform.GetChild(1).GetComponent<SpriteRenderer>();

        PlayerNumText = this.transform.GetChild(2).GetComponent<TextMesh>();

        ScoreDisplays = this.transform.GetChild(3).gameObject;
        ScoreText = ScoreDisplays.transform.GetChild(0).GetComponent<TextMesh>();
        HighscoreText = ScoreDisplays.transform.GetChild(1).GetComponent<TextMesh>();

        IndicatorScript = this.transform.GetChild(4).GetComponent<Indicator_Bar_Script>();
    }

    public void HidePrompt(){
        PressKeyPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isCountingDown){
            timer -= Time.deltaTime;
            IndicatorScript.UpdateIndicator(totalTimer, timer);
            if (timer < 0){
                isCountingDown = false;
            }
        }
    }

    public void SetOutlineColour(Color col){
        //sets the Outline colour, used when starting the game to set the colour to individual player colour
        Outline.color = col;
    }

    public void SetValues(int pNum)
    {
        //grab initial values
        playerNum = pNum + 1;//playernum starts at 0, this display starts at 1

        //display them
        PlayerNumText.text = playerNum.ToString();
    }

    public void StartCountdown(float time){
        // method tells the display's indicator bar to start counting down 
        // a set amount of time, and display that visually for the player
        timer = totalTimer = time;
        isCountingDown = true;

        //display initial values
        IndicatorScript.UpdateIndicator(totalTimer, timer);
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
