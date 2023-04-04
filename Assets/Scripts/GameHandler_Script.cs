using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//public tools for scripts to use

// Enum for the active screen
public enum ActiveScreen
{
    About,
    Title,
    MainMenu,
    AdvancedOptions,
    Game,
    PauseMenu,
    WinMenu,
    HighScore,
    Demo
}

// Enum for the game mode
public enum GameMode
{
    Endless,
    Points
}

// Enum for the player type
public enum PlayerType
{
    Human,
    Computer
}

public enum EntityType
{
    Empty,
    Self,
    Snake,
    Wall,
    NormalFood,
    DeadSnakeFood,
    GoldFood
}


public class GameHandler_Script : MonoBehaviour
{
    /////////////////////////// vars for the game handler

    //var keeps track of which screen is currently active (or shown on camera) - defaults to the title screen
    public ActiveScreen activeScreen { get; set; } = ActiveScreen.Title;

    public int currentLeader { get; set; }
    public int currentHighscore { get; set; }

    // scripts for the other screens
    public Menu_Script menuScript;
    public PauseMenu_Script pauseScreenScript;//{ get; set; }
    public WinScreen_Script winScreenScript;
    public HighScoreManager_Script highScoreManagerScript;


    // All options for the game - set in the menu screen, then passed over on game start
    public Options options { get; set; }

    // 2D array for the player inputs - defaults to OG keyboard controls
    public KeyCode[,] playerInputs { get; set; } = new KeyCode[,] {
        {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow},
        {KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D},
        {KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H},
        {KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L}
    };

    public KeyCode[,] activePlayerInputs { get; set; }

    // Array for all the wall blocks in the game
    public GameObject[] wallArr { get; set; }

    // List for all the food in the game
    private List<GameObject> foodList = new List<GameObject>();

    /////////////////////////// vars for the snakes

    // Array for the starting positions of the snakes, generated on game setup from # of players
    private Vector3[] startingPositions;

    // List for all the snakes in the game + their settings
    private Snake_Script[] snakeScripts;

    // Arrays for all the score displays + their scripts
    public GameObject[] playerGUIs;
    // always contains all 4 displays
    public PlayerGUI_Script[] playerGUIScripts { get; set; }


    /////////////////////////// prefabs

    // Prefab for the snake
    public GameObject snakePrefab;
    // Prefab for the food
    public GameObject foodPrefab;

    // Audio source for game handler
    public AudioSource gameHandlerAudio;

    // Audio source for sound effects
    public AudioSource sfxPlayer;

    //all music clips
    public AudioClip gameMusic;
    public AudioClip pauseScreenMusic;
    public AudioClip titleScreenMusic;



    //sound effects menus
    public AudioClip buttonClickSFX;
    public AudioClip gameStartSFX;
    public AudioClip splashSFX;
    public AudioClip grrSFX;
    public AudioClip pauseSFX;
    public AudioClip unPauseSFX;


    void Start()
    {
        // grab all existing walls
        wallArr = GameObject.FindGameObjectsWithTag("wall");
        //gameHandlerAudio.Pause();
        gameHandlerAudio.clip = titleScreenMusic;
        gameHandlerAudio.Play();
        sfxPlayer.pitch = 1.8f;
        sfxPlayer.PlayOneShot(grrSFX);
  
    }

    public void Pause()
    {//game paused - called from pause menu
        //play sfx
        PlaySFX(pauseSFX, 0.5f);

        //switch music playing
        gameHandlerAudio.Pause();

        //stop time/snake movement
        CancelInvoke();
        Time.timeScale = 0;
    }
    public void UnPause()
    {//game unpaused - called from pause menu
        //play sfx
        PlaySFX(unPauseSFX, 0.5f);

        //switch music playing
        gameHandlerAudio.Play();

        //resume time + snake movement
        CancelInvoke();
        InvokeRepeating("MoveSnakes", 0, options.snakeSpeed);
        Time.timeScale = 1;
    }

    //method plays a sound effect from game handler audio, overload is for playing it with a specific volume
    public void PlaySFX(AudioClip sfx)
    {
        float volume = 0.2f;
        sfxPlayer.PlayOneShot(sfx, volume);
    }
    public void PlaySFX(AudioClip sfx, float volume)
    {
        sfxPlayer.PlayOneShot(sfx, volume);
    }

