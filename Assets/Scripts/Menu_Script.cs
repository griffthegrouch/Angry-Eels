using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class Menu_Script : MonoBehaviour
{
    //menu script

    ///////////// gamemode universal vars
    private char gameMode{set;} = 'e'; // e - endless, f - first to
    private int goalPoints{set;} = 100;
    private int num_Players{set;} = 1;
    private int num_Human_Players{set;} = 1;

    //- advanced options screen menu vars
    // when loaded initially, they are set to the first preset gamemode values
    private int player1_Colour{set;}
    private int player2_Colour{set;}
    private int player3_Colour{set;}
    private int player4_Colour{set;}
    private int startingSize{set;}
    private int normalFood_GrowthAmount{set;}
    private int deadSnakeFood_GrowthAmount{set;}
    private int goldFood_GrowthAmount{set;}
    private float snakeSpeed{set;}
    private float ghostMode_Duration{set;}
    private float deathPenalty_Duration{set;}
    private float goldFood_SpawnChance{set;}
    private bool doSnakesTurnToFood{set;}

    private string[] presetsNamesArr = new string[] {
        "Custom",
        "Classic",
        "Wild"
    };
    private float[,] presetsValuesArr = new float[,] {
        {//custom
            0, 1, 2, 3,     //snake colours
            0.1f, 3, 2, 3,  //speed/durations
            1, 10, 3, 1, 30 //food options
        },
        {//classic mode
           0, 1, 2, 3,     //snake colours
            0.1f, 3, 2, 3,  //speed/durations
            1, 10, 3, 1, 30 //food options
        },
        {//wild mode
           0, 1, 2, 3,     //snake colours
            0.1f, 3, 2, 3,  //speed/durations
            1, 10, 3, 1, 30 //food options
        }
    };


    private GameHandler_Script gameHandlerScript;
    private GameObject[] AllAdvancedOptionsArr;


    //menu screen vars
    private Text gameMode_Text;
    private Text target_Text;
    private GameObject firstToSelector;
    private GameObject[] snakeIndicatorArr; //indicators for snakes
    private GameObject[] cpuIndicatorSpriteArr; //part that colours the snake grey to indicate its a computer player

    //advanced options screen vars
    private GameObject advancedOptionsScreen;
    private int selectedPreset = 0; //starts at first selected preset


    // Start is called before the first frame update
    void Start()
    {
        //gathering gameobjects
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        gameMode_Text = GameObject.Find("GameModeText").transform.GetComponent<Text>();
        target_Text = GameObject.Find("PointsText").transform.GetComponent<Text>();
        firstToSelector = GameObject.Find("FirstToPoints");

        snakeIndicatorArr = new GameObject[4];
        cpuIndicatorSpriteArr = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            snakeIndicatorArr[i] = GameObject.Find("SnakeIndicators").transform.GetChild(i).gameObject;
            cpuIndicatorSpriteArr[i] = snakeIndicatorArr[i].transform.GetChild(2).gameObject;
        }

        AllAdvancedOptionsArr = new GameObject[]{
            GameObject.Find("PresetsText"),

            GameObject.Find("P1ColourDropdown"),
            GameObject.Find("P2ColourDropdown"),
            GameObject.Find("P3ColourDropdown"),
            GameObject.Find("P4ColourDropdown"),

            GameObject.Find("InputField1"),
            GameObject.Find("InputField2"),
            GameObject.Find("InputField3"),
            GameObject.Find("InputField4"),

            GameObject.Find("Toggle"),
            GameObject.Find("InputField5"),
            GameObject.Find("InputField6"),
            GameObject.Find("InputField7"),
            GameObject.Find("InputField8"),
        };

        advancedOptionsScreen = GameObject.Find("AdvancedOptionsScreen");

        UpdateMenuScreen();
        UpdateAdvancedOptionsScreen(-1, 0);

        advancedOptionsScreen.SetActive(false);


    }

    void StartGame()
    {
        //when user clicks play, 

        int[] _iarr = {
            goalPoints,
            num_Players,
            num_Human_Players,
            player1_Colour,
            player2_Colour,
            player3_Colour,
            player4_Colour,
            startingSize,
            normalFood_GrowthAmount,
            deadSnakeFood_GrowthAmount,
            goldFood_GrowthAmount
        };

        float[] _farr = {
            snakeSpeed,
            ghostMode_Duration,
            deathPenalty_Duration,
            goldFood_SpawnChance
        };

        //send vars to game handler to use, 
        gameHandlerScript.SetAllVars(//char c, bool b, int[] iArr, float[] fArr
            gameMode,
            doSnakesTurnToFood,
            _iarr,
            _farr
        );

        //setup game
        gameHandlerScript.LoadGame();

        //start game
        gameHandlerScript.StartGame();

        //then close menu
        HideMenu();
    }

    void ShowMenu()
    {
        //not currently used
    }
    void HideMenu()
    {
        this.gameObject.SetActive(false);
        advancedOptionsScreen.SetActive(false);
        //called when game starts to hide menu
    }

    void UpdateMenuScreen()
    {
        //called when a button is pressed or value changes on the main menu screen to reflect changes

        //remove the target points selector if the gamemode is endless
        if (gameMode == 'e')
        {
            firstToSelector.SetActive(false);
        }
        else if (gameMode == 'f')
        {
            firstToSelector.SetActive(true);
        }

        //display the snake indicators correctly
        for (int i = 0; i < 4; i++)
        {
            if (i < num_Players)
            {
                snakeIndicatorArr[i].SetActive(true);
            }
            else
            {
                snakeIndicatorArr[i].SetActive(false);
            }
            if (i < (num_Human_Players))
            {
                cpuIndicatorSpriteArr[i].SetActive(false);
            }
            else
            {
                cpuIndicatorSpriteArr[i].SetActive(true);
            }
        }
    }

    void UpdateAdvancedOptionsScreen(int _optionIndex, float _value)
    {
        //called when a button is pressed or value changes on the advanced options screen to reflect changes
        //_optionIndex is the var to be changed _value is the new value to be used
        // if _optionIndex is 0 (using a preset), update all of them

        switch (_optionIndex)
        {
            case 0://update all using a preset
                AllAdvancedOptionsArr[0].GetComponent<Text>().text = presetsNamesArr[selectedPreset];
                goto case 1;

            case 1://p1 Colour
                player1_Colour = AllAdvancedOptionsArr[1].GetComponent<Dropdown>().value = ((int)presetsValuesArr[selectedPreset, 0]);
                if(_optionIndex != 0){break;}else {goto case 2;}
            case 2://p2 Colour
                player2_Colour = AllAdvancedOptionsArr[2].GetComponent<Dropdown>().value = ((int)presetsValuesArr[selectedPreset, 1]);
                if(_optionIndex != 0){break;}else {goto case 3;}
            case 3://p3 Colour
                player3_Colour = AllAdvancedOptionsArr[3].GetComponent<Dropdown>().value = ((int)presetsValuesArr[selectedPreset, 2]);
                if(_optionIndex != 0){break;}else {goto case 4;}
            case 4://p4 Colour
                player4_Colour = AllAdvancedOptionsArr[4].GetComponent<Dropdown>().value = ((int)presetsValuesArr[selectedPreset, 3]);
                if(_optionIndex != 0){break;}else {goto case 5;}

            case 5://snake speed
                snakeSpeed = presetsValuesArr[selectedPreset, 4];
                AllAdvancedOptionsArr[5].GetComponent<Text>().text = snakeSpeed.ToString();
                if(_optionIndex != 0){break;}else {goto case 6;}

            case 6://snake starting size
                startingSize = ((int)presetsValuesArr[selectedPreset, 5]);
                AllAdvancedOptionsArr[6].GetComponent<Text>().text = startingSize.ToString();
                if(_optionIndex != 0){break;}else {goto case 7;}

            case 7://ghosted duration
                ghostMode_Duration = presetsValuesArr[selectedPreset, 6];
                AllAdvancedOptionsArr[7].GetComponent<Text>().text = ghostMode_Duration.ToString();
                if(_optionIndex != 0){break;}else {goto case 8;}

            case 8://death penalty duration
                deathPenalty_Duration = presetsValuesArr[selectedPreset, 7];
                AllAdvancedOptionsArr[8].GetComponent<Text>().text = deathPenalty_Duration.ToString();
                if(_optionIndex != 0){break;}else {goto case 9;}

            case 9://do snakes turn to food
                doSnakesTurnToFood = AllAdvancedOptionsArr[9].GetComponent<Toggle>().isOn = (1 == presetsValuesArr[selectedPreset, 8]);
                if(_optionIndex != 0){break;}else {goto case 10;}

            case 10://gold food spawn chance
                goldFood_SpawnChance = presetsValuesArr[selectedPreset, 9];
                AllAdvancedOptionsArr[10].GetComponent<Text>().text = goldFood_SpawnChance.ToString();
                if(_optionIndex != 0){break;}else {goto case 11;}

            case 11://normal food growth
                normalFood_GrowthAmount = ((int)presetsValuesArr[selectedPreset, 10]);
                AllAdvancedOptionsArr[11].GetComponent<Text>().text = normalFood_GrowthAmount.ToString();
                if(_optionIndex != 0){break;}else {goto case 12;}

            case 12://dead snake food growth
                deadSnakeFood_GrowthAmount = ((int)presetsValuesArr[selectedPreset, 11]);
                AllAdvancedOptionsArr[12].GetComponent<Text>().text = deadSnakeFood_GrowthAmount.ToString();
                if(_optionIndex != 0){break;}else {goto case 13;}

            case 13://gold food growth
                goldFood_GrowthAmount = ((int)presetsValuesArr[selectedPreset, 12]);
                AllAdvancedOptionsArr[13].GetComponent<Text>().text = goldFood_GrowthAmount.ToString();
                break;

            default:
                break;
        }
    }




    ////////////////////////////////////////////// main menu screen buttons
    public void StartBtn()
    {
        StartGame();
    }

    public void GameModeLeftBtn()
    {
        if (gameMode == 'e')
        {
            gameMode = 'f';
            gameMode_Text.text = "First To:";

        }
        else
        {
            gameMode = 'e';
            gameMode_Text.text = "Endless";
        }
        UpdateMenuScreen();
    }
    public void GameModeRightBtn()
    {
        //simple because theres only one option to toggle through
        GameModeLeftBtn();
    }

    public void PointsLeftBtn()
    {
        if (goalPoints >= 100)
        {
            goalPoints -= 50;
        }
        target_Text.text = goalPoints.ToString();
        UpdateMenuScreen();
    }

    public void PointsRightBtn()
    {
        goalPoints += 50;
        target_Text.text = goalPoints.ToString();
        UpdateMenuScreen();
    }

    public void Num_PlayersLeftBtn()
    {
        if (num_Players >= 2)
        {
            num_Players -= 1;
        }
        UpdateMenuScreen();
    }
    public void Num_PlayersRightBtn()
    {
        if (num_Players <= 3)
        {
            num_Players += 1;
        }
        UpdateMenuScreen();
    }
    public void HumanPlayersLeftBtn()
    {
        if (num_Human_Players >= 2)
        {
            num_Human_Players -= 1;
        }
        UpdateMenuScreen();
    }
    public void HumanPlayersRightBtn()
    {
        if (num_Human_Players <= 3)
        {
            num_Human_Players += 1;
        }
        UpdateMenuScreen();
    }

    public void AdvancedOptionsOpenBtn()
    {
        //open advanced options menu
        advancedOptionsScreen.SetActive(true);
    }




    //////////////////////////////////////////////// advanced options screen buttons
    public void AdvancedOptionsCloseBtn()
    {
        //open advanced options menu
        advancedOptionsScreen.SetActive(false);
    }

    public void PresetsLeftBtn()
    {
        selectedPreset -= 1;
        if (selectedPreset < 0)
        {
            selectedPreset = presetsNamesArr.Length - 1;
        }
        UpdateAdvancedOptionsScreen(0, 0);
    }
    public void PresetsRightBtn()
    {
        selectedPreset += 1;
        if (selectedPreset > presetsNamesArr.Length - 1)
        {
            selectedPreset = 0;
        }
        UpdateAdvancedOptionsScreen(0, 0);
    }

    public void Player1ColourValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(1, change.value);
    }
    public void Player2ColourValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(2, change.value);
    }
    public void Player3ColourValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(3, change.value);
    }
    public void Player4ColourValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(4, change.value);
    }
    public void SnakeSpeedValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(5, (float)change);
    }
    public void StartingSizeValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(6, change.value);
    }
    public void GhostedDurationValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(7, change.value);
    }
    public void DeathPenaltyValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(8, change.value);
    }
    public void DoSnakesTurnIntoFoodValueChanged(Toggle toggle)
    {
        float f = 0;
        if (toggle.isOn) { f = 1; }
        UpdateAdvancedOptionsScreen(9, f);
    }
    public void GoldFoodSpawnChanceValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(10, change.value);
    }
    public void NormalFoodGrowthAmountValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(11, change.value);
    }
    public void DeadSnakeFoodGrowthAmountValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(12, change.value);
    }
    public void GoldFoodGrowthAmountValueChanged(Dropdown change)
    {
        UpdateAdvancedOptionsScreen(13, change.value);
    }

}
