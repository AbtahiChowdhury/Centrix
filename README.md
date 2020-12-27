# Centrix - Game Design Document

**Team Name:** Re:Flex Studio

**Team Members:** Abtahi Chowdhury, Safwan Shahid, Warin Wohab and Samuel Fils

___
### Project Description

The following Game Design Document provides an overview on the design and implementation details for the PC game Centrix. This game is a rhythm based bullet hell game. To survive, the player has to dodge bullets that are targeted at the player in rhythmic patterns. This concept is highly inspired by the game, Soundodger+.

___
### Technical Description

- Targeted Platform
  - PC
- Game Engine
  - Unity 2D
- Version Control
  - Github
  - Current repository for this project is [https://github.com/AbtahiChowdhury/Centrix](https://github.com/AbtahiChowdhury/Centrix)

___
### Player Movement and Controls

In a 2D coordinate system the player can move in the x-axis and y-axis. Like many keyboard games, if the WASD or arrow keys are pressed, the player moves in the corresponding direction. This also works with the mouse. For gamepad users, the left joystick can be used for movement. Only one type of movement control takes priority. The speed in which the player moves is constant. However, there is a restriction to movement. The player&#39;s movement is bounded within the circular stage.

The player also has the ability to clear the stage with a bomb. By pressing the key Z or gamepad control (PS4) X, the spawned bullets are destroyed. The restriction to this ability is that the player can only use it three threes in a given level. If the player wants to pause the game, the key Esc or gamepad control (PS4) O can be pressed.

___
### Level Design

When the gameplay scene is loaded, a predetermined number of bullets spawners will be placed evenly around the stage. These spawners will then begin to rotate around the edge of the stage. Bullet spawning is done using these spawners. A bullet will spawn in the center of the spawner with an initial direction vector facing towards the center of the stage. The initial direction vector is then rotated by a given theta and that final vector is normalized and set as the direction the bullet will move in. The bullets themselves move in that direction with a speed that is determined by a variable in GameManager. The spawning of the bullets will happen both automatically using beat detection and manually using a series of queues. Automatic bullet spawning will occur when the AudioSyncer passes the bias variable in GameManager. Then this happens, 2 bullets are spawned with random direction. Manual Bullet spawning is done using a queue that holds the beat the bullet should be spawn in on, the spawner the bullet spawns on, and the theta offset on the normal direction vector. The bullets will destroy themselves when they exit the stage. This queue is filled at the beginning of the level with the use of various helper functions that allow the creation of patterns using various parameters.

Alongside the bullet queue, there are various other queues that are used to make the levels more dynamic. The following is a list of all the queues used:

- Spawner rotation queue: changes the rotation speed and direction of the spawners using the beat the event should happen and the new rotation speed.
- Bullet speed queue: changes the speed of all bullets using the beat the event should happen and the new speed.
- Toggle random bullet spawning queue: toggles the spawning of random bullets using the beat the event should happen.
- AudioSyncer bias queue: changes the AudioSyncer bias value using the beat the event should happen.
- AudioSyncer timestep queue: changes the AudioSyncer timestep value using the beat the event should happen.

All of these queues, alongside the bullet spawning queue, are filled with predetermined values when the scene is loaded. On each frame, the first value in each queue is checked and the event is performed accordingly.

___
### Collision Detection

Both the player and bullets have circular hitboxes at the center of the sprite. However the hitboxes will be smaller than the sprite themselves. This is done to allow the player to have more freedom of movement. If the player collides into a bullet, the bullet will change color from black to red and the accuracy will go down. If a bullet collides with a bomb, it will be destroyed.

___
### Bomb Mechanics

On each level, the player is given 3 bombs to use throughout the level. They are not penalized for using it, however they do not have to use all 3 of the bombs. When the bomb is used it will spawn a circle where the player is and will begin to rapidly expand from that point. As the circle expands, any bullets it comes in contact will be destroyed. Once it reaches a certain size, the circle and bomb will be destroyed.

___
### Beat Detection

The stage &amp; spawners are 2D circle game objects that will respond to the beat of the currently playing music. The stage will perform a beat animation and the spawners will shoot bullets, creating a nice rhythmic based visual. This beat detection uses an open source Unity AudioSyncer developed by RenaissanceCode. The AudioSyncer uses an array for spectrum data that is produced from the currently playing music. This data is then used to compare loudness of the previous and current audio frame. If the difference of those two values passed a threshold it will be counted as a beat.

___
### Scoring

The score is calculated using 2 different formulas. One will calculate the accuracy and the other will calculate the final score of the player. The following formula will calculate the accuracy:

```csharp
Accuracy = Mathf.Clamp((bulletsFired - bulletsHit) / bulletsFired, 0f, 100f)
```

When calculating accuracy the bulletsFired variable will only track the bullets fired through the automatic bullet firing through beat detection. The bullets fired manually are not counted towards the accuracy calculation. This is done to make hitting manually fired bullets more punishing than the automatically fired bullets as they are fired to surround the player and not directly hit them. The following formula will calculate the final score:

```csharp
FinalScore = Mathf.Lerp(0, 1000000f, accuracy)
```

This final score is then displayed to the player in an end-of-game screen.

___
### Levels &amp; Progression

The game has 3 levels for the player to choose from with each level using a different song. All 3 levels will be available for the player to choose from at all times. Once the user selects the &quot;Play&quot; option from the main menu, they have the option to choose one of these levels. A music may have high or low moments for activity, and the difficulty of the game will show this. This will make each level unique and challenging. The overall difficulty of the level will increase as the player progresses through the levels.

___
### UI/UX

The main menu will consist of 3 total buttons. There will be a &quot;Play&quot; button which allows the user to select a level to play, once directed to the Level Selection Menu. There will also be a &quot;Settings&quot; button which gives the user the ability to change various settings. Lastly there will be an &quot;Quit&quot; button that closes the game.

The player can click the &quot;Settings&quot; button to go to the settings menu. In the menu, the player has the opportunity to change the sensitivity of the movement before playing the game. The reason the player can only change it before the start of the game is because if the player is allowed to do it during a level, he or she can constantly change it based on how the level goes. The sensitivity has a range from 1 to 10 and can only be changed to whole numbers. For example, a player can change the sensitivity to 5, but can not change it to 5.1. After the player has finished changing their sensitivity, they can press the &quot;Back&quot; button and return to the main menu.

If the player finishes a level, the player will see their score and accuracy displayed and can click on three buttons. They can press the &quot;Main Menu&quot; button which directs the user to the main menu. They can also click on the &quot;Play Again&quot;, which restarts the level the player just beat. There will also be a &quot;Level Selection&quot; which directs the user to a screen where he/she can select the level of their choice. Lastly, there will be an &quot;Quit&quot; option which closes the application.

If the player fails to survive, the player will see their score, accuracy, or health displayed and can click on three buttons. They can press the &quot;Main Menu&quot; button which directs the user to the main menu. They can also click on the &quot;Retry&quot;, which restarts the level the player just lost in. There will also be a &quot;Level Selection&quot; which directs the user to a screen where he/she can select the level of their choice. Lastly, there will be an &quot;Exit&quot; option which closes the application.

___
# Breakdown of Roles

**Abtahi Chowdhury:**

- Spawner mechanics &amp; Bullet spawning
  - Mechanics behind how the spawners are positioned and move throughout the level
  - Mechanics behind how bullets are spawned into the stage and how they will move
- Level design
  - Mechanics behind changing spawner rotation speed and direction dynamically during runtime
  - Mechanics behind changing bullet speed dynamically during runtime
  - Mechanics behind toggling random bullet spawning dynamically during runtime
  - Mechanics behind changing AudioSyncer bias dynamically during runtime
  - Mechanics behind changing AudioSyncer timestep dynamically during runtime
  - Putting all of these together to make dynamic and fun to play levels
- Collision detection
  - Mechanics behind how bullets interact with the player
- Bomb mechanics
  - Mechanics behind spawning in an expanding bomb that clears the field
- Scoring
  - Running accuracy and final score calculations and displays

**Samuel Fils:**

- Player Movement
  - Mechanics behind using 3 different controls that are described in previous sections. Gamepad, mouse, and arrow keys are the main controls for gameplay.
- Controls
  - Mechanics behind player movement based on current sensitivity &amp; boundary.
- Beat Detection (AudioSyncer)
  - Using Unity&#39;s audio spectrum function to detect a beat from a current song based on the amplitude of it as described in previous sections.
- Colorful Visualization
  - Mechanics behind beat animations based on the current beat. Also other colorful lighting visualizations based on the beat.

**Warin Wohab:**

- UI/UX
  - Created the main menu and neon theme
  - Created buttons that lead to their corresponding pages
  - Enabled level selection buttons to load the game screen to their corresponding levels
  - Created Menu script file to add functionality to menu buttons
  - Created and enabled back button to switch between pages.
  - Tested all aspects of the game to identify bugs and errors.

**Safwan Shahid:**

- UI/UX:
  - Contributed to the main menu by putting together descriptions of the instructions and controls for the game.
  - Created a pause menu that appears when pressing the &#39;esc&#39; button. When the menu appears the rest of the game must be deactivated. It will stop the music, the generated particles along with the player object.
  - Created settings menu that allows the user to change the sensitivity of the movement before starting the game.
- Collaboration:
  - Tested various drafts of the game and collaborated with teammates on how to better improve what we had through zoom meetings.
- Website:
  - Created and organized a website that can properly represent us and our game.