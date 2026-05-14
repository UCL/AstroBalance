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

## Adaptive difficulty

Difficulty is increased between games by increasing the overall time limit.

Progression is as follows (all mentioned parameters are configurable on `StarSeekManager`):
- games start with a time limit of `minTimeLimit`
- a difficulty upgrade occurs when the player has completed `nGamesToUpgrade` games _in a row_ with:
  - the same time limit 
  - an average of `timeLimitUpgradeRate` stars collected per second
- For each upgrade, the time limit is increased by `timeLimitIncrement` (up to a maximum of `maxTimeLimit`).
- From then on, all games use the `maxTimeLimit`. 

## Save data

Data is saved to `StarSeekScores.csv`, with one row per played game. Values are:

- `gameNumber`: a unique id per played star seek game
- `sessionNumber`: the session this game was played in (corresponds to sessionNumber in [`SessionSummary.csv`](./session-summary.md))
- `date`: the date the game was played in format YYYY-MM-DD
- `startTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `endTime`: the game end time in format HH:MM:ss (local time - see startTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `timeLimitSeconds`: the time limit set for this game in seconds
- `gameDurationSeconds`: how long the game was played (rounded to the nearest second). If the game was played through to completion this will be equal to timeLimitSeconds; if they exited early, it will be less.
- `nStarsCollected`: the number of stars collected during the game
- `adaptiveLevel`: an integer (1 or above) representing the current difficulty level. Every time the time limit is increased, this level increases by one.
