# Star Seek

The star seek mini-game uses gaze + head position to collect stars that appear at the edges of the screen.

## Main objects / values to edit during play testing

- **StarSeekManager**: values related to the overall win condition of the game
  - Minimum game time limit (seconds)
  - Maximum game time limit (seconds)
  - How many seconds to increase the time limit, if the upgrade rate is met
  - Upgrade rate: (number of stars collected / game time limit) i.e. average stars collected per second, must be above this value to increase the time limit of future games.
  - Number of games in a row that must meet the upgrade rate

- **StarGenerator**: values related to spawning stars
  - Min distance between stars and the edge of the screen
  - Number of rows + columns in star spawn grid
  - Grid positions to exclude from star spawning (e.g. those that overlap with UI elements)
  - Min distance between spawned stars

- **Prefabs/StarSeekStar**: values related to 'locking on' to a star
  - Time required to collect a star (with both gaze + head pose crosshair aligned)
  - Level of bloom (glow) for a star with a single or double lock
