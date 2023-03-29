using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen_Script : MonoBehaviour
{// A reference to the game handler script
    public GameHandler_Script gameHandlerScript;
    
    // The win screen game object
    public GameObject winScreen;

    //the animator for the "winner" title
    public Animator winnerTextAnimator;

    //the save btn
    public GameObject saveBtn;

    //the text for the title
    public Text titleText;

    //the text for the message displaying which player won and how many points
    public Text winnerNameText;

    //the entering player name
    public InputField nameInput;

    public HighScoreManager_Script highScoreManagerScript;


    private int score;

    // Start is called before the first frame update
    void Start()
    {
        //move screen into position on game load
        transform.localPosition = Vector2.zero;

        //hide the pause screen by default
        Close();
    }

    public void OpenGameWon(int playerNum, int _score){
        titleText.text = "Winner!";
        score = _score;
        winnerNameText.text =  "Player " + playerNum + " with " + score + " points";
        Open();
    }

    public void OpenSaveScore(int playerNum, int _score){
        bool b = highScoreManagerScript.IsThisAHighscore(gameHandlerScript.options.ruleSet, _score);
        titleText.text = b ? "New Highscore!" : "Not very impressive...";
        score = _score;
        winnerNameText.text =  "Player " + playerNum + " with " + score + " points";
        Open();
    }



    public void Open()
    {        
        winScreen.SetActive(true);
        winnerTextAnimator.SetBool("screenOpen", true);
        LockSaveBtn();
    }

    public void Close()
    {
        winnerTextAnimator.SetBool("screenOpen", false);
        winScreen.SetActive(false);
        LockSaveBtn();
        
    }

    public void LockSaveBtn(){
       nameInput.text = "";
       saveBtn.SetActive(false);
    }
    public void UnlockSaveBtn(){
       saveBtn.SetActive(true);
    }
    public void SaveBtn(){
        if(gameHandlerScript.activeScreen != ActiveScreen.WinMenu){
            Debug.Log("trying to press a button thats currently inactive");
            return;
        } 
        highScoreManagerScript.SaveHighScore(nameInput.text, score, gameHandlerScript.options.ruleSet);
        Close();
        highScoreManagerScript.Open();
    }

    public void RestartBtn()
    {
        if(gameHandlerScript.activeScreen != ActiveScreen.WinMenu){
            Debug.Log("trying to press a button thats currently inactive");
            return;
        } 
        Close();
        gameHandlerScript.RestartGame();
    }

    public void ReturnHomeBtn()
    {
        if(gameHandlerScript.activeScreen != ActiveScreen.WinMenu){
            Debug.Log("trying to press a button thats currently inactive");
            return;
        } 
        Close();
        gameHandlerScript.OpenMenuScreen(ActiveScreen.MainMenu);   
    }

    public void ExitGameBtn()
    {
        if(gameHandlerScript.activeScreen != ActiveScreen.WinMenu){
            Debug.Log("trying to press a button thats currently inactive");
            return;
        } 
        gameHandlerScript.ExitGame();
    }

    // Update is called once per frame
    void Update()
    {   

    
        
    }

}