    public void ExitGame()
    { // called to close the game
      // Application.Quit() does not work in the editor so need to compile different game exit code for build
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    //called from the pause menu - restarts game exactly how it was started
    public void RestartGame()
    {
        EndGame();
        InitializeGame(options);
    }

    //called from the pause menu - ends game and returns to the main menu
    public void LeaveGameReturnHome()
    {
        EndGame();
        Time.timeScale = 1;
        OpenMenuScreen(ActiveScreen.MainMenu);
    }

    public void OpenMenuScreen(ActiveScreen screen)
    {
        activeScreen = screen;
        menuScript.Open(screen);
    }

    public void OpenSaveScoreScreen()
    {
        EndGame();
        activeScreen = ActiveScreen.WinMenu;
        winScreenScript.OpenSaveScore(currentLeader, currentHighscore);
    }
    public void OpenWinScreen()
    {
        activeScreen = ActiveScreen.WinMenu;
        winScreenScript.OpenGameWon(currentLeader, currentHighscore);
    }

    public void OpenHighScoreScreen()
    {
        activeScreen = ActiveScreen.HighScore;
        highScoreManagerScript.Open();
    }



    public void EndGame()
    {
        gameHandlerAudio.clip = titleScreenMusic;
        gameHandlerAudio.Play();
        pauseScreenScript.Reset();
        foreach (var script in playerGUIScripts)
        {
            script.Reset();
        }
        foreach (var snake in snakeScripts)
        {
            if (snake != null)
            {
                Destroy(snake.gameObject);
            }
        }
        foreach (var food in foodList)
        {
            Destroy(food.gameObject);
        }
        foodList.Clear();
        CancelInvoke();
    }


    //called by player indicators to communicate their current scores
    public void UpdateScore(int playerNum, int score)
    {
        if (score > currentHighscore)
        {
            currentLeader = playerNum;
            currentHighscore = score;
        }

        if (options.gameMode == GameMode.Points && score >= options.goalPoints)
        {
            EndGame();
            winScreenScript.OpenGameWon(playerNum, score);
            OpenWinScreen();
        }

    }

    // Initialize the game called from the menu on game start
    public void InitializeGame(Options _options)
    {

        currentHighscore = 0;//restart highscore tracker

        activeScreen = ActiveScreen.Game;

        pauseScreenScript.ShowPrompt();

        //play sfx
        sfxPlayer.Stop();
        sfxPlayer.pitch = 1f;
        PlaySFX(splashSFX, 2f);

        //play music
        gameHandlerAudio.clip = gameMusic;
        gameHandlerAudio.Play();

        // Set options
        options = _options;

        // Initialize the player arrays to the size of the amt of players
        snakeScripts = new Snake_Script[options.numPlayers];
        playerGUIScripts = new PlayerGUI_Script[options.numPlayers];
        startingPositions = new Vector3[options.numPlayers];

        activePlayerInputs = new KeyCode[options.numPlayers, 4];
        int activePlayerCounter = 0;
        // Initialize the player displays and scripts
        for (int i = 0; i < 4; i++)
        {
            //getting the player controls / inputs
            if (options.activePlayers[i] == true)
            {
                activePlayerCounter++;
                for (int j = 0; j < 4; j++)
                {   //loop through all 4 inputs and map them to the active player's controls
                    activePlayerInputs[activePlayerCounter - 1, j] = playerInputs[i, j];
                }
            }
            if (i < options.numPlayers)
            {
                //calculate spawn positions (calculates equal horizontal positions for each snake with space on each side)
                int spawnPosX = 25 / (options.numPlayers + 1) * (i + 1);

                if (options.numPlayers == 4 && spawnPosX < 12)
                {
                    //manual adjustment to move the first two snakes' spawn positions to the left one space 
                    //IF playing 4 player, since the spots are calulated by integers and theres 25 spots, it doesnt round down these two numbers properly
                    spawnPosX -= 1;
                }
                startingPositions[i] = new Vector3(spawnPosX, -1, 0) + this.transform.position;

                // Get the player display and script
                playerGUIScripts[i] = playerGUIs[i].GetComponent<PlayerGUI_Script>();

                // Show GUI
                playerGUIScripts[i].ShowGUI();

                // Initialize/spawn player
                InitializePlayer(i);
            }
        }
        // Initialize the food
        SpawnFood(-1, default, EntityType.NormalFood);

        // calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnakes", 0, options.snakeSpeed);
    }

    // Initialize a player/snake
    private void InitializePlayer(int playerIndex)
    {
        //position it will spawn
        Vector3 pos = startingPositions[playerIndex];

        //instantiating the snake+script
        Snake_Script newSnakeScript = Instantiate(snakePrefab, this.transform.position, new Quaternion(0, 0, 0, 0), this.transform).GetComponent<Snake_Script>();

        //determining all the settings for the snake
        PlayerSettings playerSettings = new PlayerSettings(
            playerIndex, this, playerGUIScripts[playerIndex],
            new KeyCode[] { activePlayerInputs[playerIndex, 0], activePlayerInputs[playerIndex, 1], activePlayerInputs[playerIndex, 2], activePlayerInputs[playerIndex, 3] },
            options.playerColours[playerIndex], startingPositions[playerIndex],
            options.startingSize, options.snakeSpeed, options.ghostModeDuration, options.deathPenaltyDuration,
            options.normalFoodGrowthAmount, options.deadSnakeFoodGrowthAmount, options.goldFoodGrowthAmount,
            options.doSnakesTurnToFood
        );

        //passing all the relevant information to the new snake
        newSnakeScript.SetupSnake(playerSettings);

        //adding new snake script to local snakescripts arr
        snakeScripts[playerIndex] = newSnakeScript;
    }

    private void MoveSnakes()
    {
        Dictionary<Vector2, List<Snake_Script>> newPositionCounts = new Dictionary<Vector2, List<Snake_Script>>();

        // Move all non-ghosted snakes one space and store their new positions
        foreach (Snake_Script snakeScript in snakeScripts)
        {
            if (snakeScript.snakeState != SnakeState.Ghosted)
            {
                Vector2 newPosition = snakeScript.TryMoveSnake();

                if (newPositionCounts.ContainsKey(newPosition))
                {
                    newPositionCounts[newPosition].Add(snakeScript);
                }
                else
                {
                    newPositionCounts[newPosition] = new List<Snake_Script> { snakeScript };
                }
            }
            else
            {
                // If ghosted, snake doesn't enter collision matrix
                snakeScript.TryMoveSnake();
            }
        }

        // Check for collisions and kill the collided snakes
        foreach (List<Snake_Script> collidedSnakes in newPositionCounts.Values)
        {
            if (collidedSnakes.Count > 1)
            {
                foreach (Snake_Script snakeScript in collidedSnakes)
                {
                    if (snakeScript.snakeState != SnakeState.Ghosted)
                    {
                        snakeScript.Die();
                    }
                }
            }
        }
    }




    public EntityType CheckPos(int playerNum, Vector3 pos, bool destroyFood)
    {
        if (playerNum != -1)//a snake calling method
        {
            // check if bumping into self
            if (snakeScripts[playerNum].CheckForSnakeAtPos(pos, false))
            {
                return EntityType.Self;
            }
            // check if bumping into other snakes
            for (int i = 0; i < options.numPlayers; i++)
            {
                if (i != playerNum && snakeScripts[i].CheckForSnakeAtPos(pos, false))
                {
                    if (snakeScripts[i].snakeState == SnakeState.Ghosted)
                    {
                        return EntityType.Empty; //if snake in location is ghosted, return empty
                    }
                    else
                    {
                        return EntityType.Snake; //if snake in location isnt ghosted, return snake
                    }

                }
            }
        }
        else// game handler calling this method
        {
            // check if position contains any snakes
            foreach (Snake_Script snake in snakeScripts)
            {
                if (snake.CheckForSnakeAtPos(pos, true))
                {
                    return EntityType.Snake;
                }
            }
        }
        // check if position contains a wall
        foreach (GameObject wall in wallArr)
        {
            if (pos == wall.transform.position)
            {
                return EntityType.Wall;
            }
        }
        // check if position contains food
        foreach (GameObject food in foodList)
        {
            if (pos == food.transform.position)
            {
                EntityType entityType;
                if (food.tag == EntityType.NormalFood.ToString())
                {
                    entityType = EntityType.NormalFood;
                }
                else if (food.tag == EntityType.DeadSnakeFood.ToString())
                {
                    entityType = EntityType.DeadSnakeFood;
                }
                else if (food.tag == EntityType.GoldFood.ToString())
                {
                    entityType = EntityType.GoldFood;
                }
                else
                {
                    //food type was not identified
                    return EntityType.Empty;
                }
                if (destroyFood)
                {
                    switch (entityType)
                    {
                        case EntityType.NormalFood:
                            SpawnFood(-1, default, EntityType.NormalFood);
                            break;
                        case EntityType.DeadSnakeFood:
                            break;
                        case EntityType.GoldFood:
                            SpawnFood(-1, default, EntityType.NormalFood);
                            break;
                    }
                    foodList.Remove(food);
                    Destroy(food);
                    return entityType;
                }
            }
        }
        // if there's nothing in the spot,
        return EntityType.Empty;
    }

    public void SpawnFood(int playerNum, Vector3 pos, EntityType foodType)
    {
        // spawning at a random position
        if (pos == default)
        {
            // find a new position for the food
            pos = new Vector3(Random.Range(0, 25), Random.Range(0, 25), 0) + this.transform.position;

            while (CheckPos(-1, pos, false) != EntityType.Empty)
            {
                pos = new Vector3(Random.Range(0, 25), Random.Range(0, 25), 0) + this.transform.position;
            }
        }
        //if attempting to spawn at a specific position
        else if (CheckPos(playerNum, pos, false) != EntityType.Self)
        {
            //Debug.Log("cant spawn food here, pos: " + pos);
            // if trying to spawn food on an existing food, don't
            return;
        }
        GameObject newFood = Instantiate(foodPrefab, pos, new Quaternion(0, 0, 0, 0), this.transform);
        switch (foodType)
        {
            case EntityType.NormalFood:
                // chance to turn into golden food
                if ((int)Random.Range(0, 100) < options.goldFoodSpawnChance)
                {
                    foodType = EntityType.GoldFood;
                    goto case EntityType.GoldFood;
                }
                break;
            case EntityType.DeadSnakeFood:
                // colour food
                newFood.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.23f);
                break;
            case EntityType.GoldFood:
                // colour food
                newFood.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                break;
        }

        newFood.tag = foodType.ToString();
        foodList.Add(newFood);

    }



}