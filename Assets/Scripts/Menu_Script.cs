using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum RuleSet
{
    Custom,
    Classic,
    Casual,
    Wild
}
public class Menu_Script : MonoBehaviour
{
    private GameMode gameMode = GameMode.Endless;// The currently selected game mode - defaulted to endless
    private int goalPoints = 0; // The goal points for the "First to" game mode - defaulted to 0
    private int numPlayers = 1; // The number of players in the game - defaulted to 1
    private bool[] activePlayers = {true,false,false,false}; //which players are in the game - defaults to just player 1
    private Options options = new Options();    // The values for all options

    // The names and values for the preset options
    private Dictionary<string, Options> presetOptions = new Dictionary<string, Options>
    {
            // Ruleset ruleSet 
            // f snakeSpeed, i startingSize, f ghostModeDuration, f deathPenaltyDuration,
            // i normalFoodGrowthAmount, f goldFoodSpawnChance,  i goldFoodGrowthAmount, b doSnakesTurnToFood
        { "Custom", new Options() },
        { "Classic", new Options(
            RuleSet.Classic,
            0.12f, 3, 2f, 0f, 
            1, 30, 20, false
        ) },
        { "Casual", new Options(
            RuleSet.Casual, 
            0.1f, 5, 3f, 3f, 
            3, 15, 25, true
        ) },
        { "Wild", new Options(
            RuleSet.Wild, 
            0.05f, 3, 2f, 5f, 
            5, 15, 50, true
        ) }
    };
    // The currently selected preset index - defaults to classic mode
    private int selectedPreset = 1;

    public Dictionary<string, Color> SnakeColoursDictionary = new Dictionary<string, Color>
    {
        { "Green", Color.Lerp(Color.green, Color.black, .2f)},
        { "Red", Color.red},
        { "Blue", Color.blue},
        { "Orange", Color.Lerp(Color.yellow, Color.black, .2f) + Color.red},
        { "Gray", Color.gray},
        { "Purple", Color.red + Color.blue},
        //{ "Brown", Color.red + Color.green},
        { "Teal", Color.Lerp(Color.green, Color.black, .2f) + Color.blue},
        { "Pink", Color.magenta}
    };
    private GameHandler_Script gameHandlerScript;// A reference to the game handler script
    private Transform menuParent;//the parent object to the menus - used to move them all together
    private GameObject titleScreen;//the title screen object
    private GameObject mainMenuScreen;// The menu screen object
    private GameObject advancedOptionsScreen;// The advanced options screen object

    private GameObject[] playerIndicators;// The player indicators game objects

    private int[] playerColoursInts = new int[]{0,1,2,3};   //the default eel colours (defaults to 1,2,3,4)
    //all the gameobjects (buttons and text displays) on the menu screens that have changing displays
    Dictionary<string, GameObject> MenuScreenObjects;

    void Start()    // Start is called before the first frame update
    {
        // Get a reference to the game handler script
        gameHandlerScript = GameObject.Find("GameHandler").GetComponent<GameHandler_Script>();

        //grab the menu parent
        menuParent = GameObject.Find("MenuParent").transform;
        //grab the title menu screen
        titleScreen = GameObject.Find("TitleScreen");
        //grab the main menu screen
        mainMenuScreen = GameObject.Find("MainMenuScreen");
        //grab the advanced options menu screen
        advancedOptionsScreen = GameObject.Find("AdvancedOptionsScreen (1)");

        //move screens into position on game load
        menuParent.localPosition = Vector2.zero;
        titleScreen.transform.localPosition = Vector2.zero;
        mainMenuScreen.transform.localPosition = new Vector2(0, -500);
        advancedOptionsScreen.transform.localPosition = new Vector2(0, -500);


        //grab all the menu screen's changing objects 
        MenuScreenObjects = new Dictionary<string, GameObject>
        {
            //title screen objects - none

            //main menu objects
            { "GameModeBlock",  GameObject.Find("GameModeBlock")},
            { "RulesetBlock",  GameObject.Find("RulesetBlock")},
            
            //advanced options objects
            { "PresetsText",  GameObject.Find("PresetsText")},
            { "CustomizeBtn",  GameObject.Find("CustomizeBtn")},
            { "P1ColourDropdown",  GameObject.Find("P1ColourDropdown")},
            { "P2ColourDropdown",  GameObject.Find("P2ColourDropdown")},
            { "P3ColourDropdown",  GameObject.Find("P3ColourDropdown")},
            { "P4ColourDropdown",  GameObject.Find("P4ColourDropdown")},
            { "SnakeSpeed",  GameObject.Find("InputField1")},
            { "StartingSize",  GameObject.Find("InputField2")},
            { "GhostModeDuration",  GameObject.Find("InputField3")},
            { "DeathPenaltyDuration",  GameObject.Find("InputField4")},
            { "NormalFoodGrowthAmount",  GameObject.Find("InputField5")},
            { "GoldFoodSpawnChance",  GameObject.Find("InputField6")},
            { "GoldFoodGrowthAmount",  GameObject.Find("InputField7")},
            { "DoSnakesTurnIntoFood",  GameObject.Find("Toggle1")},
        };

        //setup main menu screen

        // Get the player indicators 
        playerIndicators = new GameObject[4];
        Transform indicatorsParent = GameObject.Find("PlayerIndicators").transform;
        for (int i = 0; i < 4; i++) { playerIndicators[i] = indicatorsParent.GetChild(i).gameObject; }

        //setup advanced options screen
        //reset, then set the colour selector dropdowns to contain all the script-definied snake colour values
        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().ClearOptions();
        
        List<string> colourNames = new List<string>(SnakeColoursDictionary.Keys);
        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().AddOptions(colourNames);

        MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[0];
        MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[1];
        MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[2];
        MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().value = playerColoursInts[3];

        //setup player indicators
        ResetPlayers();
        
        //setup the preset selector and display the default values
        DisplayPreset(selectedPreset);
        //lock the controls if displaying a preset (not custom)
        UpdateAdvancedOptionsLock();

        //then update gamemode indicator to reflect the default selection
        UpdateGameMode(options.gameMode);

        //then hide the advanced options screen by default
        HideAdvancedOptions();
    }


