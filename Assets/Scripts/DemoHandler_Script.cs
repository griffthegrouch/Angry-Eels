using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoHandler_Script : MonoBehaviour
{
    public GameHandler_Script gameHandlerScript;
    public Menu_Script menuScript;

    private GameObject demoSnakePrefab;
    private GameObject demoSnakeSegmentPrefab;

    public PlayerGUI_Script playerGUIScript;
    private DemoSnake_Script snakeScript { get; set; }
    private Color eelColour = Color.green;
    private Color snakeColour = Color.red;
    private GameObject food;

    public GameObject demoGUI;
    public GameObject returnHomBtn;

    public Text demoText;
    public Text tutorialCounterText;

    public GameObject eelIndicator;
    public GameObject snakeIndicator;

    private string[] instructions = new string[]{
        "Basics:\n To move in a certain direction, just press the direction you want to go. The Eel will continue to move in the direction its facing.",
        "Holding:\n You can also just hold the direction you want to go.",
        "One-Step:\n If you're already moving in a certain direction, but want to move 'one-step' to the side, hold your current direction and quickly tap the sideways direction you want to go.",
        "Staricase:\n If you want to move in a 'staircase' pattern, hold two directions at the same time and your eel will move automatically.",
        "U-Turn:\n To make a quick U-turn, hold the backwards direction and quickly tap the sideways direction opposite to the direction you were originally facing.",
        "Great Job!\n Returning home.",
        "Great Job!"
    };

    private string comparisonText = "Press SPACEBAR to switch between the eel and the snake!\nThere is no goal, this mode is just a demo!";
    private string retroSnakeText = "This demo mode is endless and you cant save your score.\nEat food to grow";

    // Ruleset ruleSet 
    // f snakeSpeed, i startingSize, f ghostModeDuration, f deathPenaltyDuration,
    // i normalFoodGrowthAmount, f goldFoodSpawnChance,  i goldFoodGrowthAmount, b doSnakesTurnToFood
    private Options options { get; set; } = new Options(
        RuleSet.Demo,
        0.1f, 5, 0f, 0f,
        1, 0, 25, false
    );

    // Start is called before the first frame update
    void Start()
    {
        demoSnakePrefab = Resources.Load("Prefabs/DemoSnake") as GameObject;
        demoSnakeSegmentPrefab = Resources.Load("Prefabs/DemoSnakeSegment") as GameObject;

        HideAllGUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void HideAllGUI(){
        demoText.enabled = false;
        tutorialCounterText.enabled = false;

        eelIndicator.SetActive(false);
        snakeIndicator.SetActive(false);
        demoGUI.SetActive(false);
        returnHomBtn.SetActive(false);
    }
    public void ShowEel(bool isEel)
    {
        eelIndicator.SetActive(isEel);
        snakeIndicator.SetActive(!isEel);
    }

    public void UpdateTutorialText(int tutNum, string counterText)
    {
        if (tutNum == -1)
        {
            demoText.text = "";
            tutorialCounterText.text = "";
            demoText.enabled = false;
            tutorialCounterText.enabled = false;
        }
        else
        {
            demoText.text = instructions[tutNum];
            tutorialCounterText.text = counterText;
        }
    }

    public void ReturnToMenu()
    {
        EndGame();

        HideAllGUI();

        gameHandlerScript.OpenMenuScreen(ActiveScreen.About);
    }

    public void EndGame()
    {
        //pauseScreenScript.Reset();
        playerGUIScript.Reset();

        if (snakeScript != null)
        {
            Destroy(snakeScript.gameObject);
        }

        if (food != null)
        {
            Destroy(food.gameObject);
        }

        CancelInvoke();
    }


    public void StartTutorial()
    {
        //start up game
        InitializeGame();

        //set demo mode
        snakeScript.SwitchDemoMode(DemoMode.Tutorial);

        //setup tutorial GUI
        demoText.enabled = true;
        tutorialCounterText.enabled = true;

        //calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnake", 0, options.snakeSpeed);
    }



    public void StartComparison()
    {
        //start up game
        InitializeGame();

        //set demo mode
        snakeScript.SwitchDemoMode(DemoMode.Comparison);

        //setup comparison GUI
        demoText.enabled = true;
        demoText.text = comparisonText;
        ShowEel(true);

        // Initialize the food
        SpawnFood(-1, default, EntityType.NormalFood);

        //calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnake", 0, options.snakeSpeed);
    }
    public void StartRetroSnake()
    {
        //start up game
        InitializeGame();

        //set demo mode
        snakeScript.SwitchDemoMode(DemoMode.RetroSnake);

        //setup the retro snake game GUI
        demoText.enabled = true;
        demoText.text = retroSnakeText;
        ShowEel(false);

        // Initialize the food
        SpawnFood(-1, default, EntityType.NormalFood);

        //calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnake", 0, options.snakeSpeed);
    }
    public void InitializeGame()
    {
        gameHandlerScript.activeScreen = ActiveScreen.Demo;

        //setup eel gui
        playerGUIScript.demoMode = true;
        playerGUIScript.ShowGUI();
        returnHomBtn.SetActive(true);

        //position it will spawn
        Vector3 pos = new Vector3(12, -1, 0) + this.transform.position;

        // Initialize/spawn player
        InitializeDemoSnake(pos);

        //setup the GUI
        demoGUI.SetActive(true);
    }

    // Initialize a player/snake
    private void InitializeDemoSnake(Vector2 pos)
    {
        //instantiating the snake+script
        snakeScript = Instantiate(demoSnakePrefab, this.transform.position, new Quaternion(0, 0, 0, 0), this.transform).GetComponent<DemoSnake_Script>();

        //determining all the settings for the snake
        PlayerSettings playerSettings = new PlayerSettings(
            0, gameHandlerScript, playerGUIScript,
            new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow },
            eelColour, pos,
            options.startingSize, options.snakeSpeed, options.ghostModeDuration, options.deathPenaltyDuration,
            options.normalFoodGrowthAmount, options.deadSnakeFoodGrowthAmount, options.goldFoodGrowthAmount,
            options.doSnakesTurnToFood
        );

        //passing all the relevant information to the new snake
        snakeScript.SetupSnake(playerSettings, this, demoSnakeSegmentPrefab, snakeColour);
    }

    private void MoveSnake()
    {
        // loop through all snakes and attempt to move them one space
        snakeScript.TryMoveSnake();
    }

    public EntityType CheckPos(int playerNum, Vector3 pos, bool destroyFood)
    {
        if (playerNum != -1)//a snake calling method
        {
            // check if bumping into self
            if (snakeScript.CheckForSnakeAtPos(pos, false))
            {
                return EntityType.Self;
            }
        }
        else// game handler calling this method
        {
            // check if position contains any snakes
            if (snakeScript.CheckForSnakeAtPos(pos, true))
            {
                return EntityType.Snake;
            }
        }
        // check if position contains a wall
        foreach (GameObject wall in gameHandlerScript.wallArr)
        {
            if (pos == wall.transform.position)
            {
                return EntityType.Wall;
            }
        }
        // check if position contains food
        if (food == null)
        {
            return EntityType.Empty;
        }
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
                Destroy(food.gameObject);
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
                return entityType;
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
            Debug.Log("cant spawn here, pos: " + pos);
            // if trying to spawn food on an existing food, don't
            return;
        }
        food = Instantiate(gameHandlerScript.foodPrefab, pos, new Quaternion(0, 0, 0, 0), this.transform);
        switch (foodType)
        {
            case EntityType.NormalFood:
                // chance to turn into golden food
                if ((int)Random.Range(1, options.goldFoodSpawnChance) == 1)
                {
                    foodType = EntityType.GoldFood;
                    goto case EntityType.GoldFood;
                }
                break;
            case EntityType.DeadSnakeFood:
                // colour food
                food.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.23f);
                break;
            case EntityType.GoldFood:
                // colour food
                food.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                break;
        }
        food.tag = foodType.ToString();
    }
}
