using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerSettings
{
/////////////////////////// vars required to setup snake
    public int playerNum;
    public GameHandler_Script gameHandler_Script;
    public PlayerDisplay_Script playerDisplay_Script;

    // Prefab for the snake segment
    public GameObject segmentPrefab;
    public KeyCode[] playerInputs;
    public PlayerType playerType;
    public Color playerColour;
    public Vector2 startingPos;
    // Starting size of the snake
    public int startingSize;
    public float snakeSpeed;
    public float ghostModeDuration;
    public float deathPenaltyDuration;
    public int normalFoodGrowthAmount;
    public int deadSnakeFoodGrowthAmount;
    public int goldFoodGrowthAmount;
    public bool doSnakesTurnToFood;

    public PlayerSettings(int _playerNum, 
    GameHandler_Script _gameHandler_Script, PlayerDisplay_Script _playerDisplay_Script, 
    GameObject _segmentPrefab, KeyCode[] _playerInputs, PlayerType _playerType, 
    Color _playerColour, Vector2 _startingPos, int _startingSize, float _snakeSpeed,
    float _ghostModeDuration, float _deathPenaltyDuration,
    int _normalFoodGrowthAmount, int _deadSnakeFoodGrowthAmount, int _goldFoodGrowthAmount,
    bool _doSnakesTurnToFood
    ){
        playerNum = _playerNum;
        gameHandler_Script = _gameHandler_Script;
        playerDisplay_Script = _playerDisplay_Script;
        segmentPrefab = _segmentPrefab;
        playerInputs = _playerInputs;
        playerColour = _playerColour;
        startingPos = _startingPos;
        startingSize = _startingSize;
        snakeSpeed = _snakeSpeed;
        ghostModeDuration = _ghostModeDuration;
        deathPenaltyDuration = _deathPenaltyDuration;
        normalFoodGrowthAmount = _normalFoodGrowthAmount;
        deadSnakeFoodGrowthAmount = _deadSnakeFoodGrowthAmount;
        goldFoodGrowthAmount = _goldFoodGrowthAmount;
        doSnakesTurnToFood = _doSnakesTurnToFood;

        
    }

}


public class Snake_Script : MonoBehaviour
{

    private PlayerSettings playerSettings;
    private GameObject snakeHead;

    public Sprite segmentSpriteStraight;
    public Sprite segmentSpriteCurved;
    public Sprite segmentSpriteTail;

    // Flag indicating whether the snake is alive or not
    private bool isAlive = false;
    // List of all the segments of the snake
    private List<GameObject> segments = new List<GameObject>();
    // Current score of the snake
    private int score = 0;
    // Highest score achieved by the snake
    private int highscore = 0;
    // keeps track of how many segments need to be grown
    private int storedSegments = 0;
    // Flag indicating whether the snake can die or not
    private bool canDie = true;
    // Flag indicating whether the snake is flashing due to eating gold food
    private bool isFlashing = false;
    // Duration of flashing after eating gold food
    private float flashDuration = 0.3f;
    // Time remaining for the flashing effect
    private float flashTime = 0;

    // Color for the snake head and segments
    Color colourBase;
    // Color for the alternating segments
    Color colourAlt;
    // Color for the outline of snakes head and segments
    //Color colourOutline;

    // Color for the flashing effect
    private Color colFlashing1 = new Color(1, 1, 0);
    // Color for the flashing effect
    private Color colFlashing2 = new Color(0.9f, 0.9f, 0);
    // Color for the flashing outline effect
    private Color colFlashingOutline = new Color(0.8f, 0.8f, 0);
    // Flag indicating whether the snake is ghosting or not
    private bool isGhosting = false;
    // Time at which the snake turned ghost
    private float ghostedOnSpawnTime;
    // Duration of ghosting    // Duration of ghosting cycles - bounces snake's opacity between two values during ghosting

