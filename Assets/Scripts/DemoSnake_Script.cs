using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DemoMode
{
    Tutorial,
    Comparison,
    RetroSnake
}
public class DemoSnake_Script : MonoBehaviour
{
    public DemoMode demoMode { get; set; }
    //to check if it should show retro snake graphics or eel
    private bool isEel = true;

    private GameObject demoSegmentPrefab;

    private int tutorialStage = -1;
    private string additionalText = "";
    private char lastDirection = 'x';

    private int tutorialCounter = 0;
    public float timeLeft = 0;

    private DemoHandler_Script demoHandlerScript;
    private PlayerSettings playerSettings;
    private DemoSnakeColourHandler_Script colourHandler;
    private GameObject snakeHead;

    public SnakeState snakeState { get; set; } = SnakeState.Dead; // flag indicating which state the snake is currently in - defaults to dead
    private List<GameObject> segments = new List<GameObject>();    // List of all the segments of the snake

    private int score = 0;    // Current score of the snake (how many segments long)
    private int storedSegments = 0;    // keeps track of how many segments need to be grown
    private float deathTimer = 0;    // Timer remaining for when player death penalty is active


    //vars for snake movement
    private Dictionary<KeyCode, Vector2> inputDirections;
    private char currentDirection;   // current direction of the snake 
    private char verticalBufferDirection;   // next vertical (U or D) direction of the snake (as user inputted)
    private char horizontalBufferDirection; // next horizontal (L or R) direction of the snake (as user inputted)
    private MoveMentStyle moveMentStyle = MoveMentStyle.None;

    //sound effects
    public AudioSource sfxPlayer;

    public GameObject segmentPrefab;
    public Sprite segmentSpriteStraight;
    public Sprite segmentSpriteCurved;
    public Sprite segmentSpriteTail;
    public AudioClip eatNormalFoodSFX;
    public AudioClip eatGoldFoodSFX1;
    public AudioClip eatGoldFoodSFX2;
    public AudioClip eatDeadSnakeFoodSFX;

    public AudioClip crashSnakeSFX;
    public AudioClip crashSelfSFX;
    public AudioClip crashWallSFX1;
    public AudioClip crashWallSFX2;




    public void SetupSnake(PlayerSettings _playerSettings, DemoHandler_Script _demoHandlerScript, GameObject _demoSegmentPrefab, Color retroSnakeColour)
    {

        playerSettings = _playerSettings;

        demoHandlerScript = _demoHandlerScript;

        demoSegmentPrefab = _demoSegmentPrefab;

        snakeHead = this.transform.GetChild(0).gameObject;

        inputDirections = new Dictionary<KeyCode, Vector2>()
        {
            { playerSettings.playerInputs[0], Vector2.up },
            { playerSettings.playerInputs[1], Vector2.down },
            { playerSettings.playerInputs[2], Vector2.left },
            { playerSettings.playerInputs[3], Vector2.right }
        };

        //setup the gui
        playerSettings.playerGUIScript.SetValues(playerSettings.gameHandler_Script, playerSettings.playerNum, playerSettings.playerColour);

        //setting up the colour handler
        colourHandler = GetComponent<DemoSnakeColourHandler_Script>();
        colourHandler.Setup(this, playerSettings.playerColour, retroSnakeColour, snakeHead.GetComponent<SpriteRenderer>());

        //prepare the snake to starting state
        ResetSnake();
    }
    public void PlaySFX(AudioClip sfx)
    {
        float volume = 0.2f;
        sfxPlayer.PlayOneShot(sfx, volume);
    }
    public void PlaySFX(AudioClip sfx, float volume)
    {
        sfxPlayer.PlayOneShot(sfx, volume);
    }
    // Method to start the game for the snake
    private void StartGame()
    {
        //hide the prompt telling player to start the game
        playerSettings.playerGUIScript.HidePrompt();

        // Reset the snake to its starting values
        ResetSnake();

        // Set the state to alive
        snakeState = SnakeState.Alive;

        score = 1;// starts at score 1 because its just a head to start

        Grow(playerSettings.startingSize - 1);// starting size -1 because it starts as a head segment

        if (playerSettings.ghostModeDuration > 0)
        {//if ghosting is enabled, start the game off ghosted for the ruleset's declared duration
            colourHandler.StartGhostMode(playerSettings.ghostModeDuration);
            // Set the countdown display to show the duration of the ghosting
            playerSettings.playerGUIScript.StartGhostMode(playerSettings.ghostModeDuration);
        }



    }