    public void StartGame()    // Starts the game, sends all options values to the game handler and tells it to start
    {
        // Start the game using the current game mode, goal points, number of players, and advanced options
        // get all advanced options
        options = presetOptions.ElementAt(selectedPreset).Value;

        //gettings all the player colours
        playerColoursInts[0] = MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[1] = MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[2] = MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>().value;
        playerColoursInts[3] = MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>().value;
        Color[] playerColours = new Color[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {   playerColours[i] = SnakeColoursDictionary.ElementAt(playerColoursInts[i]).Value;    }
        
        //setting the player colours
        options.playerColours = playerColours;

        //setting all the rest of the values for the current options
        options.gameMode = this.gameMode;
        options.goalPoints = this.goalPoints;
        options.activePlayers = this.activePlayers;
        options.numPlayers = this.numPlayers;

        //close the menu's windows
        Close();

        //tell the handler to start the game
        gameHandlerScript.InitializeGame(options);
    }

    //called every frame
    void Update()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            //if a player presses a key while on menu screen
            for (int i = 0; i < 4; i++)
            {
                if (Input.GetKeyDown(gameHandlerScript.playerInputs[i,0]) ||Input.GetKeyDown(gameHandlerScript.playerInputs[i,1]) ||Input.GetKeyDown(gameHandlerScript.playerInputs[i,2]) ||Input.GetKeyDown(gameHandlerScript.playerInputs[i,3]) )
                {
                    //if player isnt active - add them
                    if(activePlayers[i] == false)
                    {
                        AddPlayer(i);
                    }
                    else
                    {   //if a player is active and presses up button 
                        if (Input.GetKeyDown(gameHandlerScript.playerInputs[i,0]))
                        {
                            //cycle their player colour positively one increment
                            IncrementPlayerColour(i, 1);
                        }
                        //if a player is active and presses down button
                        if (Input.GetKeyDown(gameHandlerScript.playerInputs[i,1]))
                        {   //cycle their player colour negatively one increment
                            IncrementPlayerColour(i, -1);
                        }
                    }
                }
            }
        }
        else
        {
            //Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void Open(ActiveScreen screen){// Open the menu screens, able to open to a specific screen   
        switch (screen)
        {
            case ActiveScreen.Title:
                gameHandlerScript.activeScreen = ActiveScreen.Title;
                menuParent.localPosition = Vector2.zero;
                break;
            case ActiveScreen.MainMenu:
                gameHandlerScript.activeScreen = ActiveScreen.MainMenu;
                menuParent.localPosition = new Vector2(0,500);
                break;
            case ActiveScreen.AdvancedOptions:
                gameHandlerScript.activeScreen = ActiveScreen.AdvancedOptions;
                ShowAdvancedOptions();
                menuParent.localPosition = new Vector2(0,500);
                break;
            default:
                Debug.LogError("opened menu but didnt set active screen to a valid menu screen");
                break;
        }
        menuParent.gameObject.SetActive(true);
    }
    public void Close()
    {
        // Close the menu screens
        menuParent.gameObject.SetActive(false);
    }