    private float ghostDuration = .6f;
    // Time remaining for the ghosting effect
    private float ghostTime = 0;
    // Opacity for the ghosting effect
    private float ghostOpacity1 = 0.2f;
    // Opacity for the ghosting effect
    private float ghostOpacity2 = 0.6f;
    // Color for the ghosting effect
    private Color colGhost1;
    // Color for the ghosting effect
    private Color colGhost2;
    // Color for the ghosting outline effect
    private Color colGhostOutline;
    // Duration player is unable to play after dying
    private float deathPenaltyDuration;
    // current direction of the snake 
    char currentDirection;
    // next direction of the snake (as user inputted)
    char bufferDirection;
    // Flag indicating whether the game is currently being played or not

    // Coroutine for the flashing effect
    private Coroutine flasherCoroutine;
    // Coroutine for the ghosting effect
    private Coroutine ghosterCoroutine;


    public void SetupSnake(PlayerSettings _playerSettings){

        playerSettings = _playerSettings;
        snakeHead = this.transform.GetChild(0).gameObject;

        Color col = playerSettings.playerColour;
        colourBase = col;//new Color(col.r - 0.2f, col.g - 0.2f, col.b - 0.2f);
        colourAlt = Color.Lerp(col, Color.white, .42f);
        //colourOutline = new Color(col.r - 0.55f, col.g - 0.55f, col.b - 0.55f);

        playerSettings.playerDisplay_Script.SetoutlineColour(col);
        playerSettings.playerDisplay_Script.SetValues(playerSettings.playerNum);

        //setting the colours of the snakes
        snakeHead.GetComponent<SpriteRenderer>().color = colourBase;

        //prepare the snake to starting state
        ResetSnake();
    }

    // Method to start the game for the snake
    public void StartGame()
    {
        playerSettings.playerDisplay_Script.HidePrompt();

        // Reset the snake to its starting values
        ResetSnake();

        Grow(playerSettings.startingSize - 1);// -1 because it starts as a head!

        StartCoroutine(GhostFor(playerSettings.ghostModeDuration));

        // Set the isAlive flag to true
        isAlive = true;
    }

    // Method to reset the snake to its starting values
    public void ResetSnake()
    {
        // Reset the position of the snake head to the starting position
        snakeHead.transform.position = playerSettings.startingPos;

        // Initialize the current and next direction of the snake to up
        currentDirection = bufferDirection = 'u';

        score = 0;
        UpdateScore();

    }

    private void Update()
    {
        if (isFlashing)
        {
            FlashColour();
        }

        if (isGhosting)
        {
            Debug.Log("ghosting");
            FlashGhost();
        }

        if (isAlive == false)
        {//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(playerSettings.playerInputs[0]) || Input.GetKeyDown(playerSettings.playerInputs[1]) || Input.GetKeyDown(playerSettings.playerInputs[2]) || Input.GetKeyDown(playerSettings.playerInputs[3]))
            {
                //start game
                StartGame();
            }
        }

        if (Input.GetKeyDown(playerSettings.playerInputs[0]))
        {
            //Debug.Log("pressing up");
            //prevents attempts to change direction to current axis
            if (currentDirection != 'u' && currentDirection != 'd')
            {
                bufferDirection = 'u';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[1]))
        {
            //Debug.Log("pressing down");
            //prevents attempts to change direction to current axis
            if (currentDirection != 'u' && currentDirection != 'd')
            {
                bufferDirection = 'd';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[2]))
        {
            //Debug.Log("pressing left");
            //prevents attempts to change direction to current axis
            if (currentDirection != 'l' && currentDirection != 'r')
            {
                bufferDirection = 'l';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[3]))
        {
            //Debug.Log("pressing right");
            //prevents attempts to change direction to current axis
            if (currentDirection != 'l' && currentDirection != 'r')
            {
                bufferDirection = 'r';
            }
        }
        //Debug.Log("current dir: " + currentDirection);
        //Debug.Log("buffered dir: " + bufferDirection);
    }

    public void Die()
    {
        //stop the game
        isAlive = false;

        //reset snake values
        StopAllCoroutines();
        isFlashing = false;
        isGhosting = false;
        canDie = true;
        ResetSnakeColours();

        if (playerSettings.doSnakesTurnToFood)
        {
            
            playerSettings.gameHandler_Script.SpawnFood(playerSettings.playerNum, snakeHead.transform.position, EntityType.DeadSnakeFood);
            foreach (GameObject seg in segments)
            {
                playerSettings.gameHandler_Script.SpawnFood(playerSettings.playerNum, seg.transform.position, EntityType.DeadSnakeFood);
                Destroy(seg);
            }
        }
        else{
            foreach (GameObject seg in segments)
            {
                Destroy(seg);
            }
        }
        snakeHead.transform.position = playerSettings.startingPos;
        segments.Clear();
        playerSettings.playerDisplay_Script.UpdateScore(0);
        playerSettings.playerDisplay_Script.StopCountdown();

    }

