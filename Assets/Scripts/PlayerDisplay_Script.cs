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

    use flow ->
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
    GameHandler_Script gameHandlerScript;
    Indicator_Bar_Script indicatorScript;
    SpriteRenderer outline;

    TextMesh playerNumText;
        int playerNum = 0;


    GameObject pressKeyPrompt;//the cover of the display that shows "press any key to start"
    // - which dissapears permanently after the player presses a button

    GameObject scoreDisplays;
    TextMesh scoreText;
    TextMesh highscoreText;
    int score = 0;
    int highscore = 0;

    bool isCountingDown = false;
    float totalTimer = 1;
    float timer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        pressKeyPrompt = this.transform.GetChild(0).gameObject;

        outline = this.transform.GetChild(1).GetComponent<SpriteRenderer>();

        playerNumText = this.transform.GetChild(2).GetComponent<TextMesh>();

        scoreDisplays = this.transform.GetChild(3).gameObject;
        scoreText = scoreDisplays.transform.GetChild(0).GetComponent<TextMesh>();
        highscoreText = scoreDisplays.transform.GetChild(1).GetComponent<TextMesh>();

        indicatorScript = this.transform.GetChild(4).GetComponent<Indicator_Bar_Script>();
        StopCountdown();
    }

    public void HidePrompt(){
        pressKeyPrompt.SetActive(false);
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
