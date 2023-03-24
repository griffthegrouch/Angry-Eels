using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetroSnake_Script : MonoBehaviour
{
    private PlayerSettings playerSettings;
    private PlayerResources playerResources;
    
    private GameObject snakeHead;

    public SnakeState snakeState { get; set; } = SnakeState.Dead; // flag indicating which state the snake is currently in - defaults to dead
    private List<GameObject> segments = new List<GameObject>();    // List of all the segments of the snake

    private int score = 0;    // Current score of the snake (how many segments long)
    private int storedSegments = 0;    // keeps track of how many segments need to be grown
    private char currentDirection;   // current direction of the snake 
    



    public void SetupSnake(PlayerSettings _playerSettings, PlayerResources _playerResources)
    {

        playerSettings = _playerSettings;
        playerResources = _playerResources;
        snakeHead = this.transform.GetChild(0).gameObject;
        //colour the head
        snakeHead.GetComponent<SpriteRenderer>().color = playerSettings.playerColour;

        //setup the gui
        playerSettings.playerGUIScript.SetValues(playerSettings.gameHandler_Script, playerSettings.playerNum, playerSettings.playerColour);

        //prepare the snake to starting state
        ResetSnake();
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

    }

    // Method to reset the snake to its starting vGoldenalues
    public void ResetSnake()
    {
        // Set the state to alive
        snakeState = SnakeState.Dead;

        //stop any temporary state
        playerSettings.playerGUIScript.StopCountdown();

        // delete all segments of the snake
        storedSegments = 0;
        foreach (GameObject seg in segments)
        {
            Destroy(seg);
        }
        segments.Clear();

        // Reset the position + rotation of the snake head to the starting position
        snakeHead.transform.position = playerSettings.startingPos;
        snakeHead.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        // Initialize the current and next direction of the snake to up so it moves up on spawn
        currentDirection = 'u';

        score = 0;
        UpdateScore();
    }

    public void Die()
    {
        //stop the game
        snakeState = SnakeState.Dead;

        // Reset the snake to its starting values
        ResetSnake();

    }

    private void Update()
    {
        if (snakeState == SnakeState.Dead)
        {//if snake isnt moving, pressing any button will reset it
            if (Input.GetKeyDown(playerSettings.playerInputs[0]) || Input.GetKeyDown(playerSettings.playerInputs[1]) || Input.GetKeyDown(playerSettings.playerInputs[2]) || Input.GetKeyDown(playerSettings.playerInputs[3]))
            {
                //start game
                StartGame();
                return;
            }
        }

        //if snake isnt dead, you can control it
        if (snakeState != SnakeState.Dead)
        {
            //keydown corresponds to the direction snake will move
            if (Input.GetKeyDown(playerSettings.playerInputs[0])){
                currentDirection = 'u';
            }
            else if (Input.GetKeyDown(playerSettings.playerInputs[1])){
                currentDirection = 'd';
            }
            else if (Input.GetKeyDown(playerSettings.playerInputs[2])){
                currentDirection = 'l';
            }
            else if (Input.GetKeyDown(playerSettings.playerInputs[3])){
                currentDirection = 'r';
            }
        }
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

        //setting the new target position
        Vector3 targetPos = snakeHead.transform.position + offset;
        // 3 
        //check if theres anything in the target position
        switch (playerSettings.gameHandler_Script.CheckPos(playerSettings.playerNum, targetPos, true))
        {
            case EntityType.NormalFood://if spot was food then eat the food
                //play sfx
                EatFood(playerSettings.normalFoodGrowthAmount);
                goto case EntityType.Empty;//the act as if the target spot was empty

            case EntityType.DeadSnakeFood://if spot was food then eat the food
                goto case EntityType.NormalFood;//retrosnake only knows normal food

            case EntityType.GoldFood://if spot was food then eat the food
                goto case EntityType.NormalFood;//retrosnake only knows normal food

            case EntityType.Wall://if spot was a wall ---> die
                //play sfx
                playerSettings.gameHandler_Script.PlaySFX(playerResources.crashSFX);
                Die();
                break;

            case EntityType.Self:
                //play sfx
                goto case EntityType.Snake;

            case EntityType.Snake://if spot was a snake --->die
                if (snakeState == SnakeState.Alive)
                {
                    //play sfx
                    playerSettings.gameHandler_Script.PlaySFX(playerResources.chompSFX);
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
            GameObject newSegment = Instantiate(playerResources.segmentPrefab, snakeHead.transform.position, Quaternion.identity, transform);

            //set the colour of it
            newSegment.GetComponent<SpriteRenderer>().color = playerSettings.playerColour;

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
        Sprite newSprite = playerResources.segmentSpriteStraight;
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
                    newSprite = playerResources.segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 90);
                }
                //determing if its a vertical straight segment
                else if (previousSegmentDir.x == 0 && nextSegmentDir.x == 0)
                {
                    newSprite = playerResources.segmentSpriteStraight;
                    newRotation = new Vector3(0, 0, 0);
                }
                else
                {
                    newSprite = playerResources.segmentSpriteCurved;
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
            if((segments.IndexOf(seg) == segments.Count-1) && !includeLastSeg){
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