    public void TryMoveSnake()
    {//called by game handler
        //1 - checks if able to move at all
        //2 - determine the target spot
        //3 - checks if anything is occupying target spot
        //    + switch statement to respond to what is in target spot


        //1
        if (!isAlive)
        {
            return;
        }

        //2
        currentDirection = bufferDirection;
        
        Vector3 offset = Vector3.zero;
        Vector3 newHeadRotation = Vector3.zero;
        switch (currentDirection)
        {
            case 'u':
                offset = new Vector3(0, 1, 0);
                newHeadRotation = new Vector3(0, 0, 0);
                break;

            case 'd':
                offset = new Vector3(0, -1, 0);
                newHeadRotation = new Vector3(0, 0, 180);
                break;

            case 'l':
                offset = new Vector3(-1, 0, 0);
                newHeadRotation = new Vector3(0, 0, 90);
                break;

            case 'r':
                offset = new Vector3(1, 0, 0);
                newHeadRotation = new Vector3(0, 0, 270);
                break;

            default:
                break;
        }

        //setting the new target position
        Vector3 targetPos = snakeHead.transform.position + offset;

        // 3 
        //check if theres anything in the target position
        switch (playerSettings.gameHandler_Script.CheckPos(playerSettings.playerNum, targetPos, true))
        {
            case EntityType.NormalFood://if spot was food then eat the food
                EatFood(playerSettings.normalFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.DeadSnakeFood://if spot was food then eat the food
                EatFood(playerSettings.deadSnakeFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.GoldFood://if spot was food then eat the food
                //flash gold for the duration that the snake is growing from the extra food
                if (isFlashing)
                {
                    StopCoroutine(flasherCoroutine);
                }
                EatFood(playerSettings.goldFoodGrowthAmount);
                flasherCoroutine = StartCoroutine(FlashFor(playerSettings.snakeSpeed * playerSettings.goldFoodGrowthAmount));
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.Empty:   //if the target spot is empty
                //snake is able to move, then move the snake's head to the target and rotate it accordingly
                MoveSnake(targetPos, newHeadRotation);
                break;

            case EntityType.Wall://if spot was a wall ---> die
                Die();
                break;

            case EntityType.Self:
                goto case EntityType.Snake;

            case EntityType.Snake://if spot was a snake --->die
                if (canDie)
                {
                    Die();
                }
                else
                {
                    goto case EntityType.Empty;
                }
                break;

            default:
                break;
        }

    }
    void MoveSnake(Vector3 newPos, Vector3 newHeadRotation)
    {
        //0 - grow if able
        //if snake has stored segments, grow one segment, and reflect the change 
        if(storedSegments > 0){

            storedSegments -= 1;
            // Instantiate a new segment prefab 
            GameObject newSegment = Instantiate(playerSettings.segmentPrefab, snakeHead.transform.position, Quaternion.identity, transform);

            // Set the color of the segment outline to the player colour
            newSegment.GetComponent<SpriteRenderer>().color = colourBase;

            // Set the color of the segment fill to regular or alt colour
            Color col = colourAlt;
            if(segments.Count % 2 == 1) col = colourBase;
            newSegment.transform.GetComponent<SpriteRenderer>().color = col;

            // Add the new segment to the list of segments
            segments.Add(newSegment);
        }

        //1- move the head
        Vector3 oldPos = snakeHead.transform.position;
        //snake is able to move, then move the snake's head to the target and rotate it accordingly
        snakeHead.transform.position = newPos;
        snakeHead.transform.rotation = Quaternion.Euler(newHeadRotation);

        //2- move the segments 

        //move the snake's segments toward where the head used to be
        Vector3 tempPos;

        //cycles through the snake segments from top to bottom and moves them to where the next segment was
        for (int i = 0; i < segments.Count; i++)
        {
            if (oldPos == segments[i].transform.position)
            {
                //if the segment was overlapping the previous one, dont move anymore segments
                return;
            }
            tempPos = segments[i].transform.position;
            segments[i].transform.position = oldPos;
            oldPos = tempPos;
            
        }

        //after colouring snake correctly, update its segment's sprites + facing directions
        UpdateSegmentDirections();

    }
    void UpdateSegmentDirections()
    {
        //changes the segment's sprites to the correct one (tail or body) and rotation

        //uses the relative position of the previous and next segments to determine 
        //what sprite + angle to use for each segment (straight, curved, or tail)

        //direction of the seg ahead of current one (starts on head)
        Vector2 previousSegmentDir =  Vector2.zero;
        Vector2 nextSegmentDir = Vector2.zero;
        Vector3 newRotation = Vector3.zero;
        Sprite newSprite;
        GameObject currentSeg;
        

        //cycle through each segment
        for (int i = 0; i < segments.Count; i++)
        {
            currentSeg = segments[i];

            //determining the relative positions of the segments before and after current segment
            // the first segment (uses snake head as previous seg)
            if(i == 0){
                previousSegmentDir = snakeHead.transform.position - segments[i].transform.position; 
                nextSegmentDir = segments[i+1].transform.position - segments[i].transform.position; 
            }
            //the last segment (only needs previous seg value)
            else if(i == segments.Count -1){
                previousSegmentDir = segments[i-1].transform.position - segments[i].transform.position; 
            }
            else{//all the segments between
                previousSegmentDir = segments[i-1].transform.position - segments[i].transform.position; 
                nextSegmentDir = segments[i+1].transform.position - segments[i].transform.position; 
            }
            
            //if its not the last segment 
            if(i != segments.Count - 1){
                //determing if its a horizontal straight segment
                if(previousSegmentDir.y == 0 && nextSegmentDir.y == 0){
                    newSprite = segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 90);
                }
                //determing if its a vertical straight segment
                else if(previousSegmentDir.x == 0 && nextSegmentDir.x == 0){
                    newSprite = segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 0);
                }
                else {
                    newSprite = segmentSpriteCurved;
                    //if its not a straight segment, determine which direction the curve needs to go
                    
                    switch (previousSegmentDir + nextSegmentDir)
                    {
                        case var value when value == new Vector2(-1,1):     //above + left
                            newRotation = new Vector3(0, 0, 180);
                            break;
                        case var value when value == new Vector2(1, 1):     //above + right
                            newRotation = new Vector3(0, 0, 90);
                            break;
                        case var value when value == new Vector2(-1,-1):    //below+  left
                            newRotation = new Vector3(0, 0, 270);
                            break;
                        case var value when value == new Vector2(1,-1):     //below + right
                            newRotation = new Vector3(0, 0, 0);
                            break;
                        default:
                            break;
                    }
                }

            }
            //if its the last segment (tail)
            else{
                newSprite = segmentSpriteTail;

                //using the previous segment's relative position to determine which way to angle tail
                switch (previousSegmentDir)
                {
                    case var value when value == Vector2.up: //above
                        newRotation = new Vector3(0, 0, 0);
                        break;
                    case var value when value == Vector2.down: //below
                        newRotation = new Vector3(0, 0, 180);
                        break;
                    case var value when value == Vector2.left: //to the left
                        newRotation = new Vector3(0, 0, 90);
                        break;
                    case var value when value == Vector2.right: // to the right
                        newRotation = new Vector3(0, 0, 270);
                        break;
                    default:
                        break;
                }
            }

            //setting the new sprite and rotation
            currentSeg.GetComponent<SpriteRenderer>().sprite = newSprite;
            
            currentSeg.transform.rotation = Quaternion.Euler(newRotation);   
            
        }
    }

