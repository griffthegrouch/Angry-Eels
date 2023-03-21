using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeColourHandler_Script : MonoBehaviour
{
    //this script handles colouring the snake 
    // includes the original snake colours + gold flashing + ghost opacity flashing
    private Snake_Script snakeScript;
    private Color playerColour;
    private List<SpriteRenderer> renderers;  // List of all the snakes' renderers (head and segments)

    private Color colourEven;   // Color for the snake head and odd numbered segments
    private Color colourOdd;   // Color for the even numbered segments   


    private float bounceTimer = 0;  // Timer used for colour bouncing and opacity bouncing
    private float bounceSpeed;      // temp speed used for the bouncing colour + opacity functions
    private Color bounceColour1;    // temp colour used for the bouncing colour function
    private Color bounceColour2;    // temp colour used for the bouncing colour function
    private float bounceOpacity1;    // temp float used for the bouncing opacity function
    private float bounceOpacity2;    // temp float used for the bouncing opacity function


    private float goldBounceSpeed = 0.3f;    // speed snake flashes gold
    private Color goldColour1 = new Color(1, 1, 0);       // Color for the gold flashing effect highest value
    private Color goldColour2 = new Color(0.9f, 0.9f, 0); // Color for the gold flashing effect lowest value
    private Coroutine goldCoroutine;     // Coroutine for the flashing effect

    private float ghostBounceSpeed = .6f; // Duration of ghosting cycles - bounces snake's opacity between two values during ghosting
    private float ghostOpacity1 = 0.2f;   // Opacity for the ghosting effect of the snake head and odd numbered segments
    private float ghostOpacity2 = 0.6f;   // Opacity for the ghosting effect of the even numbered segments   
    private Coroutine ghostCoroutine;     // Coroutine for the ghosting effect


    public void Setup(Snake_Script script, Color playerColour, SpriteRenderer headRenderer)
    {
        snakeScript = script;

        colourEven = playerColour;//new Color(col.r - 0.2f, col.g - 0.2f, col.b - 0.2f);
        colourOdd = Color.Lerp(playerColour, Color.white, .42f);//setting alt colour based off original

        renderers = new List<SpriteRenderer>(); 
        AddRenderer(headRenderer);
    }

    public void AddRenderer(SpriteRenderer newRenderer)
    {
        //add the new renderer to the list
        renderers.Add(newRenderer);

        // Set the color of the segment correctly based on if its even or odd
        Color col = (renderers.Count+1) % 2 == 1 ? colourEven : colourOdd;
        newRenderer.color = col;
    }
 
    public void ClearSegments()//resets renderers list except for snake's head
    {
        SpriteRenderer headRenderer = renderers[0];
        renderers.Clear();
        AddRenderer(headRenderer);
    }

    // Update is called once per frame
    void Update()
    {
        if (snakeScript.snakeState == SnakeState.Golden)
        {
            BounceColour();
        }

        if (snakeScript.snakeState == SnakeState.Ghosted)
        {
            BounceOpacity();
        }
    }

    public void StartGoldMode(float duration)//call this to make the snake flash gold for a certain duration
    {
        if (snakeScript.snakeState == SnakeState.Golden)
        {
            StopCoroutine(goldCoroutine);
        }
        goldCoroutine = StartCoroutine(GoldFor(duration));
    }

    public void StartGhostMode(float duration)//call this to make the snake ghosted for a certain duration
    {
        ghostCoroutine = StartCoroutine(GhostFor(duration));
    }

    private IEnumerator GoldFor(float duration)//call this to make the snake flash gold for a certain duration
    {
        // set the temp vars to be used for bouncing snake between gold colours
        bounceColour1 = goldColour1;
        bounceColour2 = goldColour2;

        bounceSpeed = goldBounceSpeed;
        bounceTimer = 0;

        // Set the state to gold
        snakeScript.snakeState = SnakeState.Golden;

        // Wait for the duration of the golden time
        yield return new WaitForSeconds(duration);

        //return snake to regular state
        snakeScript.snakeState = SnakeState.Alive;

        // Reset the snake's colors to their normal colors
        ResetSnakeColours();
    }
    private void BounceColour()//this bounces the snake's colours between the two colours
    {
        // Calculate the current color of the snake by lerping between goldColour1 and goldColour2
        Color colour = Color.Lerp(bounceColour1, bounceColour2, bounceTimer);

        // Set the colors of the snake's head and segments
        SetSnakeColours(colour, colour);

        // Increment the flash time
        bounceTimer += Time.deltaTime / bounceSpeed;

        // If the flash time has reached 1, reset it and swap the flashing colors
        if (bounceTimer >= 1)
        {
            bounceTimer = 0;
            Color tempColor = bounceColour1;
            bounceColour1 = bounceColour2;
            bounceColour1 = tempColor;
        }
    }
    private IEnumerator GhostFor(float duration)    //call this to make the snake ghosted for a duration
    {
        // set the temp vars to be used for bouncing snake between ghost opacities
        bounceOpacity1 = ghostOpacity1;
        bounceOpacity2 = ghostOpacity2;

        bounceSpeed = ghostBounceSpeed;
        bounceTimer = 0;

        // Set the state to ghosted
        snakeScript.snakeState = SnakeState.Ghosted;

        // Wait for the duration of the ghosting
        yield return new WaitForSeconds(duration);

        //set the state back to normal
        snakeScript.snakeState = SnakeState.Alive;

        // Reset the snake's colors to their normal colors
        ResetSnakeColours();
    }
    private void BounceOpacity() //bounces the snake's opacity between the set values over the set speed
    {
        // Calculate the current opacity of the snake by lerping between ghostOpacity1 and ghostOpacity2
        float opacity = Mathf.Lerp(bounceOpacity1, bounceOpacity2, bounceTimer);

        // Set the colors of the snake's head and segments
        SetSnakeOpacity(opacity);

        // Increment the ghost time
        bounceTimer += Time.deltaTime / bounceSpeed;

        // If the ghost time has reached 1, reset it and swap the ghosting colors
        if (bounceTimer >= 1)
        {
            bounceTimer = 0;
            Color tempColor = bounceColour1;
            bounceColour1 = bounceColour2;
            bounceColour1 = tempColor;
        }
    }

    private void SetSnakeColours(Color _colourEven, Color _colourOdd)// Color _colourOutline)
    {
        // Set the color of the snake head and segments
        for (int i = 0; i < renderers.Count; i++)
        {
            Color col = i % 2 == 1 ? _colourEven : _colourOdd;
            renderers[i].color = col;
        }
    }
    private void SetSnakeOpacity(float opacity)
    {
        Color baseColorWithOpacity = colourEven;
        baseColorWithOpacity.a = opacity;

        Color altColorWithOpacity = colourOdd;
        altColorWithOpacity.a = opacity;

        SetSnakeColours(baseColorWithOpacity, altColorWithOpacity);
    }

    private void ResetSnakeColours()
    {
        // Reset the snake's colors to the original outline and base colors
        SetSnakeColours(colourEven, colourOdd);
    }
}