    public void IncrementPlayerColour(int pNum, int i){
        //called to cycle player colours in menu display, also changing the advanced options menu values 

        //setup a couple useful vars
        int totalColours = SnakeColoursDictionary.Count;
        Dropdown[] dropdowns = new Dropdown[]{
            MenuScreenObjects["P1ColourDropdown"].GetComponent<Dropdown>(),
            MenuScreenObjects["P2ColourDropdown"].GetComponent<Dropdown>(),
            MenuScreenObjects["P3ColourDropdown"].GetComponent<Dropdown>(),
            MenuScreenObjects["P4ColourDropdown"].GetComponent<Dropdown>()
        };
        int newColourVal = playerColoursInts[pNum] += i;
        //if value is outside of range, subtract or add total number of colours until it is
        while (newColourVal < 0 || newColourVal > totalColours-1)
        {
            if(newColourVal > totalColours-1){
                //if player colour is bigger than max value, rollover from min value
                newColourVal -= totalColours;
            }else if (newColourVal < 0){
                //if player colour is smaller than min value, rollover from max value
                newColourVal += totalColours;
            }
        }

        //save new colour val
        playerColoursInts[pNum] = newColourVal;        
        //set the player indicator to the correct colour
        playerIndicators[pNum].transform.GetChild(2).GetChild(1).GetComponent<Image>().color = SnakeColoursDictionary.ElementAt(playerColoursInts[pNum]).Value;
        //update advanced options screen colour selectors
        dropdowns[pNum].value = newColourVal;
    }

    //////////////////////////////////////////////////////// title screen buttons + methods

