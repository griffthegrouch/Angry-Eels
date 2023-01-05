using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public tools for scripts to use
// Enum for the game mode
    public enum GameMode
    {
        Endless,
        FirstTo
    }

    // Enum for the player type
    public enum PlayerType
    {
        Human,
        Computer
    }

    public enum FoodType
    {
        NormalFood,
        DeadSnakeFood,
        GoldFood
    }

    public enum EntityType
    {
        Empty,
        Self,
        Snake,
        Wall,
        NFood,
        DFood,
        GFood
    }

    // A class for the advanced options
    public class AdvancedOptions
    {
        public int[] playerColours = new int[4];
        public PlayerType[] playerTypes = new PlayerType[4];
        public float snakeSpeed = 0.1f;
        public float ghostModeDuration = 3;
        public float deathPenaltyDuration = 2;
        public int startingSize = 10;
        public int normalFoodGrowthAmount = 3;
        public int deadSnakeFoodGrowthAmount = 1;
        public int goldFoodGrowthAmount = 30;
        public float goldFoodSpawnChance = 1;
        public bool doSnakesTurnToFood = false;
    }





















public class GameHandler_Script : MonoBehaviour
{
    // Vars for the game mode, goal points, and number of players
    private GameMode gameMode;
    private int goalPoints;
    private int numPlayers;
    private int numHumanPlayers;

    // Advanced options for the game
    public AdvancedOptions advancedOptions;

    // Colors for the snakes
    private Color[] snakeColours = new Color[] {
       Color.green, Color.red, Color.blue, Color.gray, Color.yellow + Color.blue, Color.magenta, Color.yellow + Color.red
    };

    // Array for the starting positions of the snakes
    private Vector3[] startingPositions = new Vector3[4];

    // Array for the player colors
    private Color[] playerColours;

    // 2D array for the player inputs
    private KeyCode[,] playerInputs = new KeyCode[,] {
        {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow},
        {KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D},
        {KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H},
        {KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L}
    };

    // Array for all the wall blocks in the game
    private GameObject[] wallArr;

    // List for all the food in the game
    private List<GameObject> foodList = new List<GameObject>();

    // List for all the snakes in the game
    private List<Snake_Script> snakeScripts = new List<Snake_Script>();

    // Array for all the score displays
    private GameObject[] playerDisplays;
    private PlayerDisplay_Script[] playerDisplayScripts;

    // Prefab for the snake
    private GameObject snakePrefab;
    // Prefab for the snake segment
    private GameObject snakeSegmentPrefab;
    // Prefab for the food
    private GameObject foodPrefab;

    void Start(){
        // grab all resources
        snakePrefab = Resources.Load("Snake") as GameObject;
        snakeSegmentPrefab = Resources.Load("SnakeSegment") as GameObject;
        foodPrefab = Resources.Load("Food") as GameObject;
    }

    // Set the game mode and goal points from the menu script
    public void SetGameMode(GameMode mode, int points)
    {
        // Set the game mode
        gameMode = mode;

        // Set the goal points
        goalPoints = points;
    }

    // Set the number of players and number of human players from the menu script
    public void SetNumPlayers(int players, int humanPlayers)
    {
        // Set the number of players
        numPlayers = players;

        // Set the number of human players
        numHumanPlayers = humanPlayers;
    }

    // Set the advanced options from the menu script
    public void SetAdvancedOptions(AdvancedOptions options)
    {
        // Set the advanced options
        advancedOptions = options;
    }

    // Initialize the game
    public void InitializeGame()
    {
        // Initialize the player colors array
        playerColours = new Color[numPlayers];

        // Initialize the player displays array and scripts array
        playerDisplays = new GameObject[numPlayers];
        playerDisplayScripts = new PlayerDisplay_Script[numPlayers];

        // Initialize the staring positions
        

        // Initialize the player displays and scripts
        for (int i = 0; i < numPlayers; i++)
        {
            //calculate spawn positions (calculates equal horizontal positions for each snake with space on each side)
            int spawnPosX = 25/(numPlayers + 1) * (i+1);

            if(numPlayers == 4 && spawnPosX < 12){
                //manual adjustment to move the first two snakes' spawn positions to the left one space 
                //IF playing 4 player, since the spots are calulated by integers and theres 25 spots, it doesnt round down these two numbers properly
                spawnPosX -= 1;
            }
            startingPositions[i] = new Vector3(spawnPosX,-1,0);

            // Get the player display and script
            playerDisplays[i] = GameObject.Find("PlayerDisplay" + (i + 1));
            playerDisplayScripts[i] = playerDisplays[i].GetComponent<PlayerDisplay_Script>();

            // Set the player color
            playerColours[i] = snakeColours[advancedOptions.playerColours[i]];

            // Set the player type
            PlayerType playerType = advancedOptions.playerTypes[i];

            // Initialize/spawn player
            InitializePlayer(i, playerType);
        }

        // Initialize the food
        SpawnFood(-1, default, FoodType.NormalFood);
    }

