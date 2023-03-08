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

    // Audio source for pause screen
    private AudioSource pauseScreenAudio;

    // The var to keep track if the game is running
    public bool gameIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        pauseScreenAudio = GetComponent<AudioSource>();

        //grab the main menu screen
        pauseScreen = GameObject.Find("PauseScreen");

        //grab the pause title animator
        pauseTextAnimator = GameObject.Find("PauseText").GetComponent<Animator>();

        //grab the pause button
        pauseBtn = GameObject.Find("PauseBtn");

        //hide the pause screen by default
        HideScreen();
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

    public void ShowScreen()
    {        
        pauseScreen.SetActive(true);
        pauseTextAnimator.SetBool("screenOpen", true);
    }

    public void HideScreen()
    {
        pauseTextAnimator.SetBool("screenOpen", false);
        pauseScreen.SetActive(false);
    }



    public void Pause()
    {
        gameIsRunning = false;

        pauseScreenAudio.Play(0);

        ShowScreen();
        HidePauseBtn();
        gameHandlerScript.Pause(true);
        
    }

    public void Unpause()
    {
        gameIsRunning = true;

        pauseScreenAudio.Pause();

        HideScreen();
        ShowPauseBtn();

        gameHandlerScript.Pause(false);
    }

    public void RestartBtn()
    {
        pauseScreenAudio.Pause();
        HideScreen();
        gameHandlerScript.RestartGame();
        
    }

    public void ReturnHomeBtn()
    {
        pauseScreenAudio.Pause();
        HideScreen();
        gameHandlerScript.EndGame();
        gameHandlerScript.ReturnHome();   
    }

    // Update is called once per frame
    void Update()
    {   
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && (gameIsRunning == true))
        {
            Pause();
        }
        else if ((Input.GetKeyDown(KeyCode.Space)) && (pauseScreen.activeSelf == true))
        {
            Unpause();
        }
        
        
    }

}
