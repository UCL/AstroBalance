# Zero Gravity

The zero gravity mini-game asks the player to copy various poses, and awards points for keeping still in each.

## Main objects / values to edit during play testing

- **ZeroGravityManager**: values related to timers and scores
  - The number of seconds each pose is displayed
  - The number of seconds of countdown before pose hold begins
  - The number of seconds each pose must be held
  - The score awarded for a hold of a chosen number of seconds
  - The number of seconds the pose must be held for a score increase

- **SwayLine**: values related to movement of the sway line
  - The number of unity units moved per mm of head movement
  - The limit of head movement before scoring stops
  - The colour when the head is in / out of range
  - The minimum number of seconds between increases to nTimesOutOfRange (this is the number of 'falls' for the save data). This minimum interval helps to prevent teetering on the edge of the in-range zone being counted many times.

- **PoseAvatar**: values related to display of poses
  - Sprites for each pose
  - Text explanations for each pose

## Adaptive difficulty

Zero gravity has no adaptive difficulty. The poses, time limits and so on are the same for every game.
  
## Save data

Data is saved to `ZeroGravityScores.csv`, with one row per pose. This means there may be _multiple_ rows per played game. Values are:

- `gameNumber`: a unique id per played zero gravity game
- `sessionNumber`: the session this game was played in (corresponds to sessionNumber in [`SessionSummary.csv`](./session-summary.md))
- `poseNumber`: a unique id per pose. This resets each time the game is played, so the first pose of each game has poseNumber=1.
- `date`: the date of the game session in format YYYY-MM-DD
- `startTime`: the game start time in format HH:MM:ss. This is the local time (e.g. if your computer is set to UK time - this is UK time).
- `endTime`: the game end time in format HH:MM:ss (local time - see startTime description)
- `gameCompleted`: whether this game was completed. If they exited early, this will be false.
- `poseType`: the name of the pose (this is the same as the filename of the displayed pose image png)
- `poseTimeLimitSeconds`: the time limit in seconds for each pose
- `poseDurationSeconds`: the number of seconds the player tried this pose (rounded to the nearest second). If they completed the pose, this will be equal to poseTimeLimitSeconds; if they exited early, it will be less.
- `balanceStabilityScore`: the balance stability score for this pose (this is equal to the displayed score increase shown while playing the game). Currently, 5 points are awarded per second the player stays in bounds. If the player exited before scoring for this pose started, this value will be left blank.
- `falls`: the number of falls while holding this pose. A 'fall' is counted each time the player's head moves out of bounds (with a minimum time interval between falls of `nTimesOutOfRangeCooldown` - this is adjustable on `SwayLine` as mentioned above). The player is out of bounds if, for example, they move too far left or right (in the game, this causes the sway line to change colour from white to black), or the tracker can no longer detect the player (for example, if they step backwards too far). If the player exited before scoring for this pose started, this value will be left blank.
