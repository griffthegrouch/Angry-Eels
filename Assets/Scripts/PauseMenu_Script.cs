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
    private bool gameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        //move screen into position on game load
        transform.localPosition = Vector2.zero;

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
        //HidePauseBtn();
    }

     // Update is called once per frame
    void Update()
    {   
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && (gameHandlerScript.activeScreen == ActiveScreen.Game))
        {   // if gamescreen is active and you press space or click, pause game
            Pause();
        }
        else if ((Input.GetKeyDown(KeyCode.Space)) && (gameHandlerScript.activeScreen == ActiveScreen.PauseMenu))
        {   // if pause screen is active and you press space, unpause game
            Unpause();
        }
    }


    //////////////////////////////////////////////////////// pause menu buttons
    public void ExitGameBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.PauseMenu){
            gameHandlerScript.ExitGame();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void ResumeBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.PauseMenu){
            Unpause();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
     public void RestartBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.PauseMenu){
            Unpause();
            gameHandlerScript.RestartGame();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void ReturnHomeBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.PauseMenu){
            Unpause();
            gameHandlerScript.ReturnHome(); 
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }  
    }
    

    public void Pause()
    {
        gameHandlerScript.activeScreen = ActiveScreen.PauseMenu;
        pauseScreenAudio.Play(0);

        ShowScreen();
        gameHandlerScript.Pause();
    }

    public void Unpause()
    {
        gameHandlerScript.activeScreen = ActiveScreen.Game;
        pauseScreenAudio.Pause();

        HideScreen();
        gameHandlerScript.UnPause();
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


    public void Reset(){        
        pauseScreenAudio.Pause();
        HideScreen();
    }
}
