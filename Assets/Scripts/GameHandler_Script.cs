using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler_Script : MonoBehaviour
{
    /* 

    ***Game Handler Script 
    controls the ability to set game settings

    script sets up the game, and acts as the access point for the snakes to interact with

    -initializing the snakes, walls, and food
    ->then setting all the values for the snakes (controls, colours, prefabs, etc.)

    ***Access points
    public string CheckPos()
        -called by snakes to determine what is in x location

    public SpawnFood()
        -called by self and snakes when spawning any kind of food



    Handler script story
    1 -> game starts/loads

    2 -> grabs values from user-set settings in out-of-game settings (MENU SCRIPT)

    3 -> grabs prefabs and converts variables into "usable data"

    4 -> spawns snakes and begins the game

    5 -> game starts and handler moves snakes and is accesible for snake scripts when required
        - when snakes need to check what is occupying a position on the map
            -> call this script requesting it, then recieve what is in that position
        -if the snake is trying to move to a food block
            -> destroy the food block - if appropriate, generate a new food block

    */


        //all these vars are set by menu script
        private char gameMode{set;} // e - endless, f - first to
        private int goalPoints{set;}
        private int num_Players{set;}
        private int num_Human_Players{set;}
        //- advanced options screen menu vars
        // when loaded initially, they are set to the first preset gamemode values
        private int player1_Colour {set;}
        private int player2_Colour {set;}
        private int player3_Colour {set;}
        private int player4_Colour {set;}
        private int startingSize {set;}
        private int normalFood_GrowthAmount {set;}
        private int deadSnakeFood_GrowthAmount {set;}
        private int goldFood_GrowthAmount{set;}
        private float snakeSpeed{set;}
        private float ghostMode_Duration{set;}
        private float deathPenalty_Duration{set;}
        private float goldFood_SpawnChance{set;}
        private bool doSnakesTurnToFood{set;}

    private Color[] snakeColours = new Color[] {
       Color.green, Color.red, Color.blue, Color.gray, Color.yellow + Color.blue, Color.magenta, Color.yellow + Color.red  
    };

    private Vector3[] starting_Positions = new Vector3[4];
    private Color[] player_Colours;

    private KeyCode[,] playerInputs = new KeyCode[,] {
        {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow},
        {KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D},
        {KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H},
        {KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L}
    };

    //array keeps track of all the wall blocks in the game
    private GameObject[] wallArr;

    //list keeps track of all the food in the game
    private List<GameObject> foodList = new List<GameObject>();

    //list keeps track of all the snakes in the game
    private List<Snake_Script> snakeScripts = new List<Snake_Script>();

    //Array keeps track of all 4 score displays
    private GameObject[] playerDisplays;
    private PlayerDisplay_Script[] playerDisplayScripts;

    private GameObject snakePrefab;
    private GameObject snakeSegmentPrefab;
    private GameObject foodPrefab;


    void DebugAllVars(){
        
        Debug.Log("gameMode: " + gameMode);
        Debug.Log("goalPoints: " + goalPoints);
        Debug.Log("num_Players: " + num_Players);
        Debug.Log("num_Human_Players: " + num_Human_Players);
        Debug.Log("player1_Colour: " + player1_Colour);
        Debug.Log("player2_Colour: " + player2_Colour);
        Debug.Log("player3_Colour: " + player3_Colour);
        Debug.Log("player4_Colour: " + player4_Colour);
        Debug.Log("startingSize: " + startingSize);
        Debug.Log("normalFood_GrowthAmount: " + normalFood_GrowthAmount);
        Debug.Log("deadSnakeFood_GrowthAmount: " + deadSnakeFood_GrowthAmount);
        Debug.Log("goldFood_GrowthAmount: " + goldFood_GrowthAmount);
        Debug.Log("snakeSpeed: " + snakeSpeed);
        Debug.Log("ghostMode_Duration: " + ghostMode_Duration);
        Debug.Log("deathPenalty_Duration: " + deathPenalty_Duration);
        Debug.Log("goldFood_SpawnChance: " + goldFood_SpawnChance);
        Debug.Log("doSnakesTurnToFood: " + doSnakesTurnToFood);

    }
    // Start is called before the first frame update
    void Start(){
        // grab all resources
        snakePrefab = Resources.Load("Snake") as GameObject;
        snakeSegmentPrefab = Resources.Load("SnakeSegment") as GameObject;
        foodPrefab = Resources.Load("Food") as GameObject;
        
        // grab all existing walls
        wallArr = GameObject.FindGameObjectsWithTag("wall");

    }
    public void LoadGame(){//called by the menu script

        // organize vars into more easily usable formats
        player_Colours = new Color[] {snakeColours[player1_Colour], snakeColours[player2_Colour], snakeColours[player3_Colour], snakeColours[player4_Colour]};

        //grab player displays and set vars
        playerDisplays = new GameObject[4];
        playerDisplayScripts = new PlayerDisplay_Script[4];
        for (int i = 0; i < playerDisplays.Length; i++)
        {
            playerDisplays[i] = GameObject.Find("PlayerDisplays").transform.GetChild(i).gameObject;
            playerDisplays[i].SetActive(false);
            playerDisplayScripts[i] = playerDisplays[i].GetComponent<PlayerDisplay_Script>();
            playerDisplayScripts[i].SetValues(i);
            playerDisplayScripts[i].SetOutlineColour(player_Colours[i]);
        }

        for (int i = 0; i < num_Players; i++)
        {
            //calculate spawn position (calculates equal horizontal positions for each snake with space on each side)
            int spawnPosX = 25/(num_Players + 1) * (i+1);

            if(num_Players == 4 && spawnPosX < 12){
                //manual adjustment to move the first two snakes' spawn positions to the left one space 
                //IF playing 4 player, since the spots are calulated by integers and theres 25 spots, it doesnt round down these two numbers properly
                spawnPosX -= 1;
            }
            starting_Positions[i] = new Vector3(spawnPosX,-1,0);

            //create the snake
            Snake_Script newSnake = Instantiate(snakePrefab, new Vector3(-10, -10 ,0), new Quaternion(0,0,0,0), this.transform.parent).GetComponent<Snake_Script>();
            
            // enable the snake's score display
            playerDisplays[i].SetActive(true);

            //set the snake's values and objects
            newSnake.SetObjects(//GameHandler_Script handlerScript,  GameObject head, GameObject seg
                this,
                newSnake.transform.GetChild(0).gameObject, 
                snakeSegmentPrefab
            );

            newSnake.SetValues(//int playernum, float snakeSpeed, int startingSize, Vector3 startingPos, int foodgrowRate, bool doesTurnIntoFood
                i,
                snakeSpeed,
                startingSize,
                starting_Positions[i],
                normalFood_GrowthAmount,
                deadSnakeFood_GrowthAmount,
                goldFood_GrowthAmount,
                doSnakesTurnToFood,
                ghostMode_Duration
            );

            newSnake.SetInputs(//KeyCode[] inputs
                new KeyCode[] {playerInputs[i,0], playerInputs[i,1], playerInputs[i,2], playerInputs[i,3]}
            );

            newSnake.SetPlayerDisplay(//GameObject scoreDisplay, GameObject highScoreDisplay
                playerDisplayScripts[i]
            );

            newSnake.SetColour(//Color col
                player_Colours[i]
            );

            //add the snake to the list
            snakeScripts.Add(newSnake);
            
        }

    }
    public void StartGame(){//called by the menu script when the game is loaded

        DebugAllVars();

        //spawn the first bit of food and start the game
        SpawnFood(-1, default(Vector3), "normalFood");

        //calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnakes", 0, snakeSpeed);   

    }

    private void MoveSnakes(){
        foreach (Snake_Script snakeScript in snakeScripts)
        {
            snakeScript.TryMoveSnake();
        }
    }

    public void EndGame(){
        CancelInvoke();
    }






    public string CheckPos(int playerNum, Vector3 pos, bool DestroyFood){
        //used to check if there is food or danger in a grid position

        // playerNum is the snake sending the request,
        // pos is the location to check, 
        // DestroyFood is a bool to check if the food found should be eaten

        //empty, self, otherSnake, wall, normalFood, snakeFood, GoldFood

        //returns 'e' for empty, 's' for snake, 'f' for food, or 'w' for wall
        
        if(playerNum != -1)
        {
            //check if bumping into self
            if(snakeScripts[playerNum].CheckForSnakeAtPos(pos)){
                return "self";
            }

            //check if bumping into other snakes
            for (int i = 0; i < num_Players; i++)
            {   
                if (i != playerNum){
                    if(snakeScripts[i].CheckForSnakeAtPos(pos)){
                        return "snake";
                    }
                }
            }
        }
        else{
            foreach (Snake_Script snake in snakeScripts)
            {   //check if bumping into any snakes
                if(snake.CheckForSnakeAtPos(pos)){
                    return "snake";
                }
            }
        }

        foreach (GameObject wall in wallArr)
        {   //check if bumping into wall
            if (pos == wall.transform.position){
                return "wall";
            }
        }

        foreach (GameObject food in foodList)
        {   //check if bumping into food
            if (pos == food.transform.position){
                string type = food.tag;
                switch (type)
                {
                    case "normalFood":
                        if(DestroyFood){
                            SpawnFood(-1, default(Vector3), "normalFood");
                        }
                        break;

                    case "snakeFood":
                        break;

                    case "goldFood":
                        if(DestroyFood){
                            SpawnFood(-1, default(Vector3), "normalFood");
                        }
                        break;

                    default:
                    break;
                }
                if(DestroyFood){
                    foodList.Remove(food);
                    Destroy(food);
                }
                return type;
            }
        }

        //if theres nothing in the spot,
        return "empty";
    }

    public void SpawnFood(int playerNum, Vector3 pos, string foodType)
    {
        if (pos == default){
            //find a new position for the food
            pos = new Vector3(Random.Range(0,25), Random.Range(0,25), 0);

            while (CheckPos(-1, pos, false) != "empty"){
                pos = new Vector3(Random.Range(0,25), Random.Range(0,25), 0);
            }
        }
        else if (CheckPos(-1, pos, false) == "empty"){
            //Debug.Log("not empty");
            //if trying to spawn food on an existing food, dont
            return;
        }
        GameObject newFood = Instantiate(foodPrefab, pos, new Quaternion(0,0,0,0), this.transform);

        switch (foodType)
        {
            case "normalFood":
                //chance to turn into golden food
                if((int)Random.Range(1,goldFood_SpawnChance) == 1){
                    //Debug.Log("winner");
                    foodType = "goldFood";
                    goto case "goldFood";
                }
                break;
            case "snakeFood":
                //colour food
                newFood.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.23f);
                break;
            case "goldFood":
                //colour food
                newFood.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
            break;
        }
        newFood.tag = foodType;
        foodList.Add(newFood);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