    public void EatFood(int amount)
    {
        Grow(amount);
    }

    void UpdateScore()
    {
        if (score > highscore)
        {
            highscore = score;
        }
        playerSettings.playerDisplay_Script.UpdateScore(score);
    }

    // Method to make the snake grow by a certain amount
    public void Grow(int amount)
    {
        //increase the number of stored segments (will be grown when trying to move)
        storedSegments += amount;
        // Increase the score by the amount of growth
        score += amount;

        // Update the score display
        UpdateScore();
        
        
    }

    public bool CheckForSnakeAtPos(Vector3 pos)
    {
        if (pos == snakeHead.transform.position)
        {
            return true;
        }
        foreach (GameObject seg in segments)
        {
            if (pos == seg.transform.position)
            {
                return true;
            }
        }
        return false;
    }

    // Method to make the snake flash gold for a certain duration
    private IEnumerator FlashFor(float duration)
    {
        // Set the countdown display to show the duration of the golden time
        playerSettings.playerDisplay_Script.StartCountdown(duration, Color.yellow);

        // Set the flashing and invincibility flags to true
        isFlashing = true;
        canDie = false;

        // Wait for the duration of the golden time
        yield return new WaitForSeconds(duration);

        // Set the flashing and invincibility flags to false
        isFlashing = false;
        canDie = true;

        // Reset the snake's colors to their normal colors
        ResetSnakeColours();
    }
    private void FlashColour()
    {
        // Calculate the current color of the snake by lerping between colFlashing1 and colFlashing2
        Color colour = Color.Lerp(colFlashing1, colFlashing2, flashTime);

        // Set the colors of the snake's head and segments
        SetSnakeColours(colFlashing1, colFlashing2);// colour);

        // Increment the flash time
        flashTime += Time.deltaTime / flashDuration;

        // If the flash time has reached 1, reset it and swap the flashing colors
        if (flashTime >= 1)
        {
            flashTime = 0;
            Color tempColour = colFlashing1;
            colFlashing1 = colFlashing2;
            colFlashing2 = tempColour;
        }
    }
    private IEnumerator GhostFor(float duration)
    {
        // Set the countdown display to show the duration of the ghosting
        playerSettings.playerDisplay_Script.StartCountdown(duration, Color.white);

        // Set the invincibility and ghosting flags to true
        canDie = false;
        isGhosting = true;

        // Wait for the duration of the ghosting
        yield return new WaitForSeconds(duration);

        // Set the invincibility and ghosting flags to false
        canDie = true;
        isGhosting = false;

        // Reset the snake's colors to their normal colors
        ResetSnakeColours();
    }
    private void FlashGhost()
    {
        // Calculate the current opacity of the snake by lerping between ghostOpacity1 and ghostOpacity2
        float opacity = Mathf.Lerp(ghostOpacity1, ghostOpacity2, ghostTime);

        // Set the colors of the snake's head and segments
        SetSnakeOpacity(opacity);

        // Increment the ghost time
        ghostTime += Time.deltaTime / ghostDuration;

        // If the ghost time has reached 1, reset it and swap the ghosting colors
        if (ghostTime >= 1)
        {
            ghostTime = 0;
            Color tempColor = colGhost1;
            colGhost1 = colGhost2;
            colGhost2 = tempColor;
        }
    }

    void SetSnakeColours(Color _colourBase, Color _colourAlt)// Color _colourOutline)
    {
        // Set the color of the snake's outline and base
        snakeHead.GetComponent<SpriteRenderer>().color = _colourBase;

        // Set the color of the segments' outline and base
        for (int i = 0; i < segments.Count; i++)
        {
            Color col = _colourAlt;
            if(i%2 == 1) col = _colourBase;
            segments[i].transform.GetComponent<SpriteRenderer>().color = col;
        }
    }
    void SetSnakeOpacity(float opacity)
    {
        Color baseColorWithOpacity = colourBase;
        baseColorWithOpacity.a = opacity;

        Color altColorWithOpacity = colourAlt;
        altColorWithOpacity.a = opacity;

        //Color outlineColorWithOpacity = colourOutline;
        //outlineColorWithOpacity.a = opacity;

        SetSnakeColours(baseColorWithOpacity, altColorWithOpacity);//, outlineColorWithOpacity);
    }

    void ResetSnakeColours()
    {
        // Reset the snake's colors to the original outline and base colors
        SetSnakeColours(colourBase, colourAlt);//, colourOutline);
    }
}