    // Method to reset the snake to its starting vGoldenalues
    public void ResetSnake()
    {
        // Set the state to alive
        snakeState = SnakeState.Dead;

        //stop any temporary state
        colourHandler.StopAllCoroutines();
        playerSettings.playerGUIScript.StopCountdown();

        // delete all segments of the snake
        storedSegments = 0;

        if (playerSettings.doSnakesTurnToFood && score > 0)
        {   //if snake turns into food, spawn food where snake was
            demoHandlerScript.SpawnFood(playerSettings.playerNum, snakeHead.transform.position, EntityType.DeadSnakeFood);
            foreach (GameObject seg in segments)
            {
                demoHandlerScript.SpawnFood(playerSettings.playerNum, seg.transform.position, EntityType.DeadSnakeFood);
                Destroy(seg);
            }
        }
        else
        {
            foreach (GameObject seg in segments)
            {
                Destroy(seg);
            }
        }
        segments.Clear();

        //resets colour handler (empties list or renderers)
        colourHandler.ClearSegments(isEel);

        // Reset the position + rotation of the snake head to the starting position
        snakeHead.transform.position = playerSettings.startingPos;
        snakeHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // Initialize the current and next direction of the snake to up so it moves up on spawn
        horizontalBufferDirection = 'x';
        currentDirection = verticalBufferDirection = 'u';
        moveMentStyle = MoveMentStyle.None;

        score = 0;
        UpdateScore();

    }

    public void Die()
    {
        //stop the game
        snakeState = SnakeState.Dead;

        // Reset the snake to its starting values
        ResetSnake();

        //if death penalty is on, set up death timer and change snake colour
        if (playerSettings.deathPenaltyDuration != 0)
        {
            playerSettings.playerGUIScript.StartDeathPenaltyMode(playerSettings.deathPenaltyDuration);
            deathTimer = playerSettings.deathPenaltyDuration;
        }
    }

    private void Update()
    {
        //Debug.Log(snakeState);
        if (timeLeft > 0)
        {
            CountDown();
        }
        else if ((timeLeft <= 0) && tutorialStage == 5)
        {
            demoHandlerScript.ReturnToMenu();
        }

        if (deathTimer != 0)
        {//if death timer is active, tick it down
            deathTimer -= Time.deltaTime;
            if (deathTimer <= 0)
            { //death timer over
                deathTimer = 0;
            }
            return;
        }

        if (snakeState == SnakeState.Dead)
        {//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(playerSettings.playerInputs[0]) || Input.GetKeyDown(playerSettings.playerInputs[1]) || Input.GetKeyDown(playerSettings.playerInputs[2]) || Input.GetKeyDown(playerSettings.playerInputs[3]))
            {
                //start game
                StartGame();
                moveMentStyle = MoveMentStyle.Tap;
                return;
            }
        }

        //if snake isnt dead, you can input buffered controls
        if (snakeState != SnakeState.Dead)
        {
            if (isEel)
            {
                InputLogic();
            }
            else
            {
                if (Input.GetKeyDown(playerSettings.playerInputs[0]))
                {
                    if (lastDirection != 'd')
                    {
                        currentDirection = 'u';
                    }
                }
                if (Input.GetKeyDown(playerSettings.playerInputs[1]))
                {
                    if (lastDirection != 'u')
                    {
                        currentDirection = 'd';
                    }
                }
                if (Input.GetKeyDown(playerSettings.playerInputs[2]))
                {
                    if (lastDirection != 'r')
                    {
                        currentDirection = 'l';
                    }
                }
                if (Input.GetKeyDown(playerSettings.playerInputs[3]))
                {
                    if (lastDirection != 'l')
                    {
                        currentDirection = 'r';
                    }
                }
            }
        }

        //if you press left control, switch between eel and snake
        if (Input.GetKeyDown(KeyCode.Space) && (demoMode == DemoMode.Comparison))
        {
            isEel = !isEel;
            colourHandler.ShowEel(isEel);
            demoHandlerScript.ShowEel(isEel);
        }
    }

