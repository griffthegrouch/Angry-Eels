using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Script : MonoBehaviour
{
    // The currently selected game mode
    private GameMode gameMode = GameMode.Endless;

    // The goal points for the "First to" game mode
    private int goalPoints = 100;

    // The number of players in the game
    private int numPlayers = 1;

    // The number of human players in the game
    private int numHumanPlayers = 1;

    // The values for the advanced options
    private AdvancedOptions advancedOptions;

    // The names and values for the preset advanced options
    private Dictionary<string, AdvancedOptions> presetOptions = new Dictionary<string, AdvancedOptions>
    {
        { "Custom", new AdvancedOptions() },
        { "Classic", new AdvancedOptions() },
        { "Wild", new AdvancedOptions() }
    };

    // A reference to the game handler script
    private GameHandler_Script gameHandlerScript;

    // The game objects for the advanced options screen
    private GameObject[] advancedOptionsObjects;

    // The snake indicator game objects
    private GameObject[] snakeIndicators;

    // The sprite renderers for the CPU indicator sprites
    private SpriteRenderer[] cpuIndicatorSprites;

    // The advanced options screen game object
    private GameObject advancedOptionsScreen;

    // The currently selected preset index
    private int selectedPreset = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        advancedOptionsScreen = GameObject.Find("AdvancedOptionsScreen");

        advancedOptionsScreen.SetActive(false);

        // Get the game objects for the advanced options screen
        advancedOptionsObjects = new GameObject[]
        {
        GameObject.Find("PresetsText"),
        GameObject.Find("P1ColourDropdown"),
        GameObject.Find("P2ColourDropdown"),
        GameObject.Find("P3ColourDropdown"),
        GameObject.Find("P4ColourDropdown"),
        GameObject.Find("InputField1"),
        GameObject.Find("InputField2"),
        GameObject.Find("InputField3"),
        GameObject.Find("InputField4"),
        GameObject.Find("InputField5"),
        GameObject.Find("InputField6"),
        GameObject.Find("InputField7"),
        GameObject.Find("InputField8"),
        GameObject.Find("InputField9"),
        GameObject.Find("InputField10"),
        GameObject.Find("InputField11"),
        GameObject.Find("InputField12"),
        GameObject.Find("Toggle1"),
        GameObject.Find("Toggle2"),
        GameObject.Find("Toggle3")
        };

        // Get the snake indicator game objects
        snakeIndicators = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            snakeIndicators[i] = GameObject.Find("SnakeIndicators").transform.GetChild(i).gameObject;
        }

        // Get the sprite renderers for the CPU indicator sprites
        cpuIndicatorSprites = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            cpuIndicatorSprites[i] = GameObject.Find("CPUIndicators").transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>();

        }
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
        UpdateGameModeText();
        UpdateFirstToSelectorVisibility();
    }
    // Check for input to increase or decrease the goal points
    public void IncreaseGoalPoints()
    {
        goalPoints += 10;
        UpdatePlayerIndicators();
    }
    public void DecreaseGoalPoints()
    {
        if (goalPoints > 20){
        goalPoints -= 10;}
        UpdatePlayerIndicators();
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
        UpdateAdvancedOptionsScreen();
    }
    public void CloseAdvancedOptions()
    {
        // Close the advanced options screen
        advancedOptionsScreen.SetActive(false);
    }

    // Check for input to switch between presets on the advanced options screen
    public void PreviousPreset()
    {
        // Decrease the selected preset index, wrapping around to the last preset if necessary
        selectedPreset--;
        if (selectedPreset < 0)
        {
            selectedPreset = presetOptions.Count - 1;
        }
        UpdateAdvancedOptionsScreen();
    }
    public void NextPreset()
    {
        // Increase the selected preset index, wrapping around to the first preset if necessary
        selectedPreset++;
        if (selectedPreset >= presetOptions.Count)
        {
            selectedPreset = 0;
        }
        UpdateAdvancedOptionsScreen();
    }

    // Check for input to start the game
    public void StartGame()
    {
        // Start the game using the current game mode, goal points, number of players, and advanced options
        gameHandlerScript.SetGameMode(gameMode, goalPoints);
        gameHandlerScript.SetNumPlayers(numPlayers, numHumanPlayers);
        gameHandlerScript.SetAdvancedOptions(advancedOptions);
        gameHandlerScript.InitializeGame();
    }
    // Update the game mode text
    private void UpdateGameModeText()
    {
        // Get the game mode text game object
        Text gameModeText = GameObject.Find("GameModeText").GetComponent<Text>();

        // Set the text to the current game mode
        switch (gameMode)
        {
            case GameMode.Endless:
                gameModeText.text = "Endless";
                break;
            case GameMode.FirstTo:
                gameModeText.text = "First to";
                break;
        }
    }

    // Update the visibility of the "First to" points selector
    private void UpdateFirstToSelectorVisibility()
    {
        // Get the "First to" points selector game object
        GameObject firstToSelector = GameObject.Find("FirstToPoints");

        // Show or hide the selector based on the current game mode
        if (gameMode == GameMode.FirstTo)
        {
            firstToSelector.SetActive(true);
        }
        else
        {
            firstToSelector.SetActive(false);
        }
    }

    // Update the player indicator game objects
    private void UpdatePlayerIndicators()
    {
        // Show or hide the player indicator game objects based on the current number of players
        for (int i = 0; i < 4; i++)
        {
            if (i < numPlayers)
            {
                snakeIndicators[i].SetActive(true);
            }
            else
            {
                snakeIndicators[i].SetActive(false);
            }
        }
    }

    // Toggle the type (human or computer) of the player at the given index
    private void TogglePlayerType(int playerIndex)
    {
        // Make sure the player index is valid
        if (playerIndex >= 0 && playerIndex < 4)
        {
            // Toggle the type of the player
            if (advancedOptions.playerTypes[playerIndex] == PlayerType.Human)
            {
                advancedOptions.playerTypes[playerIndex] = PlayerType.Computer;
            }
            else
            {
                advancedOptions.playerTypes[playerIndex] = PlayerType.Human;
            }

            // Update the player indicator game object to reflect the new player type
            UpdatePlayerIndicator(playerIndex);
        }
    }

    // Update the player indicator game object at the given index
    private void UpdatePlayerIndicator(int playerIndex)
    {
        // Make sure the player index is valid
        if (playerIndex >= 0 && playerIndex < 4)
        {
            // Get the CPU indicator sprite renderer for the player
            SpriteRenderer cpuIndicatorSprite = cpuIndicatorSprites[playerIndex];

            // Show or hide the CPU indicator sprite based on the player type
            if (advancedOptions.playerTypes[playerIndex] == PlayerType.Human)
            {
                cpuIndicatorSprite.enabled = false;
            }
            else
            {
                cpuIndicatorSprite.enabled = true;
            }
        }
    }

    // Update the advanced options screen with the values from the currently selected preset
    private void UpdateAdvancedOptionsScreen()
    {
        // Get the name of the currently selected preset
        int i = 0;
        string presetName = "Default";
        foreach (string item in presetOptions.Keys)
        {
            if (selectedPreset == i) presetName = item;
            i++;
        }

        // Get the advanced options for the preset
        AdvancedOptions options = presetOptions[presetName];

        // Update the advanced options screen with the values from the preset's advanced options
        UpdateAdvancedOptionsScreen(options);
    }

    // Update the advanced options screen with the values from the given advanced options
    public void UpdateAdvancedOptionsScreen(AdvancedOptions options)
    {
        // Set the values of the dropdown menus for player colours
        GameObject.Find("P1ColourDropdown").GetComponent<Dropdown>().value = options.playerColours[0];
        GameObject.Find("P2ColourDropdown").GetComponent<Dropdown>().value = options.playerColours[1];
        GameObject.Find("P3ColourDropdown").GetComponent<Dropdown>().value = options.playerColours[2];
        GameObject.Find("P4ColourDropdown").GetComponent<Dropdown>().value = options.playerColours[3];


        // Set the values of the input fields for speed and duration values
        GameObject.Find("InputField1").GetComponent<InputField>().text = options.snakeSpeed.ToString();
        GameObject.Find("InputField2").GetComponent<InputField>().text = options.ghostModeDuration.ToString();
        GameObject.Find("InputField3").GetComponent<InputField>().text = options.deathPenaltyDuration.ToString();
        GameObject.Find("InputField4").GetComponent<InputField>().text = options.startingSize.ToString();
        GameObject.Find("InputField5").GetComponent<InputField>().text = options.normalFoodGrowthAmount.ToString();
        GameObject.Find("InputField6").GetComponent<InputField>().text = options.deadSnakeFoodGrowthAmount.ToString();
        GameObject.Find("InputField7").GetComponent<InputField>().text = options.goldFoodGrowthAmount.ToString();
        GameObject.Find("InputField8").GetComponent<InputField>().text = options.goldFoodSpawnChance.ToString();

        // Set the value of the toggle for the "do snakes turn to food" option
        GameObject.Find("Toggle1").GetComponent<Toggle>().isOn = options.doSnakesTurnToFood;
    }

    public void SetAdvancedOptionsScreenToMenu()
    {
        // Set the values of the dropdown menus for player colours
        advancedOptions.playerColours[0] =  GameObject.Find("P1ColourDropdown").GetComponent<Dropdown>().value;
        advancedOptions.playerColours[1] =  GameObject.Find("P2ColourDropdown").GetComponent<Dropdown>().value;
        advancedOptions.playerColours[2] =  GameObject.Find("P3ColourDropdown").GetComponent<Dropdown>().value;
        advancedOptions.playerColours[3] =  GameObject.Find("P4ColourDropdown").GetComponent<Dropdown>().value;

        // Set the values of the input fields for speed and duration values
        advancedOptions.snakeSpeed = float.Parse(GameObject.Find("InputField1").GetComponent<InputField>().text);
        advancedOptions.ghostModeDuration = float.Parse(GameObject.Find("InputField2").GetComponent<InputField>().text);
        advancedOptions.deathPenaltyDuration = float.Parse(GameObject.Find("InputField3").GetComponent<InputField>().text);
        advancedOptions.startingSize = int.Parse(GameObject.Find("InputField4").GetComponent<InputField>().text);
        advancedOptions.normalFoodGrowthAmount = int.Parse(GameObject.Find("InputField5").GetComponent<InputField>().text);
        advancedOptions.deadSnakeFoodGrowthAmount = int.Parse(GameObject.Find("InputField6").GetComponent<InputField>().text);
        advancedOptions.goldFoodGrowthAmount = int.Parse(GameObject.Find("InputField7").GetComponent<InputField>().text);
        advancedOptions.goldFoodSpawnChance = float.Parse(GameObject.Find("InputField8").GetComponent<InputField>().text);

        // Set the value of the toggle for the "do snakes turn to food" option
        advancedOptions.doSnakesTurnToFood = GameObject.Find("Toggle1").GetComponent<Toggle>().isOn;
    }
}