    public void TitleStartBtn()    // btn press to go to main
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.Title){
            gameHandlerScript.activeScreen = ActiveScreen.MainMenu;
            ResetPlayers();
            StartCoroutine(TransitionMenuParent(new Vector2(0,0), new Vector2(0,500), .75f));
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }

    private IEnumerator TransitionMenuParent(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            menuParent.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        menuParent.transform.localPosition = endPos;
    }

    public void TitleViewHighScoresBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.Title){
        
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void TitleAboutBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.Title){
        
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }

    public void TitleExitGameBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.Title){
            gameHandlerScript.ExitGame();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    

    //////////////////////////////////////////////////////// main menu screen buttons + methods
    void AddPlayer(int pNum){ //sets a player (called by index) as active and shows their indicator
        if(activePlayers[pNum] == false){
            activePlayers[pNum] = true; //set the player to be active
            playerIndicators[pNum].transform.GetChild(1).gameObject.SetActive(false);  //hide the presskey prompt
            playerIndicators[pNum].transform.GetChild(2).gameObject.SetActive(true);   //show the player indicator
            playerIndicators[pNum].transform.GetChild(2).GetChild(1).GetComponent<Image>().color = SnakeColoursDictionary.ElementAt(playerColoursInts[pNum]).Value;//set the player indicator to the correct colour
            numPlayers += 1;
        }
    }

    void ResetPlayers(){//loops through all players and indicators and resets them - keeps player one active
        numPlayers = 1;
        for (int i = 1; i < 4; i++)
        {
            activePlayers[i] = false;
            playerIndicators[i].transform.GetChild(1).gameObject.SetActive(true);   //show the presskey prompt
            playerIndicators[i].transform.GetChild(2).gameObject.SetActive(false);  //hide the player indicator
            playerIndicators[i].transform.GetChild(2).GetChild(1).GetComponent<Image>().color = Color.white;    //reset the indicator to white
        }

    }

    public void MainMenuBackBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            gameHandlerScript.activeScreen = ActiveScreen.Title;
            StartCoroutine(TransitionMenuParent(new Vector2(0,500), new Vector2(0,0), .75f));
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }

    public void MainMenuGamemodeIncreaseBtn()    // btn press to increase target goal points
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            goalPoints += 10;
            UpdateGameMode(GameMode.Points);
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void MainMenuGamemodeDecreaseBtn()   // btn press to decrease target goal points
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            if (goalPoints >= 10)//if goalpoints exist, decrease them by 10 and display the value
            {
                goalPoints -= 10;
                UpdateGameMode(GameMode.Points);
            }
            if(goalPoints <= 0){
                UpdateGameMode(GameMode.Endless);//if points hit 0 or negative, switch to endless gamemode
            }
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void UpdateGameMode(GameMode gm){ //called to update the gamemode + display (endless or # points)
        if(gm == GameMode.Endless){
            if (gameMode != GameMode.Endless) { gameMode = GameMode.Endless;}//set gamemode
            goalPoints = 0; //set points to 0
            MenuScreenObjects["GameModeBlock"].transform.GetChild(1).GetComponent<Text>().text ="Endless GameMode"; //display gamemode
            MenuScreenObjects["GameModeBlock"].transform.GetChild(2).gameObject.SetActive(false); //hide the negative button (points are already at 0)
        }
        else if (gm == GameMode.Points){
            if (gameMode != GameMode.Points) { gameMode = GameMode.Points;}//set gamemode
            MenuScreenObjects["GameModeBlock"].transform.GetChild(1).GetComponent<Text>().text = goalPoints.ToString() + " points to win"; //display points+message
            MenuScreenObjects["GameModeBlock"].transform.GetChild(2).gameObject.SetActive(true); //show the negative button (points are past 0)
        }
    }

    public void MainMenuRulesetIncreaseBtn(){//btn press the right ruleset btn on main menu
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            NextPreset();
            //why reinvent the wheel, calls the next preset button on the advanced options page
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void MainMenuRulesetDecreaseBtn(){//btn press the left ruleset btn on main menu
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            PreviousPreset();
            //why reinvent the wheel, calls the next preset button on the advanced options page
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void MainMenuStartRetroSnakeBtn(){        //unused
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
        
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void MainMenuCustomizeRulesBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            gameHandlerScript.activeScreen = ActiveScreen.AdvancedOptions;
            ShowAdvancedOptions();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void MainMenuPlayRandomBtn(){        //unused
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
        
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }

    }
    public void MainMenuStartBtn(){
        if(gameHandlerScript.activeScreen == ActiveScreen.MainMenu){
            StartGame();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }


    //////////////////////////////////////////////////////// advanced options screen buttons + methods
    public void AdvancedOptionsCloseBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.AdvancedOptions){
            if(selectedPreset == 0){//if using the custom preset, first save the values from the screen
                SaveAdvancedOptionsFromScreen();   
            }
            gameHandlerScript.activeScreen = ActiveScreen.MainMenu;//switch active screen to main menu
            HideAdvancedOptions();//close advanced options
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void AdvancedOptionsStartBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.AdvancedOptions){
            if(selectedPreset == 0){//if using the custom preset, first save the values from the screen
                SaveAdvancedOptionsFromScreen();   
            }
            HideAdvancedOptions();
            StartGame();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void AdvancedOptionsPresetDecreaseBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.AdvancedOptions){
            PreviousPreset();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void AdvancedOptionsPresetIncreaseBtn()
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.AdvancedOptions){
            NextPreset();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }
    public void AdvancedOptionsCustomizeBtn()//saves the preset's values to the custom preset, it allows the user to customize an existing preset
    {
        if(gameHandlerScript.activeScreen == ActiveScreen.AdvancedOptions){
            //save the selected preset options to override the custom preset
            presetOptions["Custom"] = presetOptions.ElementAt(selectedPreset).Value;
            //set the selected preset back to custom
            selectedPreset = 0;
            DisplayPreset(selectedPreset);
            UpdateAdvancedOptionsLock();
        }
        else
        {
            Debug.Log("trying to press a button thats currently inactive");
        }
    }

    public void ShowAdvancedOptions()
    {
        // Open the advanced options screen
        advancedOptionsScreen.SetActive(true);
    }
    public void HideAdvancedOptions()
    {   //save the displayed advanced options from screen first
        SaveAdvancedOptionsFromScreen();
        // Close the advanced options screen
        advancedOptionsScreen.SetActive(false);
    }


    public void NextPreset()//selects next preset and displays it on menus
    {
        // Increase the selected preset index, wrapping around to the first preset if necessary
        if(selectedPreset == 0){
            //save the shown advanced options to custom
            SaveAdvancedOptionsFromScreen();
        }
        selectedPreset ++;

        if (selectedPreset >= presetOptions.Count)
        {
            selectedPreset = 0;            
        }

        DisplayPreset(selectedPreset);
        UpdateAdvancedOptionsLock();
    }
    public void PreviousPreset()//selects previous preset and displays it on menus
    {
        // Decrease the selected preset index, wrapping around to the last preset if necessary
        if(selectedPreset==0){//if changing from custom options preset, save options first
            SaveAdvancedOptionsFromScreen();
        }

        selectedPreset--;

        if (selectedPreset < 0)
        {
            selectedPreset = presetOptions.Count - 1;
        }
        DisplayPreset(selectedPreset);
        UpdateAdvancedOptionsLock();
    }
    public void UseRetroSnakePreset()//unused
    {
        //when playing the retro snake game
    }

    public void UpdateAdvancedOptionsLock(){
        //if preset shown is not custom, set menu components to not interactable
        bool b = false;
        if(selectedPreset == 0){
            //if not displaying a preset, lock controls
            b = true;
        }
        //locks the "cutomize button" - only available when looking at existing presets (not custom)
        MenuScreenObjects["CustomizeBtn"].GetComponent<Button>().interactable = !b;
        //used to lock menu controls when displaying a preset
        MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["StartingSize"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().interactable = b;
        MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().interactable = b;
    }

    private void UseAdvancedOptionsFromPreset(int i)
    {   //updates the script's options variables from the selected preset and displays them on menus
        // Get the advanced options for the preset
        Options tempOptions = presetOptions.ElementAt(i).Value;

        // Set the values of the input fields for speed and duration values
        options.snakeSpeed = tempOptions.snakeSpeed;
        options.startingSize = tempOptions.startingSize;
        options.ghostModeDuration = tempOptions.ghostModeDuration;
        options.deathPenaltyDuration = tempOptions.deathPenaltyDuration;
        options.normalFoodGrowthAmount = tempOptions.normalFoodGrowthAmount;
        options.goldFoodSpawnChance = tempOptions.goldFoodSpawnChance;
        options.goldFoodGrowthAmount = tempOptions.goldFoodGrowthAmount;

        // Set the value of the toggle for the "do snakes turn to food" option
        options.doSnakesTurnToFood = tempOptions.doSnakesTurnToFood;

        //reflect changes on screen
        DisplayPreset(i);
    }

    private void DisplayPreset(int i)
    {   //updates the advanced options screen from the script's selected preset option
        Options tempOptions = presetOptions.ElementAt(i).Value;

        //display the title of the preset
        MenuScreenObjects["PresetsText"].GetComponent<Text>().text = presetOptions.ElementAt(i).Key; //displays the currently selected preset name on advanced options
        MenuScreenObjects["RulesetBlock"].transform.GetChild(1).GetComponent<Text>().text = presetOptions.ElementAt(selectedPreset).Key; ; //displays the currently selected preset name on main menu

        // Set the values of the input fields for speed and duration values
        MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().text = tempOptions.snakeSpeed.ToString();
        MenuScreenObjects["StartingSize"].GetComponent<InputField>().text = tempOptions.startingSize.ToString();
        MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().text = tempOptions.ghostModeDuration.ToString();
        MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().text = tempOptions.deathPenaltyDuration.ToString();
        MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().text = tempOptions.normalFoodGrowthAmount.ToString();
        MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().text = tempOptions.goldFoodSpawnChance.ToString();
        MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().text = tempOptions.goldFoodGrowthAmount.ToString();
        // Set the value of the toggle for the "do snakes turn to food" option
        MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().isOn = tempOptions.doSnakesTurnToFood;
    }
    public void SaveAdvancedOptionsFromScreen() //saves the displayed options to "custom" preset
    {
        Options tempOptions = new Options();//create temp place to store advanced options
        tempOptions.ruleSet = RuleSet.Custom;
        //grab all values displayed on advanced options screen
        tempOptions.snakeSpeed = float.Parse(MenuScreenObjects["SnakeSpeed"].GetComponent<InputField>().text);
        tempOptions.startingSize = int.Parse(MenuScreenObjects["StartingSize"].GetComponent<InputField>().text);
        tempOptions.ghostModeDuration =  float.Parse(MenuScreenObjects["GhostModeDuration"].GetComponent<InputField>().text);
        tempOptions.deathPenaltyDuration = float.Parse(MenuScreenObjects["DeathPenaltyDuration"].GetComponent<InputField>().text);
        tempOptions.normalFoodGrowthAmount = int.Parse(MenuScreenObjects["NormalFoodGrowthAmount"].GetComponent<InputField>().text);
        tempOptions.goldFoodSpawnChance = float.Parse(MenuScreenObjects["GoldFoodSpawnChance"].GetComponent<InputField>().text);
        tempOptions.goldFoodGrowthAmount = int.Parse(MenuScreenObjects["GoldFoodGrowthAmount"].GetComponent<InputField>().text);
        tempOptions.doSnakesTurnToFood = MenuScreenObjects["DoSnakesTurnIntoFood"].GetComponent<Toggle>().isOn;
        //save options to custom preset
        presetOptions["Custom"]= tempOptions;
    }

}