    // method switches demo modes for the eel
    public void SwitchDemoMode(DemoMode mode)
    {
        demoMode = mode;

        switch (mode)
        {
            case DemoMode.Tutorial:
                isEel = true;
                tutorialStage = 0;
                tutorialCounter = -1;
                moveMentStyle = MoveMentStyle.Tap;
                break;

            case DemoMode.Comparison:
                isEel = true;
                break;

            case DemoMode.RetroSnake:
                isEel = false;
                break;
            default:
                break;
        }
        colourHandler.ShowEel(isEel);
        TutorialLogic();
    }

    private void TutorialLogic()
    {
        bool counterChanged = false;

        if (timeLeft > 0)
        {
            return;
        }
        switch (tutorialStage)
        {
            case 0:
                if (moveMentStyle == MoveMentStyle.Tap)
                {
                    counterChanged = true;
                    tutorialCounter += 1;
                    additionalText = " Taps: " + tutorialCounter + "/10";
                    if (tutorialCounter >= 10)
                    {

                        StartTimer(3f);
                        tutorialStage += 1;
                        tutorialCounter = -1;
                        moveMentStyle = MoveMentStyle.Hold;
                        goto case 1;
                    }
                }
                break;
            case 1:
                if (moveMentStyle == MoveMentStyle.Hold)
                {
                    counterChanged = true;
                    tutorialCounter += 1;
                    additionalText = " Holds: " + tutorialCounter + "/30";
                    if (tutorialCounter >= 30)
                    {
                        StartTimer(3f);
                        tutorialStage += 1;
                        tutorialCounter = -1;
                        moveMentStyle = MoveMentStyle.OneStep;
                        goto case 2;
                    }
                }
                break;
            case 2:
                if (moveMentStyle == MoveMentStyle.OneStep)
                {
                    counterChanged = true;
                    tutorialCounter += 1;
                    additionalText = " One-Steps: " + tutorialCounter + "/10";
                    if (tutorialCounter >= 10)
                    {
                        StartTimer(3f);
                        tutorialStage += 1;
                        tutorialCounter = -1;
                        moveMentStyle = MoveMentStyle.StairCase;
                        goto case 3;
                    }
                }
                break;
            case 3:
                if (moveMentStyle == MoveMentStyle.StairCase)
                {
                    counterChanged = true;
                    tutorialCounter += 1;
                    additionalText = " Staircases: " + tutorialCounter + "/30";
                    if (tutorialCounter >= 30)
                    {
                        StartTimer(3f);
                        tutorialStage += 1;
                        tutorialCounter = -1;
                        moveMentStyle = MoveMentStyle.UTurn;
                        goto case 4;
                    }
                }
                break;
            case 4:
                if (moveMentStyle == MoveMentStyle.UTurn)
                {
                    counterChanged = true;
                    tutorialCounter += 1;
                    additionalText = " U-Turns: " + tutorialCounter + "/5";
                    if (tutorialCounter >= 5)
                    {
                        StartTimer(5f);
                        tutorialStage += 1;
                        tutorialCounter = -1;
                        goto case 5;
                    }
                }
                break;
            case 5:

                break;
            default:
                break;
        }
        if (counterChanged)
        {
            demoHandlerScript.UpdateTutorialText(tutorialStage, additionalText);
        }


    }

    private void StartTimer(float time)
    {
        timeLeft = time;
    }
    private void CountDown()
    {

        timeLeft -= Time.deltaTime;
        int seconds = Mathf.RoundToInt(timeLeft);

        if (seconds <= 0)
        {
            demoHandlerScript.UpdateTutorialText(tutorialStage, additionalText);
            return;
        }
        if (timeLeft < 1 && timeLeft > 0)
        {
            seconds = 1;
        }
        if (tutorialStage == 5)
        {
            demoHandlerScript.UpdateTutorialText(5, seconds.ToString());
        }
        else
        {
            demoHandlerScript.UpdateTutorialText(6, seconds.ToString());
        }

    }

