using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen_Script : MonoBehaviour
{// A reference to the game handler script
    private GameHandler_Script gameHandlerScript;
    
    // The win screen game object
    private GameObject winScreen;

    //the animator for the "winner" title
    private Animator winnerTextAnimator;

    //the text for the title
    private Text titleText;

    //the text for the message displaying which player won and how many points
    private Text winnerNameText;

    //the entering player name
    private InputField nameInput;

    private HighScoreManager_Script highScoreManagerScript;


    int score;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        // Get a reference to the highscore manager
        highScoreManagerScript = GameObject.Find("HighScoreMenu").GetComponent<HighScoreManager_Script>();

        //grab the main menu screen
        winScreen = GameObject.Find("WinScreen");

        //grab the title animator
        winnerTextAnimator = GameObject.Find("WinnerTitleText").GetComponent<Animator>();

        //grab the screen title text
        titleText = GameObject.Find("WinnerTitleText").GetComponent<Text>();

        //grab the winner name text
        winnerNameText = GameObject.Find("WinnerName").GetComponentInChildren<Text>();

        nameInput = GameObject.Find("NameEntry").GetComponent<InputField>();

        //hide the pause screen by default
        HideScreen();
    }

    public void GameWon(int playerNum, int _score){
        score = _score;
        winnerNameText.text =  "Player " + playerNum + " with " + score + " points";
        ShowScreen();
    }
    public void ShowScreen()
    {        
        winScreen.SetActive(true);
        winnerTextAnimator.SetBool("screenOpen", true);
    }

    public void HideScreen()
    {
        winnerTextAnimator.SetBool("screenOpen", false);
        winScreen.SetActive(false);
    }

    public void SaveBtn(){
        highScoreManagerScript.SaveHighScore(nameInput.text, score, gameHandlerScript.options.RuleSet);
        highScoreManagerScript.ShowScreen();
    }

    public void RestartBtn()
    {
        HideScreen();
        gameHandlerScript.RestartGame();
    }

    public void ReturnHomeBtn()
    {
        HideScreen();
        gameHandlerScript.ReturnHome();   
    }

    public void CloseGameBtn()
    {
        gameHandlerScript.CloseGame();
    }

    // Update is called once per frame
    void Update()
    {   

    
        
    }

}
