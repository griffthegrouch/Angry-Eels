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
    public char CheckPos()
        -called by snakes to determine what is in x location


    
    ***Snake Game 
    player story
    1 -> starts with GUI
    2 -> choose game settings
        -choose number of players
        -choose what players are human vs CPU
        -choose player's colours
        -choose game mode (regular - single food, hungry - multiple foods)
        -choose settings (do snakes turn into food when they die, do snakes respawn, grow amount, staring size, etc)
        -choose stage (original, preset map, or random obstacles)
    3 -> click play 
    4 -> GUI leaves and snake game is displayed 
    5 -> player presses any key and game starts then continues indefinitely

    Handler script story
    0 -> game starts/loads
    1 -> grabs values from user-set settings
    2 -> grabs prefabs and converts variables into "usable data" 
    3 -> creates the stage, starting food, and spawns snakes
    4 -> starts moving snakes
        - when snakes need to check what is occupying a position on the map
            -> call this script requesting it, then recieve a char to signal what is in that position
        -if the snake is trying to move to a food block
            -> destroy the food block - if appropriate, generate a new food block



    TODO
                    

        make handler for game to choose all game options outside of play mode.
        make this handler move the snakes at random orders, so that when they run into each other, its not the first one that trumps the other

        make snake untargetable and flash for a couple seconds when spawning to prevent camping it
        when snake dies, make it turn into food,
        make number of lives
        make random obstacle maker, and preset stages to choose from.
        make high score file to keep reading from.
        add powerups (speed, grow a lot, untargettable)
        make options for food (multiple random spawning, single one that grows you then moves, toggle for dead snakes becoming food )
        golden food that moves/bounces around
    
    DONE
                    make one snake script to use for all snakes
                    make colour selection option per snake
                    make slightly alternating colours on the segments
    */

    //public 
    public Color Player1_Colour;
    public Color Player2_Colour;
    public Color Player3_Colour;
    public Color Player4_Colour;

    public int Number_Of_Players;

    public int NormalFood_Grow_Amount;
    public int DeadSnake_Grow_Amount;
    public int GoldFood_Grow_Amount;
    
    public int Starting_Size;
    public float Snake_Speed; //typically 0.1f
    public bool Do_Snakes_Turn_Into_Food;
    public int GoldFood_Spawn_Chance; //when spawning normal food, picks a random number between 1 - (this #)
    [Range(0,100)]

    public int Death_Penatly_Timer;





    Vector3[] StartingPositions = new Vector3[4] {
        new Vector3(3,-1,0), 
        new Vector3(9,-1,0),
        new Vector3(15,-1,0),
        new Vector3(21,-1,0)
        };
    Color[] Player_Colours;

    KeyCode[,] PlayerInputs = new KeyCode[,] {
        {KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow},
        {KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D},
        {KeyCode.Y, KeyCode.H, KeyCode.G, KeyCode.J},
        {KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D}

    };

    //array keeps track of all the wall blocks in the game
    GameObject[] wallArr;

    //list keeps track of all the food in the game
    List<GameObject> foodList = new List<GameObject>();

    //list keeps track of all the snakes in the game
    List<Snake_Script> snakeScripts = new List<Snake_Script>();

    //Array keeps track of all 4 score displays
    GameObject[] PlayerDisplays;
    PlayerDisplay_Script[] PlayerDisplayScripts;

    GameObject SnakePrefab;
    GameObject SnakeSegmentPrefab;
    GameObject FoodPrefab;

    // Start is called before the first frame update
    void Start(){


        // grab all resources
        SnakePrefab = Resources.Load("Snake") as GameObject;
        SnakeSegmentPrefab = Resources.Load("SnakeSegment") as GameObject;
        FoodPrefab = Resources.Load("Food") as GameObject;
        
        // grab all existing walls
        wallArr = GameObject.FindGameObjectsWithTag("wall");

        // organize vars into more easily usable formats
        Player_Colours = new Color[] {Player1_Colour, Player2_Colour, Player3_Colour, Player4_Colour};

        //grab player displays and set vars
        PlayerDisplays = new GameObject[GameObject.Find("PlayerDisplays").transform.childCount];
        PlayerDisplayScripts = new PlayerDisplay_Script[PlayerDisplays.Length];
        for (int i = 0; i < PlayerDisplays.Length; i++)
        {
            PlayerDisplays[i] = GameObject.Find("PlayerDisplays").transform.GetChild(i).gameObject;
            PlayerDisplays[i].SetActive(false);
            PlayerDisplayScripts[i] = PlayerDisplays[i].GetComponent<PlayerDisplay_Script>();
            PlayerDisplayScripts[i].SetValues(i, Death_Penatly_Timer, Color.red, Color.green);
        }

        for (int i = 0; i < Number_Of_Players; i++)
        {
            //create the snake
            Snake_Script newSnake = Instantiate(SnakePrefab, new Vector3(0,-10,0), new Quaternion(0,0,0,0), this.transform.parent).GetComponent<Snake_Script>();
            // enable the snake's score display
            PlayerDisplays[i].SetActive(true);

            //set the snake's values and objects
            newSnake.SetObjects(//GameHandler_Script handlerScript,  GameObject head, GameObject seg
                this,
                newSnake.transform.GetChild(0).gameObject, 
                SnakeSegmentPrefab
            );

            newSnake.SetValues(//int playernum, int startingSize, Vector3 startingPos, int foodgrowRate, bool doesTurnIntoFood
                i,
                Starting_Size,
                StartingPositions[i],
                NormalFood_Grow_Amount,
                DeadSnake_Grow_Amount,
                GoldFood_Grow_Amount,
                Do_Snakes_Turn_Into_Food
            );

            newSnake.SetInputs(//KeyCode[] inputs
                new KeyCode[] {PlayerInputs[i,0], PlayerInputs[i,1], PlayerInputs[i,2], PlayerInputs[i,3]}
            );

            newSnake.SetPlayerDisplay(//GameObject scoreDisplay, GameObject highScoreDisplay
                PlayerDisplayScripts[i]
            );

            newSnake.SetColour(//Color col
                Player_Colours[i]);


            //add the snake to the list
            snakeScripts.Add(newSnake);


        }
        


        StartGame();


    }

    void StartGame(){

        //spawn the first bit of food and start the game
        SpawnFood(default(Vector3), "normalFood");

        //calls Movesnake every user-set time increment to move the snakes
        InvokeRepeating("MoveSnakes", 0, Snake_Speed);   
    }

    void EndGame(){
        CancelInvoke();
    }

    void MoveSnakes(){
        foreach (Snake_Script snakeScript in snakeScripts)
        {
            snakeScript.TryMoveSnake();
        }
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
            for (int i = 0; i < Number_Of_Players; i++)
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
                            SpawnFood(default(Vector3), "normalFood");
                        }
                        break;

                    case "snakeFood":
                        break;

                    case "goldFood":
                        if(DestroyFood){
                            SpawnFood(default(Vector3), "normalFood");
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

    public void SpawnFood(Vector3 pos, string foodType)
    {

        if (pos == default){
            //find a new position for the food
            pos = new Vector3(Random.Range(0,25), Random.Range(0,25), 0);

            while (CheckPos(-1, pos, false) != "empty"){
                pos = new Vector3(Random.Range(0,25), Random.Range(0,25), 0);
            }

        }
        GameObject newFood = Instantiate(FoodPrefab, pos, new Quaternion(0,0,0,0), this.transform);
        switch (foodType)
        {
            case "normalFood":
                //chance to turn into golden food
                if((int)Random.Range(1,GoldFood_Spawn_Chance) == 1){
                    Debug.Log("winner");
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
