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

    // The pause prompt GameObject
    private GameObject pausePrompt;

    // Audio source for pause screen
    private AudioSource pauseScreenAudio;


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
        pausePrompt = GameObject.Find("PausePrompt");

        //hide the pause screen by default
        Close();
        HidePrompt();
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
        if (gameHandlerScript.activeScreen != ActiveScreen.PauseMenu)
        {
            Debug.Log("trying to press a button thats currently inactive");
            return;
        }
        gameHandlerScript.ExitGame();

    }
    public void ResumeBtn()
    {
        if (gameHandlerScript.activeScreen != ActiveScreen.PauseMenu)
        {
            Debug.Log("trying to press a button thats currently inactive");
            return;
        }
        Unpause();

    }
    public void RestartBtn()
    {
        if (gameHandlerScript.activeScreen != ActiveScreen.PauseMenu)
        {
            Debug.Log("trying to press a button thats currently inactive");
            return;
        }
        Unpause();
        gameHandlerScript.RestartGame();

    }
    public void ReturnHomeBtn()
    {
        if (gameHandlerScript.activeScreen != ActiveScreen.PauseMenu)
        {
            Debug.Log("trying to press a button thats currently inactive");
            return;
        }
        Unpause();
        HidePrompt();
        gameHandlerScript.LeaveGameReturnHome();

    }
    public void EndGameAndSaveScore()
    {
        if (gameHandlerScript.activeScreen != ActiveScreen.PauseMenu)
        {
            Debug.Log("trying to press a button thats currently inactive");
            return;
        }
        //gameHandlerScript.
    }







    public void Pause()
    {
        gameHandlerScript.activeScreen = ActiveScreen.PauseMenu;
        pauseScreenAudio.Play(0);

        HidePrompt();

        Open();
        gameHandlerScript.Pause();
    }

    public void Unpause()
    {
        gameHandlerScript.activeScreen = ActiveScreen.Game;
        pauseScreenAudio.Pause();

        ShowPrompt();

        Close();
        gameHandlerScript.UnPause();
    }


    public void Open()
    {
        pauseScreen.SetActive(true);
        pauseTextAnimator.SetBool("screenOpen", true);
    }

    public void Close()
    {
        pauseTextAnimator.SetBool("screenOpen", false);
        pauseScreen.SetActive(false);
    }
    public void ShowPrompt()
    {
        pausePrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        pausePrompt.SetActive(false);
    }


    public void Reset()
    {
        pauseScreenAudio.Pause();
        Close();
    }
}
