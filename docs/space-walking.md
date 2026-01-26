# Space walking

The space walking mini-game scores the player on completed steps (up / down / left / right), followed by a return to the centre.

## Main objects / values to edit during play testing

- **SpaceWalkingManager**: values related to the overall win condition of the game
  - Number of completed steps required to win the game
  - Seconds to first tile activation

- **DirectionTiles**: values related to measured steps
  - Number of mm of head movement to count as a step
  - Number of mm of step tolerance (i.e. step +/- tolerance is still counted as a step)
  - Starting distance in mm from the screen

- **Left / Backward / Forward / Right / Centre Tile (inside Direction Tiles) - all instances of the ArrowTile prefab**: values related to tile appearance
  - Colour of activated tile
  - Number of seconds to show the particle system when stepping on a tile