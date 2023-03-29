// Imports necessary for this script
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
This class is used to control the display for each individual player.
It displays the player's number, color, score, highscore, and a timer.
It also has indicators to show the player's status (alive, ghosted, golden, or death penalty).
*/
public class PlayerGUI_Script : MonoBehaviour
{
    // Variables to hold references to the GUI elements
    public GameObject parent; // The parent to all GUI elements
    public Text playerNumText; // Text that shows the player's number
    public Image playerColourSprite; // Image that shows the player's color
    public Text scoreText; // Text that shows the player's current score
    public Text highscoreText; // Text that shows the player's highscore
    public GameObject pressKeyPrompt; // Object that prompts the player to press any key to begin
    public GameObject timerBar; // Object that shows the timer bar
    public GameObject chargeBar; // Object that shows how full the timer is
    public Image chargeBarSprite; // Image component of the charge bar object
    public GameObject ghostIndicator; // Object that shows the ghost indicator
    public GameObject goldIndicator; // Object that shows the gold indicator
    public GameObject deadIndicator; // Object that shows the death penalty indicator

    // Variables used by the script
    public GameHandler_Script gameHandlerScript; // Reference to the GameHandler_Script
    private int playerNum; // The player's number
    private Color playerColour; // The player's color
    private float timer; // The timer value
    private float maxTimerValue; // The maximum timer value
    private int score; // The player's current score
    private int highscore; // The player's highscore

    public bool demoMode { get; set; } = false; //is the gui being run in demo mode

    // Start is called before the first frame update
    void Start()
    {
        // Hide all indicators
        HideAllIndicators();
        // Hide GUI elements
        HideGUI();
    }

    // Reset the player GUI to its initial state
    public void Reset()
    {
        // Stop the countdown and hide all indicators
        StopCountdown();
        HideAllIndicators();
        // Update the score to 0 and show the prompt
        UpdateScore(0);
        ShowPrompt();
        // Hide GUI elements
        HideGUI();
    }

    // Update is called once per frame
    void Update()
    {
        // If timer is bigger than 0, countdown the timer
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                // If timer is negative, set it to
                // 0 and hide all indicators
                timer = 0;
                HideAllIndicators();
            }

            // Update the timer bar to show how much time is left
            UpdateTimerBar(timer);
        }
    }

    // Show the player GUI
    public void ShowGUI()
    {
        parent.SetActive(true);
    }

    // Hide the player GUI
    public void HideGUI()
    {
        parent.SetActive(false);
    }

    // Show the prompt for the player to press any key to begin
    public void ShowPrompt()
    {
        pressKeyPrompt.SetActive(true);
    }

    // Hide the prompt for the player to press any key to begin
    public void HidePrompt()
    {
        pressKeyPrompt.SetActive(false);
    }

    // Hide all indicators
    public void HideAllIndicators()
    {
        timerBar.SetActive(false);
        ghostIndicator.SetActive(false);
        goldIndicator.SetActive(false);
        deadIndicator.SetActive(false);
    }

    // Set the initial values for the player GUI
    public void SetValues(GameHandler_Script script, int pNum, Color pColour)
    {
        // Set the reference to the GameHandler_Script
        gameHandlerScript = script;
        // Set the player's number and color
        playerNum = pNum + 1; // Player number starts at 0, this display starts at 1
        playerColour = pColour;
        // Set the player's number in the GUI
        playerNumText.text = playerNum.ToString();
        // Set the player's color in the GUI
        playerColourSprite.color = playerColour;
    }

    // Start the ghost mode for the player with the given duration
    public void StartGhostMode(float time)
    {
        // Start the countdown and set the timer bar color to gray
        StartCountdown(time);
        SetTimerColour(Color.gray);
        // Show the ghost indicator
        ghostIndicator.SetActive(true);
    }

    // Start the gold mode for the player with the given duration
    public void StartGoldMode(float time)
    {
        // Start the countdown and set the timer bar color to yellow
        StartCountdown(time);
        SetTimerColour(Color.yellow);
        // Show the gold indicator
        goldIndicator.SetActive(true);
    }

    // Start the death penalty mode for the player with the given duration
    public void StartDeathPenaltyMode(float time)
    {
        // Start the countdown and set the timer bar color to black
        StartCountdown(time);
        SetTimerColour(Color.black);
        // Show the death penalty indicator
        deadIndicator.SetActive(true);
    }

    // Set the color of the timer bar
    public void SetTimerColour(Color col)
    {
        chargeBarSprite.color = col;
    }

    // Start the countdown for the timer with the given duration
    public void StartCountdown(float time)
    {
        // Stop the countdown, set the timer value and maximum timer value, and show the timer bar
        StopCountdown();
        timer = maxTimerValue = time;
        timerBar.SetActive(true);
    }

    // Stop the countdown for the timer and hide all indicators
    public void StopCountdown()
    {
        timer = 0;
        HideAllIndicators();
        // Set the timer bar color to green
        SetTimerColour(Color.green);
    }

    // Update the player's score and highscore in the GUI and in the GameHandler_Script
    public void UpdateScore(int s)
    {
        score = s;
        // Update the current score in the GUI
        scoreText.text = score.ToString();
        // If the current score is higher than the highscore, update the highscore in the GUI
        if (score > highscore)
        {
            highscore = score;
            highscoreText.text = highscore.ToString();
        }

        //if the GUI is being used in non-demo mode, update the gamehandler with score
        if (!demoMode)
        {
            // Update the score in the GameHandler_Script
            gameHandlerScript.UpdateScore(playerNum, score);
        }
    }

    // Update the size of the charge bar in the timer to show how much time is left
    public void UpdateTimerBar(float value)
    {
        // Calculate the percentage of time remaining
        float percentValue = value / maxTimerValue;
        // Calculate the x scale of the charge bar
        float xScale = percentValue * 1; // 1 is the maximum size of the charge bar

        // Set the new scale of the charge bar
        chargeBar.transform.localScale = new Vector3(xScale, 1, 0);
    }

}
