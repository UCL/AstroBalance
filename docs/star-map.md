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

## Save data

Data is saved to `StarMapScores.csv`, with one row per trial (i.e. one row per attempt at repeating a sequence of stars). This means there are _multiple_ rows per games session. Values are:

- `sessionNumber`: a unique id per game session
- `trialNumber`: a unique id per trial (i.e. per attempt at repeating a sequence of stars). This resets each game session, so the first trial of each session has trialNumber=1.
- `sessionDate`: the date of the game session in format YYYY-MM-DD
- `sessionStartTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `sessionEndTime`: the game end time in format HH:MM:ss (local time - see sessionStartTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `sequenceType`: The order in which the player repeated the sequence - either Forward or Backward. 
- `sequenceLength`: The number of stars in the sequence.
- `responseCorrect`: Whether the player repeated the sequence correctly.
- `responseTimeSeconds`: The number of seconds the player took to repeat the sequence.
- `maxSequenceLength`: The maximum length of sequence _correctly_ repeated in this game session (will be set to the same value for all trials in this game session).
- `constellationSize`: The size of constellation used for this game session - either 'Small' or 'Large'.