    private void InputLogic()
    {
        // if snake is waiting to do a one-spot movement, stop buffer from overwriting it.
        if ((moveMentStyle == MoveMentStyle.Tap) || (moveMentStyle == MoveMentStyle.OneStep) || (moveMentStyle == MoveMentStyle.UTurn))
        {
            return;
        }
        Vector2 directionsTapped = Vector2.zero;
        Vector2 directionsHeld = Vector2.zero;
        // Iterate through the input directions dictionary to determine the tapped and held directions
        foreach (var kvp in inputDirections)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                directionsTapped += kvp.Value;
            }
            if (Input.GetKey(kvp.Key))
            {
                directionsHeld += kvp.Value;
            }
        }

        // var to track if the player has released the original direction
        bool hasReleasedDirection = true;
        foreach (var kvp in inputDirections)
        {
            if (Input.GetKey(kvp.Key))
            {
                hasReleasedDirection = false;
                break;
            }
        }
        if (hasReleasedDirection)
        {
            // if has released a direction, reset both buffer directions (for held directions smoothly)
            verticalBufferDirection = 'x';
            horizontalBufferDirection = 'x';
        }

        ////////////////////////////////////////////////// Holding (Holding only 1 direction)
        // If 1 direction is held
        if ((directionsHeld.x != 0 && directionsHeld.y == 0) || (directionsHeld.y != 0 && directionsHeld.x == 0))
        {

            if (directionsHeld != Vector2.zero)
            {
                // Handle the case when a horizontal direction is held
                if (directionsHeld.x != 0)
                {
                    moveMentStyle = MoveMentStyle.Hold;
                    horizontalBufferDirection = directionsHeld.x > 0 ? 'r' : 'l';
                    verticalBufferDirection = 'x';
                }
                // Handle the case when a vertical direction is held
                else if (directionsHeld.y != 0)
                {
                    moveMentStyle = MoveMentStyle.Hold;
                    verticalBufferDirection = directionsHeld.y > 0 ? 'u' : 'd';
                    horizontalBufferDirection = 'x';
                }
            }
        }
        // If 2 directions are held
        ////////////////////////////////////////////////// Staircase (holding 2 directions)
        else if (directionsHeld.x != 0 && directionsHeld.y != 0)
        {
            // Diagonal direction handling
            if (currentDirection == 'l' || currentDirection == 'r')
            {
                moveMentStyle = MoveMentStyle.StairCase;
                verticalBufferDirection = directionsHeld.y > 0 ? 'u' : 'd';
                horizontalBufferDirection = 'x';
            }
            else
            {
                moveMentStyle = MoveMentStyle.StairCase;
                horizontalBufferDirection = directionsHeld.x > 0 ? 'r' : 'l';
                verticalBufferDirection = 'x';
            }
        }

        ////////////////////////////////////////////////// Taps
        // prevent moving in the opposite direction without going adjacent first
        switch (directionsTapped.y) // vertical direction taps
        {
            case 1:
                // tapped up
                if (currentDirection != 'd')
                {
                    //checking if U-turned (holding the opposite direction from current + tapping sideways)
                    if ((directionsHeld.x == 1 && currentDirection == 'l') ||
                        (directionsHeld.x == -1 && currentDirection == 'r'))
                    {
                        moveMentStyle = MoveMentStyle.UTurn;
                    }
                    else
                    {//checking if tapped or one-stepped
                        moveMentStyle = moveMentStyle == MoveMentStyle.StairCase ? MoveMentStyle.OneStep : MoveMentStyle.Tap;
                    }
                    verticalBufferDirection = 'u';
                }
                break;
            case -1:
                // tapped down
                if (currentDirection != 'u')
                {
                    //checking if U-turned (holding the opposite direction from current + tapping sideways)
                    if ((directionsHeld.x == 1 && currentDirection == 'l') ||
                        (directionsHeld.x == -1 && currentDirection == 'r'))
                    {
                        moveMentStyle = MoveMentStyle.UTurn;
                    }
                    else
                    {//checking if tapped or one-stepped
                        moveMentStyle = moveMentStyle == MoveMentStyle.StairCase ? MoveMentStyle.OneStep : MoveMentStyle.Tap;
                    }
                    verticalBufferDirection = 'd';
                }
                break;
        }

        // Modify the horizontal direction tap check for rule 2 and to prevent moving in the opposite direction without going adjacent first
        switch (directionsTapped.x) // horizontal direction taps
        {
            case 1:
                // tapped right
                if (currentDirection != 'l')
                {
                    //checking if U-turned (holding the opposite direction from current + tapping sideways)
                    if ((directionsHeld.y == -1 && currentDirection == 'u') ||
                        (directionsHeld.y == 1 && currentDirection == 'd'))
                    {
                        moveMentStyle = MoveMentStyle.UTurn;
                    }
                    else
                    {//checking if tapped or one-stepped
                        moveMentStyle = moveMentStyle == MoveMentStyle.StairCase ? MoveMentStyle.OneStep : MoveMentStyle.Tap;

                    }
                    horizontalBufferDirection = 'r';
                }
                break;
            case -1:
                // tapped left
                if (currentDirection != 'r')
                {
                    //checking if U-turned (holding the opposite direction from current + tapping sideways)
                    if ((directionsHeld.y == -1 && currentDirection == 'u') ||
                        (directionsHeld.y == 1 && currentDirection == 'd'))
                    {
                        moveMentStyle = MoveMentStyle.UTurn;
                    }
                    else
                    {//checking if tapped or one-stepped
                        moveMentStyle = moveMentStyle == MoveMentStyle.StairCase ? MoveMentStyle.OneStep : MoveMentStyle.Tap;

                    }
                    horizontalBufferDirection = 'l';
                }
                break;
        }
        //Debug.Log("moveMentStyle" + moveMentStyle);
    }


    private void UseBuffer()
    {
        //moving up or down and have a sideways input
        if ((currentDirection == 'u' || currentDirection == 'd') && horizontalBufferDirection != 'x')
        {
            currentDirection = horizontalBufferDirection;
            horizontalBufferDirection = 'x';
        } //moving sideways and have an up or down input
        else if ((currentDirection == 'l' || currentDirection == 'r') && verticalBufferDirection != 'x')
        {
            currentDirection = verticalBufferDirection;
            verticalBufferDirection = 'x';
        }

        if (tutorialStage != -1)
        {
            TutorialLogic();
        }

        moveMentStyle = MoveMentStyle.Automatic;
    }





    public Vector2 TryMoveSnake()
    {//called by game handler, returns the snake's target position
     //1 - checks if able to move at all
     //2 - determine the target spot
     //3 - checks if anything is occupying target spot
     //    + switch statement to respond to what is in target spot



        //1
        if (snakeState == SnakeState.Dead)//if snake is dead, it cant move, exit method
        {
            return snakeHead.transform.position;
        }

        //2
        //buffer determines which direction the snake should go with moves in a queue
        UseBuffer();

        //use current position and current direction to determine when head will move to
        Vector3 offset = Vector3.zero;
        Vector3 newHeadRotation = Vector3.zero;
        switch (currentDirection)
        {
            case 'u':
                offset = Vector2.up;
                newHeadRotation = new Vector3(0, 0, 0);
                break;

            case 'd':
                offset = Vector2.down;
                newHeadRotation = new Vector3(0, 0, 180);
                break;

            case 'l':
                offset = Vector2.left;
                newHeadRotation = new Vector3(0, 0, 90);
                break;

            case 'r':
                offset = Vector2.right;
                newHeadRotation = new Vector3(0, 0, 270);
                break;

            default:
                break;
        }

        lastDirection = currentDirection;

        //setting the new target position
        Vector3 targetPos = snakeHead.transform.position + offset;

        // if ghosted, cant eat food so it wont destroy food when running into it
        bool hungry = snakeState == SnakeState.Ghosted ? false : true;
        // 3 
        //check if theres anything in the target position
        switch (demoHandlerScript.CheckPos(playerSettings.playerNum, targetPos, hungry))
        {
            case EntityType.NormalFood://if spot was food then eat the food
                //play sfx
                PlaySFX(eatNormalFoodSFX, 2f);

                EatFood(playerSettings.normalFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.DeadSnakeFood://if spot was food then eat the food
                //play sfx
                PlaySFX(eatDeadSnakeFoodSFX, 0.1f);

                EatFood(playerSettings.deadSnakeFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.GoldFood://if spot was food then eat the food
                //play sfx
                PlaySFX(eatGoldFoodSFX1, 2f);
                PlaySFX(eatGoldFoodSFX2, .5f);

                EatFood(playerSettings.goldFoodGrowthAmount);

                //calculate the length of gold mode based on the amt of food eaten
                float goldModeDuration = playerSettings.snakeSpeed * playerSettings.goldFoodGrowthAmount;
                //flash gold for the duration that the snake is growing from the extra food
                colourHandler.StartGoldMode(goldModeDuration);
                // Set the gui to gold mode for the set duration
                playerSettings.playerGUIScript.StartGoldMode(goldModeDuration);

                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.Wall://if spot was a wall ---> die
                //play sfx
                PlaySFX(crashWallSFX1, 2f);
                PlaySFX(crashWallSFX2, 2f);
                Die();
                break;

            case EntityType.Self:
                //play sfx
                PlaySFX(crashSelfSFX, .5f);

                goto case EntityType.Snake;

            case EntityType.Snake://if spot was a snake --->die
                if (snakeState == SnakeState.Alive)
                {
                    //play sfx
                    PlaySFX(crashSnakeSFX, .5f);

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
        if (storedSegments > 0)
        {

            storedSegments -= 1;
            // Instantiate a new segment prefab 
            GameObject newSegment = Instantiate(demoSegmentPrefab, snakeHead.transform.position, Quaternion.identity, transform);

            //set the colour of it
            colourHandler.AddRenderer(newSegment.transform.GetChild(0).GetComponent<SpriteRenderer>(), isEel);

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
        Vector2 previousSegmentDir = Vector2.zero;
        Vector2 nextSegmentDir = Vector2.zero;
        Vector3 newRotation = Vector3.zero;
        Sprite newSprite = segmentSpriteStraight;
        GameObject currentSeg;

        //cycle through each segment
        for (int i = 0; i < segments.Count; i++)
        {
            currentSeg = segments[i];

            //determining the relative positions of the segments before and after current segment
            // the first segment (uses snake head as previous seg)
            if (i == 0)
            {
                previousSegmentDir = snakeHead.transform.position - segments[i].transform.position;
                nextSegmentDir = segments[i + 1].transform.position - segments[i].transform.position;
            }
            //the last segment (only needs previous seg value)
            else if (i == segments.Count - 1)
            {
                previousSegmentDir = segments[i - 1].transform.position - segments[i].transform.position;
            }
            else
            {//all the segments between
                previousSegmentDir = segments[i - 1].transform.position - segments[i].transform.position;
                nextSegmentDir = segments[i + 1].transform.position - segments[i].transform.position;
            }

            //if its not the last segment 
            if (i != segments.Count - 1)
            {
                //determing if its a horizontal straight segment
                if (previousSegmentDir.y == 0 && nextSegmentDir.y == 0)
                {
                    newSprite = segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 90);
                }
                //determing if its a vertical straight segment
                else if (previousSegmentDir.x == 0 && nextSegmentDir.x == 0)
                {
                    newSprite = segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 0);
                }
                else
                {
                    newSprite = segmentSpriteCurved;
                    //if its not a straight segment, determine which direction the curve needs to go

                    switch (previousSegmentDir + nextSegmentDir)
                    {
                        case var value when value == new Vector2(-1, 1):     //above + left
                            newRotation = new Vector3(0, 0, 180);
                            break;
                        case var value when value == new Vector2(1, 1):     //above + right
                            newRotation = new Vector3(0, 0, 90);
                            break;
                        case var value when value == new Vector2(-1, -1):    //below+  left
                            newRotation = new Vector3(0, 0, 270);
                            break;
                        case var value when value == new Vector2(1, -1):     //below + right
                            newRotation = new Vector3(0, 0, 0);
                            break;
                        default:
                            break;
                    }
                }

            }
            //if its the last segment (tail)
            else
            {
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
            currentSeg.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = newSprite;
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

    public bool CheckForSnakeAtPos(Vector3 pos, bool includeLastSeg)
    {   //when calling from other snakes, dont include the check on the last segment, 
        //      so snake cant collide with it's own tail (or others)
        if (pos == snakeHead.transform.position)
        {
            return true;
        }
        foreach (GameObject seg in segments)
        {
            //if not checking last segment
            if ((segments.IndexOf(seg) == segments.Count - 1) && !includeLastSeg)
            {
                return false;
            }
            if (pos == seg.transform.position)
            {
                return true;
            }
        }
        return false;
    }

}