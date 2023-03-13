using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
    class used to control the display for each individual player

    when game begins, displays a message prompting the player to press any key to begin
    display shows the player's number, colour,
    score, highscore,
    also has a timer and indicators to show the player's status (alive, ghosted, golden, or death penalty)

    use flow ->
        --> displays exist when game starts,
            handler hides the unused displays
            handler calls each display's (SetValues) and updates them with all info
            displays show press button prompt
        --> player presses button
            press btn prompt is hidden
            snake spawns
            display updates constantly to display the player's score
        --> when player changes to a temporary state (ghosted, golden, or death penalty)
            indicator object is shown
            timer bar displays the duration of time for the current state
*/


public class PlayerGUI_Script : MonoBehaviour
{

    //playernum text
    private Text playerNum;

    //player colour spriterenderer
    private SpriteRenderer playerColour;


    //score text
    private Text scoreText;
    //highscore text
    private Text highscoreText;

    //press key to begin object
    private GameObject pressKeyPrompt;


    //timer object
    private GameObject timer;
    //timer bar object
    private GameObject timerBar;


    //ghost indicator object
    private GameObject ghostIndicator;
    //gold indicator object
    private GameObject goldIndicator;
    //dead indicator object
    private GameObject deadIndicator;

   
    // Start is called before the first frame update
    void Start()
    {
        playerColour = this.GetChild[0].GetChild[1].GetComponent<SpriteRenderer>();
        playerNum = this.GetChild[1].GetChild[0].GetComponent<Text>();

        pressKeyPrompt = this.GetChild[4];

        scoreText = this.GetChild[2].GetChild[1].GetComponent<Text>();
        highscoreText = this.GetChild[2].GetChild[3].GetComponent<Text>();

        timer = this.GetChild[3].GetChild[0].GameObject;
        timerBar = timer.GetChild[1].GameObject;

        ghostIndicator = this.GetChild[3].GetChild[1];
        goldIndicator = this.GetChild[3].GetChild[2];
        deadIndicator = this.GetChild[3].GetChild[3];
    }

    
    // Update is called once per frame
    void Update()
    {
        if (isCountingDown){
            timer -= Time.deltaTime;
            indicatorScript.UpdateIndicator(totalTimer, timer);
            if (timer < 0){
                isCountingDown = false;
                indicatorScript.UpdateIndicator(totalTimer, 0);
            }
        }
    }

    public void ShowPrompt(){
        pressKeyPrompt.SetActive(true);
    }
    public void HidePrompt(){
        pressKeyPrompt.SetActive(false);
    }

    public void SetoutlineColour(Color col){
        //sets the outline colour, used when starting the game to set the colour to individual player colour
        outline.color = col;
    }

    public void SetValues(int pNum, GameHandler_Script script)
    {
        gameHandlerScript = script;

        //grab initial values
        playerNum = pNum + 1;//playernum starts at 0, this display starts at 1

        //display them
        playerNumText.text = playerNum.ToString();
    }

    public void StartCountdown(float time, Color col){
        StopCountdown();
        // method tells the display's indicator bar to start counting down 
        // a set amount of time, and display that visually for the player
        timer = totalTimer = time;
        isCountingDown = true;

        indicatorScript.SetChargeColour(col);

        //display initial values
        indicatorScript.UpdateIndicator(totalTimer, timer);
    }

    public void StopCountdown(){
        // method tells the display's indicator bar to stop counting down 
        // stops the charge bar and sets it to 0
        timer = 0;
        isCountingDown = false;

        indicatorScript.SetChargeColour(Color.green);

        //display initial values
        indicatorScript.UpdateIndicator(1, 0);
    }    

    public void UpdateScore(int s)
    {
        score = s;

        //displaying current score
        scoreText.text = score.ToString();

        //checking if current score is higher than highscore
        if (highscore < score)
        {
            highscore = score;
            highscoreText.text = highscore.ToString();
        }

        gameHandlerScript.UpdateScore(playerNum, score);
    }



}
