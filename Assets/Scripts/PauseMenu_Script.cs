using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu_Script : MonoBehaviour
{
    // A reference to the game handler script
    private GameHandler_Script gameHandlerScript;

    // The menu screen game object
    private GameObject pauseScreen;

    private Animator pauseTextAnimator;

    // The menu screen game object
    private GameObject pauseBtn;

    // The var to keep track if the game is running
    bool gameIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        //grab the main menu screen
        pauseScreen = GameObject.Find("PauseScreen");

        //grab the pause title animator
        pauseTextAnimator = GameObject.Find("PauseText").GetComponent<Animator>();

        //grab the pause button
        pauseBtn = GameObject.Find("PauseBtn");

        //hide the pause screen by default


        HidePauseScreen();
        HidePauseBtn();

    }



    public void ShowPauseBtn()//hidden at the start of the game
    {
        pauseBtn.SetActive(true);
        gameIsRunning = true;
    }

    public void HidePauseBtn()//hidden at the start of the game
    {
        pauseBtn.SetActive(false);
    }

    public void ShowPauseScreen()
    {        
        pauseScreen.SetActive(true);
        pauseTextAnimator.SetBool("gamePaused", true);
    }

    public void HidePauseScreen()
    {
        pauseTextAnimator.SetBool("gamePaused", false);
        pauseScreen.SetActive(false);
    }



    public void PauseBtn()
    {
        if(gameIsRunning == true){
            gameIsRunning = false;
            ShowPauseScreen();
            HidePauseBtn();
            gameHandlerScript.Pause(true);
        }
    }

    public void ResumeBtn()
    {
        if(gameIsRunning == false){
            gameIsRunning = true;
            HidePauseScreen();
            ShowPauseBtn();

            gameHandlerScript.Pause(false);
        }
    }

    public void RestartBtn()
    {
        gameHandlerScript.RestartGame();
        ResumeBtn();
    }

    public void ReturnHomeBtn()
    {
        HidePauseBtn();
        HidePauseScreen();

        gameHandlerScript.ReturnHome();   
    }

    // Update is called once per frame
    void Update()
    {   
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && (pauseScreen.activeSelf == false))
        {
            PauseBtn();
        }
        else if ((Input.GetKeyDown(KeyCode.Space)) && (pauseScreen.active == true))
        {
            Debug.Log("resume button clicked");
            ResumeBtn();
        }
        
    }

}
