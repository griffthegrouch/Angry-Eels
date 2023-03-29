using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu_Script : MonoBehaviour
{
    // A reference to the game handler script
    public GameHandler_Script gameHandlerScript;

    // The menu screen game object
    public GameObject pauseScreen;
    public Animator pauseTextAnimator;
    // The pause prompt GameObject
    public GameObject pausePrompt;

    public Text scoreDisplayText;

    // Audio source for pause screen
    public AudioSource pauseScreenAudio;


    // Start is called before the first frame update
    void Start()
    {
        //move screen into position on game load
        transform.localPosition = Vector2.zero;

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
        Close();
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
        Close();
        HidePrompt();
        gameHandlerScript.OpenSaveScoreScreen();
    }

    public void DisplayScore(){
        int leader = gameHandlerScript.currentLeader;
        int leaderScore = gameHandlerScript.currentHighscore;

        string scoreText = "Player " + leader + " is in the lead with " + leaderScore + " points!";
        scoreDisplayText.text = scoreText;
    }



    public void Pause()
    {
        gameHandlerScript.activeScreen = ActiveScreen.PauseMenu;
        pauseScreenAudio.Play(0);

        HidePrompt();

        //display score
        DisplayScore();

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
        Time.timeScale = 1;
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
