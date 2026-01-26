# Star Map

The star map mini-game highlights a pattern of stars that the player must repeat in the same
or opposite order.

## Main objects / values to edit during play testing

- **StarMapManager**: values related to the overall win condition of the game
  - The number of sequences that must be correctly repeated to win.

- **Constellation**: values related to showing / selecting sequences of stars in the constellation
  - Minimum number of stars in a sequence
  - Number of incorrect sequences before reducing length of the next
  - Length of time to highlight stars when showing a new sequence, or completing a correct / incorrect sequence
  - Length of time to delay before highlighting stars

- **Stars 1-10 (inside Constellation/stars) - all instances of the StarMapStar prefab**: 
  values related to selection of stars
  - Number of seconds of held gaze required to select a star
  - Amount of size increase on correct selection
  - Amount of size decrease on incorrect selection
  - Colour for correct selection
  - Colour for incorrect selection