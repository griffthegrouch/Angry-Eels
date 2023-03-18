using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerSettings
{
/////////////////////////// vars required to setup snake
    public int playerNum{get; set;}
    public GameHandler_Script gameHandler_Script{get; set;}
    public PlayerGUI_Script playerGUIScript{get; set;}
    public KeyCode[] playerInputs{get; set;}

    public Color playerColour{get; set;}
    public Vector2 startingPos{get; set;}
    public int startingSize{get; set;}    // Starting size of the snake
    public float snakeSpeed{get; set;}
    public float ghostModeDuration{get; set;}    // Duration player is invincible directly after spawning
    public float deathPenaltyDuration{get; set;}    // Duration player is unable to play after dying
    public int normalFoodGrowthAmount{get; set;}
    public int deadSnakeFoodGrowthAmount{get; set;}
    public int goldFoodGrowthAmount{get; set;}
    public bool doSnakesTurnToFood{get; set;}

    public PlayerSettings(int _playerNum, 
    GameHandler_Script _gameHandler_Script, PlayerGUI_Script _playerGUIScript, 
    KeyCode[] _playerInputs,
    Color _playerColour, Vector2 _startingPos, int _startingSize, float _snakeSpeed,
    float _ghostModeDuration, float _deathPenaltyDuration,
    int _normalFoodGrowthAmount, int _deadSnakeFoodGrowthAmount, int _goldFoodGrowthAmount,
    bool _doSnakesTurnToFood
    ){
        playerNum = _playerNum;
        gameHandler_Script = _gameHandler_Script;
        playerGUIScript = _playerGUIScript;
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
public class PlayerResources
{
    public GameObject segmentPrefab{get; set;}
    public Sprite segmentSpriteStraight {get; set;}
    public Sprite segmentSpriteCurved {get; set;}
    public Sprite segmentSpriteTail {get; set;}
    public AudioClip chompSFX {get; set;}
    public AudioClip deathSFX {get; set;}
    public AudioClip yummySFX {get; set;}
    public AudioClip crashSFX {get; set;}
    public AudioClip popSFX {get; set;}
    public AudioClip popMultipleSFX {get; set;}
    public AudioClip powerUpSFX {get; set;}
    public PlayerResources(
        GameObject _segmentPrefab, Sprite _segmentSpriteStraight, Sprite _segmentSpriteCurved, Sprite _segmentSpriteTail,
        AudioClip _chompSFX, AudioClip _deathSFX, AudioClip _yummySFX, AudioClip _crashSFX, 
        AudioClip _popSFX, AudioClip _popMultipleSFX, AudioClip _powerUpSFX)
    {
        segmentPrefab = _segmentPrefab;
        segmentSpriteStraight = _segmentSpriteStraight;
        segmentSpriteCurved = _segmentSpriteCurved;
        segmentSpriteTail = _segmentSpriteTail;
        chompSFX = _chompSFX;
        deathSFX = _deathSFX;
        yummySFX = _yummySFX;
        crashSFX = _crashSFX;
        popSFX = _popSFX;
        popMultipleSFX = _popMultipleSFX;
        powerUpSFX = _powerUpSFX;
    }
}


public class Snake_Script : MonoBehaviour
{
    private PlayerSettings playerSettings;
    private PlayerResources playerResources;
    private GameObject snakeHead;
    private bool isAlive = false;    // Flag indicating whether the snake is alive or not
    private List<GameObject> segments = new List<GameObject>();    // List of all the segments of the snake
    private int score = 0;    // Current score of the snake (how many segments long)
    private int storedSegments = 0;    // keeps track of how many segments need to be grown
    private bool canDie = true;    // Flag indicating whether the snake can die or not
    private bool isFlashing = false;       // Flag indicating whether the snake is flashing due to eating gold food
    private float flashDuration = 0.3f;    // Duration of flashing after eating gold food
    private float flashTimer = 0;    // Time remaining for the flashing effect
    private Color colourBase;   // Color for the snake head and odd numbered segments
    private Color colourAlt;    // Color for the even numbered segments
    private Color colFlashing1 = new Color(1, 1, 0);    // Color for the gold flashing effect of the snake head and odd numbered segments
    private Color colFlashing2 = new Color(0.9f, 0.9f, 0);    // Color for the flashing effect
    private bool isGhosting = false;    // Flag indicating whether the snake is ghosting or not
    private float ghostDuration = .6f;  // Duration of ghosting cycles - bounces snake's opacity between two values during ghosting
    private float ghostTimer = 0;    // Time remaining for the ghosting effect
    private float ghostOpacity1 = 0.2f;   // Opacity for the ghosting effect of the snake head and odd numbered segments
    private float ghostOpacity2 = 0.6f;   // Opacity for the ghosting effect of the even numbered segments   
    private Color colGhost1;    // Color for the ghosting effect of the snake head and odd numbered segments
    private Color colGhost2;    // Color for the ghosting effect of the even numbered segments
    private float deathTimer = 0;    // Timer remaining for when player death penalty is active
    private char currentDirection;   // current direction of the snake 
    private char verticalBufferDirection;   // next vertical (U or D) direction of the snake (as user inputted)
    private char horizontalBufferDirection; // next horizontal (L or R) direction of the snake (as user inputted)
    private Coroutine flasherCoroutine;     // Coroutine for the flashing effect
    private Coroutine ghosterCoroutine;     // Coroutine for the ghosting effect

    public void SetupSnake(PlayerSettings _playerSettings, PlayerResources _playerResources){

        playerSettings = _playerSettings;
        playerResources = _playerResources;

        snakeHead = this.transform.GetChild(0).gameObject;

        Color col = playerSettings.playerColour;
        colourBase = col;//new Color(col.r - 0.2f, col.g - 0.2f, col.b - 0.2f);
        colourAlt = Color.Lerp(col, Color.white, .42f);
        //colourOutline = new Color(col.r - 0.55f, col.g - 0.55f, col.b - 0.55f);

        playerSettings.playerGUIScript.SetValues( playerSettings.gameHandler_Script, playerSettings.playerNum, playerSettings.playerColour);

        //setting the colours of the snakes
        snakeHead.GetComponent<SpriteRenderer>().color = colourBase;

        //prepare the snake to starting state
        ResetSnake();
    }

    // Method to start the game for the snake
    private void StartGame()
    {
        playerSettings.playerGUIScript.HidePrompt();

        // Reset the snake to its starting values
        ResetSnake();

        //reset the direction and buffer on the snake so it moves up originally
        verticalBufferDirection = 'x';
        currentDirection = horizontalBufferDirection = 'u';

        score = 1;

        Grow(playerSettings.startingSize - 1);// -1 because it starts as a head!

        ghosterCoroutine = StartCoroutine(GhostFor(playerSettings.ghostModeDuration));

        // Set the isAlive flag to true
        isAlive = true;
    }

    // Method to reset the snake to its starting values
    public void ResetSnake()
    {
        // Reset the position of the snake head to the starting position
        snakeHead.transform.position = playerSettings.startingPos;
        snakeHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // Initialize the current and next direction of the snake to up
        verticalBufferDirection = 'x';
        currentDirection = horizontalBufferDirection = 'u';

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
            FlashGhost();
        }

        if(deathTimer != 0){
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0){ 
                deathTimer = 0;
                ResetSnakeColours();
            }
            return;
        }

        if (isAlive == false)
        {//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(playerSettings.playerInputs[0]) || Input.GetKeyDown(playerSettings.playerInputs[1]) || Input.GetKeyDown(playerSettings.playerInputs[2]) || Input.GetKeyDown(playerSettings.playerInputs[3]))
            {
                //start game
                StartGame();
                return;
            }
        }

        if (Input.GetKeyDown(playerSettings.playerInputs[0]))
        {   //pressing up
            //prevents attempts to buffer direction to current axis - unless theres already a buffer on the other axis
            if ( (currentDirection != 'u' && currentDirection != 'd') || (horizontalBufferDirection != 'x') )
            {
                verticalBufferDirection = 'u';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[1]))
        {   //pressing down
            //prevents attempts to buffer direction to current axis - unless theres already a buffer on the other axis
            if ( (currentDirection != 'u' && currentDirection != 'd') || (horizontalBufferDirection != 'x') )
            {
                verticalBufferDirection = 'd';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[2]))
        {   //pressing left
            //prevents attempts to buffer direction to current axis - unless theres already a buffer on the other axis
            if ( (currentDirection != 'l' && currentDirection != 'r') || (verticalBufferDirection != 'x') )
            {
                horizontalBufferDirection = 'l';
            }
        }
        if (Input.GetKeyDown(playerSettings.playerInputs[3]))
        {   //pressing right
            //prevents attempts to buffer direction to current axis - unless theres already a buffer on the other axis
            if ( (currentDirection != 'l' && currentDirection != 'r') || (verticalBufferDirection != 'x') )
            {
                horizontalBufferDirection = 'r';
            }
        }
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
        segments.Clear();
        playerSettings.playerGUIScript.UpdateScore(0);
        playerSettings.playerGUIScript.StopCountdown();

        // Reset the snake to its starting values
        ResetSnake();


        //if death penalty is on, set up death timer and change snake colour
        if(playerSettings.deathPenaltyDuration != 0){
            SetSnakeColours(Color.grey, Color.grey);
            playerSettings.playerGUIScript.StartDeathPenaltyMode(playerSettings.deathPenaltyDuration);
            deathTimer = playerSettings.deathPenaltyDuration;
        }        

    }

    public Vector2 TryMoveSnake()
    {//called by game handler
        //1 - checks if able to move at all
        //2 - determine the target spot
        //3 - checks if anything is occupying target spot
        //    + switch statement to respond to what is in target spot

        // returns the target position

        //1
        if (!isAlive)
        {
            return snakeHead.transform.position;
        }

        //2
        if ((currentDirection == 'u' || currentDirection == 'd') && horizontalBufferDirection != 'x'){
            //currently moving vertically - and have a horizontal buffered direction
            currentDirection = horizontalBufferDirection;
            horizontalBufferDirection = 'x';
        }
        else if ((currentDirection == 'l' || currentDirection == 'r') && verticalBufferDirection != 'x'){
            //currently moving horizontal - and have a vertical buffered direction
            currentDirection = verticalBufferDirection;
            verticalBufferDirection = 'x';
        }
        
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
                //play sfx
                if(playerSettings.normalFoodGrowthAmount >= 3){
                    playerSettings.gameHandler_Script.PlaySFX(playerResources.popMultipleSFX);
                }else{
                    playerSettings.gameHandler_Script.PlaySFX(playerResources.popSFX);
                }
                EatFood(playerSettings.normalFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.DeadSnakeFood://if spot was food then eat the food
                //play sfx
                playerSettings.gameHandler_Script.PlaySFX(playerResources.popSFX);
                
                EatFood(playerSettings.deadSnakeFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.GoldFood://if spot was food then eat the food
                //play sfx
                playerSettings.gameHandler_Script.PlaySFX(playerResources.yummySFX);
                playerSettings.gameHandler_Script.PlaySFX(playerResources.popMultipleSFX);
                playerSettings.gameHandler_Script.PlaySFX(playerResources.powerUpSFX);

                //flash gold for the duration that the snake is growing from the extra food
                if (isFlashing)
                {
                    StopCoroutine(flasherCoroutine);
                }
                EatFood(playerSettings.goldFoodGrowthAmount);
                flasherCoroutine = StartCoroutine(FlashFor(playerSettings.snakeSpeed * playerSettings.goldFoodGrowthAmount));
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.Wall://if spot was a wall ---> die
                //play sfx
                playerSettings.gameHandler_Script.PlaySFX(playerResources.crashSFX);
                //playerSettings.gameHandler_Script.PlaySFX(deathSFX, 10);
                Die();
                break;

            case EntityType.Self:
                //play sfx
                goto case EntityType.Snake;

            case EntityType.Snake://if spot was a snake --->die
                if (canDie)
                {
                    //play sfx
                    playerSettings.gameHandler_Script.PlaySFX(playerResources.chompSFX);
                    //playerSettings.gameHandler_Script.PlaySFX(deathSFX, 20);
                    Die();
                }
                else
                {
                    goto case EntityType.Empty;
                }
                break;

            case EntityType.Empty:   //if the target spot is empty
                //snake is able to move, then move the snake to the "empty" location
                MoveSnake(targetPos, newHeadRotation);
                break;

            default:
                break;
        }

        return targetPos;

    }
    void MoveSnake(Vector3 newPos, Vector3 newHeadRotation)
    {
        //0 - grow if able
        //if snake has stored segments, grow one segment, and reflect the change 
        if(storedSegments > 0){

            storedSegments -= 1;
            // Instantiate a new segment prefab 
            GameObject newSegment = Instantiate(playerResources.segmentPrefab, snakeHead.transform.position, Quaternion.identity, transform);
            Debug.Log(newSegment.transform.position);
            // Set the color of the segment outline to the player colour
            newSegment.GetComponent<SpriteRenderer>().color = colourBase;

            // Set the color of the segment fill to regular or alt colour
            Color col = colourAlt;
            if(segments.Count % 2 == 1) col = colourBase;
            newSegment.transform.GetComponent<SpriteRenderer>().color = col;

            // Add the new segment to the list of segments
            segments.Add(newSegment);
        }
        Debug.Log(snakeHead);
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
        Sprite newSprite = playerResources.segmentSpriteStraight;
        GameObject currentSeg;
        
        Debug.Log(newSprite);

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
                    newSprite = playerResources.segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 90);
                }
                //determing if its a vertical straight segment
                else if(previousSegmentDir.x == 0 && nextSegmentDir.x == 0){
                    newSprite = playerResources.segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 0);
                }
                else {
                    newSprite = playerResources.segmentSpriteCurved;
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
                newSprite = playerResources.segmentSpriteTail;

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

    private void EatFood(int amount)
    {
        Grow(amount);
    }

    void UpdateScore()
    {
        playerSettings.playerGUIScript.UpdateScore(score);
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
        //if ghosted - override it
        isGhosting = false;

        // Set the countdown display to show the duration of the golden time
        playerSettings.playerGUIScript.StartGoldMode(duration);

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
        Color colour = Color.Lerp(colFlashing1, colFlashing2, flashTimer);

        // Set the colors of the snake's head and segments
        SetSnakeColours(colFlashing1, colFlashing2);// colour);

        // Increment the flash time
        flashTimer += Time.deltaTime / flashDuration;

        // If the flash time has reached 1, reset it and swap the flashing colors
        if (flashTimer >= 1)
        {
            flashTimer = 0;
            Color tempColour = colFlashing1;
            colFlashing1 = colFlashing2;
            colFlashing2 = tempColour;
        }
    }
    private IEnumerator GhostFor(float duration)
    {
        // Set the countdown display to show the duration of the ghosting
        playerSettings.playerGUIScript.StartGhostMode(duration);

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
        float opacity = Mathf.Lerp(ghostOpacity1, ghostOpacity2, ghostTimer);

        // Set the colors of the snake's head and segments
        SetSnakeOpacity(opacity);

        // Increment the ghost time
        ghostTimer += Time.deltaTime / ghostDuration;

        // If the ghost time has reached 1, reset it and swap the ghosting colors
        if (ghostTimer >= 1)
        {
            ghostTimer = 0;
            Color tempColor = colGhost1;
            colGhost1 = colGhost2;
            colGhost2 = tempColor;
        }
    }

    private void SetSnakeColours(Color _colourBase, Color _colourAlt)// Color _colourOutline)
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
    private void SetSnakeOpacity(float opacity)
    {
        Color baseColorWithOpacity = colourBase;
        baseColorWithOpacity.a = opacity;

        Color altColorWithOpacity = colourAlt;
        altColorWithOpacity.a = opacity;

        //Color outlineColorWithOpacity = colourOutline;
        //outlineColorWithOpacity.a = opacity;

        SetSnakeColours(baseColorWithOpacity, altColorWithOpacity);//, outlineColorWithOpacity);
    }

    private void ResetSnakeColours()
    {
        // Reset the snake's colors to the original outline and base colors
        SetSnakeColours(colourBase, colourAlt);//, colourOutline);
    }
}