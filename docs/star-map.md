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

## Adaptive difficulty

Difficulty is increased in two ways:
- within each game, the length of the sequence adapts to player performance
- between games, the size of the constellation may increase

### Within game (sequence length)

Note: all mentioned parameters can be adjusted on `Constellation`.

- Every game, the first sequence contains `minSequenceLength` stars.
- On each correct guess, the length of the sequence increases by one star (up to a maximum of the number of stars in the constellation).
- If the player repeats a sequence of a certain length incorrectly `maxIncorrectSequence` times, the sequence length is reduced by one star.
- If the sequence length has been reduced, the game will end the next time the player correctly repeats a sequence.

### Between games (constellation size)

Progression is as follows (all mentioned parameters are configurable on `StarMapManager`):
- games start with the small constellation (5 stars total)
- if the player completes a total of `maxScoreGames` _in a row_ with the maximum score (i.e. they repeated a sequence of length == the number of stars in the small constellation), the size is upgraded to the large constellation (10 stars)
- From then on, all games use the large constellation. 

## Save data

Data is saved to `StarMapScores.csv`, with one row per trial (i.e. one row per attempt at repeating a sequence of stars). This means there may be _multiple_ rows per played game. Values are:

- `gameNumber`: a unique id per played star map game
- `sessionNumber`: the session this game was played in (corresponds to sessionNumber in [`SessionSummary.csv`](./session-summary.md))
- `trialNumber`: a unique id per trial (i.e. per attempt at repeating a sequence of stars). This resets each time the game is played, so the first trial of each game has trialNumber=1.
- `date`: the date of the game session in format YYYY-MM-DD
- `startTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `endTime`: the game end time in format HH:MM:ss (local time - see startTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `sequenceType`: The order in which the player repeated the sequence - either Forward or Backward. 
- `sequenceLength`: The number of stars in the sequence.
- `responseCorrect`: Whether the player repeated the sequence correctly (will be blank if the player exited before completing their response).
- `responseTimeSeconds`: The number of seconds the player took to repeat the sequence, rounded to 2 decimal places. This entry will be blank if the player exited before completing their response.
- `totalNumberTrials`: The total number of trials in this game (will be set to the same value for all rows from a single game).
- `maxSpan`: The maximum length of sequence _correctly_ repeated in this game session (will be set to the same value for all trials in a single game).
- `constellationSize`: The size of constellation used for this game - either 'Small' or 'Large'.
