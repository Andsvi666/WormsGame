# Worms Game in Console

Classic worms type of game made using C# and fully working within console terminal.

## Description

Worms is a classic turn-based strategy game played between two or more teams, where the goal is to defeat all opposing teams by eliminating their worms using a variety of weapons and environmental mechanics. The game is played in rounds, and during each round one worm from every team gets a turn; in the following round, different worms from each team take their turns, and this cycle continues until the game ends. At the start of a worm’s turn, it receives a randomly selected weapon, and if the worm already owns that weapon, additional ammunition is added instead. During its turn, a worm can perform several actions, including walking and jumping using a limited number of movement points that are consumed while moving, selecting an enemy worm to attack, and choosing a weapon to use. Each weapon has its own properties such as damage, range, and area-of-effect radius. Some maps include water, represented by the absence of tiles at the bottom of the map, and if a worm moves into water, it dies immediately.

Game also has system of being able to save it and play it from saved state. After each round user is asked if he wants to create a saved game state log and it is created then when starting the game it can be chosed from one of the logs and continued instead of being played as a new game. When game is fully finished a seperate log is also created with history of all actions taken during game. folder "GameLogs" contains all of this and files within it should not be edited but they can be removed as it just stores game saves in a sense.


### Dependencies

* .NET Runtime (or .NET SDK if running from source)

### Installing

* Donwload project from github
* Unzip the folder in desired location

### Executing program

* Within main folder click "run.bat" to run the project

Or

* Run the terminal
* Select project folder
* Write the following command
```
dotnet run
```

## Help

Since the game is played entirely within the terminal, all gameplay is controlled through keyboard input and typed commands. These commands must be entered exactly as described by the game; for example, if the command instructs the player to type "Walk", it must be written precisely as "Walk" and not as "WALK" or "walk". Command input is case-sensitive, and correct capitalization is required for the game to recognize and execute commands properly.

