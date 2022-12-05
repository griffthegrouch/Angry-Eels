using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class Menu_Script : MonoBehaviour
{
    //menu script
    bool isOptionsScreenOpen = false;

    char GameMode = 'e'; // e - endless, f - first to
    int GoalPoints = 100;
    int NumPlayers = 1;
    int NumHumans = 1;

    private char GameMode; // e - endless, f - first to
    private int GoalPoints;
    private int Num_Players;
    private int Num_Human_Players;
    //- advanced options screen menu vars
    // when loaded initially, they are set to the first preset gamemode values
    private int Player1_Colour;
    private int Player2_Colour;
    private int Player3_Colour;
    private int Player4_Colour;
    private int StartingSize;
    private int NormalFood_GrowthAmount;
    private int DeadSnakeFood_GrowthAmount;
    private int GoldFood_GrowthAmount;
    private float SnakeSpeed;
    private float GhostMode_Duration;
    private float DeathPenalty_Duration;
    private float GoldFood_SpawnChance;
    private bool DoSnakesTurnToFood;
    float[] PresetsArr = new float[] {
        {   0, 1, 2, 3,     //snake colours
            0.1f, 3, 2, 3,  //speed/durations
            1, 10, 3, 1, 30 //food options
        }
    };


    Text GameMode_Text;
    Text Target_Text;

    GameObject FirstToSelector;
    GameObject[] SnakeIndicatorArr; //indicators for snakes
    GameObject[] CPUIndicatorSpriteArr; //part that colours the snake grey to indicate its a computer player


    // Start is called before the first frame update
    void Start()
    {        
        //gathering gameobjects
        GameMode_Text = GameObject.Find("GameModeText").transform.GetComponent<Text>();
        Target_Text = GameObject.Find("PointsText").transform.GetComponent<Text>();
        FirstToSelector = GameObject.Find("FirstToPoints");
        

        SnakeIndicatorArr = new GameObject[4];
        CPUIndicatorSpriteArr = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            SnakeIndicatorArr[i] = GameObject.Find("SnakeIndicators").transform.GetChild(i).gameObject;
            CPUIndicatorSpriteArr[i] = SnakeIndicatorArr[i].transform.GetChild(2).gameObject;
        }
        UpdateMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void UpdateMenu(){

        //display the snake indicators correctly

        if (GameMode == 'e'){
            FirstToSelector.SetActive(false);
        }
        else if (GameMode == 'f'){
            FirstToSelector.SetActive(true);
        }
        
        for (int i = 0; i < 4; i++)
        {
            if(i < NumPlayers){
                SnakeIndicatorArr[i].SetActive(true);
            } else {
                SnakeIndicatorArr[i].SetActive(false);
            }
            if(i < ( NumHumans)){
                CPUIndicatorSpriteArr[i].SetActive(false);
            }
            else {
                CPUIndicatorSpriteArr[i].SetActive(true);
            }
        }
    }

    public void StartBtn(){
        UpdateMenu();
    }


    public void GameModeLeftBtn(){
        if (GameMode == 'e'){
            GameMode = 'f';
            GameMode_Text.text = "First To:"; 
            
        }
        else{
            GameMode = 'e';
            GameMode_Text.text = "Endless"; 
        }
        UpdateMenu();
    }
    public void GameModeRightBtn(){
        GameModeLeftBtn();
    }


    public void PointsLeftBtn(){
        if (GoalPoints >= 100){
            GoalPoints -= 50;
        }
        Target_Text.text = GoalPoints.ToString();
        UpdateMenu();
    }
    public void PointsRightBtn(){
        GoalPoints += 50;
        Target_Text.text = GoalPoints.ToString();
        UpdateMenu();
    }


    public void NumPlayersLeftBtn(){
        if (NumPlayers >= 2){
            NumPlayers -= 1;
        }
        UpdateMenu();   
    }
    public void NumPlayersRightBtn(){
        if (NumPlayers <= 3){
            NumPlayers += 1;
        }
        UpdateMenu();
    }

    public void HumanPlayersLeftBtn(){
        if (NumHumans >= 2){
            NumHumans -= 1;
        }
        UpdateMenu();
    }
    public void HumanPlayersRightBtn(){
        if (NumHumans <= 3){
            NumHumans += 1;
        }
        UpdateMenu();
        UpdateMenu();
    }

    public void AdvancedOptionsOpenBtn(){
        //open advanced options menu
        isOptionsScreenOpen = true;
    }
    public void AdvancedOptionsCloseBtn(){
        //open advanced options menu
        isOptionsScreenOpen = false;
    }
}
