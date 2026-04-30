# Space walking

The space walking mini-game scores the player on completed steps (up / down / left / right), followed by a return to the centre.

## Main objects / values to edit during play testing

- **SpaceWalkingManager**: values related to the overall win condition of the game
  - Minimum game time limit (seconds)
  - Maximum game time limit (seconds)
  - How many seconds to increase the time limit, if the upgrade rate is met
  - Upgrade rate: (number of complete steps / game time limit) i.e. average complete steps per second. A 'complete' step is a step out + a step back to the centre. The rate must be above this value to increase the time limit of future games.
  - Number of games in a row that must meet the upgrade rate
  - Seconds to first tile activation
  - Whether to force enabling of head turns (this is useful for testing, when you don't want to play through the games required to reach this difficulty level)

- **DirectionTiles**: values related to measured steps
  - Number of mm of head movement to count as a step
  - Number of mm of step tolerance (i.e. step +/- tolerance is still counted as a step)
  - Starting distance in mm from the screen

- **Left / Backward / Forward / Right / Centre Tile (inside Direction Tiles) - all instances of the ArrowTile prefab**: values related to tile appearance
  - Colour of activated tile
  - Number of seconds to show the particle system when stepping on a tile

- **HeadTurnArrowLeft / Right / Up / Down prefabs**: values related to head turn sequence (only at highest difficulty level)
  - Start / end head angle in degrees (start = 0 arrow fill; end = full arrow fill)
  - Number of seconds to delay before destroying the arrow on full fill
  - Label text for the arrow
  - Colour of arrow fill

## Save data

Data is saved to `SpaceWalkingScores.csv`, with one row per game session. Values are:

- `sessionNumber`: a unique id per game session
- `sessionDate`: the date of the game session in format YYYY-MM-DD
- `sessionStartTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `sessionEndTime`: the game end time in format HH:MM:ss (local time - see sessionStartTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `timeLimitSeconds`: the time limit set for this game in seconds
- `gameDurationSeconds`: how long the game was played in seconds. If the game was played through to completion this will be equal to timeLimitSeconds; if they exited early, it will be less.
- `nCompleteSteps`: number of completed steps during this game. One complete step = a step out and back to the centre.
- `headTurnsActive`: whether head turn arrows were active for this game (only active at highest difficulty level)
- `nCompleteHeadTurns`: number of completed head turns (will be 0 if `headTurnsActive` is false)
- `adaptiveLevel`: an integer (1 or above) representing the current difficulty level. This increases by 1 every time the time limit is increased, and when head turns are activated at the highest difficulty level.