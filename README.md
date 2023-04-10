# Angry Eels
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

### Gamemode preset feature with multiple "rule sets" for players to choose from and customize further if desired

### Customizable game rules with control over every aspect of gameplay including:
- Speed
- Food types and spawn rates
- Food growth values
- Eel colors
- Number of players
- Starting size
- Death penalty timer 
- and many more

-High score tracking and top 10 leaderboard for each Rule-Set
-Visually appealing menus with user-friendly navigation and simple controls for customization
-Pause game function, restart
-In-game GUI for each player that shows their score, status, and remaining status duration
-Visually appealing graphics and animations for the eels and special effects for their statuses


## Development
Creating Angry Eels was a fun and challenging experience. While making it into an entire game with all the bells and whistles was enjoyable, the most fun I had creating the game was developing the logic for adding tight controls and advanced movement options. It was quite a challenge, but I was thrilled with the end result. With Angry Eels, players can enjoy a classic game with modern features and a unique spin that makes it stand out from other snake games.

## Credits
All the code was written by Griffin Atkinson
All the game art, music, and sound effects were sourced from free game assets websites and free Unity asset content packs, royalty free*
### Links
All UI images
https://craftpix.net/freebies/island-game-gui/

ghost icon
   https://www.vecteezy.com/vector-art/8358647-ghost-in-cute-kawaii-cartoon-style-vector-flat-design-illustration-simple-modern-shape-for-halloween-asset-or-icon-    element-editable
    
snake + eel icon
   https://www.clipartmax.com/

fish (food)
    https://charatoon.com/?dl=1261
    
title music
    https://www.chosic.com/download-audio/27131/
    
pause screen music
    https://www.zapsplat.com/music/easy-cheesy-fun-up-tempo-funky-retro-action-arcade-game-music-great-for-menu-or-pause-sections/

game music
    https://www.zapsplat.com/music/aiming-high-an-electronic-based-energetic-piece-with-80s-elements-and-warm-arpeggios/

splash sound effect
    https://pixabay.com/sound-effects/splash-6213/

*all other sound effects from Zapsplat.com, not requesting credit*


## Future Plans + Ideas
### Quality of life
-make an in-game display showing how much food is worth, and spawn rates
-add controller support
-add ability to save and name custom gamemodes
-remove death penalties entirely for single player
-make time-trial (goal points but for speed record)
-make "caves"-indicators for where snakes will spawn from

### New Features

-add computer players
-add online multiplayer
-add more visual variations of the eel to choose from
-add challenge mode, where the player has to try and get through an obstacle course
-add animation for eels to open their mouths when they get close to food and eat when they get food
-add a big update to the types of food
    powerup to eat other snakes that  "cuts" them off where you intersect them, turning them into food
    make bigger fish that can appear that can be eated in piece "leaving bones in the middle"
    "live" food / fish that can move slowly
-pufferfish/bomb to avoid
-make number of lives
-make battle royale mode, where eels dont respawn until theyre all dead
-make random obstacle maker, and preset stages to choose from.
-make different levels 
   - different level sizes
   - obstacles,
   - warp pipes to other side of level
   - level randomizer
   - level editor
-add powerups (speed, grow a lot, untargettable, drop bomb, )
-make options for food (multiple random spawning, single one that grows you then moves, toggle for dead snakes becoming food )
-golden food that moves/bounces around?
-boss eel (2-3x the size of normal eel)
   


