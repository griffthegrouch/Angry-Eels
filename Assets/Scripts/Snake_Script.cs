using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake_Script : MonoBehaviour
{

        /*
        snake script

        ***Desc.
        this script controls one snake in the game.
        it is used for multiple snakes if playing multiplayer, 
        the values that control it and colour it are set automatically by the Game_Handler object

        when game starts snake is reset to starting values inc. position and size.
        when a button is pressed, the snake starts moving untill it dies
        the snake can move in one of 4 directions, one block at a time
        it moves through a method, called every x seconds by the gamehandler script
        if the snake hits food, it eats it and grows larger,
        if the snake hits another snake or a wall, it dies and is reset, with the option of turning into food


        ***Access points
        Setters -
            void SetInputs(keyCode, keyCode, keyCode, keyCode)  
                    - used by the Game_Handler to set the inputs that control this snake
            void SetColours()
                    - used by the Game_Handler to set the colours of this snake

            void TryMoveSnake(){
                    -used by the Game_Handler to move all the snakes in unison
            }

        Getters -
            bool CheckForSnakeAtPos(Vector3 pos)
                    - used by other snakes to determine if theyre colliding with this snake at given position
        
        Methods
            public void Grow(){}

            public void Die(){}
        






        ***TODO
        make snake face change when dead
        make snake untargetable and flash for a couple seconds when spawning to prevent camping it
        when snake dies, make it turn into food,
        make number of lives

    */



    //set by game handler - defines which snake this is
    int PlayerNum;
    //how big the snake starts out
    int Starting_Size;
    //how much the snake grows when it eats food
    int NormalFood_Grow_Amount; int DeadSnake_Grow_Amount; int GoldFood_Grow_Amount;

    //where the snake spawns
    Vector3 Starting_Pos;

    //if snakes turn into food when they die
    bool DoesTurnIntoFood;


    // the direction the snake is currently facing/moving Up,Down,Left,Right = u,d,l,r
    char snake_direction = 'u';

    //colours of the worm - base is set by handler, the others are generated from the base
    Color Col_Base;//snake head and alternating segments are this colour
    Color Col_Alt;//snake alternating segments are this colour
    Color Col_Outline;//outline of snake head and segments are this colour

    // colours used for flashing gold
    //snake transitions from this colour to the next and back
    Color Col_Flashing1 = new Color(1, 1, 0);
    Color Col_Flashing2 = new Color(0.9f, 0.9f, 0);
    //outline of snake head and segments are this colour
    Color Col_FlashingOutline = new Color(0.8f, 0.8f, 0);

    //used to determine if the snake can die - changed for temporary amounts of time
    bool canDie = true;

    //used for changing the colour of the snake for a temporary amount of time
    bool isFlashing = false;
    float FlashDuration = 0.3f;
    float flashTime = 0;

    //used for ghosting the snake for a temporary amount of time
    bool isGhosting = false;
    float GhostDuration = 0.4f;
    float ghostTime = 0;
    float GhostAmount1 = 0.1f;
    float GhostAmount2 = 0.3f;
    Color Col_Ghost1;
    Color Col_Ghost2;
    Color Col_GhostOutline;



    //bool tracks current score
    private int score = 0;
    //bool tracks highest score achieved
    private int highscore = 0;

    //player display displays score, highscore, as well as prompts and shows you length of temporary snake modes
    PlayerDisplay_Script PlayerDisplayScript;

    //controls for snake [Up, Down, Let, Right]
    KeyCode[] controlsArr;
    


    //a bool determining if the game is currently being played
    bool gameOn = false;
    //bool determining if the snake is currently waiting for an input to change
    bool snakeIsWait = true;





    //keeps track of all segments of the snake
    List<GameObject> segmentsList = new List<GameObject>();

    //a prefab of a segment of snake
    GameObject SegmentPrefab;
    //the first segment of snake, already existing
    GameObject SnakeHead;





    //game controlling script
    GameHandler_Script Handler_Script;

    // setter for snakes values]
    public void SetObjects(GameHandler_Script handlerScript,  GameObject head, GameObject seg){
        Handler_Script = handlerScript;
        SnakeHead = head;
        SegmentPrefab = seg;
    }
    public void SetValues(int playernum, int startingSize, Vector3 startingPos, int normalFood_Grow_Amount, int deadSnake_Grow_Amount, int goldFood_Grow_Amount, bool doesTurnIntoFood){
        PlayerNum = playernum;
        Starting_Size = startingSize;
        Starting_Pos = startingPos;
        NormalFood_Grow_Amount = normalFood_Grow_Amount;
        DeadSnake_Grow_Amount = deadSnake_Grow_Amount;
        GoldFood_Grow_Amount = goldFood_Grow_Amount;
        DoesTurnIntoFood = doesTurnIntoFood;
    }
    public void SetInputs(KeyCode[] inputs){
        controlsArr = inputs;
    }
    public void SetPlayerDisplay(PlayerDisplay_Script playerDisplayScript){
        PlayerDisplayScript = playerDisplayScript;
    }
    public void SetColour(Color col){
        Col_Base = new Color(col.r - 0.2f, col.g - 0.2f, col.b - 0.2f);
        Col_Outline = new Color(col.r - 0.55f, col.g - 0.55f, col.b - 0.55f);
        Col_Alt = new Color(col.r - 0.3f, col.g - 0.3f, col.b - 0.3f);

        //setting the colours of the snakes
        SnakeHead.GetComponent<SpriteRenderer>().color = Col_Outline;
        SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
    }

    public void TryMoveSnake(){
        if (gameOn){
            MoveSnakeHead();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    //startgame is called to refresh and start the game fresh
    void StartGame(){

        //destroy all snake segments
        foreach (GameObject seg in segmentsList)
        {
            Destroy(seg);
        }
        segmentsList = new List<GameObject>();

        //reset the score
        score = 0;

        //reset the snake's values
        SnakeHead.transform.position = Starting_Pos;

        snake_direction = 'u';

        Grow(Starting_Size);

        gameOn = true;
        snakeIsWait = true;

        StartCoroutine(GhostFor(2f));

        
        
    }

    // Update is called once per frame
    void Update() 
    {
        if(isFlashing){
            FlashColor();
        }

        if(isGhosting){
            FlashGhost();
        }
        
        
        if (gameOn == false){//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(controlsArr[0]) || Input.GetKeyDown(controlsArr[1]) || Input.GetKeyDown(controlsArr[2]) || Input.GetKeyDown(controlsArr[3]))
            { 
            //start game
            StartGame();
            }
        }

        if (snakeIsWait == false){
            if (Input.GetKeyDown(controlsArr[0])){
                //prevents attempts to change direction to current axis
                if( snake_direction != 'u' && snake_direction != 'd'){
                    snake_direction = 'u';
                    snakeIsWait = true;
                }
            }
            if (Input.GetKeyDown(controlsArr[1])){
                //prevents attempts to change direction to current axis
                if( snake_direction != 'u' && snake_direction != 'd'){
                    snake_direction = 'd';
                    snakeIsWait = true;
                }
            }
            if (Input.GetKeyDown(controlsArr[2])){
                    //prevents attempts to change direction to current axis
                    if( snake_direction != 'l' && snake_direction != 'r'){
                    snake_direction = 'l';
                    snakeIsWait = true;
                }
            }
            if (Input.GetKeyDown(controlsArr[3])){
                    //prevents attempts to change direction to current axis
                    if( snake_direction != 'l' && snake_direction != 'r'){
                    snake_direction = 'r';
                    snakeIsWait = true;
                }
            }          
        }


    }

    void MoveSnakeHead(){//moves the head of the snake which the body follows

        if (!gameOn){
            return;
        }
        snakeIsWait = false; //object is no longer waiting to move

        Vector3 offset = new Vector3(); //the direction and amount the snake will move

        Vector3 newHeadRotation = new Vector3(); //the rotation of the snake's head

        switch (snake_direction)
        {
            case 'u':
            offset = new Vector2(0,1);
            newHeadRotation = new Vector3(0,0,0);
            break;

            case 'd':
            offset = new Vector2(0,-1);
            newHeadRotation = new Vector3(0,0,180);
            break;

            case 'l':
            offset = new Vector2(-1,0);
            newHeadRotation = new Vector3(0,0,90);
            break;

            case 'r':
            offset = new Vector2(1,0);
            newHeadRotation = new Vector3(0,0,270);
            break;
            
            default:
            break;
        }

        //setting the new target position
        Vector3 targetPos = SnakeHead.transform.position + offset;

        //check if theres anything in the target position
        switch (Handler_Script.CheckPos(PlayerNum, targetPos, true))
        {
            case "normalFood"://if spot was food then eat the food
            EatFood(NormalFood_Grow_Amount);
            goto case "empty";//the act as if the target spot was empty

            case "snakeFood"://if spot was food then eat the food
            EatFood(DeadSnake_Grow_Amount);
            goto case "empty";//the act as if the target spot was empty

            case "goldFood"://if spot was food then eat the food
            StartCoroutine(FlashFor(5f));
            EatFood(GoldFood_Grow_Amount);
            goto case "empty";//the act as if the target spot was empty



            case "empty":   //if the target spot is empty
            //snake is able to move, then move the snake's head to the target and rotate it accordingly
            SnakeHead.transform.position = targetPos;
            SnakeHead.transform.rotation = Quaternion.Euler(newHeadRotation);
            //move the snake's segments toward where the head used to be
            MoveSegments(targetPos - offset);
            break;

            case "wall"://if spot was a wall
            //die
            Die();
            break;

            case "self":
            goto case "snake";

            case "snake"://if spot was a snake
            if (canDie){
                Die();
            }else{
                goto case "empty";
            }
            break;
            
            default:
            break;
        }
        
    }

    void EatFood(int amount){

        //increase and update score 
        score += amount;
        UpdateScore();

        //grow snake

        Grow(amount);

    }

    void UpdateScore()
    {
        PlayerDisplayScript.UpdateScore(score);
    }


    public void Grow(int amount){
        //called to add a segment(s) to the snake
        for (int i = 0; i < amount; i++)
        {
        GameObject newSeg = Instantiate(SegmentPrefab, SnakeHead.transform.position, new Quaternion(0,0,0,0), this.transform);
        newSeg.GetComponent<SpriteRenderer>().color = Col_Outline;
        if( segmentsList.Count%2 == 1){//odd num segment is base colour
            newSeg.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
        }
        else{//even num seg is alt colour
            newSeg.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Alt;
        }
        segmentsList.Add(newSeg);
        }
    }

    public void Die(){
        //stop the game
        gameOn = false;
        if (DoesTurnIntoFood){
            Handler_Script.SpawnFood(SnakeHead.transform.position, "snakeFood");
            SnakeHead.transform.position = Starting_Pos;
            foreach (GameObject seg in segmentsList)
            {
                Handler_Script.SpawnFood(seg.transform.position, "snakeFood");
                Destroy(seg);
            }
            segmentsList.Clear();
            
        }

    }

    public bool CheckForSnakeAtPos(Vector3 pos){ 
        if (pos == SnakeHead.transform.position){
            return true;
        }
        foreach (GameObject seg in segmentsList)
        {
            if (pos == seg.transform.position){
                return true;
            }
        }
        return false;
    }

 

    void MoveSegments(Vector3 oldPos){
        Vector3 tempPos;
        //cycles through the snake segments from top to bottom and moves them to where the next segment was
        for (int i = 0; i < segmentsList.Count; i++)
        {
            if (oldPos == segmentsList[i].transform.position){
                //if the segment was overlapping the previous one, dont move anymore segments
                return;
            }
            tempPos = segmentsList[i].transform.position;
            segmentsList[i].transform.position = oldPos;
            oldPos = tempPos;
        }

    }

    IEnumerator FlashFor(float time){
        isFlashing = true;
        canDie = false;

        yield return new WaitForSeconds (time);
        isFlashing = false;
        canDie = true;

        SnakeHead.GetComponent<SpriteRenderer>().color = Col_Outline;
        SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
        
        for (int i = 0; i < segmentsList.Count; i++)
        {
            if( i%2 == 1){
                //odd num segment is base colour
                segmentsList[i].transform.GetComponent<SpriteRenderer>().color = Col_Outline;
                segmentsList[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
            }
            else{
                //even num seg is alt colour
                segmentsList[i].transform.GetComponent<SpriteRenderer>().color = Col_Outline;
                segmentsList[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Alt;
            }
        }
    }


    void FlashColor(){



        Color col = Color.Lerp(Col_Flashing1, Col_Flashing2, flashTime);
        SnakeHead.GetComponent<SpriteRenderer>().color = Col_FlashingOutline;
        SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = col;
        
        foreach (GameObject seg in segmentsList)
        {
            seg.transform.GetComponent<SpriteRenderer>().color = Col_FlashingOutline;
            seg.transform.GetChild(0).GetComponent<SpriteRenderer>().color = col;
        }

        if (flashTime < 1){ 
            flashTime += (Time.deltaTime/FlashDuration);
        }
        else{
            flashTime = 0;
            Color tempCol = Col_Flashing1;
            Col_Flashing1 = Col_Flashing2;
            Col_Flashing2 = tempCol;
        }
    }



    IEnumerator GhostFor(float time){

        //set display to show time's duration
        PlayerDisplayScript.StartCountDown(time);
        
        canDie = false;
        isGhosting = true;

        yield return new WaitForSeconds (time);

        canDie = true;
        isGhosting = false;
        

        SnakeHead.GetComponent<SpriteRenderer>().color = Col_Outline;
        SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
        
        for (int i = 0; i < segmentsList.Count; i++)
        {
            if( i%2 == 1){
                //odd num segment is base colour
                segmentsList[i].transform.GetComponent<SpriteRenderer>().color = Col_Outline;
                segmentsList[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Base;
            }
            else{
                //even num seg is alt colour
                segmentsList[i].transform.GetComponent<SpriteRenderer>().color = Col_Outline;
                segmentsList[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = Col_Alt;
            }
        }

        
    }

    void FlashGhost(){

        float opacity = Mathf.Lerp(GhostAmount1, GhostAmount2, ghostTime);

        //float opacity = GhostAmount1;//= float.Lerp(Col_Ghost1, Col_Ghost2, ghostTime);

        Color col1 = SnakeHead.transform.GetComponent<SpriteRenderer>().color;
        Color col2 = SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        col1.a = opacity;
        col2.a = opacity;
        SnakeHead.GetComponent<SpriteRenderer>().color = col1;
        SnakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = col2;
        
        foreach (GameObject seg in segmentsList)
        {
            col1 = seg.transform.GetComponent<SpriteRenderer>().color;
            col2 = seg.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            col1.a = opacity;
            col2.a = opacity;
            seg.transform.GetComponent<SpriteRenderer>().color = col1;
            seg.transform.GetChild(0).GetComponent<SpriteRenderer>().color  = col2;
        }

        if (ghostTime < 1)
        {
            ghostTime += (Time.deltaTime/GhostDuration);
        }
        else
        {
            ghostTime = 0;
            Color tempCol = Col_Ghost1;
            Col_Ghost1 = Col_Ghost2;
            Col_Ghost2 = tempCol;
        }

    }

}
