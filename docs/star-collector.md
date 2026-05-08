# Star Collector

The star collector mini-game uses a ship (controlled by head rotation) to collect a wave of stars falling from the
top of the screen.

## Main objects / values to edit during play testing

- **StarCollectorManager**: values related to the overall win condition of the game, and head velocity save data
  - Min / max time limit in seconds
  - Time limit increment in seconds (if time limit upgrade % is met)
  - Number of games in a row that must meet the upgrade % to increase the time limit
  - The length of the time window used to evaluate player performance
  - The % of stars that must be collected to increase speed or time limit
  - The maximum number of head yaw readings to keep in the buffer at one time (these are used for the head velocity save data)
  - The minimum number of head yaw readings needed to calculate a head velocity
  - The number of seconds to calculate head yaw velocity over
 
- **StarGenerator**: values related to generation of the wave of stars
  - Min, max and base star speed
  - The amount the star speed increases per upgrade
  - Shape of the wave (e.g. width, swerve, star sampling)

- **Ship**: values related to ship movement
  - The amount the ship moves per degree of head movement (X By Degrees)
  
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
- `headVelocityDegPerSecMean`: Mean head yaw velocity (left-right rotation) measured in degrees per second. Rounded to 2 decimal places. Periods of time when the player is out of range of the tracker are excluded.
- `headVelocityDegPerSecPeak`: Peak head yaw velocity (left-right rotation) measured in degrees per second. Rounded to 2 decimal places. Periods of time when the player is out of range of the tracker are excluded.
- `headVelocityDegPerSecSD`: Standard deviation of head yaw velocity (left-right rotation) measured in degrees per second. Rounded to 2 decimal places. Periods of time when the player is out of range of the tracker are excluded.