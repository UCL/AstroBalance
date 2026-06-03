# Star Collector

The star collector mini-game uses a ship (controlled by head rotation) to collect a wave of stars falling from the
top of the screen.

## Main objects / values to edit during play testing

- **StarCollectorManager**: values related to the overall win condition of the game, and head speed save data
  - Min / max time limit in seconds
  - Time limit increment in seconds (if time limit upgrade % is met)
  - Number of games in a row that must meet the upgrade % to increase the time limit
  - The length of the time window used to evaluate player performance
  - The % of stars that must be collected to increase speed or time limit
  - The minimum number of head yaw readings needed to calculate a head speed
  - The number of seconds between head yaw speed samples for the save data
  - Whether to write sampled speeds to a text file (star-collector-speeds.txt)
 
- **StarGenerator**: values related to generation of the wave of stars
  - Min, max and base star speed
  - The amount the star speed increases per upgrade
  - Shape of the wave (e.g. width, swerve, star sampling)

- **Ship**: values related to ship movement
  - The amount the ship moves per degree of head movement (X By Degrees)

## Adaptive difficulty

Difficulty is increased in two ways:
- within each game the star fall speed adapts to player performance
- between games, the overall game time limit increases

### Within game (star speed)

Note: all mentioned parameters can be adjusted on `StarGenerator` / `StarCollectorManager`.

- Every game, stars start falling at the same rate of `baseStarSpeed`.
- Every `difficultyWindowSeconds`, the star fall speed either increases or decreases by `speedIncrement`:
  - If the player collects more than `speedUpgradePercent` of the stars that fell in the last `difficultyWindowSeconds`, it increases.
  - Otherwise it decreases.
- Star speed is capped between `minStarSpeed` and `maxStarSpeed`.

### Between games (time limit)

Progression is as follows (all mentioned parameters are configurable on `StarCollectorManager`):
- games start with a time limit of `minTimeLimit`
- a difficulty upgrade occurs when the player has completed `nGamesToUpgrade` games _in a row_ with:
  - the same time limit 
  - more than `timeLimitUpgradePercent` of all stars in the game collected
- For each upgrade, the time limit is increased by `timeLimitIncrement` (up to a maximum of `maxTimeLimit`).
- From then on, all games use the `maxTimeLimit`. 
  
## Save data

Data is saved to `StarCollectorScores.csv`, with one row per played game. Values are:

- `gameNumber`: a unique id per played star collector game
- `sessionNumber`: the session this game was played in (corresponds to sessionNumber in [`SessionSummary.csv`](./session-summary.md))
- `date`: the date of the game session in format YYYY-MM-DD
- `startTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `endTime`: the game end time in format HH:MM:ss (local time - see startTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `timeLimitSeconds`: the time limit set for this game in seconds
- `gameDurationSeconds`: how long the game was played (rounded to the nearest second). If the game was played through to completion this will be equal to timeLimitSeconds; if they exited early, it will be less.
- `nStarsCollected`: the number of stars collected during the game
- `percentStarsCollected`: the percent of all stars collected during the game, rounded to 2 decimal places.
- `adaptiveLevel`: an integer (1 or above) representing the current difficulty level. Every time the game time limit is increased, this level increases by one.
- `finalStarFallSpeed`: Speed of falling stars (unity units per second) at the end of the game.

**Note**: all head speed measures below are calculated as follows. Every `samplingIntervalSeconds` (a configurable parameter in `StarCollectorManager`), the absolute differences of head yaw angle between consecutive readings in the time period are summed together and divided by the total difference in time to give a speed in degrees per second.
If the player goes out of range at any point in those `samplingIntervalSeconds` (i.e. the tracker can no longer detect them), then that speed measurement is discarded. 

At the end of the game, all sampled speed measurements are used to calculate the summary statistics below:

- `headSpeedDegPerSecMean`: Mean head yaw speed (left-right rotation) measured in degrees per second. Rounded to 2 decimal places. 
- `headSpeedDegPerSecPeak`: Peak head yaw speed (left-right rotation) measured in degrees per second. Rounded to 2 decimal places. 
- `headSpeedDegPerSecSD`: Standard deviation of head yaw speed (left-right rotation) measured in degrees per second. Rounded to 2 decimal places.