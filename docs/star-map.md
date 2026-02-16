# Star Map

The star map mini-game highlights a pattern of stars that the player must repeat in the same
or opposite order.

## Main objects / values to edit during play testing

- **StarMapManager**: values related to the overall win condition of the game
  - The number of maximum score games in a row required to upgrade from the small to the large constellation.

- **Constellation (on the SmallConstellation and LargeConstellation prefabs)**: values related to showing / selecting sequences of stars in the constellation
  - Minimum number of stars in a sequence
  - Number of incorrect sequences before reducing length of the next
  - Length of time to highlight stars when showing a new sequence, or completing a correct / incorrect sequence
  - Length of time to delay before and after highlighting stars

- **StarMapStar prefab**: values related to selection of stars
  - Amount of size increase on correct selection
  - Amount of size decrease on incorrect selection
  - Colour for correct selection
  - Colour for incorrect selection