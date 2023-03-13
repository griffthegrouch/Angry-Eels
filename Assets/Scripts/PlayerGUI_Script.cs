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
    //the parent to all gui elements
    private GameObject parent;

    //playernum text
    private Text playerNumText;

    //player colour spriterenderer
    private Image playerColourSprite;


    //score text
    private Text scoreText;
    //highscore text
    private Text highscoreText;

    //press key to begin object
    private GameObject pressKeyPrompt;


    //timer bar object
    private GameObject timerBar;
    //how full the timer is
    private GameObject chargeBar;
    private Image chargeBarSprite;


    //ghost indicator object
    private GameObject ghostIndicator;
    //gold indicator object
    private GameObject goldIndicator;
    //dead indicator object
    private GameObject deadIndicator;

    //vars used by the script
    private GameHandler_Script gameHandlerScript;
    private int playerNum;
    private Color playerColour;
    private float timer;
    private float maxTimerValue;
    private int score;
    private int highscore;

   
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.GetChild(0).gameObject;


        playerColourSprite = parent.transform.GetChild(0).GetChild(0).GetComponent<Image>();
        playerNumText = parent.transform.GetChild(4).GetChild(0).GetComponent<Text>();

        pressKeyPrompt = parent.transform.GetChild(2).gameObject;

        scoreText = parent.transform.GetChild(1).GetChild(1).GetComponent<Text>();
        highscoreText = parent.transform.GetChild(1).GetChild(3).GetComponent<Text>();

        timerBar = parent.transform.GetChild(3).GetChild(0).gameObject;
        chargeBar = timerBar.transform.GetChild(1).gameObject;
        chargeBarSprite  = chargeBar.GetComponent<Image>();

        ghostIndicator = parent.transform.GetChild(3).GetChild(1).gameObject;
        goldIndicator = parent.transform.GetChild(3).GetChild(2).gameObject;
        deadIndicator = parent.transform.GetChild(3).GetChild(3).gameObject;

        HideAllIndicators();
        //hide GUIs to start
        HideGUI();
    }

    
    // Update is called once per frame
    void Update()
    {
                Debug.Log("showing timer" + timer);
        //if timer is bigger than 0
        if (timer > 0){
            //countdown the timer
            timer -= Time.deltaTime;

            if (timer <= 0){//if timer is negative, set it to 0
                timer = 0;
                HideAllIndicators();
            }

            UpdateTimerBar(timer);
        }
    }
    public void ShowGUI(){
        parent.SetActive(true);
    }
    public void HideGUI(){
        parent.SetActive(false);
    }

    public void ShowPrompt(){
        pressKeyPrompt.SetActive(true);
    }
    public void HidePrompt(){
        pressKeyPrompt.SetActive(false);
    }

    public void HideAllIndicators(){
        timerBar.SetActive(false);
        ghostIndicator.SetActive(false);
        goldIndicator.SetActive(false);
        deadIndicator.SetActive(false);
    }

    public void SetValues(GameHandler_Script script, int pNum, Color pColour)
    {
        //grab script
        gameHandlerScript = script;

        //grab initial values
        playerNum = pNum + 1;//playernum starts at 0, this display starts at 1
        playerColour = pColour;

        //display the player numbers
        playerNumText.text = playerNum.ToString();

        //sets the outline colour, used when starting the game to set the colour to individual player colour
        playerColourSprite.color = playerColour;

    }

    public void StartGhostMode(float time){
        StartCountdown(time);
        SetTimerColour(Color.gray);
        ghostIndicator.SetActive(true);
    }
    public void StartGoldMode(float time){
        StartCountdown(time);
        SetTimerColour(Color.yellow);
        goldIndicator.SetActive(true);
    }

    public void StartDeathPenaltyMode(float time){
        StartCountdown(time);
        SetTimerColour(Color.black);
        deadIndicator.SetActive(true);
    }

    public void SetTimerColour(Color col){
        chargeBarSprite.color = col;
    }

    public void StartCountdown(float time){
        
        StopCountdown();
        timer = maxTimerValue = time;

        timerBar.SetActive(true);

    }

    public void StopCountdown(){
        // method tells the display's indicator bar to stop counting down 
        // stops the charge bar and sets it to 0
        timer = 0;
        HideAllIndicators();
        SetTimerColour(Color.green);
    }    

    public void UpdateTimerBar(float value){
        //sets the size of the charge bar inside the timer to show how much time is left
        //max size is 1 (scale.x = 1)
        float percentValue = value / maxTimerValue;

        float xScale =  percentValue * 1;

        chargeBar.transform.localScale = new Vector3(xScale, 1, 0);
    }

    public void UpdateScore(int s)
    {
        score = s;

        //displaying current score
        scoreText.text = score.ToString();

        //checking if current score is higher than highscore
        if (score > highscore)
        {
            highscore = score;
            highscoreText.text = highscore.ToString();
        }

        gameHandlerScript.UpdateScore(playerNum, score);
    }

}
