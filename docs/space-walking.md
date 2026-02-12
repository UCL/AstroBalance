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