    // Initialize a human player
    private void InitializePlayer(int playerIndex, PlayerType playertype)
    {
        Vector3 pos  = startingPositions[playerIndex];

        Snake_Script newSnakeScript = Instantiate(snakePrefab, pos, new Quaternion(0, 0, 0, 0), this.transform).GetComponent<Snake_Script>();;
        newSnakeScript.SetReferences(
            this, playerDisplayScripts[playerIndex], snakeSegmentPrefab, playerIndex, advancedOptions.snakeSpeed, advancedOptions.startingSize,
            pos, advancedOptions.ghostModeDuration, advancedOptions.deathPenaltyDuration);
    }

    public EntityType CheckPos(int playerNum, Vector3 pos, bool destroyFood)
    {
        if (playerNum != -1)
        {
            // check if bumping into self
            if (snakeScripts[playerNum].CheckForSnakeAtPos(pos))
            {
                return EntityType.Self;
            }

            // check if bumping into other snakes
            for (int i = 0; i < numPlayers; i++)
            {
                if (i != playerNum && snakeScripts[i].CheckForSnakeAtPos(pos))
                {
                    return EntityType.Snake;
                }
            }
        }
        else
        {
            // check if bumping into any snakes
            foreach (Snake_Script snake in snakeScripts)
            {
                if (snake.CheckForSnakeAtPos(pos))
                {
                    return EntityType.Snake;
                }
            }
        }

        // check if bumping into wall
        foreach (GameObject wall in wallArr)
        {
            if (pos == wall.transform.position)
            {
                return EntityType.Wall;
            }
        }

        // check if bumping into food
        foreach (GameObject food in foodList)
        {
            if (pos == food.transform.position)
            {
                EntityType entityType;
                if(food.tag == FoodType.NormalFood.ToString()){
                    entityType = EntityType.NFood;
                }
                else if(food.tag == FoodType.DeadSnakeFood.ToString()){
                    entityType = EntityType.DFood;
                }
                else if(food.tag == FoodType.GoldFood.ToString()){
                    entityType = EntityType.GFood;
                }
                else{
                    //food type was not identified
                    Debug.Log("food couldnt be identified");
                    return EntityType.Empty;
                }
                if (destroyFood)
                {
                    switch (entityType)
                    {
                        case EntityType.NFood:
                            SpawnFood(-1, default, FoodType.NormalFood);
                            break;
                        case EntityType.DFood:
                            break;
                        case EntityType.GFood:
                            SpawnFood(-1, default, FoodType.NormalFood);
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

    public void SpawnFood(int playerNum, Vector3 pos, FoodType foodType)
    {
        if (pos == default)
        {
            // find a new position for the food
            pos = new Vector3(Random.Range(0, 25), Random.Range(0, 25), 0);

            while (CheckPos(-1, pos, false) != EntityType.Empty)
            {
                pos = new Vector3(Random.Range(0, 25), Random.Range(0, 25), 0);
            }
        }
        else if (CheckPos(-1, pos, false) != EntityType.Empty)
        {
            // if trying to spawn food on an existing food, don't
            return;
        }

        GameObject newFood = Instantiate(foodPrefab, pos, new Quaternion(0, 0, 0, 0), this.transform);

        switch (foodType)
        {
            case FoodType.NormalFood:
                // chance to turn into golden food
                if ((int)Random.Range(1, advancedOptions.goldFoodSpawnChance) == 1)
                {
                    foodType = FoodType.GoldFood;
                    goto case FoodType.DeadSnakeFood;
                }
                break;
            case FoodType.DeadSnakeFood:
                // colour food
                newFood.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.23f);
                break;
            case FoodType.GoldFood:
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