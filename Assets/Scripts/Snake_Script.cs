using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake_Script : MonoBehaviour
{
    // Game handler script that controls the game
    private GameHandler_Script gameHandlerScript;
    // Reference to the player display script
    private PlayerDisplay_Script playerDisplayScript;



    // Player number that identifies this snake
    private int playerNum;
    // Speed of the snake
    private float snakeSpeed;
    // Starting position of the snake
    private Vector2 startingPos;
    // Starting size of the snake
    private int startingSize;
    // Color of the snake's base segments
    private Color colBase;
    // Color of the snake's alternate segments
    private Color colAlt;
    // Color of the snake's outline
    private Color colOutline;
    // Flag indicating whether the snake is alive or not
    private bool isAlive = false;
    // Flag indicating whether the snake is waiting for input to move
    private bool snakeIsWait = true;
    // List of all the segments of the snake
    private List<GameObject> segments = new List<GameObject>();
    // Prefab for the snake segment
    private GameObject segmentPrefab;
    // The first segment of the snake, which is already existing
    private GameObject snakeHead;
    // Key codes for the controls of the snake
    private KeyCode[] controls;
    // Current score of the snake
    private int score = 0;
    // Highest score achieved by the snake
    private int highscore = 0;
    // Flag indicating whether the snake can die or not
    private bool canDie = true;
    // Flag indicating whether the snake is flashing due to eating gold food
    private bool isFlashing = false;
    // Duration of flashing after eating gold food
    private float flashDuration = 0.3f;
    // Time remaining for the flashing effect
    private float flashTime = 0;
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
    // Duration of ghosting
    private float ghostDuration = 0.4f;
    // Time remaining for the ghosting effect
    private float ghostTime = 0;
    // Opacity for the ghosting effect
    private float ghostOpacity1 = 0.1f;
    // Opacity for the ghosting effect
    private float ghostOpacity2 = 0.3f;
    // Color for the ghosting effect
    private Color colGhost1;
    // Color for the ghosting effect
    private Color colGhost2;
    // Color for the ghosting outline effect
    private Color colGhostOutline;
    // Duration player is unable to play after dying
    private float deathPenaltyDuration;
    // Initialize the current direction of the snake to up
    char currentDirection;
    // Initialize the next direction of the snake to up
    char bufferDirection;
    // Flag indicating whether the game is currently being played or not
    private bool isPlaying = false;
    // Flag indicating whether the snake is currently waiting for an input to move or not
    private bool isWaiting = true;
    // Coroutine for the flashing effect
    private Coroutine flasherCoroutine;
    // Coroutine for the ghosting effect
    private Coroutine ghosterCoroutine;

    // Method to set the necessary references and values for the snake
    public void SetReferences(GameHandler_Script handler, PlayerDisplay_Script display, GameObject segmentPrefab, int _playerNumber, float speed, int _startingSize, Vector3 _startingPos, float _ghostDuration, float _deathPenaltyDuration)
    {
        // Set the reference to the game handler script
        gameHandlerScript = handler;
        // Set the reference to the player display script
        playerDisplayScript = display;
        // Set the prefab for the snake segments
        this.segmentPrefab = segmentPrefab;
        // Set the player number of the snake
        playerNum = _playerNumber;
        // Set the speed of the snake
        snakeSpeed = speed;
        // Set the starting size of the snake
        startingSize = _startingSize;
        // Set the starting position of the snake
        startingPos = _startingPos;
        // Set the duration of the ghost mode powerup
        ghostDuration = _ghostDuration;
        // Set the duration of the death penalty powerup
        deathPenaltyDuration = _deathPenaltyDuration;
        // Initialize the list of snake segments
        segments = new List<GameObject>();
        // Initialize the current direction of the snake to up
        currentDirection = 'u';
        // Initialize the next direction of the snake to up
        bufferDirection = 'u';
        // Initialize the score of the snake to 0
        score = 0;
        // Initialize the highscore of the snake to 0
        highscore = 0;
        // Initialize the isAlive flag to false
        isAlive = false;
        // Initialize the isGhosting flag to false
        isGhosting = false;
        // Initialize the isFlashing flag to false
        isFlashing = false;
    }

    // Method to start the game for the snake
    public void StartGame()
    {
        // Reset the snake to its starting values
        ResetSnake();

        // Set the isAlive flag to true
        isAlive = true;
    }

    // Method to reset the snake to its starting values
    public void ResetSnake()
    {
        // Reset the position of the snake head to the starting position
        snakeHead.transform.position = startingPos;

    }

    private void Update()
    {
        if (isFlashing)
        {
            FlashColor();
        }

        if (isGhosting)
        {
            FlashGhost();
        }

        if (isAlive == false)
        {//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(controls[0]) || Input.GetKeyDown(controls[1]) || Input.GetKeyDown(controls[2]) || Input.GetKeyDown(controls[3]))
            {
                //start game
                StartGame();
            }
        }

        if (Input.GetKeyDown(controls[0]))
        {
            //prevents attempts to change direction to current axis
            if (currentDirection != 'u' && currentDirection != 'd')
            {
                bufferDirection = 'u';
            }
        }
        if (Input.GetKeyDown(controls[1]))
        {
            //prevents attempts to change direction to current axis
            if (currentDirection != 'u' && currentDirection != 'd')
            {
                bufferDirection = 'd';
            }
        }
        if (Input.GetKeyDown(controls[2]))
        {
            //prevents attempts to change direction to current axis
            if (currentDirection != 'l' && currentDirection != 'r')
            {
                bufferDirection = 'l';
            }
        }
        if (Input.GetKeyDown(controls[3]))
        {
            //prevents attempts to change direction to current axis
            if (currentDirection != 'l' && currentDirection != 'r')
            {
                bufferDirection = 'r';
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

        if (gameHandlerScript.advancedOptions.doSnakesTurnToFood)
        {

            gameHandlerScript.SpawnFood(playerNum, snakeHead.transform.position, FoodType.DeadSnakeFood);
            snakeHead.transform.position = startingPos;
            foreach (GameObject seg in segments)
            {
                gameHandlerScript.SpawnFood(playerNum, seg.transform.position, FoodType.DeadSnakeFood);
                Destroy(seg);
            }
            segments.Clear();

        }
        playerDisplayScript.UpdateScore(0);
        playerDisplayScript.StopCountdown();

    }

    void TryMoveSnake()
    {
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
        Vector3 offset = Vector3.zero;
        Vector3 newHeadRotation = Vector3.zero;
        switch (bufferDirection)
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
        switch (gameHandlerScript.CheckPos(playerNum, targetPos, true))
        {
            case EntityType.NFood://if spot was food then eat the food
                EatFood(gameHandlerScript.advancedOptions.normalFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.DFood://if spot was food then eat the food
                EatFood(gameHandlerScript.advancedOptions.deadSnakeFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.GFood://if spot was food then eat the food
                //flash gold for the duration that the snake is growing from the extra food
                if (isFlashing)
                {
                    StopCoroutine(flasherCoroutine);
                }
                flasherCoroutine = StartCoroutine(FlashFor(snakeSpeed * gameHandlerScript.advancedOptions.goldFoodGrowthAmount));
                EatFood(gameHandlerScript.advancedOptions.goldFoodGrowthAmount);
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

    }

    public void EatFood(int amount)
    {

        //increase and update score 
        score += amount;
        UpdateScore();

        //grow snake

        Grow(amount);

    }

    void UpdateScore()
    {
        if (score > highscore)
        {
            highscore = score;
        }
        playerDisplayScript.UpdateScore(score);
    }





    // Method to make the snake grow by a certain amount
    public void Grow(int amount)
    {
        // Increase the score by the amount of growth
        score += amount;

        // Update the score display
        UpdateScore();

        // Add the specified number of segments to the snake
        for (int i = 0; i < amount; i++)
        {
            // Instantiate a new segment prefab
            GameObject newSegment = Instantiate(segmentPrefab, transform.position, Quaternion.identity, transform.parent);

            // Set the color of the segment outline to the player color
            newSegment.GetComponent<SpriteRenderer>().color = colOutline;

            // Set the color of the segment fill to the alternate color
            newSegment.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colAlt;

            // Add the new segment to the list of segments
            segments.Add(newSegment);
        }
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






    void ResetSnakeColours()
    {
        //setting the snake back to normal colours

        //setting the head colours 
        snakeHead.GetComponent<SpriteRenderer>().color = colOutline;
        snakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colBase;

        //going through each segment and recolouring it appropriately
        for (int i = 0; i < segments.Count; i++)
        {
            if (i % 2 == 1)
            {
                //odd num segment is base colour
                segments[i].transform.GetComponent<SpriteRenderer>().color = colOutline;
                segments[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = colBase;
            }
            else
            {
                //even num seg is alt colour
                segments[i].transform.GetComponent<SpriteRenderer>().color = colOutline;
                segments[i].transform.GetChild(0).GetComponent<SpriteRenderer>().color = colAlt;
            }
        }
    }

    // Method to make the snake flash gold for a certain duration
    private IEnumerator FlashFor(float duration)
    {
        // Set the countdown display to show the duration of the golden time
        playerDisplayScript.StartCountdown(duration);

        // Set the flashing and invincibility flags to true
        isFlashing = true;
        canDie = false;

        // Wait for the duration of the golden time
        yield return new WaitForSeconds(duration);

        // Set the flashing and invincibility flags to false
        isFlashing = false;
        canDie = true;

        // Reset the snake's colors to their normal colors
        ResetSnakeColors();
    }
    private void FlashColor()
    {
        // Calculate the current color of the snake by lerping between colFlashing1 and colFlashing2
        Color color = Color.Lerp(colFlashing1, colFlashing2, flashTime);

        // Set the colors of the snake's head and segments
        SetSnakeColors(colFlashingOutline, color);

        // Increment the flash time
        flashTime += Time.deltaTime / flashDuration;

        // If the flash time has reached 1, reset it and swap the flashing colors
        if (flashTime >= 1)
        {
            flashTime = 0;
            Color tempColor = colFlashing1;
            colFlashing1 = colFlashing2;
            colFlashing2 = tempColor;
        }
    }
    private IEnumerator GhostFor(float duration)
    {
        // Set the countdown display to show the duration of the ghosting
        playerDisplayScript.StartCountdown(duration);

        // Set the invincibility and ghosting flags to true
        canDie = false;
        isGhosting = true;

        // Wait for the duration of the ghosting
        yield return new WaitForSeconds(duration);

        // Set the invincibility and ghosting flags to false
        canDie = true;
        isGhosting = false;

        // Reset the snake's colors to their normal colors
        ResetSnakeColors();
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
            colGhost1 = tempColor;
        }
    }

    void SetSnakeColors(Color outlineColor, Color baseColor)
    {
        // Set the color of the snake's outline and base
        snakeHead.GetComponent<SpriteRenderer>().color = outlineColor;
        snakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = baseColor;

        // Set the color of the segments' outline and base
        foreach (GameObject seg in segments)
        {
            seg.transform.GetComponent<SpriteRenderer>().color = outlineColor;
            seg.transform.GetChild(0).GetComponent<SpriteRenderer>().color = baseColor;
        }
    }
    void SetSnakeOpacity(float opacity)
    {
        Color outlineColorWithOpacity = colOutline;
        outlineColorWithOpacity.a = opacity;

        Color baseColorWithOpacity = colBase;
        baseColorWithOpacity.a = opacity;

        // Set the color of the snake's outline and base
        snakeHead.GetComponent<SpriteRenderer>().color = outlineColorWithOpacity;
        snakeHead.transform.GetChild(0).GetComponent<SpriteRenderer>().color = baseColorWithOpacity;

        // Set the color of the segments' outline and base
        foreach (GameObject seg in segments)
        {
            seg.transform.GetComponent<SpriteRenderer>().color = outlineColorWithOpacity;
            seg.transform.GetChild(0).GetComponent<SpriteRenderer>().color = baseColorWithOpacity;
        }
    }



    void ResetSnakeColors()
    {
        // Reset the snake's colors to the original outline and base colors
        SetSnakeColors(colOutline, colBase);
    }
}