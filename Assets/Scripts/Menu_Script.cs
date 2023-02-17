using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class Menu_Script : MonoBehaviour
{
    // The currently selected game mode - defaulted to endless
    private GameMode gameMode = GameMode.Endless;

    // The goal points for the "First to" game mode - defaulted to 100
    private int goalPoints = 100;

    // The number of players in the game - defaulted to 1
    private int numPlayers = 1;

    // The number of human players in the game - defaulted to 1
    private int numHumanPlayers = 1;

    
    // The values for all options
    private Options options = new Options();

    // The names and values for the preset options
    private Dictionary<string, Options> presetOptions = new Dictionary<string, Options>
    {
            //f snakeSpeed, f ghostModeDuration, f deathPenaltyDuration,
            //i startingSize, i normalFoodGrowthAmount, i deadSnakeFoodGrowthAmount, 
            //i goldFoodGrowthAmount, f goldFoodSpawnChance, b doSnakesTurnToFood
        { "Custom", new Options() },
        { "Classic", new Options(
            0.12f, 2, 0, 
            3, 1, 0, 
            10, 20, false
        ) },
        { "Fun", new Options(
            0.1f, 5, 1.5f, 
            5, 3, 1, 
            30, 25, true
        ) },
        { "Wild", new Options(
            0.05f, 3, 0, 
            15, 10, 1, 
            50, 10, true
        ) }
    };

    public Dictionary<string, Color> SnakeColoursDictionary = new Dictionary<string, Color>
    {
        { "Green", Color.Lerp(Color.green, Color.black, .2f)},
        { "Red", Color.red},
        { "Blue", Color.blue},
        { "Orange", Color.Lerp(Color.yellow, Color.black, .2f) + Color.red},
        { "Gray", Color.gray},
        { "Purple", Color.red + Color.blue},
        //{ "Brown", Color.red + Color.green},
        { "Teal", Color.Lerp(Color.green, Color.black, .2f) + Color.blue},
        { "Pink", Color.magenta}
    };

    // A reference to the game handler script
    private GameHandler_Script gameHandlerScript;

    // The menu screen game object
    private GameObject menuScreen;
    // The advanced options screen game object
    private GameObject advancedOptionsScreen;


    //"First to" points selector game object
    private GameObject firstToSelector;
    //"First to" points selector text
    private Text firstToText;

    // The snake indicator game objects
    private GameObject[] snakeIndicators;

    // The sprite renderers for the CPU indicator sprites
    private GameObject[] cpuIndicatorSprites;

    //all the gameobjects (buttons and text displays) on the menu screens
    Dictionary<string, GameObject> MenuScreenObjects;
    



    // The currently selected preset index
    private int selectedPreset;

    //the currently selected snake colours (defaults to 1,2,3,4)
    private int[] playerColoursInts = new int[]{0,1,2,3};


    // Start is called before the first frame update
    void Start()
    {

        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        //grab the main menu screen
        menuScreen = GameObject.Find("Menu");

        //grab the second menu screen
        advancedOptionsScreen = GameObject.Find("AdvancedOptionsScreen");


        //grab all the menu screen's changing objects 
        MenuScreenObjects = new Dictionary<string, GameObject>
        {
            //menu options
            { "GameModeText",  GameObject.Find("GameModeText")},
            { "PointsText",  GameObject.Find("PointsText")},
            { "SnakeIndicators",  GameObject.Find("SnakeIndicators")},

            //advanced options
            { "PresetsText",  GameObject.Find("PresetsText")},
            { "P1ColourDropdown",  GameObject.Find("P1ColourDropdown")},
            { "P2ColourDropdown",  GameObject.Find("P2ColourDropdown")},
            { "P3ColourDropdown",  GameObject.Find("P3ColourDropdown")},
            { "P4ColourDropdown",  GameObject.Find("P4ColourDropdown")},
            { "SnakeSpeed",  GameObject.Find("InputField1")},
            { "StartingSize",  GameObject.Find("InputField2")},
            { "GhostModeDuration",  GameObject.Find("InputField3")},
            { "DeathPenaltyDuration",  GameObject.Find("InputField4")},
            { "GoldFoodSpawnChance",  GameObject.Find("InputField5")},
            { "NormalFoodGrowthAmount",  GameObject.Find("InputField6")},
            { "DeadSnakeFoodGrowthAmount",  GameObject.Find("InputField7")},
            { "GoldFoodGrowthAmount",  GameObject.Find("InputField8")},
            { "DoSnakesTurnIntoFood",  GameObject.Find("Toggle1")},
        };

        //reset, then set the colour selector dropdowns to contain all the script-definied snake colour values
        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        
        List<string> colourNames = new List<string>(SnakeColoursDictionary.Keys);
        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);

        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[0];
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[1];
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[2];
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[3];

        // Get the snake indicator game objects + CPU indicator sprites
        snakeIndicators = new GameObject[4];
        cpuIndicatorSprites = new GameObject[4];

        //get the first to points selector
        firstToSelector = GameObject.Find("FirstToPoints");
        //get the first to points text
        firstToText = firstToSelector.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        for (int i = 0; i < 4; i++)
        {
            snakeIndicators[i] = GameObject.Find("SnakeIndicators").transform.GetChild(i).gameObject;
            cpuIndicatorSprites[i] = snakeIndicators[i].transform.GetChild(2).gameObject;
        }

        //setup the preset selector and display the default values
        selectedPreset = 1;
        UseAdvancedOptionsFromPreset();

        //then update player indicators to reflect the default values
        UpdatePlayerIndicators();

        //then update gamemode indicator to reflect the default selection
        UpdateGameMode();

        //then hide the advanced options screen by default
        CloseAdvancedOptions();
    }

        // Check for input to start the game
    public void StartGame()
    {
        // Start the game using the current game mode, goal points, number of players, and advanced options
        PlayerType[] playerTypes = new PlayerType[numPlayers];
        Color[] playerColours = new Color[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            if(numHumanPlayers > i){
                playerTypes[i] = PlayerType.Human;
            }
            else playerTypes[i] = PlayerType.Computer;

            playerColours[i] = SnakeColoursDictionary.ElementAt(playerColoursInts[i]).Value;
        }

        //setting all the rest of the values for the current options
        options.gameMode = this.gameMode;
        options.goalPoints = this.goalPoints;
        options.numPlayers = this.numPlayers;
        options.numHumanPlayers = this.numHumanPlayers; 
        options.playerTypes = playerTypes;
        options.playerColours = playerColours;
        gameHandlerScript.InitializeGame(options);

        menuScreen.SetActive(false);
    }






    // Check for input to switch game modes
    public void SwitchGameMode()
    {
        // Toggle the game mode between "Endless" and "First to"
        if (gameMode == GameMode.Endless)
        {
            gameMode = GameMode.FirstTo;
        }
        else
        {
            gameMode = GameMode.Endless;
        }
        // Update the game mode text and the visibility of the "First to" points selector
        UpdateGameMode();

    }
    // Check for input to increase or decrease the goal points
    public void IncreaseGoalPoints()
    {
        goalPoints += 10;
        firstToText.text = goalPoints.ToString();
    }
    public void DecreaseGoalPoints()
    {
        if (goalPoints > 20)
        {
            goalPoints -= 10;
        }
        firstToText.text = goalPoints.ToString();
    }

    // Check for input to increase or decrease the number of players
    public void AddPlayer()
    {
        // Increase the number of players, but not above the maximum of 4
        numPlayers = Mathf.Min(numPlayers + 1, 4);
        UpdatePlayerIndicators();
    }
    public void RemovePlayer()
    {
        // Decrease the number of players, but not below the minimum of 1
        numPlayers = Mathf.Max(numPlayers - 1, 1);
        UpdatePlayerIndicators();
    }
    public void AddHumanPlayer()
    {
        // Increase the number of human players, but not above the maximum of 4
        numHumanPlayers = Mathf.Min(numHumanPlayers + 1, 4);
        UpdatePlayerIndicators();
    }
    public void RemoveHumanPlayer()
    {
        // Decrease the number of human players, but not below the minimum of 1
        numHumanPlayers = Mathf.Max(numHumanPlayers - 1, 0);
        UpdatePlayerIndicators();
    }

    // Check for input to open or close the advanced options screen
    public void OpenAdvancedOptions()
    {
        // Open the advanced options screen
        advancedOptionsScreen.SetActive(true);
    }
    public void CloseAdvancedOptions()
    {
        GetAdvancedOptionsFromScreen();
        // Close the advanced options screen
        advancedOptionsScreen.SetActive(false);
    }

    // Check for input to switch between presets on the advanced options screen
    public void PreviousPreset()
    {
        // Decrease the selected preset index, wrapping around to the last preset if necessary
        if(selectedPreset==0){//if changing from custom options preset, save options first
            GetAdvancedOptionsFromScreen();
        }

        selectedPreset--;

        if (selectedPreset < 0)
        {
            selectedPreset = presetOptions.Count - 1;
        }
        DisplayPreset(selectedPreset);
        UpdateAdvancedOptionsLock();
    }
    public void NextPreset()
    {
        // Increase the selected preset index, wrapping around to the first preset if necessary
        if(selectedPreset==0){
            GetAdvancedOptionsFromScreen();
        }

        selectedPreset++;

        if (selectedPreset >= presetOptions.Count)
        {
            selectedPreset = 0;            
        }

        DisplayPreset(selectedPreset);
        UpdateAdvancedOptionsLock();
    }
    public void UsePreset()
    {
        //when use preset is clicked - 
        //use the currently selected preset
        if(selectedPreset==0){
            GetAdvancedOptionsFromScreen();
        }
        UseAdvancedOptionsFromPreset();
    }

    public void UpdateAdvancedOptionsLock(){
        //if preset shown is not custom, set menu components to not interactable

        bool b = false;
        if(selectedPreset == 0){
            //if not displaying a preset, lock controls
            b = true;
        }
        //used to lock menu controls when displaying a preset
        MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["StartingSize"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["DeadSnakeFoodGrowthAmount"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().interactable = b;
    }



    // Update the game mode text
    private void UpdateGameMode()
    {
        // Get the game mode text game object
        Text gameModeText = GameObject.Find("GameModeText").GetComponent<Text>();
        switch (gameMode)
        {
            case GameMode.Endless:
                // Set the text to the current game mode
                gameModeText.text = "Endless";
                // Show or hide the selector based on the current game mode
                firstToSelector.SetActive(false);
                break;
            case GameMode.FirstTo:
                // Set the text to the current game mode
                gameModeText.text = "First to";
                // Show or hide the selector based on the current game mode
                firstToSelector.SetActive(true);
                break;
        }
    }

    // Update the player indicator game objects
    private void UpdatePlayerIndicators()
    {
        // Show or hide the player indicator game objects based on the current number of players
        for (int i = 0; i < 4; i++)
        {
            if (i < numPlayers) snakeIndicators[i].SetActive(true);
            else snakeIndicators[i].SetActive(false);

            if (i < numHumanPlayers) cpuIndicatorSprites[i].SetActive(false);
            else cpuIndicatorSprites[i].SetActive(true);
        }
    }

    private void UseAdvancedOptionsFromPreset()
    {   //updates the script's options variables from the selected preset

        // Get the advanced options for the preset
        Options _options = presetOptions.ElementAt(selectedPreset).Value;

        // Set the values of the input fields for speed and duration values
        options.snakeSpeed = _options.snakeSpeed;
        options.startingSize = _options.startingSize;
        options.ghostModeDuration = _options.ghostModeDuration;
        options.deathPenaltyDuration = _options.deathPenaltyDuration;
        options.normalFoodGrowthAmount = _options.normalFoodGrowthAmount;
        options.deadSnakeFoodGrowthAmount = _options.deadSnakeFoodGrowthAmount;
        options.goldFoodGrowthAmount = _options.goldFoodGrowthAmount;
        options.goldFoodSpawnChance = _options.goldFoodSpawnChance;

        // Set the value of the toggle for the "do snakes turn to food" option
        options.doSnakesTurnToFood = _options.doSnakesTurnToFood;

        //save the selected preset options to override the custom preset
        presetOptions["Custom"]= _options;

        //set the selected preset back to custom - 
        selectedPreset = 0;
        UpdateAdvancedOptionsLock();

        //reflect changes on screen
        DisplayPreset(selectedPreset);
    }

    private void DisplayPreset(int i)
    {   //updates the menu screen from the script's selected preset option
        Options _options = presetOptions.ElementAt(i).Value;

        //display the title of the preset
        MenuScreenObjects["PresetsText"].GetComponent<Text>().text = presetOptions.ElementAt(i).Key;

        // Set the values of the input fields for speed and duration values
        MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().text = _options.snakeSpeed.ToString();
        MenuScreenObjects["StartingSize"].GetComponent<InputField>().text = _options.startingSize.ToString();
        MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().text = _options.ghostModeDuration.ToString();
        MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().text = _options.deathPenaltyDuration.ToString();
        MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().text = _options.normalFoodGrowthAmount.ToString();
        MenuScreenObjects["DeadSnakeFoodGrowthAmount"].GetComponent<InputField>().text = _options.deadSnakeFoodGrowthAmount.ToString();
        MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().text = _options.goldFoodGrowthAmount.ToString();
        MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().text = _options.goldFoodSpawnChance.ToString();

        // Set the value of the toggle for the "do snakes turn to food" option
        MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().isOn = _options.doSnakesTurnToFood;

    }

    public void GetAdvancedOptionsFromScreen()
    {   //updates the script's options variables from the menu screen

        // Set the values of the dropdown menus for player colours
        playerColoursInts[0] = MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[1] = MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[2] = MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[3] = MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().value;

        // Set the values of the input fields for speed and duration values
        options.snakeSpeed = float.Parse(MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().text);
        options.startingSize = int.Parse(MenuScreenObjects["StartingSize"].GetComponent<InputField>().text);
        options.ghostModeDuration = float.Parse(MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().text);
        options.deathPenaltyDuration = float.Parse(MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().text);
        options.normalFoodGrowthAmount = int.Parse(MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().text);
        options.deadSnakeFoodGrowthAmount = int.Parse(MenuScreenObjects["DeadSnakeFoodGrowthAmount"].GetComponent<InputField>().text);
        options.goldFoodGrowthAmount = int.Parse(MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().text);
        options.goldFoodSpawnChance = float.Parse(MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().text);

        // Set the value of the toggle for the "do snakes turn to food" option
        options.doSnakesTurnToFood = MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().isOn;

        //save options to custom preset
        presetOptions["Custom"]= options;
    }

}