# SimpleSnake
 Angry Eels is a modern take on the classic game "Snake" developed in the Unity Game Engine by Griffin Atkinson. With customizable game rules, advanced gameplay features, competetive multiplayer similar to the classic game "Tron" and visually appealing graphics and sounds, Angry Eels takes the classic gameplay to the next level.

## Original Game
The player controls a snake that moves on a grid in 4 directions, with the objective of collecting food that grows the snake one segment. If the snake hits itself or a wall, it dies and the game is over. The objective of the game is to compete to grow the snake as large as possible for a high score.

## Modern Features
### Advanced input+movement system that allows the player to make difficult movements easily:
- Buffers inputs so that the player does not need to be precise
- Can hold a direction to continue in that direction
- While holding a movement can tap a sideways direction to move "one-step" in that direction before continuing the original direction
- Can hold two directions together to move in a "staircase" path automatically
- Can hold backwards then tap a side direction to automatically u-turn

### Multiplayer 
Local multiplayer mode for up to 4 players on the same or different keyboards, with different game modes including race to a certain number of points and endless play for high scores

### Advanced game-handler that allows quality-of-life enhancements:
- Moves eels in sync while checking if eels collide (after moving them all) so they'll both die instead of the first one trumping the latter
- Eels can't collide with their own tail (or tail of other eels, which you can in original snake game)
- "Ghosting" feature that makes eels invulnerable for a set amount of time on spawn to solve the problem of the player being trapped at spawn with an obstacle in front  of them, killing them on spawn:
- Rare "golden apple" feature where there's a chance for the food to be a golden apple that turns your snake invincible for a short duration, able to go through itself   and other snakes to allow players the chance to play very aggressive and add an element of risk

### Different varieties of food and spawning modes, fun statuses for the eels, optional death penalty for players

###Gamemode preset feature with multiple "rule sets" for players to choose from and customize further if desired

### Customizable game rules with control over every aspect of gameplay including:
- Speed
- Food types and spawn rates
- Food growth values
- Eel colors
- Number of players
- Starting size
- Death penalty timer 
- and many more

### High score tracking and top 10 leaderboard for each Rule-Set

### Visually appealing menus with user-friendly navigation and simple controls for customization

### Pause game function, restart

### In-game GUI for each player that shows their score, status, and remaining status duration

### Visually appealing graphics and animations for the eels and special effects for their statuses


## Development
Creating Angry Eels was a fun and challenging experience. While making it into an entire game with all the bells and whistles was enjoyable, the most fun I had creating the game was developing the logic for adding tight controls and advanced movement options. It was quite a challenge, but I was thrilled with the end result. With Angry Eels, players can enjoy a classic game with modern features and a unique spin that makes it stand out from other snake games.

## Credits
All the code was written by Griffin Atkinson
All the game art, music, and sound effects were sourced from free game assets websites and free Unity asset content packs
### Links


## Future Plans











      TODO + IDEAS BELOW

***********************************************************************************************
    Project Notes
***********************************************************************************************

    when developing on windows and mac, the file systems 
    In Unix systems the end of a line is represented with a line feed (LF). 
    In windows a line is represented with a carriage return (CR) and a line feed (LF) thus (CRLF). 
    when you get code from git that was uploaded from a unix system they will only have an LF.

    the .gitignore helps tremendously with getting the project to run on both OS (mac + windows)
   
   
***********************************************************************************************
    About the Project
***********************************************************************************************

    Angry eels was developed by Griffin Atkinson
    the concept was to recreate the classic game "snake" but thought it would be fun 
    to add many modern features (it was!) 
    The name "angry eels" is to give it a unique spin and separate it from more basic snake games 
    also allow for a fun look for the graphics and audio
    
    Original game: The player controls snake that moves on a grid in 4 directions,
    the objective is to collect food that grows the snake one segment, 
    if the snake hits itself or a wall it dies and the game is over.

    Modern features:
    - game rules:
        - Customizable variables that control every aspect of the gameplay 
            (speed, food types + spawn rates, food growth, enable/disable features, eel colours, # of players, starting size, death timer, + many more)
        - gamemode preset feature that saves multiple "rule sets" for the player to choose from to jump in, 
            and also able to customize further if desired

    - gameplay:
        - advanced input system that allows the player to buffer vertical and horizontal inputs,
            allowing for much tighter controls
        - advanced gameplay loop that allows to check if eels collide theyll 
            both die instead of the first one trumping the latter
        - "ghosting" the eels on spawn for a set amount of time,
            to solve the problem of the player being trapped at spawn with an opstacle killing them on spawn
       

    - visuals and sounds:
        - in-game display for each player that displays information regarding thier score and
            indicators + timers for players to see the remaining duration and current status of their eel
        - visually appealing menu that allows user-friendly navigation of game information,
            and simple controls for customization of the game settings
        - menus and multiple screens (start/menu screen, advanced settings menu, pause menu, game screen)

    - features:
        - multiplayer! (up to 4 eels)
        - multiple game modes (race to # of points OR endless, where players play forever and try to set high scores)
        - pause game function
        - restart + return to menu functions
        - different varieties of food and spawning modes
        - fun statuses for the eels
        - optional death penalty for the players that disallow the players to move for a set amount of time after dying
        - visually appealing graphic animation for the eels and special effects for special statuses of the eels




***********************************************************************************************
    User Story
***********************************************************************************************

    player story
    1 -> Game opens with Menu screen GUI

    2 -> choose game settings
        -choose number of players
        -choose what players are human vs CPU
        -choose player's colours
        -choose game mode (regular - single food, hungry - multiple foods)
        -choose settings (do snakes turn into food when they die, do snakes respawn, grow amount, staring size, etc)
        ?-choose stage (original, preset map, or random obstacles)

    3 -> click start 

    4 -> GUI leaves and snake game is displayed

    5 -> player presses any key and game stars

    6 -> players move the snake using directional controls, collecting food and trying to avoid dying

    7 -> player may pause the game at any time, openning the pause menu and freezing the game - also displaying the options to resume, restart the game, or return to home screen
            (optionally if gamemode is endless, displays a button to save highscore)
    
    8b -> resume is clicked -> returns to 4
    8b -> restart is clicked -> returns to 4
    8c -> return home is clicked -> returns to 1
    8d -> save highscore is clicked -> opens highscore screen -> player inputs highscore and has option to return home

    9 -> game continues indefinitely or until goal points are reached

    10 -> goal points are reached -> win screen appears, allowing player to restart game or return home



***********************************************************************************************
    TODO
***********************************************************************************************

    WHATS LEFT  BEFORE PUBLISHING


    - add save highscore button
    
    - make the about screen (dev info, idea behind it, retro vs new snake comparison demo mode)

    - add a couple better sounds (death sound from worms, splashing, victory sound / music)


    movement
        - add the ability to (chase the tail) where you wont collide with a snake's tail if its moving
    - add the option to use original snake controls and show the contrast between the new control version
    - add button on home screen specifically to test new controls vs original snake game controls

    - add a button on home screen to jump in and play retro snake game
    - add a quick play random button


***********************************************************************************************
    Ideas
***********************************************************************************************

    powerup to eat other snakes that  "cuts" them off where you intersect them, turning them into food
    make bigger fish that can appear that can be eated in piece "leaving bones in the middle"
    "live" food / fish that can move slowly
    make number of lives
    make random obstacle maker, and preset stages to choose from.

    make different levels 
        - different level sizes
        - obstacles,
        - warp pipes to other side of level

    add powerups (speed, grow a lot, untargettable)
    make options for food (multiple random spawning, single one that grows you then moves, toggle for dead snakes becoming food )
    golden food that moves/bounces around
   


***********************************************************************************************
    Unfixable? Bugs
***********************************************************************************************
    was unable to manipulate "AdvancedOptionsScreen" through script at all debugs were saying it was moved when it wasnt
    changing the name to "AdvancedOptionsScreen (1)" fixed the issue entirely, changing it back still causes the issues
    very strange



***********************************************************************************************
    Free Art + Music Assets / Credits
***********************************************************************************************

    ghost icon
        https://www.vecteezy.com/vector-art/8358647-ghost-in-cute-kawaii-cartoon-style-vector-flat-design-illustration-simple-modern-shape-for-halloween-asset-or-icon-element-editable

